// packages/rizzyui/src/js/lib/components/rzCommand.js
export default function(Alpine) {
    Alpine.data('rzCommand', () => ({
        search: '',
        selectedValue: null,
        selectedIndex: -1,
        items: [],
        itemsById: new Map(),
        filteredItems: [],
        filteredItemIndexById: new Map(),
        filteredItemIndexByValue: new Map(),
        groupTemplates: new Map(),
        activeDescendantId: null,
        isOpen: false,
        isEmpty: true,
        isLoading: false,
        error: null,
        totalMatches: 0,
        firstRender: true,

        loop: false,
        shouldFilter: true,
        itemsUrl: null,
        fetchTrigger: 'immediate',
        serverFiltering: false,
        dataItemTemplateId: null,
        maxRender: 100,
        _dataFetched: false,
        _debounceTimer: null,
        _listRef: null,
        _lastQuery: '',
        _lastMatchedItems: [],

        showLoading() { return this.isLoading; },
        hasError() { return this.error !== null; },
        notHasError() { return this.error == null; },
        shouldShowEmpty() { return this.isEmpty && this.search && !this.isLoading && !this.error; },
        shouldShowEmptyOrError() { return (this.isEmpty && this.search && !this.isLoading) || this.error !== null; },

        init() {
            this.loop = this.$el.dataset.loop === 'true';
            this.shouldFilter = this.$el.dataset.shouldFilter !== 'false';
            this.selectedValue = this.$el.dataset.selectedValue || null;
            this.itemsUrl = this.$el.dataset.itemsUrl || null;
            this.fetchTrigger = this.$el.dataset.fetchTrigger || 'immediate';
            this.serverFiltering = this.$el.dataset.serverFiltering === 'true';
            this.dataItemTemplateId = this.$el.dataset.templateId || null;

            const parsedMaxRender = Number.parseInt(this.$el.dataset.maxRender || '100', 10);
            this.maxRender = Number.isFinite(parsedMaxRender) && parsedMaxRender > 0 ? parsedMaxRender : 100;

            const itemsScriptId = this.$el.dataset.itemsId;
            let staticItems = [];
            if (itemsScriptId) {
                const itemsScript = document.getElementById(itemsScriptId);
                if (itemsScript) {
                    try {
                        staticItems = JSON.parse(itemsScript.textContent || '[]');
                    } catch (e) {
                        console.error(`RzCommand: Failed to parse JSON from script tag #${itemsScriptId}`, e);
                    }
                }
            }

            if (staticItems.length > 0 && !this.dataItemTemplateId) {
                console.error('RzCommand: `Items` were provided, but no `<CommandItemTemplate>` was found to render them.');
            }

            if (staticItems.length > 0) {
                const preparedItems = staticItems.map(item => ({
                    ...item,
                    id: item.id || `static-item-${crypto.randomUUID()}`,
                    isDataItem: true
                }));
                this.registerItems(preparedItems, { suppressFilter: true });
            }

            if (this.itemsUrl && this.fetchTrigger === 'immediate') {
                this.fetchItems();
            } else {
                this.filterAndSortItems();
            }

            this.$watch('search', (newValue) => {
                this.firstRender = !(newValue || '').trim();
                if (this.serverFiltering) {
                    clearTimeout(this._debounceTimer);
                    this._debounceTimer = setTimeout(() => {
                        this.fetchItems(newValue);
                    }, 300);
                    return;
                }

                this.filterAndSortItems();
            });

            this.$watch('selectedIndex', (newIndex, oldIndex) => {
                if (oldIndex > -1) {
                    const oldItem = this.filteredItems[oldIndex];
                    if (oldItem) {
                        const oldEl = this.$el.querySelector(`[data-command-item-id="${oldItem.id}"]`);
                        if (oldEl) {
                            oldEl.removeAttribute('data-selected');
                            oldEl.setAttribute('aria-selected', 'false');
                        }
                    }
                }

                if (newIndex > -1 && this.filteredItems[newIndex]) {
                    const selectedItem = this.filteredItems[newIndex];
                    this.activeDescendantId = selectedItem.id;

                    const el = this.$el.querySelector(`[data-command-item-id="${selectedItem.id}"]`);
                    if (el) {
                        el.setAttribute('data-selected', 'true');
                        el.setAttribute('aria-selected', 'true');
                        el.scrollIntoView({ block: 'nearest' });
                    }

                    const newValue = selectedItem.value;
                    if (this.selectedValue !== newValue) {
                        this.selectedValue = newValue;
                        this.$dispatch('rz:command:select', { value: newValue });
                    }
                } else {
                    this.activeDescendantId = null;
                    this.selectedValue = null;
                }
            });

            this.$watch('selectedValue', (newValue) => {
                const index = this.filteredItemIndexByValue.get(newValue ?? '') ?? -1;
                if (this.selectedIndex !== index) {
                    this.selectedIndex = index;
                }
            });
        },

        registerList(listComponent) {
            this._listRef = listComponent;
            this._renderList();
        },

        _renderList() {
            if (this._listRef && typeof this._listRef.renderList === 'function') {
                this._listRef.renderList();
            }
        },

        normalizeItem(item) {
            const name = item.name || item.value || '';
            const keywordsArray = Array.isArray(item.keywords) ? item.keywords : [];
            return {
                ...item,
                name,
                keywords: keywordsArray,
                _order: item._order ?? this.items.length,
                _searchText: `${name} ${keywordsArray.join(' ')}`.toLowerCase()
            };
        },

        registerItems(items, options = {}) {
            const suppressFilter = options.suppressFilter === true;
            let registeredAny = false;

            items.forEach(item => {
                if (!item.id) {
                    item.id = `command-item-${crypto.randomUUID()}`;
                }

                if (this.itemsById.has(item.id)) {
                    return;
                }

                const normalizedItem = this.normalizeItem(item);
                this.items.push(normalizedItem);
                this.itemsById.set(normalizedItem.id, normalizedItem);
                registeredAny = true;
            });

            if (!suppressFilter && registeredAny && !this.serverFiltering) {
                this.filterAndSortItems();
            }
        },

        registerItem(item) {
            this.registerItems([item]);
        },

        unregisterItem(itemId) {
            if (!this.itemsById.has(itemId)) {
                return;
            }

            this.itemsById.delete(itemId);
            this.items = this.items.filter(item => item.id !== itemId);
            this.filterAndSortItems();
        },

        registerGroupTemplate(name, templateContent) {
            if (name && templateContent && !this.groupTemplates.has(name)) {
                this.groupTemplates.set(name, templateContent);
            }
        },

        async fetchItems(query = '') {
            if (!this.itemsUrl) return;
            if (!this.dataItemTemplateId) {
                console.error('RzCommand: `ItemsUrl` was provided, but no `<CommandItemTemplate>` was found to render the data.');
                this.error = 'Configuration error: No data template found.';
                return;
            }

            this.isLoading = true;
            this.error = null;

            try {
                const url = new URL(this.itemsUrl, window.location.origin);
                if (this.serverFiltering && query) {
                    url.searchParams.append('q', query);
                }

                const response = await fetch(url);
                if (!response.ok) {
                    throw new Error(`Network response was not ok: ${response.statusText}`);
                }
                const data = await response.json();

                if (this.serverFiltering) {
                    const declarativeItems = this.items.filter(item => !item.isDataItem);
                    this.items = declarativeItems;
                    this.itemsById = new Map(declarativeItems.map(item => [item.id, item]));
                }

                const preparedDataItems = data.map(item => ({
                    ...item,
                    id: item.id || `data-item-${crypto.randomUUID()}`,
                    isDataItem: true
                }));

                this.registerItems(preparedDataItems, { suppressFilter: true });
                this._dataFetched = true;
            } catch (e) {
                this.error = e.message || 'Failed to fetch command items.';
                console.error('RzCommand:', this.error);
            } finally {
                this.isLoading = false;
                this.filterAndSortItems();
            }
        },

        handleInteraction() {
            if (this.itemsUrl && this.fetchTrigger === 'on-open' && !this._dataFetched) {
                this.fetchItems();
            }
        },

        commandScore(searchText, query) {
            if (!query) return 1;

            const exactIndex = searchText.indexOf(query);
            if (exactIndex === -1) {
                return 0;
            }

            if (exactIndex === 0) {
                return 120 - query.length;
            }

            const wordBoundaryIndex = searchText.indexOf(` ${query}`);
            if (wordBoundaryIndex > -1) {
                return 100 - wordBoundaryIndex;
            }

            return 80 - exactIndex;
        },

        _rebuildFilteredIndexes() {
            this.filteredItemIndexById = new Map();
            this.filteredItemIndexByValue = new Map();
            this.filteredItems.forEach((item, index) => {
                this.filteredItemIndexById.set(item.id, index);
                if (item.value != null) {
                    this.filteredItemIndexByValue.set(String(item.value), index);
                }
            });
        },

        filterAndSortItems() {
            if (this.serverFiltering && this._dataFetched) {
                this.totalMatches = this.items.length;
                this.filteredItems = this.items.slice(0, this.maxRender);
                this._rebuildFilteredIndexes();
                this._syncSelection();
                this._renderList();
                this.isOpen = this.totalMatches > 0 || this.isLoading;
                this.isEmpty = this.totalMatches === 0;
                return;
            }

            const query = (this.search || '').trim().toLowerCase();
            const candidates = query && query.startsWith(this._lastQuery)
                ? this._lastMatchedItems
                : this.items;

            const scored = [];
            const forceMounted = [];

            candidates.forEach(item => {
                if (item.forceMount) {
                    forceMounted.push(item);
                    return;
                }

                if (!this.shouldFilter || !query) {
                    scored.push({ item, score: 1 });
                    return;
                }

                const score = this.commandScore(item._searchText, query);
                if (score > 0) {
                    scored.push({ item, score });
                }
            });

            if (this.shouldFilter && query) {
                scored.sort((a, b) => {
                    if (b.score !== a.score) return b.score - a.score;
                    return (a.item._order || 0) - (b.item._order || 0);
                });
            }

            const matchedItems = scored.map(entry => entry.item);
            const allMatches = [...matchedItems, ...forceMounted];

            this._lastQuery = query;
            this._lastMatchedItems = matchedItems;

            this.totalMatches = allMatches.length;
            this.filteredItems = allMatches.slice(0, this.maxRender);
            this._rebuildFilteredIndexes();
            this._syncSelection();

            this.isOpen = this.totalMatches > 0 || this.isLoading;
            this.isEmpty = this.totalMatches === 0;
            this._renderList();
        },

        _syncSelection() {
            if (this.selectedValue) {
                const selectedIndex = this.filteredItemIndexByValue.get(String(this.selectedValue)) ?? -1;
                this.selectedIndex = selectedIndex > -1 ? selectedIndex : (this.filteredItems.length > 0 ? 0 : -1);
                return;
            }

            this.selectedIndex = this.filteredItems.length > 0 ? 0 : -1;
        },

        handleItemClick(event) {
            const host = event.target.closest('[data-command-item-id]');
            if (!host) return;

            const itemId = host.dataset.commandItemId;
            const index = this.filteredItemIndexById.get(itemId);
            if (index === undefined) return;

            const item = this.filteredItems[index];
            if (item && !item.disabled) {
                this.selectedIndex = index;
                this.$dispatch('rz:command:execute', { value: item.value });
            }
        },

        handleItemHover(event) {
            const host = event.target.closest('[data-command-item-id]');
            if (!host) return;

            const itemId = host.dataset.commandItemId;
            const index = this.filteredItemIndexById.get(itemId);
            if (index === undefined) return;

            const item = this.filteredItems[index];
            if (item && !item.disabled && this.selectedIndex !== index) {
                this.selectedIndex = index;
            }
        },

        handleKeydown(e) {
            switch (e.key) {
                case 'ArrowDown':
                    e.preventDefault();
                    this.selectNext();
                    break;
                case 'ArrowUp':
                    e.preventDefault();
                    this.selectPrev();
                    break;
                case 'Home':
                    e.preventDefault();
                    this.selectFirst();
                    break;
                case 'End':
                    e.preventDefault();
                    this.selectLast();
                    break;
                case 'Enter': {
                    e.preventDefault();
                    const item = this.filteredItems[this.selectedIndex];
                    if (item && !item.disabled) {
                        this.$dispatch('rz:command:execute', { value: item.value });
                    }
                    break;
                }
            }
        },

        selectNext() {
            if (this.filteredItems.length === 0) return;
            let i = this.selectedIndex;
            let count = 0;
            do {
                i = (i + 1 >= this.filteredItems.length) ? (this.loop ? 0 : this.filteredItems.length - 1) : i + 1;
                count++;
                if (!this.filteredItems[i]?.disabled) { this.selectedIndex = i; return; }
                if (!this.loop && i === this.filteredItems.length - 1) return;
            } while (count <= this.filteredItems.length);
        },

        selectPrev() {
            if (this.filteredItems.length === 0) return;
            let i = this.selectedIndex;
            let count = 0;
            do {
                i = (i - 1 < 0) ? (this.loop ? this.filteredItems.length - 1 : 0) : i - 1;
                count++;
                if (!this.filteredItems[i]?.disabled) { this.selectedIndex = i; return; }
                if (!this.loop && i === 0) return;
            } while (count <= this.filteredItems.length);
        },

        selectFirst() {
            if (this.filteredItems.length > 0) {
                const firstEnabledIndex = this.filteredItems.findIndex(item => !item.disabled);
                if (firstEnabledIndex > -1) this.selectedIndex = firstEnabledIndex;
            }
        },

        selectLast() {
            if (this.filteredItems.length > 0) {
                const lastEnabledIndex = this.filteredItems.map(item => item.disabled).lastIndexOf(false);
                if (lastEnabledIndex > -1) this.selectedIndex = lastEnabledIndex;
            }
        }
    }));
}
