import {
    createTable,
    getCoreRowModel,
    getFilteredRowModel,
    getPaginationRowModel,
    getSortedRowModel,
} from '@tanstack/table-core';
import { createFlexRenderPlugin, flex } from '../directives/x-flexrender.js';

let flexPluginRegistered = false;

function getByPath(source, path) {
    if (!path) return undefined;

    return path
        .split('.')
        .filter(Boolean)
        .reduce((current, key) => (current == null ? undefined : current[key]), source);
}

function parseConfig(host) {
    const scriptId = host.dataset.configId;
    if (!scriptId) {
        throw new Error('[rzDataTable] Missing data-config-id on host.');
    }

    const script = document.getElementById(scriptId);
    if (!script) {
        throw new Error(`[rzDataTable] Could not find config script '${scriptId}'.`);
    }

    return JSON.parse(script.textContent || '{}');
}

function dispatch(host, eventName, detail) {
    host.dispatchEvent(
        new CustomEvent(eventName, {
            detail,
            bubbles: true,
        })
    );
}

export default function rzDataTable() {
    return {
        table: null,
        flex,
        hasRows: false,
        isEmpty: true,
        selectedRowCount: 0,

        init() {
            if (!flexPluginRegistered && window.Alpine) {
                window.Alpine.plugin(createFlexRenderPlugin());
                flexPluginRegistered = true;
            }

            const transport = parseConfig(this.$el);
            const root = this.$el;
            const componentId = root.dataset.alpineRoot || root.id || '';
            const tableFactory = createTable();
            const state = {
                ...(transport.initialState || {}),
            };

            const syncDerivedState = () => {
                const rowModel = this.table.getRowModel();
                const selectedModel = this.table.getSelectedRowModel();
                this.hasRows = (rowModel?.rows?.length || 0) > 0;
                this.isEmpty = !this.hasRows;
                this.selectedRowCount = selectedModel?.rows?.length || 0;
            };

            const emitAggregate = () => {
                const tableState = this.table.getState();
                const detail = {
                    componentId,
                    sorting: tableState.sorting || [],
                    pagination: tableState.pagination || null,
                    globalFilter: tableState.globalFilter,
                    rowSelection: tableState.rowSelection || {},
                    columnVisibility: tableState.columnVisibility || {},
                    selectedRowCount: this.selectedRowCount,
                };

                dispatch(root, 'rz:datatable:state-changed', detail);
            };

            const makeStateHandler = (key, eventName) => {
                return (nextValue) => {
                    state[key] = typeof nextValue === 'function' ? nextValue(state[key]) : nextValue;
                    this.table.setOptions((previous) => ({
                        ...previous,
                        state,
                    }));
                    syncDerivedState();
                    emitAggregate();
                    dispatch(root, eventName, {
                        componentId,
                        [key]: state[key],
                        selectedRowCount: this.selectedRowCount,
                    });
                };
            };

            this.table = tableFactory.createInstance({
                data: transport.data || [],
                columns: transport.columns || [],
                state,
                getCoreRowModel,
                getFilteredRowModel: transport.rowModelPipeline?.filtered ? getFilteredRowModel() : undefined,
                getSortedRowModel: transport.rowModelPipeline?.sorted ? getSortedRowModel() : undefined,
                getPaginationRowModel: transport.rowModelPipeline?.paginated ? getPaginationRowModel() : undefined,
                getRowId: (row) => {
                    const value = getByPath(row, transport.rowStructure?.rowIdPath);
                    return value == null ? '' : String(value);
                },
                onSortingChange: makeStateHandler('sorting', 'rz:datatable:sort-changed'),
                onPaginationChange: makeStateHandler('pagination', 'rz:datatable:page-changed'),
                onGlobalFilterChange: makeStateHandler('globalFilter', 'rz:datatable:filter-changed'),
                onColumnFiltersChange: makeStateHandler('columnFilters', 'rz:datatable:filter-changed'),
                onRowSelectionChange: makeStateHandler('rowSelection', 'rz:datatable:selection-changed'),
                onColumnVisibilityChange: makeStateHandler('columnVisibility', 'rz:datatable:column-visibility-changed'),
                ...transport.options,
            });

            syncDerivedState();
            dispatch(root, 'rz:datatable:ready', {
                componentId,
                selectedRowCount: this.selectedRowCount,
            });
            emitAggregate();
        },

        toggleColumnVisibility(id) {
            const column = this.table?.getColumn(id);
            if (!column) {
                return;
            }

            column.toggleVisibility();
            const tableState = this.table.getState();
            this.selectedRowCount = this.table.getSelectedRowModel().rows.length;
            this.hasRows = this.table.getRowModel().rows.length > 0;
            this.isEmpty = !this.hasRows;

            dispatch(this.$el, 'rz:datatable:column-visibility-changed', {
                componentId: this.$el.dataset.alpineRoot || this.$el.id || '',
                columnVisibility: tableState.columnVisibility || {},
                selectedRowCount: this.selectedRowCount,
            });
        },

        setGlobalFilter(value) {
            if (!this.table) {
                return;
            }

            this.table.setGlobalFilter(value);
        },
    };
}
