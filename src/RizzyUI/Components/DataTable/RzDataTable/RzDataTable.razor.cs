using System.Globalization;
using System.Linq.Expressions;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using RizzyUI.Extensions;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// SSR-first data table with progressive client enhancement powered by Alpine and TanStack Table.
/// </summary>
/// <typeparam name="TItem">The row item type.</typeparam>
public partial class RzDataTable<TItem> : RzComponent<RzDataTableSlots>, IHasRzDataTableStylingProperties
{
    [Inject] private IOptions<RizzyUIConfig> RizzyUIConfig { get; set; } = default!;

    private readonly List<DataTableColumnDefinition<TItem>> _columns = [];
    private bool _isCollectingColumns;
    private bool _payloadReady;
    private string _configJson = "{}";
    private string _assets = "[]";
    private List<Dictionary<string, object?>> _projectedRows = [];

    /// <summary>
    /// Gets or sets the source items.
    /// </summary>
    [Parameter, EditorRequired] public IEnumerable<TItem>? Items { get; set; }

    /// <summary>
    /// Gets or sets a required stable row key selector.
    /// </summary>
    [Parameter, EditorRequired] public Expression<Func<TItem, object>>? RowKey { get; set; }

    /// <summary>
    /// Gets or sets whether the toolbar row is rendered.
    /// </summary>
    [Parameter] public bool ShowToolbar { get; set; } = true;

    /// <summary>
    /// Gets or sets whether global search is rendered.
    /// </summary>
    [Parameter] public bool Search { get; set; }

    /// <summary>
    /// Gets or sets search placeholder text.
    /// </summary>
    [Parameter] public string? SearchPlaceholder { get; set; }

    /// <summary>
    /// Gets or sets global search debounce delay.
    /// </summary>
    [Parameter] public int SearchDebounceMs { get; set; } = 300;

    /// <summary>
    /// Gets or sets whether the column visibility menu is rendered.
    /// </summary>
    [Parameter] public bool ShowColumnVisibility { get; set; } = true;

    /// <summary>
    /// Gets or sets whether pagination controls are rendered.
    /// </summary>
    [Parameter] public bool EnablePagination { get; set; } = true;

    /// <summary>
    /// Gets or sets the initial page index.
    /// </summary>
    [Parameter] public int InitialPageIndex { get; set; }

    /// <summary>
    /// Gets or sets the current page size.
    /// </summary>
    [Parameter] public int PageSize { get; set; } = 10;

    /// <summary>
    /// Gets or sets page-size options.
    /// </summary>
    [Parameter] public IReadOnlyList<int> PageSizeOptions { get; set; } = [10, 25, 50];

    /// <summary>
    /// Gets or sets initial global search text.
    /// </summary>
    [Parameter] public string? InitialSearch { get; set; }

    /// <summary>
    /// Gets or sets initial sorting instructions.
    /// </summary>
    [Parameter] public IReadOnlyList<DataTableSortDefinition> InitialSorting { get; set; } = [];

    /// <summary>
    /// Gets or sets initial column visibility state by column id.
    /// </summary>
    [Parameter] public IReadOnlyDictionary<string, bool>? InitialColumnVisibility { get; set; }

    /// <summary>
    /// Gets or sets initially selected row keys.
    /// </summary>
    [Parameter] public IReadOnlyList<string>? InitialSelectedKeys { get; set; }

    /// <summary>
    /// Gets or sets row selection behavior.
    /// </summary>
    [Parameter] public DataTableSelectionMode SelectionMode { get; set; } = DataTableSelectionMode.None;

