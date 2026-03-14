import {
    createTable,
    getCoreRowModel,
    getFilteredRowModel,
    getPaginationRowModel,
    getSortedRowModel,
} from '@tanstack/table-core';

function resolveValue(updater, currentValue) {
    return typeof updater === 'function' ? updater(currentValue) : updater;
}

function toDisplayText(value, nullDisplayText) {
    if (value === null || value === undefined || value === '') {
        return nullDisplayText || '';
    }
    return String(value);
}

export default function rzDataTable() {
    return {
        config: null,
        table: null,
        columnMenuOpen: false,
        searchDebounceTimer: null,
        state: {
            sorting: [],
            globalFilter: '',
            pagination: { pageIndex: 0, pageSize: 10 },
            columnVisibility: {},
            rowSelection: {},
        },

        init() {
            this.readConfig();
            this.initializeState();
            this.initializeTable();
            this.renderTable();
            this.dispatchReady();
            this.dispatchStateChange();
        },

        readConfig() {
            const configId = this.$el.dataset.configId;
            const configScript = document.getElementById(configId);
            this.config = JSON.parse(configScript?.textContent || '{}');
        },

        initializeState() {
            const initial = this.config.initialState || {};
            const selectedKeys = Array.isArray(initial.selectedKeys) ? initial.selectedKeys : [];
            const selectionMap = {};
            for (const key of selectedKeys) {
                selectionMap[key] = true;
            }

            this.state = {
                sorting: Array.isArray(initial.sorting) ? initial.sorting : [],
                globalFilter: initial.globalFilter || '',
                pagination: {
                    pageIndex: Number(initial.pagination?.pageIndex || 0),
                    pageSize: Number(initial.pagination?.pageSize || 10),
                },
                columnVisibility: initial.columnVisibility || {},
                rowSelection: selectionMap,
                columnPinning: {
                    left: Array.isArray(initial.columnPinning?.left) ? initial.columnPinning.left : [],
                    right: Array.isArray(initial.columnPinning?.right) ? initial.columnPinning.right : [],
                },
            };

            if (this.$refs.searchInput) {
                this.$refs.searchInput.value = this.state.globalFilter;
            }
            if (this.$refs.pageSizeSelect) {
                this.$refs.pageSizeSelect.value = String(this.state.pagination.pageSize);
            }
        },

        initializeTable() {
            const columns = this.config.columns.map((column) => ({
                id: column.id,
                accessorFn: (row) => row[column.field],
                enableSorting: column.sortable,
                enableHiding: column.canHide,
                meta: {
                    field: column.field,
                    align: column.align,
                    headerClass: column.headerClass,
                    cellClass: column.cellClass,
                    nullDisplayText: column.nullDisplayText,
                    hasTemplate: column.hasTemplate,
                },
            }));

            const searchableColumns = this.config.columns.filter(column => column.searchable).map(column => column.field);

            this.table = createTable({
                data: this.config.rows,
                columns,
                state: this.state,
                getRowId: (row) => row.__key,
                onSortingChange: (updater) => {
                    this.state.sorting = resolveValue(updater, this.state.sorting);
                    this.table.setOptions(prev => ({ ...prev, state: this.state }));
                    this.renderTable();
                    this.dispatchSortChange();
                    this.dispatchStateChange();
                },
                onGlobalFilterChange: (updater) => {
                    this.state.globalFilter = resolveValue(updater, this.state.globalFilter);
                    this.state.pagination.pageIndex = 0;
                    this.table.setOptions(prev => ({ ...prev, state: this.state }));
                    this.renderTable();
                    this.dispatchSearchChange();
                    this.dispatchPageChange();
                    this.dispatchStateChange();
                },
                onPaginationChange: (updater) => {
                    this.state.pagination = resolveValue(updater, this.state.pagination);
                    this.table.setOptions(prev => ({ ...prev, state: this.state }));
                    this.renderTable();
                    this.dispatchPageChange();
                    this.dispatchStateChange();
                },
                onColumnVisibilityChange: (updater) => {
                    this.state.columnVisibility = resolveValue(updater, this.state.columnVisibility);
                    this.table.setOptions(prev => ({ ...prev, state: this.state }));
                    this.renderTable();
                    this.dispatchColumnVisibilityChange();
                    this.dispatchStateChange();
                },
                globalFilterFn: (row, _, filterValue) => {
                    if (!filterValue) {
                        return true;
                    }

                    const needle = String(filterValue).toLowerCase();
                    for (const columnField of searchableColumns) {
                        const value = row.original[columnField];
                        if (value === null || value === undefined) {
                            continue;
                        }

                        if (String(value).toLowerCase().includes(needle)) {
                            return true;
                        }
                    }

                    return false;
                },
                getCoreRowModel: getCoreRowModel(),
                getSortedRowModel: getSortedRowModel(),
                getFilteredRowModel: getFilteredRowModel(),
                getPaginationRowModel: getPaginationRowModel(),
            });
        },

        handleSearchInput(event) {
            const debounceMs = Number(event.currentTarget.dataset.searchDebounceMs || this.config.searchDebounceMs || 300);
            const nextValue = event.currentTarget.value || '';

            if (this.searchDebounceTimer) {
                clearTimeout(this.searchDebounceTimer);
            }

            this.searchDebounceTimer = window.setTimeout(() => {
                this.table.setGlobalFilter(nextValue);
            }, debounceMs);
        },

        toggleSort(event) {
            const columnId = event.currentTarget.dataset.columnId;
            const column = this.table.getColumn(columnId);
            if (!column) {
                return;
            }

            const current = column.getIsSorted();
            if (current === false) {
                this.table.setSorting([{ id: columnId, desc: false }]);
                return;
            }
            if (current === 'asc') {
                this.table.setSorting([{ id: columnId, desc: true }]);
                return;
            }

            this.table.setSorting([]);
        },

        toggleColumnMenu() {
            this.columnMenuOpen = !this.columnMenuOpen;
        },

        closeColumnMenu() {
            this.columnMenuOpen = false;
        },

        toggleColumnVisibility(event) {
            const columnId = event.currentTarget.dataset.columnId;
            const checked = event.currentTarget.checked;
            this.table.setColumnVisibility({
                ...this.state.columnVisibility,
                [columnId]: checked,
            });
        },

        previousPage() {
            this.table.previousPage();
        },

        nextPage() {
            this.table.nextPage();
        },

        changePageSize(event) {
            const value = Number(event.currentTarget.value || this.state.pagination.pageSize);
            this.table.setPageSize(value);
            this.table.setPageIndex(0);
        },

        toggleSelectCurrentPage(event) {
            const checked = event.currentTarget.checked;
            const pageRows = this.table.getPaginationRowModel().rows;
            const nextSelection = { ...this.state.rowSelection };

            for (const row of pageRows) {
                if (checked) {
                    nextSelection[row.id] = true;
                } else {
                    delete nextSelection[row.id];
                }
            }

            this.state.rowSelection = nextSelection;
            this.renderTable();
            this.dispatchSelectionChange();
            this.dispatchStateChange();
        },

        handleRowClick(event) {
            const rowKey = event.currentTarget.dataset.rowKey;
            if (!rowKey) {
                return;
            }

            this.dispatchRowClick(rowKey);

            if (this.config.selectionMode === 'single') {
                this.state.rowSelection = { [rowKey]: true };
                this.renderTable();
                this.dispatchSelectionChange();
                this.dispatchStateChange();
            }
        },

        handleRowCheckboxChange(event) {
            const rowKey = event.currentTarget.dataset.rowKey;
            const checked = event.currentTarget.checked;
            if (!rowKey) {
                return;
            }

            const nextSelection = { ...this.state.rowSelection };
            if (checked) {
                nextSelection[rowKey] = true;
            } else {
                delete nextSelection[rowKey];
            }

            this.state.rowSelection = nextSelection;
            this.renderTable();
            this.dispatchSelectionChange();
            this.dispatchStateChange();
        },

        renderTable() {
            this.renderHeaderState();
            this.renderBody();
            this.renderPaginationState();
            this.syncColumnMenuCheckboxes();
            this.syncSelectAllCheckbox();
        },

        renderHeaderState() {
            const headers = this.$el.querySelectorAll('[data-column-id][aria-sort]');
            for (const header of headers) {
                const columnId = header.dataset.columnId;
                const column = this.table.getColumn(columnId);
                if (!column) {
                    continue;
                }

                const sorted = column.getIsSorted();
                const ariaSort = sorted === 'asc' ? 'ascending' : (sorted === 'desc' ? 'descending' : 'none');
                header.setAttribute('aria-sort', ariaSort);

                const indicator = this.$el.querySelector(`[data-sort-indicator="${columnId}"]`);
                if (indicator) {
                    indicator.textContent = sorted === 'asc' ? '↑' : (sorted === 'desc' ? '↓' : '');
                }

                const isVisible = column.getIsVisible();
                header.style.display = isVisible ? '' : 'none';
            }
        },

        renderBody() {
            const body = this.$refs.body;
            if (!body) {
                return;
            }

            body.innerHTML = '';

            const rows = this.config.enablePagination ? this.table.getPaginationRowModel().rows : this.table.getRowModel().rows;

            if (rows.length === 0) {
                body.appendChild(this.createEmptyRow());
                return;
            }

            for (const row of rows) {
                body.appendChild(this.createBodyRow(row));
            }
        },

        createEmptyRow() {
            const row = document.createElement('tr');
            row.className = 'border-b transition-colors';
            const cell = document.createElement('td');
            const visibleColumns = this.table.getVisibleLeafColumns().length + (this.config.selectionMode === 'multiple' ? 1 : 0);
            cell.colSpan = visibleColumns;
            cell.className = 'h-24 text-center text-muted-foreground';
            cell.textContent = this.$el.dataset.emptyText || 'No results.';
            row.appendChild(cell);
            return row;
        },

        createBodyRow(row) {
            const tr = document.createElement('tr');
            tr.className = 'border-b transition-colors';
            tr.dataset.rowKey = row.id;
            tr.addEventListener('click', this.handleRowClick.bind(this));

            if (this.config.selectionMode === 'multiple') {
                const selectionCell = document.createElement('td');
                selectionCell.className = 'px-4 py-2';
                const checkbox = document.createElement('input');
                checkbox.type = 'checkbox';
                checkbox.setAttribute('aria-label', this.$el.dataset.selectRowLabel || 'Select row');
                checkbox.dataset.rowKey = row.id;
                checkbox.checked = !!this.state.rowSelection[row.id];
                checkbox.addEventListener('click', (evt) => evt.stopPropagation());
                checkbox.addEventListener('change', this.handleRowCheckboxChange.bind(this));
                selectionCell.appendChild(checkbox);
                tr.appendChild(selectionCell);
            }

            for (const cellModel of row.getVisibleCells()) {
                tr.appendChild(this.createBodyCell(row, cellModel));
            }

            return tr;
        },

        createBodyCell(row, cellModel) {
            const td = document.createElement('td');
            const columnConfig = this.config.columns.find(c => c.id === cellModel.column.id);
            td.className = this.computeCellClass(columnConfig);

            if (columnConfig?.hasTemplate) {
                const templateNode = this.$el.querySelector(`[data-rz-datatable-template-for="${columnConfig.id}"]`);
                if (templateNode) {
                    const fragment = templateNode.content.cloneNode(true);
                    this.bindTemplateFragment(fragment, row, columnConfig, cellModel);
                    td.appendChild(fragment);
                    return td;
                }
            }

            td.textContent = toDisplayText(cellModel.getValue(), columnConfig?.nullDisplayText);
            return td;
        },

        bindTemplateFragment(fragment, row, column, cell) {
            const templateRoot = document.createElement('div');
            templateRoot.style.display = 'contents';
            templateRoot.dataset.row = JSON.stringify(row.original);
            templateRoot.dataset.column = JSON.stringify(column);
            templateRoot.dataset.cell = JSON.stringify({ id: cell.id, value: cell.getValue() });
            templateRoot.appendChild(fragment);
            this.applyTemplateBindings(templateRoot, row.original, column, { id: cell.id, value: cell.getValue() });
            while (templateRoot.firstChild) {
                fragment.appendChild(templateRoot.firstChild);
            }
        },

        applyTemplateBindings(root, row, column, cell) {
            const textNodes = root.querySelectorAll('[x-text]');
            for (const node of textNodes) {
                const binding = node.getAttribute('x-text');
                node.textContent = this.resolveTemplateExpression(binding, row, column, cell);
            }

            const classNodes = root.querySelectorAll('[:class], [x-bind\\:class]');
            for (const node of classNodes) {
                const binding = node.getAttribute(':class') || node.getAttribute('x-bind:class');
                const className = this.resolveTemplateExpression(binding, row, column, cell);
                if (className) {
                    node.classList.add(...String(className).split(' ').filter(Boolean));
                }
            }
        },

        resolveTemplateExpression(expression, row, column, cell) {
            if (!expression) {
                return '';
            }

            const parts = expression.split('.');
            if (parts.length < 2) {
                return '';
            }

            let source = null;
            if (parts[0] === 'row') {
                source = row;
            } else if (parts[0] === 'column') {
                source = column;
            } else if (parts[0] === 'cell') {
                source = cell;
            }

            if (!source) {
                return '';
            }

            let current = source;
            for (let i = 1; i < parts.length; i += 1) {
                if (current === null || current === undefined) {
                    return '';
                }
                current = current[parts[i]];
            }

            return current ?? '';
        },

        computeCellClass(columnConfig) {
            const alignClass = columnConfig?.align === 'center' ? 'text-center'
                : (columnConfig?.align === 'end' ? 'text-right' : 'text-left');
            const customClass = columnConfig?.cellClass || '';
            return ['px-4 py-2', alignClass, customClass].filter(Boolean).join(' ');
        },

        renderPaginationState() {
            if (!this.$refs.pageIndicator) {
                return;
            }

            const pageCount = this.table.getPageCount();
            const pageIndex = this.state.pagination.pageIndex;
            this.$refs.pageIndicator.textContent = pageCount > 0
                ? `Page ${pageIndex + 1} of ${pageCount}`
                : 'Page 0 of 0';
        },

        syncColumnMenuCheckboxes() {
            const checkboxes = this.$el.querySelectorAll('[data-column-id][type="checkbox"]');
            for (const checkbox of checkboxes) {
                if (checkbox === this.$refs.selectAllCheckbox || checkbox.dataset.rowKey) {
                    continue;
                }

                const columnId = checkbox.dataset.columnId;
                const column = this.table.getColumn(columnId);
                if (column) {
                    checkbox.checked = column.getIsVisible();
                }
            }
        },

        syncSelectAllCheckbox() {
            if (!this.$refs.selectAllCheckbox) {
                return;
            }

            const pageRows = this.table.getPaginationRowModel().rows;
            if (pageRows.length === 0) {
                this.$refs.selectAllCheckbox.checked = false;
                this.$refs.selectAllCheckbox.indeterminate = false;
                return;
            }

            const selectedCount = pageRows.filter(row => !!this.state.rowSelection[row.id]).length;
            this.$refs.selectAllCheckbox.checked = selectedCount === pageRows.length;
            this.$refs.selectAllCheckbox.indeterminate = selectedCount > 0 && selectedCount < pageRows.length;
        },

        getOrderedSelectedKeys() {
            return this.table.getRowModel().rows
                .map(row => row.id)
                .filter(key => !!this.state.rowSelection[key]);
        },

        dispatchEvent(name, detail) {
            this.$dispatch(`rz:datatable:${name}`, detail);
        },

        dispatchReady() {
            this.dispatchEvent('ready', { id: this.config.id });
        },

        dispatchSortChange() {
            this.dispatchEvent('sort-change', { id: this.config.id, sorting: this.state.sorting });
        },

        dispatchPageChange() {
            this.dispatchEvent('page-change', {
                id: this.config.id,
                pageIndex: this.state.pagination.pageIndex,
                pageSize: this.state.pagination.pageSize,
            });
        },

        dispatchSearchChange() {
            this.dispatchEvent('search-change', { id: this.config.id, search: this.state.globalFilter });
        },

        dispatchColumnVisibilityChange() {
            this.dispatchEvent('column-visibility-change', {
                id: this.config.id,
                visibility: this.state.columnVisibility,
            });
        },

        dispatchSelectionChange() {
            this.dispatchEvent('selection-change', {
                id: this.config.id,
                selectedKeys: this.getOrderedSelectedKeys(),
            });
        },

        dispatchRowClick(key) {
            this.dispatchEvent('row-click', { id: this.config.id, key });
        },

        dispatchStateChange() {
            this.dispatchEvent('state-change', {
                id: this.config.id,
                sorting: this.state.sorting,
                pagination: this.state.pagination,
                globalFilter: this.state.globalFilter,
                columnVisibility: this.state.columnVisibility,
                selectedKeys: this.getOrderedSelectedKeys(),
            });
        },
    };
}
