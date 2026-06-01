import assert from 'node:assert/strict';
import test from 'node:test';
import rzDataTable from '../rzDataTable.js';
import { componentBundleManifest } from '../../../runtime/componentBundleManifest.js';

class HostElement {
  constructor(config) {
    this.dataset = { configId: 'datatable-config', alpineRoot: 'orders-table' };
    this.listeners = new Map();
    this.events = [];
    this.config = config;
  }

  addEventListener(type, listener) {
    this.listeners.set(type, listener);
  }

  removeEventListener(type, listener) {
    if (this.listeners.get(type) === listener) {
      this.listeners.delete(type);
    }
  }

  dispatchEvent(event) {
    this.events.push(event);
    return true;
  }
}

function withDom(config, callback) {
  const previousDocument = globalThis.document;
  const previousCustomEvent = globalThis.CustomEvent;
  const host = new HostElement(config);

  globalThis.document = {
    getElementById(id) {
      if (id !== 'datatable-config') return null;

      return {
        textContent: JSON.stringify(config),
      };
    },
  };
  globalThis.CustomEvent = class CustomEvent {
    constructor(type, options = {}) {
      this.type = type;
      this.detail = options.detail;
      this.bubbles = options.bubbles;
    }
  };

  try {
    return callback(host);
  } finally {
    if (previousDocument === undefined) {
      delete globalThis.document;
    } else {
      globalThis.document = previousDocument;
    }

    if (previousCustomEvent === undefined) {
      delete globalThis.CustomEvent;
    } else {
      globalThis.CustomEvent = previousCustomEvent;
    }
  }
}

function createConfig(overrides = {}) {
  return {
    data: [
      { id: 'a&<"', name: 'Ada', email: 'ada@example.com' },
      { id: 'b', name: 'Grace', email: 'grace@example.com' },
      { id: 'c', name: 'Linus', email: 'linus@example.com' },
    ],
    columns: [
      { id: 'select', cell: 'rowSelection', enableSorting: false },
      { id: 'name', accessorKey: 'name', header: 'Name' },
      { id: 'email', accessorKey: 'email', header: 'Email' },
    ],
    initialState: {
      pagination: { pageIndex: 0, pageSize: 2 },
      sorting: [],
      columnVisibility: {},
      columnFilters: [],
      rowSelection: {},
    },
    options: {
      enableSorting: true,
      enableFilters: true,
      enableColumnFilters: true,
      enableGlobalFilter: true,
      enableHiding: true,
      enableRowSelection: true,
      enableMultiRowSelection: true,
      enableSortingRemoval: true,
      enableMultiSort: true,
    },
    rowModelPipeline: {
      core: 'getCoreRowModel',
      filtered: 'getFilteredRowModel',
      sorted: 'getSortedRowModel',
      paginated: 'getPaginationRowModel',
    },
    rowStructure: {
      rowIdPath: 'id',
    },
    ...overrides,
  };
}

function mount(config = createConfig()) {
  return withDom(config, host => {
    const component = rzDataTable();
    component.$el = host;
    component.$root = host;
    component.init();

    return { component, host };
  });
}

test('DataTable runtime remains owned by the lazy table bundle', () => {
  assert.equal(componentBundleManifest.rzDataTable, 'table-runtime');
});

test('generated row-selection cell has escaped row-specific accessible checkbox state', () => {
  const { component } = mount();
  const cell = component.rows[0].cells[0];
  const renderable = cell.column.columnDef.cell(cell.getContext());

  assert.equal(renderable.kind, 'html');
  assert.match(renderable.value, /type="checkbox"/);
  assert.match(renderable.value, /data-rz-datatable-row-select/);
  assert.match(renderable.value, /data-row-id="a&amp;&lt;&quot;"/);
  assert.match(renderable.value, /aria-label="Select row a&amp;&lt;&quot;"/);
  assert.match(renderable.value, /aria-checked="false"/);
});

test('sort changes dispatch aggregate, granular, and polite announcement events without unrelated granular noise', () => {
  const { component, host } = mount();
  const sortableHeader = component.headerGroups[0].headers.find(header => header.column.id === 'name');

  host.events.length = 0;
  component.sort.toggle(sortableHeader);

  const eventTypes = host.events.map(event => event.type);
  assert.deepEqual(eventTypes, [
    'rz:datatable:state-changed',
    'rz:datatable:sort-changed',
    'rz:datatable:announcement',
  ]);
  assert.deepEqual(host.events[0].detail.sorting, [{ id: 'name', desc: false }]);
  assert.deepEqual(host.events[1].detail.sorting, [{ id: 'name', desc: false }]);
  assert.equal(host.events[2].detail.reason, 'sorting');
  assert.equal(host.events[2].detail.politeness, 'polite');
  assert.equal(component.announcement.message, 'Sorted by name ascending.');
  assert.equal(component.sort.ariaSort(sortableHeader), 'ascending');
});

test('delegated generated selection checkbox updates row state with stable serializable payloads', () => {
  const { component, host } = mount();
  const changeListener = host.listeners.get('change');
  assert.equal(typeof changeListener, 'function');

  host.events.length = 0;
  changeListener({
    target: {
      checked: true,
      dataset: { rowId: 'b' },
      matches(selector) { return selector === 'input[data-rz-datatable-row-select][data-row-id]'; },
    },
  });

  const eventTypes = host.events.map(event => event.type);
  assert.deepEqual(eventTypes, [
    'rz:datatable:state-changed',
    'rz:datatable:selection-changed',
    'rz:datatable:announcement',
    'rz:datatable:row-selection-input-changed',
  ]);
  assert.deepEqual(host.events[1].detail.selectedRowIds, ['b']);
  assert.equal(host.events[1].detail.selectedRowCount, 1);
  assert.equal(host.events[2].detail.message, '1 row selected.');
  assert.equal(host.events[3].detail.rowId, 'b');
  assert.equal(host.events[3].detail.selected, true);
});

test('pagination, filter, and column visibility changes announce real state changes', () => {
  const { component, host } = mount();

  host.events.length = 0;
  component.pagination.nextPage();
  assert.equal(host.events.find(event => event.type === 'rz:datatable:page-changed').detail.pagination.pageIndex, 1);
  assert.equal(host.events.find(event => event.type === 'rz:datatable:announcement').detail.reason, 'pagination');

  host.events.length = 0;
  component.filter.setGlobalFilter('Ada');
  assert.equal(host.events.find(event => event.type === 'rz:datatable:filter-changed').detail.globalFilter, 'Ada');
  assert.equal(host.events.find(event => event.type === 'rz:datatable:announcement').detail.reason, 'filter');

  host.events.length = 0;
  component.toggleColumnVisibility('email');
  const visibilityEvent = host.events.find(event => event.type === 'rz:datatable:column-visibility-changed');
  assert.equal(visibilityEvent.detail.columnVisibility.email, false);
  assert.deepEqual(visibilityEvent.detail.visibleColumnIds, ['select', 'name']);
  assert.equal(host.events.find(event => event.type === 'rz:datatable:announcement').detail.reason, 'columnVisibility');
});

test('row id validation preserves duplicate id rejection in the runtime', () => {
  assert.throws(() => mount(createConfig({
    data: [
      { id: 'duplicate', name: 'Ada' },
      { id: 'duplicate', name: 'Grace' },
    ],
  })), /duplicate row id 'duplicate'/);
});