    /// <summary>
    /// Gets or sets a value indicating whether hover styles are applied to rows.
    /// </summary>
    [Parameter] public bool HoverRows { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether alternating row striping is enabled.
    /// </summary>
    [Parameter] public bool StripedRows { get; set; }

    /// <summary>
    /// Gets or sets table row density.
    /// </summary>
    [Parameter] public DataTableDensity Density { get; set; } = DataTableDensity.Default;

    /// <summary>
    /// Gets or sets additional classes for the inner <see cref="RzTable"/> component.
    /// </summary>
    [Parameter] public string? TableClass { get; set; }

    /// <summary>
    /// Gets or sets declarative column content.
    /// </summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets optional toolbar action content.
    /// </summary>
    [Parameter] public RenderFragment? ToolbarActions { get; set; }

    /// <summary>
    /// Gets or sets optional empty-state markup.
    /// </summary>
    [Parameter] public RenderFragment? EmptyTemplate { get; set; }

    /// <summary>
    /// Gets or sets optional loading-state markup.
    /// </summary>
    [Parameter] public RenderFragment? LoadingTemplate { get; set; }

    /// <summary>
    /// Gets or sets whether loading state is displayed.
    /// </summary>
    [Parameter] public bool Loading { get; set; }

    /// <summary>
    /// Gets or sets optional projection for template-only row fields.
    /// </summary>
    [Parameter] public Func<TItem, IReadOnlyDictionary<string, object?>>? ClientRowProjection { get; set; }

    /// <summary>
    /// Gets or sets optional logical asset keys to include on the Alpine root.
    /// </summary>
    [Parameter] public string[] ComponentAssetKeys { get; set; } = [];

    private string ConfigScriptId => $"{Id}-config";

    private int EffectivePageSize => PageSizeOptions.Contains(PageSize) ? PageSize : PageSizeOptions.FirstOrDefault(10);

    private int ColumnSpan => _columns.Count + (SelectionMode == DataTableSelectionMode.Multiple ? 1 : 0);

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        SearchPlaceholder ??= Localizer["RzDataTable.SearchPlaceholder"];
    }

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        SearchPlaceholder ??= Localizer["RzDataTable.SearchPlaceholder"];

        if (Items is null)
        {
            throw new InvalidOperationException($"{nameof(RzDataTable<TItem>)} requires {nameof(Items)}.");
        }

        if (RowKey is null)
        {
            throw new InvalidOperationException($"{nameof(RzDataTable<TItem>)} requires {nameof(RowKey)}.");
        }

        _columns.Clear();
        _projectedRows.Clear();
        _configJson = "{}";
        _assets = "[]";

        _isCollectingColumns = true;
        _payloadReady = false;

