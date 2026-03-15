import {
    createTable,
    getCoreRowModel,
    getFilteredRowModel,
    getPaginationRowModel,
    getSortedRowModel,
} from '@tanstack/table-core';

export default function rzDataTable() {
    return {
        table: null,
        config: null,
        state: null,
        columnMenuOpen: false,
        search: '',
        pageSize: 10,
        paginationSummary: '',
        canNextPage: false,
        canPreviousPage: false,
        allPageRowsSelected: false,
        somePageRowsSelected: false,

        init() {
            const selector = this.$el.dataset.configSelector;
            const script = selector ? this.$el.querySelector(selector) : null;
            if (!script) {
                return;
            }

            this.config = JSON.parse(script.textContent || '{}');
            this.search = this.config.initialState?.globalFilter ?? '';
            this.pageSize = this.config.initialState?.pagination?.pageSize ?? 10;

            this.state = {
                sorting: this.config.initialState?.sorting ?? [],
                globalFilter: this.search,
                pagination: {
                    pageIndex: this.config.initialState?.pagination?.pageIndex ?? 0,
                    pageSize: this.pageSize,
                },
                columnVisibility: this.config.initialState?.columnVisibility ?? {},
                rowSelection: this.toSelectionMap(this.config.initialState?.selectedKeys ?? []),
            };

            this.table = createTable({
                data: this.config.rows,
                columns: this.buildColumns(),
                state: this.state,
                getRowId: (row) => String(row.__key),
                getCoreRowModel: getCoreRowModel(),
                getFilteredRowModel: getFilteredRowModel(),
                getSortedRowModel: getSortedRowModel(),
                getPaginationRowModel: getPaginationRowModel(),
                globalFilterFn: this.globalFilter,
                onSortingChange: (updater) => this.applyState('sorting', updater),
                onGlobalFilterChange: (updater) => this.applyState('globalFilter', updater),
                onPaginationChange: (updater) => this.applyState('pagination', updater),
                onColumnVisibilityChange: (updater) => this.applyState('columnVisibility', updater),
                onRowSelectionChange: (updater) => this.applyState('rowSelection', updater),
            });

            this.$watch('search', (value) => {
                window.clearTimeout(this.searchDebounce);
                this.searchDebounce = window.setTimeout(() => {
                    this.table.setGlobalFilter(value);
                    this.dispatch('search-change', { search: value });
                    this.dispatchState();
                    this.refresh();
                }, this.config.searchDebounceMs || 300);
            });

            this.refresh();
            this.dispatch('ready', { id: this.config.id });
            this.dispatchState();
        },

        globalFilter(row, _columnId, value) {
            if (!value) {
                return true;
            }

            const search = String(value).toLowerCase();
            const searchable = this.config.columns.filter((column) => column.searchable);
            for (const column of searchable) {
                const rawValue = row.original[column.field];
                if (rawValue == null) {
                    continue;
                }

                if (String(rawValue).toLowerCase().includes(search)) {
                    return true;
                }
            }

            return false;
        },

        buildColumns() {
            return this.config.columns.map((column) => ({
                id: column.id,
                accessorFn: (row) => row[column.field],
                enableSorting: column.sortable,
                enableHiding: column.canHide,
                meta: column,
            }));
        },

        applyState(key, updater) {
            const next = typeof updater === 'function' ? updater(this.state[key]) : updater;
            this.state = { ...this.state, [key]: next };
            this.table.setOptions((previous) => ({ ...previous, state: this.state }));
        },

        refresh() {
            this.renderBody();
            const pagination = this.table.getState().pagination;
            const pageRows = this.table.getRowModel().rows;
            const filteredTotal = this.table.getFilteredRowModel().rows.length;
            const selectedKeys = this.getSelectedKeys();

            this.paginationSummary = `${filteredTotal} row(s)`;
            this.canPreviousPage = this.table.getCanPreviousPage();
            this.canNextPage = this.table.getCanNextPage();
            this.pageSize = pagination.pageSize;

            const pageKeys = pageRows.map((row) => row.id);
            const pageSelectedCount = pageKeys.filter((key) => selectedKeys.includes(key)).length;
            this.allPageRowsSelected = pageRows.length > 0 && pageSelectedCount === pageRows.length;
            this.somePageRowsSelected = pageSelectedCount > 0 && pageSelectedCount < pageRows.length;

            this.dispatch('sort-change', { sorting: this.state.sorting });
            this.dispatch('page-change', { pageIndex: pagination.pageIndex, pageSize: pagination.pageSize });
            this.dispatch('column-visibility-change', { visibility: this.state.columnVisibility });
            this.dispatch('selection-change', { selectedKeys: selectedKeys });
            this.dispatchState();
        },

        renderBody() {
            const tbody = this.$el.querySelector('tbody');
            if (!tbody) {
                return;
            }

            const rows = this.table.getRowModel().rows;
            tbody.innerHTML = '';

            if (rows.length === 0) {
                return;
            }

            const visibleColumns = this.table.getVisibleLeafColumns();
            for (const row of rows) {
                const tr = document.createElement('tr');
                tr.dataset.rowKey = row.id;
                tr.addEventListener('click', this.onRowClick.bind(this));

                if (this.config.selectionMode === 'multiple') {
                    const selectCell = document.createElement('td');
                    const checkbox = document.createElement('input');
                    checkbox.type = 'checkbox';
                    checkbox.checked = row.getIsSelected();
                    checkbox.dataset.rowKey = row.id;
                    checkbox.addEventListener('click', (e) => e.stopPropagation());
                    checkbox.addEventListener('change', this.onRowCheckboxChange.bind(this));
                    selectCell.appendChild(checkbox);
                    tr.appendChild(selectCell);
                }

                for (const column of visibleColumns) {
                    const td = document.createElement('td');
                    const cell = row.getAllCells().find((item) => item.column.id === column.id);
                    if (!cell) {
                        continue;
                    }

                    const template = this.$el.querySelector(`template[data-rz-datatable-template='${column.id}']`);
                    if (template) {
                        const fragment = template.content.cloneNode(true);
                        td.appendChild(fragment);
                        if (window.Alpine) {
                            if (typeof window.Alpine.addScopeToNode === 'function') {
                                window.Alpine.addScopeToNode(td, { row: row.original, column: column.columnDef.meta, cell: { value: cell.getValue(), key: row.id, columnId: column.id } });
                            }
                            if (typeof window.Alpine.initTree === 'function') {
                                window.Alpine.initTree(td);
                            }
                        }
                    } else {
                        const value = cell.getValue();
                        const nullDisplay = column.columnDef.meta?.nullDisplayText ?? '';
                        td.textContent = value == null ? nullDisplay : String(value);
                    }

                    tr.appendChild(td);
                }

                tbody.appendChild(tr);
            }

            this.syncHeaderAriaSort();
        },

        syncHeaderAriaSort() {
            const headers = this.$el.querySelectorAll('th[data-column-id]');
            for (const header of headers) {
                const id = header.dataset.columnId;
                if (!id) {
                    continue;
                }

                header.setAttribute('aria-sort', this.ariaSort(id));
            }
        },

        sortGlyph(columnId) {
            const sort = this.table.getState().sorting.find((item) => item.id === columnId);
            if (!sort) {
                return '↕';
            }

            return sort.desc ? '↓' : '↑';
        },

        ariaSort(columnId) {
            const sort = this.table.getState().sorting.find((item) => item.id === columnId);
            if (!sort) {
                return 'none';
            }

            return sort.desc ? 'descending' : 'ascending';
        },

        toggleColumnMenu() { this.columnMenuOpen = !this.columnMenuOpen; },
        closeColumnMenu() { this.columnMenuOpen = false; },

        onSortClick(event) {
            const columnId = event.currentTarget.dataset.columnId;
            if (!columnId) {
                return;
            }

            this.table.getColumn(columnId)?.toggleSorting();
            this.refresh();
        },

        onSearchInput(event) {
            this.search = event.target.value;
        },

        onColumnVisibilityToggle(event) {
            const columnId = event.target.dataset.columnId;
            if (!columnId) {
                return;
            }

            this.table.getColumn(columnId)?.toggleVisibility(!!event.target.checked);
            this.refresh();
        },

        isColumnVisible(columnId) {
            return this.table.getColumn(columnId)?.getIsVisible() ?? false;
        },

        onPageSizeChange(event) {
            const value = Number.parseInt(event.target.value, 10);
            this.table.setPageSize(value);
            this.refresh();
        },

        nextPage() {
            this.table.nextPage();
            this.refresh();
        },

        prevPage() {
            this.table.previousPage();
            this.refresh();
        },

        onRowClick(event) {
            const key = event.currentTarget.dataset.rowKey;
            if (!key) {
                return;
            }

            if (this.config.selectionMode === 'single') {
                this.state.rowSelection = { [key]: true };
                this.table.setOptions((previous) => ({ ...previous, state: this.state }));
                this.dispatch('selection-change', { selectedKeys: this.getSelectedKeys() });
            }

            this.dispatch('row-click', { key });
            this.dispatchState();
            this.refresh();
        },

        onRowCheckboxChange(event) {
            const key = event.target.dataset.rowKey;
            if (!key) {
                return;
            }

            this.table.getRow(key)?.toggleSelected(!!event.target.checked);
            this.refresh();
        },

        onSelectAllPageRows(event) {
            this.table.toggleAllPageRowsSelected(!!event.target.checked);
            this.refresh();
        },

        isSelected(key) {
            return this.table.getState().rowSelection[key] === true;
        },

        toSelectionMap(keys) {
            return Object.fromEntries((keys || []).map((key) => [String(key), true]));
        },

        getSelectedKeys() {
            const selected = new Set(Object.keys(this.table.getState().rowSelection || {}).filter((key) => this.table.getState().rowSelection[key]));
            return this.table.getRowModel().rows.map((row) => row.id).filter((id) => selected.has(id));
        },

        dispatch(eventName, detail) {
            this.$dispatch(`rz:datatable:${eventName}`, { id: this.config.id, ...detail });
        },

        dispatchState() {
            this.dispatch('state-change', {
                sorting: this.state.sorting,
                pagination: this.state.pagination,
                globalFilter: this.state.globalFilter,
                columnVisibility: this.state.columnVisibility,
                selectedKeys: this.getSelectedKeys(),
            });
        },
    };
}
