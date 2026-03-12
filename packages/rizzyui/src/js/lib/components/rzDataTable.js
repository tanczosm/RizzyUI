import {
    createTable,
    getCoreRowModel,
    getFilteredRowModel,
    getPaginationRowModel,
    getSortedRowModel,
} from '@tanstack/table-core';

export default function rzDataTable() {
    return {
        config: null,
        table: null,
        searchValue: '',
        columnMenuOpen: false,
        _selectionSet: new Set(),

        init() {
            this.config = this.readConfig();
            if (!this.config || !Array.isArray(this.config.columns)) return;

            this.searchValue = this.config.initialState?.globalFilter || '';
            this._selectionSet = new Set(this.config.initialState?.selectedKeys || []);

            this.initializeTable();
            this.syncUi();
            this.renderBody();
            this.emit('ready', { id: this.config.id });
            this.emitState();
        },

        readConfig() {
            const scriptId = this.$el.dataset.configScriptId;
            if (!scriptId) return null;
            const node = document.getElementById(scriptId);
            if (!node) return null;
            try { return JSON.parse(node.textContent || '{}'); } catch { return null; }
        },

        initializeTable() {
            const searchable = new Set(this.config.columns.filter(c => c.searchable).map(c => c.field));
            const selectionMode = this.config.selectionMode;
            const columns = this.config.columns.map((column) => ({
                id: column.id,
                accessorKey: column.field,
                enableSorting: column.sortable,
                enableHiding: column.canHide,
            }));

            const state = {
                sorting: this.config.initialState?.sorting || [],
                globalFilter: this.searchValue,
                pagination: this.config.initialState?.pagination || { pageIndex: 0, pageSize: 10 },
                columnVisibility: this.config.initialState?.columnVisibility || {},
                rowSelection: {},
            };

            for (const key of this._selectionSet) {
                state.rowSelection[key] = true;
            }

            const updateSelectionSet = (rowSelection) => {
                this._selectionSet = new Set(Object.keys(rowSelection).filter(key => rowSelection[key]));
            };

            this.table = createTable({
                data: this.config.rows || [],
                columns,
                state,
                getRowId: (row) => row.__key,
                getCoreRowModel: getCoreRowModel(),
                getFilteredRowModel: getFilteredRowModel(),
                getSortedRowModel: getSortedRowModel(),
                getPaginationRowModel: getPaginationRowModel(),
                globalFilterFn: (row, _columnId, filterValue) => {
                    if (!filterValue) return true;
                    const search = String(filterValue).toLowerCase();
                    for (const field of searchable) {
                        const value = row.original[field];
                        if (value === null || value === undefined) continue;
                        if (String(value).toLowerCase().includes(search)) return true;
                    }
                    return false;
                },
                onSortingChange: (updater) => {
                    state.sorting = this.resolveUpdater(updater, state.sorting);
                    this.table.setOptions(prev => ({ ...prev, state }));
                    this.afterStateChange('sort-change', { id: this.config.id, sorting: state.sorting });
                },
                onGlobalFilterChange: (updater) => {
                    state.globalFilter = this.resolveUpdater(updater, state.globalFilter);
                    this.searchValue = state.globalFilter || '';
                    this.table.setOptions(prev => ({ ...prev, state }));
                    this.afterStateChange('search-change', { id: this.config.id, search: this.searchValue });
                },
                onPaginationChange: (updater) => {
                    state.pagination = this.resolveUpdater(updater, state.pagination);
                    this.table.setOptions(prev => ({ ...prev, state }));
                    this.afterStateChange('page-change', { id: this.config.id, pageIndex: state.pagination.pageIndex, pageSize: state.pagination.pageSize });
                },
                onColumnVisibilityChange: (updater) => {
                    state.columnVisibility = this.resolveUpdater(updater, state.columnVisibility);
                    this.table.setOptions(prev => ({ ...prev, state }));
                    this.afterStateChange('column-visibility-change', { id: this.config.id, visibility: state.columnVisibility });
                },
                onRowSelectionChange: (updater) => {
                    state.rowSelection = this.resolveUpdater(updater, state.rowSelection);
                    if (selectionMode === 'single') {
                        const selected = Object.keys(state.rowSelection).filter(key => state.rowSelection[key]);
                        const keep = selected[selected.length - 1];
                        state.rowSelection = keep ? { [keep]: true } : {};
                    }
                    updateSelectionSet(state.rowSelection);
                    this.table.setOptions(prev => ({ ...prev, state }));
                    this.afterStateChange('selection-change', { id: this.config.id, selectedKeys: this.getSelectedKeysInRowOrder() });
                },
            });
        },

        resolveUpdater(updater, currentValue) {
            return typeof updater === 'function' ? updater(currentValue) : updater;
        },

        onSearchInput() {
            const debounceMs = Number(this.config.searchDebounceMs || 0);
            clearTimeout(this._searchTimer);
            this._searchTimer = setTimeout(() => {
                this.table.setGlobalFilter(this.searchValue || '');
            }, debounceMs);
        },

        onSortButtonClick(event) {
            const columnId = event.currentTarget.dataset.sortButton;
            const column = this.table.getColumn(columnId);
            if (!column) return;
            column.toggleSorting(column.getIsSorted() === 'asc');
        },

        onPrevPage() {
            this.table.previousPage();
        },

        onNextPage() {
            this.table.nextPage();
        },

        onPageSizeChange(event) {
            const size = Number(event.target.value);
            if (Number.isFinite(size) && size > 0) {
                this.table.setPageSize(size);
            }
        },

        onColumnVisibilityChange(event) {
            const columnId = event.target.dataset.columnToggle;
            this.table.getColumn(columnId)?.toggleVisibility(event.target.checked);
        },

        toggleColumnMenu() {
            this.columnMenuOpen = !this.columnMenuOpen;
        },

        closeColumnMenu() {
            this.columnMenuOpen = false;
        },

        onToggleSelectAll(event) {
            event.stopPropagation();
            const checked = event.target.checked;
            this.table.toggleAllPageRowsSelected(checked);
        },

        onRowCheckboxClick(event) {
            event.stopPropagation();
            const key = event.target.dataset.rowSelect;
            const row = this.table.getRow(key);
            row?.toggleSelected(event.target.checked);
        },

        onRowClick(event) {
            const key = event.currentTarget.dataset.rowKey;
            if (!key) return;
            if (this.config.selectionMode === 'single') {
                const row = this.table.getRow(key);
                if (row) {
                    this.table.resetRowSelection();
                    row.toggleSelected(true);
                }
            }
            this.emit('row-click', { id: this.config.id, key });
        },

        afterStateChange(eventName, detail) {
            this.syncUi();
            this.renderBody();
            this.emit(eventName, detail);
            this.emitState();
        },

        emitState() {
            this.emit('state-change', {
                id: this.config.id,
                sorting: this.table.getState().sorting,
                pagination: this.table.getState().pagination,
                globalFilter: this.table.getState().globalFilter || '',
                columnVisibility: this.table.getState().columnVisibility,
                selectedKeys: this.getSelectedKeysInRowOrder(),
            });
        },

        emit(name, detail) {
            this.$el.dispatchEvent(new CustomEvent(`rz:datatable:${name}`, { bubbles: true, detail }));
        },

        getSelectedKeysInRowOrder() {
            const selected = this._selectionSet;
            return this.table.getRowModel().rows.map(r => r.id).filter(id => selected.has(id));
        },

        syncUi() {
            this.syncSortHeaders();
            this.syncColumnToggles();
            this.syncPagination();
            this.syncColumnMenuAria();
        },

        syncColumnMenuAria() {
            if (!this.$refs.columnMenuButton) return;
            this.$refs.columnMenuButton.setAttribute('aria-expanded', this.columnMenuOpen ? 'true' : 'false');
        },

        syncSortHeaders() {
            for (const columnDef of this.config.columns) {
                const col = this.table.getColumn(columnDef.id);
                const header = this.$el.querySelector(`[data-column-header="${columnDef.id}"]`);
                const indicator = this.$el.querySelector(`[data-sort-indicator="${columnDef.id}"]`);
                if (!header || !col) continue;

                const sorted = col.getIsSorted();
                const ariaSort = sorted === 'asc' ? 'ascending' : sorted === 'desc' ? 'descending' : 'none';
                header.setAttribute('aria-sort', ariaSort);
                if (indicator) {
                    indicator.textContent = sorted === 'asc' ? '↑' : sorted === 'desc' ? '↓' : '↕';
                }
            }
        },

        syncColumnToggles() {
            for (const columnDef of this.config.columns) {
                const toggle = this.$el.querySelector(`[data-column-toggle="${columnDef.id}"]`);
                const col = this.table.getColumn(columnDef.id);
                if (toggle && col) {
                    toggle.checked = col.getIsVisible();
                }
            }
        },

        syncPagination() {
            const state = this.table.getState().pagination;
            const pageCount = this.table.getPageCount();
            if (this.$refs.pageSummary) {
                this.$refs.pageSummary.textContent = `Page ${state.pageIndex + 1} of ${Math.max(pageCount, 1)}`;
            }
            if (this.$refs.prevPage) {
                this.$refs.prevPage.disabled = !this.table.getCanPreviousPage();
            }
            if (this.$refs.nextPage) {
                this.$refs.nextPage.disabled = !this.table.getCanNextPage();
            }
            if (this.$refs.pageSize) {
                this.$refs.pageSize.value = String(state.pageSize);
            }
            if (this.$refs.selectAllCheckbox && this.config.selectionMode === 'multiple') {
                this.$refs.selectAllCheckbox.checked = this.table.getIsAllPageRowsSelected();
                this.$refs.selectAllCheckbox.indeterminate = this.table.getIsSomePageRowsSelected() && !this.table.getIsAllPageRowsSelected();
            }
        },

        renderBody() {
            const tbody = this.$refs.tbody;
            if (!tbody) return;
            tbody.innerHTML = '';

            const rows = this.table.getPaginationRowModel().rows;
            const visibleColumns = this.config.columns.filter(col => this.table.getColumn(col.id)?.getIsVisible());
            if (rows.length === 0) {
                const tr = document.createElement('tr');
                tr.className = 'border-b';
                const td = document.createElement('td');
                td.colSpan = visibleColumns.length + (this.config.selectionMode === 'multiple' ? 1 : 0);
                td.className = 'h-24 text-center text-sm text-muted-foreground';
                td.textContent = this.config.initialState?.globalFilter || this.searchValue ? 'No results.' : 'No rows.';
                tr.appendChild(td);
                tbody.appendChild(tr);
                return;
            }

            for (const row of rows) {
                const tr = document.createElement('tr');
                tr.className = 'border-b';
                tr.dataset.rowKey = row.id;
                tr.addEventListener('click', this.onRowClick.bind(this));

                if (this.config.selectionMode === 'multiple') {
                    const selectCell = document.createElement('td');
                    selectCell.className = 'p-2 text-center align-middle';
                    const cb = document.createElement('input');
                    cb.type = 'checkbox';
                    cb.dataset.rowSelect = row.id;
                    cb.checked = row.getIsSelected();
                    cb.setAttribute('aria-label', 'Select row');
                    cb.addEventListener('click', this.onRowCheckboxClick.bind(this));
                    selectCell.appendChild(cb);
                    tr.appendChild(selectCell);
                }

                for (const col of visibleColumns) {
                    const td = document.createElement('td');
                    td.className = `p-2 align-middle ${col.cellClass || ''}`.trim();
                    const cell = row.getAllCells().find(c => c.column.id === col.id);
                    const value = cell?.getValue();
                    if (col.hasTemplate && col.templateId) {
                        const templateNode = document.getElementById(col.templateId);
                        if (templateNode) {
                            const fragment = templateNode.content.cloneNode(true);
                            const host = document.createElement('div');
                            host.appendChild(fragment);
                            const scope = {
                                row: row.original,
                                column: col,
                                cell: { value, key: row.id, columnId: col.id }
                            };
                            this.applyTemplateBindings(host, scope);
                            while (host.firstChild) {
                                td.appendChild(host.firstChild);
                            }
                        }
                    } else {
                        td.textContent = value === null || value === undefined ? (col.nullDisplayText || '') : String(value);
                    }
                    tr.appendChild(td);
                }

                tbody.appendChild(tr);
            }
        },

        applyTemplateBindings(root, scope) {
            const textNodes = root.querySelectorAll('[x-text]');
            for (const node of textNodes) {
                const path = node.getAttribute('x-text');
                node.textContent = this.resolvePath(scope, path);
            }
            const classNodes = root.querySelectorAll('[:class], [x-bind\\:class]');
            for (const node of classNodes) {
                const path = node.getAttribute(':class') || node.getAttribute('x-bind:class');
                const className = this.resolvePath(scope, path);
                if (typeof className === 'string' && className.length > 0) {
                    node.classList.add(...className.split(' ').filter(Boolean));
                }
            }
        },

        resolvePath(scope, path) {
            if (!path) return '';
            const keys = path.split('.');
            let current = scope;
            for (const key of keys) {
                if (current === null || current === undefined) return '';
                current = current[key];
            }
            return current ?? '';
        }
    };
}
