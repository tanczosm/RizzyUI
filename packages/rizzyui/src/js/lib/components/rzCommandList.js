export default function(Alpine) {
    Alpine.data('rzCommandList', () => ({
        parent: null,
        dataItemTemplate: null,

        init() {
            const parentEl = this.$el.closest('[x-data="rzCommand"]');
            if (!parentEl) {
                console.error('CommandList must be a child of RzCommand.');
                return;
            }

            this.parent = Alpine.$data(parentEl);
            this.parent.setListController(this);

            if (this.parent.dataItemTemplateId) {
                this.dataItemTemplate = document.getElementById(this.parent.dataItemTemplateId);
            }
        },

        renderList() {
            const items = this.parent.filteredItems || [];
            const groups = this.parent.groupTemplates || new Map();
            const maxRender = this.parent.maxRender || 100;
            const visibleItems = items.slice(0, maxRender);
            const container = this.$el;
            const fragment = document.createDocumentFragment();

            const groupedItems = new Map([['__ungrouped__', []]]);
            visibleItems.forEach(item => {
                const groupName = item.group || '__ungrouped__';
                if (!groupedItems.has(groupName)) {
                    groupedItems.set(groupName, []);
                }
                groupedItems.get(groupName).push(item);
            });

            groupedItems.forEach((groupItems, groupName) => {
                if (groupItems.length === 0) return;

                const groupContainer = document.createElement('div');
                groupContainer.setAttribute('role', 'group');
                groupContainer.setAttribute('data-dynamic-item', 'true');
                groupContainer.setAttribute('data-slot', 'command-group');

                if (groupName !== '__ungrouped__') {
                    const headingTemplate = groups.get(groupName);
                    if (headingTemplate && headingTemplate.content) {
                        const headingClone = headingTemplate.content.cloneNode(true);
                        const headingEl = headingClone.firstElementChild;
                        if (headingEl) {
                            groupContainer.setAttribute('aria-labelledby', headingEl.id);
                            groupContainer.appendChild(headingClone);
                        }
                    }
                }

                groupItems.forEach(item => {
                    const itemIndex = this.parent.getFilteredIndex(item.id);
                    let itemEl;

                    if (item.isDataItem) {
                        if (!this.dataItemTemplate) {
                            return;
                        }

                        const clone = this.dataItemTemplate.content.cloneNode(true);
                        itemEl = clone.firstElementChild;
                        if (!itemEl) return;

                        Alpine.addScopeToNode(itemEl, { item });
                        Alpine.initTree(itemEl);
                    } else {
                        const templateElement = item.templateElement;
                        if (templateElement && templateElement.content) {
                            const clone = templateElement.content.cloneNode(true);
                            itemEl = clone.firstElementChild;
                        }
                    }

                    if (!itemEl) return;

                    itemEl.setAttribute('data-command-item-id', item.id);
                    itemEl.setAttribute('data-value', item.value);
                    if (item.keywords?.length) itemEl.setAttribute('data-keywords', JSON.stringify(item.keywords));
                    if (item.group) itemEl.setAttribute('data-group', item.group);
                    if (item.disabled) itemEl.setAttribute('data-disabled', 'true');
                    if (item.forceMount) itemEl.setAttribute('data-force-mount', 'true');

                    itemEl.setAttribute('role', 'option');
                    itemEl.setAttribute('aria-selected', this.parent.selectedIndex === itemIndex);
                    if (item.disabled) {
                        itemEl.setAttribute('aria-disabled', 'true');
                    }

                    if (this.parent.selectedIndex === itemIndex) {
                        itemEl.setAttribute('data-selected', 'true');
                    }

                    groupContainer.appendChild(itemEl);
                });

                fragment.appendChild(groupContainer);
            });

            container.querySelectorAll('[data-dynamic-item]').forEach(el => el.remove());
            container.appendChild(fragment);

            if (window.htmx?.process) {
                window.htmx.process(container);
            }
        }
    }));
}
