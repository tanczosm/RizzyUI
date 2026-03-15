import { createTable, getCoreRowModel, getFilteredRowModel, getPaginationRowModel, getSortedRowModel } from '@tanstack/table-core';

export default function rzDataTable() {
    return {
        config: null,
        table: null,
        state: {
            sorting: [],
            globalFilter: '',
            columnVisibility: {},
            pagination: { pageIndex: 0, pageSize: 10 },
            rowSelection: {},
        },
        searchTimer: null,
        columnMenuOpen: false,

        init() {
            this.config = this.readConfig();
            if (!this.config) return;

            this.state.sorting = this.config.initialState?.sorting ?? [];
            this.state.globalFilter = this.config.initialState?.globalFilter ?? '';
            this.state.columnVisibility = this.config.initialState?.columnVisibility ?? {};
            this.state.pagination = this.config.initialState?.pagination ?? { pageIndex: 0, pageSize: 10 };
            this.state.rowSelection = this.toSelectionMap(this.config.initialState?.selectedKeys ?? []);

            this.initializeTable();
            this.wireControls();
            this.renderFromState(true);
            this.dispatch('ready', { id: this.config.id });
            this.dispatchStateChange();
        },

        readConfig() {
            const id = this.$el.dataset.configId;
            const script = id ? document.getElementById(id) : this.$el.querySelector(`[data-rz-datatable-config]`);
            if (!script) return null;

            try {
                return JSON.parse(script.textContent || '{}');
            } catch {
                return null;
            }
        },

        initializeTable() {
            const columns = (this.config.columns || []).map((column) => ({
                id: column.id,
                accessorFn: (row) => row[column.field],
                enableSorting: !!column.sortable,
                enableHiding: !!column.canHide,
                enableGlobalFilter: !!column.searchable,
                meta: column,
            }));

            this.table = createTable({
                data: this.config.rows || [],
                columns,
                state: this.state,
                getCoreRowModel: getCoreRowModel(),
                getSortedRowModel: getSortedRowModel(),
                getFilteredRowModel: getFilteredRowModel(),
                getPaginationRowModel: getPaginationRowModel(),
                getRowId: (row) => row.__key,
                globalFilterFn: (row, _columnId, filterValue) => {
                    const value = `${filterValue ?? ''}`.trim().toLowerCase();
                    if (!value) return true;

                    return this.config.columns
                        .filter((column) => column.searchable)
                        .some((column) => {
                            const raw = row.original[column.field];
                            if (raw === null || raw === undefined) return false;
                            return `${raw}`.toLowerCase().includes(value);
                        });
                },
                onSortingChange: (updater) => {
                    this.state.sorting = this.resolveUpdater(updater, this.state.sorting);
                    this.renderFromState();
                    this.dispatch('sort-change', { id: this.config.id, sorting: this.state.sorting });
                },
                onGlobalFilterChange: (updater) => {
                    this.state.globalFilter = this.resolveUpdater(updater, this.state.globalFilter);
                    this.state.pagination.pageIndex = 0;
                    this.renderFromState();
                    this.dispatch('search-change', { id: this.config.id, search: this.state.globalFilter ?? '' });
                },
                onColumnVisibilityChange: (updater) => {
                    this.state.columnVisibility = this.resolveUpdater(updater, this.state.columnVisibility);
                    this.renderFromState();
                    this.dispatch('column-visibility-change', { id: this.config.id, visibility: this.state.columnVisibility });
                },
                onPaginationChange: (updater) => {
                    this.state.pagination = this.resolveUpdater(updater, this.state.pagination);
                    this.renderFromState();
                    this.dispatch('page-change', { id: this.config.id, pageIndex: this.state.pagination.pageIndex, pageSize: this.state.pagination.pageSize });
                },
                onRowSelectionChange: (updater) => {
                    this.state.rowSelection = this.resolveUpdater(updater, this.state.rowSelection);
                    this.renderFromState();
                    this.dispatch('selection-change', { id: this.config.id, selectedKeys: this.getSelectedKeysInRowOrder() });
                },
            });
        },

        wireControls() {
            const searchInput = this.$el.querySelector(`[data-search-input="${this.config.id}"]`);
            if (searchInput) {
                searchInput.addEventListener('input', (event) => {
                    const value = event.target.value || '';
                    clearTimeout(this.searchTimer);
                    this.searchTimer = window.setTimeout(() => {
                        this.table.setGlobalFilter(value);
                    }, this.config.searchDebounceMs || 0);
                });
            }

            const trigger = this.$el.querySelector(`[data-column-menu-trigger="${this.config.id}"]`);
            const panel = this.$el.querySelector(`[data-column-menu-panel="${this.config.id}"]`);
            if (trigger && panel) {
                trigger.addEventListener('click', () => {
                    this.columnMenuOpen = !this.columnMenuOpen;
                    this.updateColumnMenuDom(trigger, panel);
                });

                document.addEventListener('click', (event) => {
                    if (!this.columnMenuOpen) return;
                    if (this.$el.contains(event.target) && (trigger.contains(event.target) || panel.contains(event.target))) return;
                    this.columnMenuOpen = false;
                    this.updateColumnMenuDom(trigger, panel);
                });

                panel.addEventListener('keydown', (event) => {
                    if (event.key !== 'Escape') return;
                    this.columnMenuOpen = false;
                    this.updateColumnMenuDom(trigger, panel);
                    trigger.focus();
                });
            }

            this.$el.querySelectorAll('[data-column-visibility-toggle]').forEach((checkbox) => {
                checkbox.addEventListener('change', (event) => {
                    const columnId = event.target.dataset.columnVisibilityToggle;
                    const checked = !!event.target.checked;
                    this.table.getColumn(columnId)?.toggleVisibility(checked);
                });
            });

            this.$el.querySelectorAll('[data-sort-button]').forEach((button) => {
                button.addEventListener('click', () => {
                    const columnId = button.dataset.sortButton;
                    this.table.getColumn(columnId)?.toggleSorting();
                });
            });

            const prevButton = this.$el.querySelector(`[data-page-prev="${this.config.id}"]`);
            const nextButton = this.$el.querySelector(`[data-page-next="${this.config.id}"]`);
            const pageSizeSelect = this.$el.querySelector(`[data-page-size="${this.config.id}"]`);

            prevButton?.addEventListener('click', () => this.table.previousPage());
            nextButton?.addEventListener('click', () => this.table.nextPage());
            pageSizeSelect?.addEventListener('change', (event) => {
                const value = Number.parseInt(event.target.value, 10);
                if (!Number.isNaN(value) && value > 0) {
                    this.table.setPageSize(value);
                }
            });
        },

        renderFromState(initial = false) {
            this.table.setOptions((prev) => ({ ...prev, state: this.state }));
            this.syncHeaderState();
            this.renderBody();
            this.syncPaginationUi();
            this.syncVisibilityUi();

            if (!initial) {
                this.dispatchStateChange();
            }
        },

        renderBody() {
            const body = this.$el.querySelector(`[data-table-body="${this.config.id}"]`);
            if (!body) return;

            body.innerHTML = '';
            const visibleColumns = this.table.getVisibleLeafColumns();
            const rows = this.table.getRowModel().rows;
            const colspan = visibleColumns.length + (this.config.selectionMode === 'multiple' ? 1 : 0);

            if (rows.length === 0) {
                const row = document.createElement('tr');
                const cell = document.createElement('td');
                cell.colSpan = colspan;
                cell.className = 'text-muted-foreground py-8 text-center text-sm';
                cell.textContent = 'No results';
                row.appendChild(cell);
                body.appendChild(row);
                return;
            }

            rows.forEach((row) => {
                const tr = document.createElement('tr');
                tr.dataset.rowKey = row.id;
                tr.className = 'border-b transition-colors';
                tr.addEventListener('click', () => this.handleRowClick(row.id));

                if (this.config.selectionMode === 'multiple') {
                    const selectCell = document.createElement('td');
                    const checkbox = document.createElement('input');
                    checkbox.type = 'checkbox';
                    checkbox.setAttribute('aria-label', 'Select row');
                    checkbox.checked = !!this.state.rowSelection[row.id];
                    checkbox.addEventListener('click', (event) => event.stopPropagation());
                    checkbox.addEventListener('change', () => this.table.toggleRowSelected(row.id));
                    selectCell.appendChild(checkbox);
                    tr.appendChild(selectCell);
                }

                visibleColumns.forEach((column) => {
                    const td = document.createElement('td');
                    const value = row.original[column.columnDef.meta.field];
                    const hasTemplate = !!column.columnDef.meta.hasTemplate;

                    if (hasTemplate) {
                        this.renderTemplateCell(td, row.original, column.columnDef.meta, value);
                    } else {
                        td.textContent = value === null || value === undefined
                            ? (column.columnDef.meta.nullDisplayText || '')
                            : `${value}`;
                    }

                    tr.appendChild(td);
                });

                body.appendChild(tr);
            });

            this.syncSelectionUi();
        },

        renderTemplateCell(cellElement, row, column, cell) {
            const template = this.$el.querySelector(`template[data-column-template="${column.id}"]`);
            if (!template) {
                cellElement.textContent = cell === null || cell === undefined ? (column.nullDisplayText || '') : `${cell}`;
                return;
            }

            const host = document.createElement('div');
            host.appendChild(template.content.cloneNode(true));
            if (window.Alpine && typeof window.Alpine.addScopeToNode === 'function') {
                window.Alpine.addScopeToNode(host, { row, column, cell });
                window.Alpine.initTree(host);
            }
            cellElement.appendChild(host);
        },

        handleRowClick(rowKey) {
            if (this.config.selectionMode === 'single') {
                this.state.rowSelection = { [rowKey]: true };
                this.renderFromState();
                this.dispatch('selection-change', { id: this.config.id, selectedKeys: [rowKey] });
            }

            this.dispatch('row-click', { id: this.config.id, key: rowKey });
        },

        syncHeaderState() {
            const sortingMap = new Map(this.state.sorting.map((entry) => [entry.id, entry.desc ? 'descending' : 'ascending']));
            this.$el.querySelectorAll('[data-column-id]').forEach((headerCell) => {
                const columnId = headerCell.dataset.columnId;
                const state = sortingMap.get(columnId) || 'none';
                headerCell.setAttribute('aria-sort', state);

                const indicator = this.$el.querySelector(`[data-sort-indicator="${columnId}"]`);
                if (indicator) {
                    indicator.textContent = state === 'ascending' ? '↑' : (state === 'descending' ? '↓' : '↕');
                }
            });
        },

        syncPaginationUi() {
            const pageLabel = this.$el.querySelector(`[data-page-label="${this.config.id}"]`);
            const pageCount = this.table.getPageCount();
            const pageIndex = this.state.pagination.pageIndex;

            if (pageLabel) {
                pageLabel.textContent = `Page ${pageCount === 0 ? 0 : pageIndex + 1} of ${pageCount}`;
            }

            const prevButton = this.$el.querySelector(`[data-page-prev="${this.config.id}"]`);
            const nextButton = this.$el.querySelector(`[data-page-next="${this.config.id}"]`);
            if (prevButton) prevButton.disabled = !this.table.getCanPreviousPage();
            if (nextButton) nextButton.disabled = !this.table.getCanNextPage();
        },

        syncVisibilityUi() {
            this.$el.querySelectorAll('[data-column-visibility-toggle]').forEach((checkbox) => {
                const columnId = checkbox.dataset.columnVisibilityToggle;
                const visible = this.table.getColumn(columnId)?.getIsVisible();
                checkbox.checked = !!visible;
            });
        },

        syncSelectionUi() {
            const headerCheckbox = this.$el.querySelector(`[data-select-all="${this.config.id}"]`);
            if (headerCheckbox) {
                headerCheckbox.checked = this.table.getIsAllPageRowsSelected();
                headerCheckbox.indeterminate = this.table.getIsSomePageRowsSelected();
                if (!headerCheckbox.dataset.bound) {
                    headerCheckbox.dataset.bound = 'true';
                    headerCheckbox.addEventListener('change', () => {
                        this.table.toggleAllPageRowsSelected(!!headerCheckbox.checked);
                    });
                }
            }
        },

        updateColumnMenuDom(trigger, panel) {
            trigger.setAttribute('aria-expanded', this.columnMenuOpen ? 'true' : 'false');
            panel.classList.toggle('hidden', !this.columnMenuOpen);
        },

        getSelectedKeysInRowOrder() {
            const selected = this.state.rowSelection;
            return this.table
                .getSortedRowModel()
                .rows
                .filter((row) => !!selected[row.id])
                .map((row) => row.id);
        },

        dispatch(eventName, detail) {
            this.$el.dispatchEvent(new CustomEvent(`rz:datatable:${eventName}`, {
                detail,
                bubbles: true,
            }));
        },

        dispatchStateChange() {
            this.dispatch('state-change', {
                id: this.config.id,
                sorting: this.state.sorting,
                pagination: this.state.pagination,
                globalFilter: this.state.globalFilter ?? '',
                columnVisibility: this.state.columnVisibility,
                selectedKeys: this.getSelectedKeysInRowOrder(),
            });
        },

        resolveUpdater(updater, previous) {
            return typeof updater === 'function' ? updater(previous) : updater;
        },

        toSelectionMap(keys) {
            return (keys || []).reduce((acc, key) => {
                acc[key] = true;
                return acc;
            }, {});
        },
    };
}