        _ = InvokeAsync(StateHasChanged);
    }

    internal void RegisterColumn(DataTableColumnDefinition<TItem> definition)
    {
        if (!_isCollectingColumns)
        {
            return;
        }

        if (_columns.Any(c => string.Equals(c.Id, definition.Id, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"Duplicate DataTable column id '{definition.Id}'.");
        }

        _columns.Add(definition);
        _ = InvokeAsync(StateHasChanged);
    }

    private void EnsurePayloadBuilt()
    {
        if (_payloadReady)
        {
            return;
        }

        if (!_isCollectingColumns)
        {
            return;
        }

        if (_columns.Count == 0)
        {
            return;
        }

        _isCollectingColumns = false;
        BuildClientPayload();
        _payloadReady = true;
    }

    private void BuildClientPayload()
    {
        if (_columns.Count == 0)
        {
            throw new InvalidOperationException($"{nameof(RzDataTable<TItem>)} requires at least one {nameof(DataTableColumn<TItem>)}.");
        }

        var items = Items ?? [];
        var keySelector = RowKey!.Compile();

        var rows = new List<Dictionary<string, object?>>(items.Count());
        foreach (var item in items)
        {
            var key = Convert.ToString(keySelector(item), CultureInfo.InvariantCulture);
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new InvalidOperationException("RowKey selector produced a null/empty key. Stable keys are required.");
            }

            var row = new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["__key"] = key
            };

            foreach (var column in _columns)
            {
                row[ToCamelCase(column.Id)] = column.Field(item);
            }

            if (ClientRowProjection is not null)
            {
                var extra = ClientRowProjection(item);
                foreach (var entry in extra)
                {
                    if (entry.Key.Equals("__key", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    row[ToCamelCase(entry.Key)] = entry.Value;
                }
            }

            rows.Add(row);
        }

        var columnConfig = _columns.Select(c => new
        {
            id = c.Id,
            headerText = c.HeaderText,
            field = ToCamelCase(c.Id),
            sortable = c.Sortable,
            searchable = c.Searchable,
            visible = ResolveInitialColumnVisibility(c.Id, c.Visible),
            canHide = c.CanHide,
            align = c.Align.ToString().ToLowerInvariant(),
            width = c.Width,
            headerClass = c.HeaderClass,
            cellClass = c.CellClass,
            nullDisplayText = c.NullDisplayText,
            hasTemplate = c.HasTemplate
        }).ToList();

        _projectedRows = rows;

        var config = new
        {
            id = Id,
            selectionMode = SelectionMode.ToString().ToLowerInvariant(),
            search = Search,
            searchDebounceMs = SearchDebounceMs,
            enablePagination = EnablePagination,
            pageSizeOptions = PageSizeOptions,
            initialState = new
            {
                globalFilter = InitialSearch ?? string.Empty,
                pagination = new { pageIndex = InitialPageIndex, pageSize = EffectivePageSize },
                sorting = InitialSorting.Select(x => new { id = x.ColumnId, desc = x.Descending }),
                columnVisibility = columnConfig.ToDictionary(x => x.id, x => x.visible),
                selectedKeys = InitialSelectedKeys ?? [],
                columnPinning = new { left = Array.Empty<string>(), right = Array.Empty<string>() },
            },
            columns = columnConfig,
            rows
        };

        _configJson = JsonSerializer.Serialize(config);

        var assetUrls = ComponentAssetKeys
            .Select(key => RizzyUIConfig.Value.AssetUrls.TryGetValue(key, out var url) ? url : null)
            .Where(url => !string.IsNullOrWhiteSpace(url))
            .ToList();

        _assets = JsonSerializer.Serialize(assetUrls);
    }

    private bool ResolveInitialColumnVisibility(string columnId, bool fallbackVisible)
    {
        if (InitialColumnVisibility is null)
        {
            return fallbackVisible;
        }

        if (InitialColumnVisibility.TryGetValue(columnId, out var explicitValue))
        {
            return explicitValue;
        }

        return fallbackVisible;
    }

    private IEnumerable<Dictionary<string, object?>> GetSsrRows()
    {
        return _projectedRows;
    }

    private IEnumerable<Dictionary<string, object?>> GetSsrVisibleRows()
    {
        var rows = GetSsrRows();
        if (Search && !string.IsNullOrWhiteSpace(InitialSearch))
        {
            var searchColumns = _columns.Where(c => c.Searchable).Select(c => ToCamelCase(c.Id)).ToList();
            var term = InitialSearch.Trim();
            rows = rows.Where(row => searchColumns.Any(col =>
                row.TryGetValue(col, out var value)
                && value is not null
                && value.ToString()!.Contains(term, StringComparison.OrdinalIgnoreCase)));
        }

        IOrderedEnumerable<Dictionary<string, object?>>? orderedRows = null;
        foreach (var sort in InitialSorting)
        {
            var field = ToCamelCase(sort.ColumnId);
            Func<Dictionary<string, object?>, string> keySelector = row =>
                row.TryGetValue(field, out var value) ? value?.ToString() ?? string.Empty : string.Empty;

            orderedRows = orderedRows is null
                ? (sort.Descending ? rows.OrderByDescending(keySelector) : rows.OrderBy(keySelector))
                : (sort.Descending ? orderedRows.ThenByDescending(keySelector) : orderedRows.ThenBy(keySelector));
        }

        var query = (orderedRows ?? rows).AsEnumerable();

        if (EnablePagination)
        {
            query = query.Skip(Math.Max(0, InitialPageIndex) * EffectivePageSize).Take(EffectivePageSize);
        }

        return query;
    }

    private static string ToCamelCase(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        var trimmed = value.Trim();
        return char.ToLowerInvariant(trimmed[0]) + trimmed[1..];
    }

    private static string AlignClass(DataTableColumnAlign align)
        => align switch
        {
            DataTableColumnAlign.Center => "text-center",
            DataTableColumnAlign.End => "text-right",
            _ => "text-left"
        };

    /// <inheritdoc/>
    protected override TvDescriptor<RzComponent<RzDataTableSlots>, RzDataTableSlots> GetDescriptor() => Theme.RzDataTable;
}
