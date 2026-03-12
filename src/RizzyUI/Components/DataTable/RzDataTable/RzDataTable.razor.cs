using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using TailwindVariants.NET;

namespace RizzyUI;

public partial class RzDataTable<TItem> : RzComponent<RzDataTableSlots>, IHasRzDataTableStylingProperties
{
    private readonly List<DataTableColumn<TItem>> _columns = [];
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private string _serializedConfig = "{}";
    private string _assets = "[]";

    [Inject] private IOptions<RizzyUIConfig> RizzyUIConfig { get; set; } = default!;

    [Parameter, EditorRequired] public IEnumerable<TItem>? Items { get; set; }
    [Parameter, EditorRequired] public Expression<Func<TItem, object>>? RowKey { get; set; }

    [Parameter] public bool ShowToolbar { get; set; } = true;
    [Parameter] public bool Search { get; set; }
    [Parameter] public string? SearchPlaceholder { get; set; }
    [Parameter] public int SearchDebounceMs { get; set; } = 300;

    [Parameter] public bool ShowColumnVisibility { get; set; } = true;

    [Parameter] public bool EnablePagination { get; set; } = true;
    [Parameter] public int InitialPageIndex { get; set; }
    [Parameter] public int PageSize { get; set; } = 10;
    [Parameter] public IReadOnlyList<int> PageSizeOptions { get; set; } = [10, 25, 50];

    [Parameter] public string? InitialSearch { get; set; }
    [Parameter] public IReadOnlyList<DataTableSortDefinition> InitialSorting { get; set; } = [];
    [Parameter] public IReadOnlyDictionary<string, bool>? InitialColumnVisibility { get; set; }
    [Parameter] public IReadOnlyList<string>? InitialSelectedKeys { get; set; }

    [Parameter] public DataTableSelectionMode SelectionMode { get; set; } = DataTableSelectionMode.None;

    [Parameter] public bool HoverRows { get; set; } = true;
    [Parameter] public bool StripedRows { get; set; }
    [Parameter] public DataTableDensity Density { get; set; } = DataTableDensity.Default;

    [Parameter] public string? Class { get; set; }
    [Parameter] public string? TableClass { get; set; }
    [Parameter] public Func<TItem, IReadOnlyDictionary<string, object?>>? ClientRowProjection { get; set; }
    [Parameter] public string[] ComponentAssetKeys { get; set; } = [];

    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public RenderFragment? ToolbarActions { get; set; }
    [Parameter] public RenderFragment? EmptyTemplate { get; set; }
    [Parameter] public RenderFragment? LoadingTemplate { get; set; }

    internal string ConfigScriptId => $"{Id}-config";
    internal int VisibleColumnCount => _columns.Count(c => ResolveInitialColumnVisibility(c));
    internal int TotalColumnCount => _columns.Count + (SelectionMode == DataTableSelectionMode.Multiple ? 1 : 0);
    internal IEnumerable<DataTableColumn<TItem>> Columns => _columns;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        SearchPlaceholder ??= Localizer["RzDataTable.SearchPlaceholder"];
        UpdateAssets();
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (RowKey is null)
            throw new InvalidOperationException($"{nameof(RowKey)} is required.");

        SearchPlaceholder ??= Localizer["RzDataTable.SearchPlaceholder"];

