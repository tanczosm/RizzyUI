import {
    createTable,
    getCoreRowModel,
    getFilteredRowModel,
    getSortedRowModel,
    getPaginationRowModel,
    functionalUpdate,
} from '@tanstack/table-core';
import { flex } from '../directives/x-flexrender.js';

function readJsonConfig(rootEl) {
    const configId = rootEl?.dataset?.configId;
    if (!configId) {
        throw new Error('[RizzyUI] rzDataTable requires data-config-id on the alpine host.');
    }

    const scriptEl = document.getElementById(configId);
    if (!scriptEl) {
        throw new Error(`[RizzyUI] rzDataTable config script '${configId}' was not found.`);
    }

    return JSON.parse(scriptEl.textContent || '{}');
}

function getSegmentValue(source, segment) {
    if (source === null || source === undefined) {
        return undefined;
    }

    if (typeof source !== 'object') {
        return undefined;
    }

    if (Object.prototype.hasOwnProperty.call(source, segment)) {
        return source[segment];
    }

    const camelCaseSegment = segment.length > 0
        ? segment.charAt(0).toLowerCase() + segment.slice(1)
        : segment;

    if (Object.prototype.hasOwnProperty.call(source, camelCaseSegment)) {
        return source[camelCaseSegment];
    }

    return undefined;
}

function getByPath(source, path) {
    if (!path) return undefined;

    const segments = path.split('.');
    let current = source;

    for (const segment of segments) {
        current = getSegmentValue(current, segment);
        if (current === undefined) {
            return undefined;
        }
    }

    return current;
}

function normalizeColumns(columns) {
    return (columns || []).map(column => {
        const normalized = {
            ...column,
        };

        if (Array.isArray(column.columns) && column.columns.length > 0) {
            normalized.columns = normalizeColumns(column.columns);
        }

        if (column.accessorKey) {
            normalized.accessorFn = row => getByPath(row, column.accessorKey);
        }

        if (column.cell) {
            const rendererKey = column.cell;
            normalized.cell = cellContext => {
                if (rendererKey === 'rowSelection') {
                    const row = cellContext.row;
                    const isChecked = row.getIsSelected();
                    return flex.html(`<input type="checkbox" aria-label="Select row ${row.id}" ${isChecked ? 'checked' : ''} />`);
                }

                return flex.text(cellContext.getValue?.() ?? '');
            };
        } else if (!normalized.cell) {
            normalized.cell = cellContext => flex.text(cellContext.getValue?.() ?? '');
        }

        if (typeof normalized.header === 'string') {
            const headerText = normalized.header;
            normalized.header = () => flex.text(headerText);
        }

        if (typeof normalized.footer === 'string') {
            const footerText = normalized.footer;
            normalized.footer = () => flex.text(footerText);
        }

        return normalized;
    });
}

function buildRowModelGetters(pipeline) {
    const registry = {
        getCoreRowModel,
        getFilteredRowModel,
        getSortedRowModel,
        getPaginationRowModel,
    };

    const result = {};
    for (const key of [pipeline?.core, pipeline?.filtered, pipeline?.sorted, pipeline?.paginated]) {
        if (key && registry[key]) {
            result[key] = registry[key]();
        }
    }

    return result;
}


function normalizeInitialState(initialState) {
    return {
        sorting: initialState?.sorting || [],
        pagination: initialState?.pagination || { pageIndex: 0, pageSize: 10 },
        columnVisibility: initialState?.columnVisibility || {},
        columnFilters: initialState?.columnFilters || [],
        globalFilter: initialState?.globalFilter,
        rowSelection: initialState?.rowSelection || {},
        columnPinning: initialState?.columnPinning || { left: [], right: [] },
    };
}

function normalizeStatePayload(state, componentId, table) {
    return {
        componentId,
        sorting: state.sorting || [],
        pagination: state.pagination || null,
        globalFilter: state.globalFilter,
        rowSelection: state.rowSelection || {},
        columnVisibility: state.columnVisibility || {},
        selectedRowCount: table.getSelectedRowModel().rows.length,
    };
}

