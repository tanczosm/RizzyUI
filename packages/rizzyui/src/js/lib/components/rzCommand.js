// packages/rizzyui/src/js/lib/components/rzCommand.js
export default function(Alpine) {
    Alpine.data('rzCommand', () => ({
        // --- STATE ---
        search: '',
        selectedValue: null,
        selectedIndex: -1,
        items: [],
        itemsById: new Map(),
        filteredItems: [],
        filteredIndexById: new Map(),
        groupTemplates: new Map(),
        listController: null,
        activeDescendantId: null,
        isOpen: false,
        isEmpty: true,
        isLoading: false,
        error: null,

        // --- CONFIG ---
        loop: false,
        shouldFilter: true,
        itemsUrl: null,
        fetchTrigger: 'immediate',
        serverFiltering: false,
        dataItemTemplateId: null,
        maxRender: 100,
        _dataFetched: false,
        _debounceTimer: null,
        _lastSearch: '',
        _lastMatchedIds: [],

        // --- COMPUTED (CSP-Compliant Methods) ---
        showLoading() { return this.isLoading; },
        hasError() { return this.error !== null; },
        notHasError() { return this.error == null; },
        shouldShowEmpty() { return this.isEmpty && this.search && !this.isLoading && !this.error; },
        shouldShowEmptyOrError() { return (this.isEmpty && this.search && !this.isLoading) || this.error !== null; },
        renderedCount() { return Math.min(this.filteredItems.length, this.maxRender); },

        // --- LIFECYCLE ---
        init() {
            this.loop = this.$el.dataset.loop === 'true';
            this.shouldFilter = this.$el.dataset.shouldFilter !== 'false';
            this.selectedValue = this.$el.dataset.selectedValue || null;
            this.itemsUrl = this.$el.dataset.itemsUrl || null;
            this.fetchTrigger = this.$el.dataset.fetchTrigger || 'immediate';
            this.serverFiltering = this.$el.dataset.serverFiltering === 'true';
            this.dataItemTemplateId = this.$el.dataset.templateId || null;

            const parsedMaxRender = Number.parseInt(this.$el.dataset.maxRender ?? '100', 10);
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

            const normalizedStaticItems = staticItems.map(item => ({
                ...item,
                id: item.id || `static-item-${crypto.randomUUID()}`,
                isDataItem: true,
            }));
            this.registerItems(normalizedStaticItems, { suppressFilter: true });

            if (this.itemsUrl && this.fetchTrigger === 'immediate') {
                this.fetchItems();
            } else {
                this.filterAndSortItems();
            }

            this.$watch('search', (newValue) => {
                if (this.serverFiltering) {
                    clearTimeout(this._debounceTimer);
                    this._debounceTimer = setTimeout(() => {
                        this.fetchItems(newValue);
                    }, 300);
                } else {
                    this.filterAndSortItems();
                }
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
                const nextIndex = this.filteredItems.findIndex(item => item.value === newValue);
                if (this.selectedIndex !== nextIndex) {
                    this.selectedIndex = nextIndex;
                }
            });

            this.$watch('filteredItems', (items) => {
                this.isOpen = items.length > 0 || this.isLoading;
                this.isEmpty = items.length === 0;
                this.filteredIndexById = new Map(items.map((item, index) => [item.id, index]));
                this.renderList();
            });
        },

        // --- METHODS ---
        setListController(controller) {
            this.listController = controller;
            this.renderList();
        },

        renderList() {
            if (this.listController) {
                this.listController.renderList();
            }
        },

        normalizeItem(item, orderIndex = this.items.length) {
            const normalizedKeywords = Array.isArray(item.keywords) ? item.keywords : [];
            const normalizedName = item.name || item.value || '';

            return {
                ...item,
                keywords: normalizedKeywords,
                name: normalizedName,
                _order: item._order ?? orderIndex,
                _searchText: `${normalizedName} ${normalizedKeywords.join(' ')}`.trim().toLowerCase(),
            };
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
                const fetchedItems = data.map(item => ({
                    ...item,
                    id: item.id || `data-item-${crypto.randomUUID()}`,
                    isDataItem: true,
                }));

                if (this.serverFiltering) {
                    const declarativeItems = this.items.filter(item => !item.isDataItem);
                    this.items = [];
                    this.itemsById = new Map();
                    this.registerItems(declarativeItems, { suppressFilter: true });
                    this.registerItems(fetchedItems, { suppressFilter: true });
                } else {
                    this.registerItems(fetchedItems, { suppressFilter: true });
                }

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

        registerItems(items, options = {}) {
            const suppressFilter = options.suppressFilter === true;
            let didRegister = false;

            items.forEach((item) => {
                if (!item?.id || this.itemsById.has(item.id)) return;

                const normalizedItem = this.normalizeItem(item, this.items.length);
                this.items.push(normalizedItem);
                this.itemsById.set(normalizedItem.id, normalizedItem);
                didRegister = true;
            });

            if (didRegister && this.selectedIndex === -1) {
                this.selectedIndex = 0;
            }

            if (didRegister && !suppressFilter && !this.serverFiltering) {
                this.filterAndSortItems();
            }
        },

        registerItem(item) {
            this.registerItems([item]);
        },

        unregisterItem(itemId) {
            if (!this.itemsById.has(itemId)) return;

            this.itemsById.delete(itemId);
            this.items = this.items.filter(item => item.id !== itemId);
            this._lastMatchedIds = this._lastMatchedIds.filter(id => id !== itemId);
            this.filterAndSortItems();
        },

        registerGroupTemplate(name, templateElement) {
            if (name && templateElement && !this.groupTemplates.has(name)) {
                this.groupTemplates.set(name, templateElement);
            }
        },

        scoreItem(item, searchValue) {
            if (!searchValue) {
                return item.forceMount ? -1 : 1;
            }

            const text = item._searchText || '';
            const query = searchValue.toLowerCase();

            if (item.forceMount) {
                return -1;
            }

            if (text.startsWith(query)) return 1200;

            const wordBoundaryMatch = text.indexOf(` ${query}`);
            if (wordBoundaryMatch > -1) return 900 - wordBoundaryMatch;

            const containsIndex = text.indexOf(query);
            if (containsIndex > -1) return 700 - containsIndex;

            let queryIndex = 0;
            for (let i = 0; i < text.length && queryIndex < query.length; i += 1) {
                if (text[i] === query[queryIndex]) {
                    queryIndex += 1;
                }
            }

            if (queryIndex === query.length) {
                return 300;
            }

            return 0;
        },

        getCandidateIds(normalizedSearch) {
            if (!normalizedSearch || !this.shouldFilter) {
                return this.items.map(item => item.id);
            }

            if (this._lastSearch && normalizedSearch.startsWith(this._lastSearch)) {
                return this._lastMatchedIds;
            }

            return this.items.map(item => item.id);
        },

        filterAndSortItems() {
            if (this.serverFiltering && this._dataFetched) {
                this.filteredItems = [...this.items];
                this._lastMatchedIds = this.filteredItems.map(item => item.id);
                this._lastSearch = (this.search || '').toLowerCase();
                this.selectedIndex = this.filteredItems.length > 0 ? 0 : -1;
                return;
            }

            const normalizedSearch = (this.search || '').trim().toLowerCase();
            const candidateIds = this.getCandidateIds(normalizedSearch);
            const candidateItems = candidateIds
                .map(id => this.itemsById.get(id))
                .filter(Boolean);

            const scoredItems = [];
            for (let i = 0; i < candidateItems.length; i += 1) {
                const item = candidateItems[i];
                const score = this.scoreItem(item, normalizedSearch);

                if (score > 0 || item.forceMount || !normalizedSearch || !this.shouldFilter) {
                    scoredItems.push({ item, score });
                }
            }

            scoredItems.sort((a, b) => {
                if (a.item.forceMount && !b.item.forceMount) return 1;
                if (!a.item.forceMount && b.item.forceMount) return -1;
                if (b.score !== a.score) return b.score - a.score;
                return (a.item._order || 0) - (b.item._order || 0);
            });

            this.filteredItems = scoredItems.map(entry => entry.item);
            this._lastMatchedIds = this.filteredItems.map(item => item.id);
            this._lastSearch = normalizedSearch;

            if (this.selectedValue) {
                const newIndex = this.filteredItems.findIndex(item => item.value === this.selectedValue);
                this.selectedIndex = newIndex > -1 ? newIndex : (this.filteredItems.length > 0 ? 0 : -1);
            } else {
                this.selectedIndex = this.filteredItems.length > 0 ? 0 : -1;
            }
        },

        getFilteredIndex(itemId) {
            return this.filteredIndexById.get(itemId) ?? -1;
        },

        // --- EVENT HANDLERS ---
        handleItemClick(event) {
            const host = event.target.closest('[data-command-item-id]');
            if (!host) return;

            const itemId = host.dataset.commandItemId;
            const index = this.getFilteredIndex(itemId);

            if (index > -1) {
                const item = this.filteredItems[index];
                if (item && !item.disabled) {
                    this.selectedIndex = index;
                    this.$dispatch('rz:command:execute', { value: item.value });
                }
            }
        },

        handleItemHover(event) {
            const host = event.target.closest('[data-command-item-id]');
            if (!host) return;

            const itemId = host.dataset.commandItemId;
            const index = this.getFilteredIndex(itemId);

            if (index > -1) {
                const item = this.filteredItems[index];
                if (item && !item.disabled && this.selectedIndex !== index) {
                    this.selectedIndex = index;
                }
            }
        },

        // --- KEYBOARD NAVIGATION ---
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
                count += 1;
                if (!this.filteredItems[i]?.disabled) {
                    this.selectedIndex = i;
                    return;
                }
                if (!this.loop && i === this.filteredItems.length - 1) return;
            } while (count <= this.filteredItems.length);
        },

        selectPrev() {
            if (this.filteredItems.length === 0) return;
            let i = this.selectedIndex;
            let count = 0;
            do {
                i = (i - 1 < 0) ? (this.loop ? this.filteredItems.length - 1 : 0) : i - 1;
                count += 1;
                if (!this.filteredItems[i]?.disabled) {
                    this.selectedIndex = i;
                    return;
                }
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
