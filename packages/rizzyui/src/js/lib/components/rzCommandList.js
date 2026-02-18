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
            this.parent.registerList(this);

            if (this.parent.dataItemTemplateId) {
                this.dataItemTemplate = document.getElementById(this.parent.dataItemTemplateId);
            }
        },

        renderList() {
            const items = this.parent.filteredItems || [];
            const groups = this.parent.groupTemplates || new Map();
            const container = this.$el;

            container.querySelectorAll('[data-dynamic-item]').forEach(el => el.remove());

            const groupedItems = new Map([['__ungrouped__', []]]);
            items.forEach(item => {
                const groupName = item.group || '__ungrouped__';
                if (!groupedItems.has(groupName)) {
                    groupedItems.set(groupName, []);
                }
                groupedItems.get(groupName).push(item);
            });

            const fragment = document.createDocumentFragment();

            groupedItems.forEach((groupItems, groupName) => {
                if (groupItems.length === 0) {
                    return;
                }

                const groupContainer = document.createElement('div');
                groupContainer.setAttribute('role', 'group');
                groupContainer.setAttribute('data-dynamic-item', 'true');
                groupContainer.setAttribute('data-slot', 'command-group');

                if (groupName !== '__ungrouped__') {
                    const headingTemplateContent = groups.get(groupName);
                    if (headingTemplateContent) {
                        const headingClone = headingTemplateContent.cloneNode(true);
                        const headingEl = headingClone.firstElementChild;
                        if (headingEl) {
                            groupContainer.setAttribute('aria-labelledby', headingEl.id);
                            groupContainer.appendChild(headingClone);
                        }
                    }
                }

                groupItems.forEach(item => {
                    const itemIndex = this.parent.filteredItemIndexById.get(item.id);
                    let itemEl = null;

                    if (item.isDataItem) {
                        if (!this.dataItemTemplate) {
                            return;
                        }

                        const clone = this.dataItemTemplate.content.cloneNode(true);
                        itemEl = clone.firstElementChild;
                        if (!itemEl) {
                            return;
                        }

                        this.bindDataItemTemplate(itemEl, item);
                    } else if (item.templateContent) {
                        const clone = item.templateContent.cloneNode(true);
                        itemEl = clone.firstElementChild;
                    }

                    if (!itemEl) {
                        return;
                    }

                    itemEl.setAttribute('data-command-item-id', item.id);
                    itemEl.setAttribute('data-value', item.value);
                    if (item.keywords?.length) itemEl.setAttribute('data-keywords', JSON.stringify(item.keywords));
                    if (item.group) itemEl.setAttribute('data-group', item.group);
                    if (item.disabled) itemEl.setAttribute('data-disabled', 'true');
                    if (item.forceMount) itemEl.setAttribute('data-force-mount', 'true');

                    itemEl.setAttribute('role', 'option');
                    itemEl.setAttribute('aria-selected', String(this.parent.selectedIndex === itemIndex));
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

            container.appendChild(fragment);

            if (window.htmx) {
                window.htmx.process(container);
            }
        },

        resolveBindingValue(item, expression) {
            if (!expression || !expression.startsWith('item.')) {
                return null;
            }

            return expression
                .split('.')
                .slice(1)
                .reduce((current, key) => (current == null ? null : current[key]), item);
        },

        bindDataItemTemplate(itemEl, item) {
            const allNodes = [itemEl, ...itemEl.querySelectorAll('*')];

            allNodes.forEach(node => {
                const xTextExpression = node.getAttribute('x-text');
                if (xTextExpression) {
                    const value = this.resolveBindingValue(item, xTextExpression);
                    node.textContent = value == null ? '' : String(value);
                    node.removeAttribute('x-text');
                }

                const xHtmlExpression = node.getAttribute('x-html');
                if (xHtmlExpression) {
                    const value = this.resolveBindingValue(item, xHtmlExpression);
                    node.innerHTML = value == null ? '' : String(value);
                    node.removeAttribute('x-html');
                }

                [...node.attributes].forEach(attribute => {
                    if (!attribute.name.startsWith('x-bind:') && !attribute.name.startsWith(':')) {
                        return;
                    }

                    const targetAttribute = attribute.name.replace('x-bind:', '').replace(':', '');
                    const value = this.resolveBindingValue(item, attribute.value);
                    if (value != null) {
                        node.setAttribute(targetAttribute, String(value));
                    }

                    node.removeAttribute(attribute.name);
                });
            });
        }
    }));
}