        BuildConfig();
    }

    internal void RegisterColumn(DataTableColumn<TItem> column)
    {
        if (_columns.Any(c => c.Id == column.Id))
            return;

        _columns.Add(column);
        _ = InvokeAsync(StateHasChanged);
    }

    internal bool ResolveInitialColumnVisibility(DataTableColumn<TItem> column)
    {
        if (InitialColumnVisibility is not null && InitialColumnVisibility.TryGetValue(column.Id, out var visible))
            return visible;

        return column.Visible;
    }

    internal string GetCellText(DataTableColumn<TItem> column, TItem item)
    {
        var value = column.Field?.Compile().Invoke(item);
        if (value is null)
            return column.NullDisplayText ?? string.Empty;

        return Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
    }

    internal string GetRowKey(TItem item)
    {
        var raw = RowKey?.Compile().Invoke(item);
        var key = Convert.ToString(raw, CultureInfo.InvariantCulture);
        if (string.IsNullOrWhiteSpace(key))
            throw new InvalidOperationException("Row key selector produced a null/empty key.");

        return key;
    }

    internal string GetColumnAlignClass(DataTableColumnAlign align) => align switch
    {
        DataTableColumnAlign.Center => "text-center",
        DataTableColumnAlign.End => "text-right",
        _ => "text-left"
    };

    protected override TvDescriptor<RzComponent<RzDataTableSlots>, RzDataTableSlots> GetDescriptor() => Theme.RzDataTable;

    internal string GetHeaderCellCss(DataTableColumn<TItem> column)
        => $"{SlotClasses.GetHeaderCell()} {GetColumnAlignClass(column.Align)} {column.HeaderClass}".Trim();

    internal string GetBodyCellCss(DataTableColumn<TItem> column)
        => $"{SlotClasses.GetBodyCell()} {GetColumnAlignClass(column.Align)} {column.CellClass}".Trim();

    internal string GetSelectionCellCss() => $"{SlotClasses.GetBodyCell()} text-center";

    internal string GetSelectAllHeaderCss() => $"{SlotClasses.GetHeaderCell()} {GetColumnAlignClass(DataTableColumnAlign.Center)}";

    private void UpdateAssets()
    {
        var assetUrls = ComponentAssetKeys
            .Select(key => RizzyUIConfig.Value.AssetUrls.TryGetValue(key, out var url) ? url : null)
            .Where(url => !string.IsNullOrWhiteSpace(url))
            .ToArray();

        _assets = JsonSerializer.Serialize(assetUrls, _jsonOptions);
    }

    private void BuildConfig()
    {
        if (_columns.Count == 0 || Items is null || RowKey is null)
        {
            _serializedConfig = "{}";
            return;
        }

        var rows = new List<Dictionary<string, object?>>();
        foreach (var item in Items)
        {
            var row = new Dictionary<string, object?>
            {
                ["__key"] = GetRowKey(item)
            };

            foreach (var column in _columns)
            {
                var rawValue = column.Field?.Compile().Invoke(item);
                row[ToCamelCase(column.Id)] = rawValue;
            }

            if (ClientRowProjection is not null)
            {
                foreach (var pair in ClientRowProjection(item))
                {
                    row[ToCamelCase(pair.Key)] = pair.Value;
                }
            }

            rows.Add(row);
        }

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
                pagination = new { pageIndex = InitialPageIndex, pageSize = PageSize },
                sorting = InitialSorting.Select(s => new { id = s.ColumnId, desc = s.Descending }),
                columnVisibility = _columns.ToDictionary(c => c.Id, ResolveInitialColumnVisibility),
                selectedKeys = InitialSelectedKeys ?? []
            },
            columns = _columns.Select(c => new
            {
                id = c.Id,
                headerText = c.HeaderText,
                field = ToCamelCase(c.Id),
                sortable = c.Sortable,
                searchable = c.Searchable,
                visible = ResolveInitialColumnVisibility(c),
                canHide = c.CanHide,
                align = c.Align.ToString().ToLowerInvariant(),
                width = c.Width,
                hasTemplate = c.ClientTemplate is not null,
                templateId = c.ClientTemplate is not null ? $"{Id}-{c.Id}-template" : null,
                headerClass = c.HeaderClass,
                cellClass = c.CellClass,
                nullDisplayText = c.NullDisplayText
            }),
            rows
        };

        _serializedConfig = JsonSerializer.Serialize(config, _jsonOptions);
    }

    private static string ToCamelCase(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return value;

        return char.ToLowerInvariant(value[0]) + value[1..];
    }
}