export default function rzDataTable() {
    return {
        table: null,
        hasRows: false,
        isEmpty: true,
        selectedRowCount: 0,
        flex,

        init() {
            const root = this.$el;
            const componentId = root?.dataset?.alpineRoot || this.$root?.id || root?.id || null;
            const transport = readJsonConfig(root);
            const columns = normalizeColumns(transport.columns);
            const rowModelGetters = buildRowModelGetters(transport.rowModelPipeline || {});

            const state = normalizeInitialState(transport.initialState);

            this.table = createTable({
                data: transport.data || [],
                columns,
                state,
                enableSorting: transport.options?.enableSorting,
                enableFilters: transport.options?.enableFilters,
                enableColumnFilters: transport.options?.enableColumnFilters,
                enableGlobalFilter: transport.options?.enableGlobalFilter,
                enableHiding: transport.options?.enableHiding,
                enableRowSelection: transport.options?.enableRowSelection,
                enableMultiRowSelection: transport.options?.enableMultiRowSelection,
                enableSortingRemoval: transport.options?.enableSortingRemoval,
                enableMultiSort: transport.options?.enableMultiSort,
                getRowId: originalRow => {
                    const value = getByPath(originalRow, transport.rowStructure?.rowIdPath);
                    return value === null || value === undefined ? '' : String(value);
                },
                onStateChange: updater => {
                    const nextState = functionalUpdate(updater, this.table.options.state);
                    this.table.setOptions(prev => ({
                        ...prev,
                        state: nextState,
                    }));
                    this.refreshDerivedState();
                    this.dispatchStateEvents(componentId);
                },
                ...rowModelGetters,
            });

            this.refreshDerivedState();
            this.dispatchEvent('rz:datatable:ready', {
                componentId,
            });
        },

        toggleColumnVisibility(id) {
            const column = this.table?.getColumn(id);
            if (!column) {
                return;
            }

            column.toggleVisibility();
            this.refreshDerivedState();
            this.dispatchStateEvents(this.$el?.dataset?.alpineRoot || this.$root?.id || null);
        },

        setGlobalFilter(value) {
            if (!this.table) {
                return;
            }

            this.table.setGlobalFilter(value);
            this.refreshDerivedState();
            this.dispatchStateEvents(this.$el?.dataset?.alpineRoot || this.$root?.id || null);
        },

        refreshDerivedState() {
            const rows = this.table?.getRowModel?.().rows || [];
            this.hasRows = rows.length > 0;
            this.isEmpty = !this.hasRows;
            this.selectedRowCount = this.table?.getSelectedRowModel?.().rows.length || 0;
        },

        dispatchStateEvents(componentId) {
            if (!this.table) {
                return;
            }

            const state = this.table.getState();
            const payload = normalizeStatePayload(state, componentId, this.table);

            this.dispatchEvent('rz:datatable:state-changed', payload);
            this.dispatchEvent('rz:datatable:selection-changed', {
                componentId,
                rowSelection: payload.rowSelection,
                selectedRowCount: payload.selectedRowCount,
            });
            this.dispatchEvent('rz:datatable:page-changed', {
                componentId,
                pagination: payload.pagination,
            });
            this.dispatchEvent('rz:datatable:sort-changed', {
                componentId,
                sorting: payload.sorting,
            });
            this.dispatchEvent('rz:datatable:filter-changed', {
                componentId,
                globalFilter: payload.globalFilter,
            });
            this.dispatchEvent('rz:datatable:column-visibility-changed', {
                componentId,
                columnVisibility: payload.columnVisibility,
            });
        },

        dispatchEvent(name, detail) {
            this.$el.dispatchEvent(new CustomEvent(name, {
                bubbles: true,
                detail,
            }));
        },
    };
}
