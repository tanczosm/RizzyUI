using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using RizzyUI.Extensions;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// SSR-first data table that progressively enhances with Alpine.js and TanStack Table.
/// </summary>
/// <typeparam name="TItem">The row item type.</typeparam>
public partial class RzDataTable<TItem> : RzComponent<RzDataTableSlots>, IHasRzDataTableStylingProperties
{
    [Inject] private IOptions<RizzyUIConfig> RizzyUIConfig { get; set; } = default!;

    private readonly List<DataTableColumn<TItem>> _columns = [];
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private Func<TItem, object>? _rowKeySelector;
    private string _assets = "[]";
    private string _serializedConfig = "{}";

    /// <summary>
    /// Gets or sets the source items for the table.
    /// </summary>
    [Parameter, EditorRequired] public IEnumerable<TItem>? Items { get; set; }

    /// <summary>
    /// Gets or sets the row key selector expression used to produce stable row keys.
    /// </summary>
    [Parameter, EditorRequired] public Expression<Func<TItem, object>>? RowKey { get; set; }

    /// <summary>
    /// Gets or sets whether the toolbar is shown.
    /// </summary>
    [Parameter] public bool ShowToolbar { get; set; } = true;

    /// <summary>
    /// Gets or sets whether global search is enabled.
    /// </summary>
    [Parameter] public bool Search { get; set; }

    /// <summary>
    /// Gets or sets the search input placeholder.
    /// </summary>
    [Parameter] public string? SearchPlaceholder { get; set; }

    /// <summary>
    /// Gets or sets the search debounce in milliseconds.
    /// </summary>
    [Parameter] public int SearchDebounceMs { get; set; } = 300;

    /// <summary>
    /// Gets or sets whether column visibility controls are shown.
    /// </summary>
    [Parameter] public bool ShowColumnVisibility { get; set; } = true;

    /// <summary>
    /// Gets or sets whether client pagination is enabled.
    /// </summary>
    [Parameter] public bool EnablePagination { get; set; } = true;

    /// <summary>
    /// Gets or sets the initial page index.
    /// </summary>
    [Parameter] public int InitialPageIndex { get; set; }

    /// <summary>
    /// Gets or sets the page size.
    /// </summary>
    [Parameter] public int PageSize { get; set; } = 10;

    /// <summary>
    /// Gets or sets allowed page size options.
    /// </summary>
    [Parameter] public IReadOnlyList<int> PageSizeOptions { get; set; } = new[] { 10, 25, 50 };

    /// <summary>
    /// Gets or sets the initial search term.
    /// </summary>
    [Parameter] public string? InitialSearch { get; set; }

    /// <summary>
    /// Gets or sets initial sorting.
    /// </summary>
    [Parameter] public IReadOnlyList<DataTableSortDefinition> InitialSorting { get; set; } = [];

    /// <summary>
    /// Gets or sets initial column visibility by column id.
    /// </summary>
    [Parameter] public IReadOnlyDictionary<string, bool>? InitialColumnVisibility { get; set; }

    /// <summary>
    /// Gets or sets initially selected row keys.
    /// </summary>
    [Parameter] public IReadOnlyList<string>? InitialSelectedKeys { get; set; }

    /// <summary>
    /// Gets or sets row selection mode.
    /// </summary>
    [Parameter] public DataTableSelectionMode SelectionMode { get; set; } = DataTableSelectionMode.None;

    /// <summary>
    /// Gets or sets whether row hover styling is enabled.
    /// </summary>
    [Parameter] public bool HoverRows { get; set; } = true;

    /// <summary>
    /// Gets or sets whether row striping is enabled.
    /// </summary>
    [Parameter] public bool StripedRows { get; set; }

    /// <summary>
    /// Gets or sets row density.
    /// </summary>
    [Parameter] public DataTableDensity Density { get; set; } = DataTableDensity.Default;

    /// <summary>
    /// Gets or sets additional class names for the composed <see cref="RzTable"/>.
    /// </summary>
    [Parameter] public string? TableClass { get; set; }

    /// <summary>
    /// Gets or sets declarative columns.
    /// </summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets additional toolbar actions.
    /// </summary>
    [Parameter] public RenderFragment? ToolbarActions { get; set; }

    /// <summary>
    /// Gets or sets custom empty-state content.
    /// </summary>
    [Parameter] public RenderFragment? EmptyTemplate { get; set; }

    /// <summary>
    /// Gets or sets custom loading-state content.
    /// </summary>
    [Parameter] public RenderFragment? LoadingTemplate { get; set; }

    /// <summary>
    /// Gets or sets an optional additional row projection used by client templates.
    /// </summary>
    [Parameter] public Func<TItem, IReadOnlyDictionary<string, object?>>? ClientRowProjection { get; set; }

    /// <summary>
    /// Gets or sets optional component asset keys.
    /// </summary>
    [Parameter] public string[] ComponentAssetKeys { get; set; } = [];

    internal IReadOnlyList<DataTableColumn<TItem>> Columns => _columns;
    private string ConfigScriptId => $"{Id}-config";

    private IEnumerable<TItem> EffectiveItems => Items ?? Enumerable.Empty<TItem>();

    private IReadOnlyList<DataTableColumn<TItem>> VisibleColumns => _columns.Where(c => ResolveVisible(c)).ToList();

    private int BodyColspan => VisibleColumns.Count + (SelectionMode == DataTableSelectionMode.Multiple ? 1 : 0);

    private int InitialPageSize => PageSizeOptions.Contains(PageSize) ? PageSize : (PageSizeOptions.FirstOrDefault() > 0 ? PageSizeOptions.First() : 10);

    private IEnumerable<TItem> InitialRows => EffectiveItems.Take(Math.Max(1, InitialPageSize));

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

        if (RowKey is null)
            throw new InvalidOperationException("RzDataTable requires a RowKey expression.");

        _rowKeySelector = RowKey.Compile();
        SearchPlaceholder ??= Localizer["RzDataTable.SearchPlaceholder"];

        _assets = JsonSerializer.Serialize(ComponentAssetKeys
            .Select(key => RizzyUIConfig.Value.AssetUrls.TryGetValue(key, out var url) ? url : null)
            .Where(url => !string.IsNullOrWhiteSpace(url)), _jsonOptions);

        _serializedConfig = JsonSerializer.Serialize(BuildClientConfig(), _jsonOptions);
    }

    internal void RegisterColumn(DataTableColumn<TItem> column)
    {
        if (_columns.All(c => !string.Equals(c.Id, column.Id, StringComparison.Ordinal)))
            _columns.Add(column);
    }

    internal void UnregisterColumn(DataTableColumn<TItem> column)
        => _columns.Remove(column);

    private bool ResolveVisible(DataTableColumn<TItem> column)
    {
        if (InitialColumnVisibility is not null && InitialColumnVisibility.TryGetValue(column.Id, out var configured))
            return configured;

        return column.Visible;
    }

    private DataTableClientConfig BuildClientConfig()
    {
        var rows = EffectiveItems.Select(ProjectRow).ToList();
        var columns = _columns.Select(column => new DataTableClientColumn
        {
            Id = column.Id,
            HeaderText = column.HeaderText,
            Field = column.FieldName,
            Sortable = column.Sortable,
            Searchable = column.Searchable,
            Visible = ResolveVisible(column),
            CanHide = column.CanHide,
            Align = column.Align.ToString().ToKebabCase(),
            Width = column.Width,
            HeaderClass = column.HeaderClass,
            CellClass = column.CellClass,
            HasTemplate = column.ClientTemplate is not null,
            NullDisplayText = column.NullDisplayText
        }).ToList();

        return new DataTableClientConfig
        {
            Id = Id,
            SelectionMode = SelectionMode.ToString().ToKebabCase(),
            Search = Search,
            SearchDebounceMs = Math.Max(0, SearchDebounceMs),
            EnablePagination = EnablePagination,
            PageSizeOptions = PageSizeOptions,
            InitialState = new DataTableClientInitialState
            {
                GlobalFilter = InitialSearch ?? string.Empty,
                Pagination = new DataTableClientPagination
                {
                    PageIndex = Math.Max(0, InitialPageIndex),
                    PageSize = InitialPageSize
                },
                Sorting = InitialSorting.Select(s => new DataTableClientSort { Id = s.ColumnId, Desc = s.Descending }).ToList(),
                ColumnVisibility = columns.ToDictionary(c => c.Id, c => c.Visible, StringComparer.Ordinal),
                SelectedKeys = InitialSelectedKeys ?? []
            },
            Columns = columns,
            Rows = rows
        };
    }

    private Dictionary<string, object?> ProjectRow(TItem item)
    {
        var row = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["__key"] = _rowKeySelector?.Invoke(item)?.ToString() ?? string.Empty
        };

        foreach (var column in _columns)
            row[column.FieldName] = column.Evaluate(item);

        if (ClientRowProjection is not null)
        {
            foreach (var pair in ClientRowProjection(item))
                row[pair.Key] = pair.Value;
        }

        return row;
    }

    /// <inheritdoc/>
    protected override TvDescriptor<RzComponent<RzDataTableSlots>, RzDataTableSlots> GetDescriptor() => Theme.RzDataTable;

    private sealed class DataTableClientConfig
    {
        public string Id { get; set; } = string.Empty;
        public string SelectionMode { get; set; } = "none";
        public bool Search { get; set; }
        public int SearchDebounceMs { get; set; }
        public bool EnablePagination { get; set; }
        public IReadOnlyList<int> PageSizeOptions { get; set; } = [];
        public DataTableClientInitialState InitialState { get; set; } = new();
        public IReadOnlyList<DataTableClientColumn> Columns { get; set; } = [];
        public IReadOnlyList<Dictionary<string, object?>> Rows { get; set; } = [];
    }

    private sealed class DataTableClientInitialState
    {
        public string GlobalFilter { get; set; } = string.Empty;
        public DataTableClientPagination Pagination { get; set; } = new();
        public IReadOnlyList<DataTableClientSort> Sorting { get; set; } = [];
        public IReadOnlyDictionary<string, bool> ColumnVisibility { get; set; } = new Dictionary<string, bool>();
        public IReadOnlyList<string> SelectedKeys { get; set; } = [];
    }

    private sealed class DataTableClientPagination
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }

    private sealed class DataTableClientSort
    {
        public string Id { get; set; } = string.Empty;
        public bool Desc { get; set; }
    }

    private sealed class DataTableClientColumn
    {
        public string Id { get; set; } = string.Empty;
        public string HeaderText { get; set; } = string.Empty;
        public string Field { get; set; } = string.Empty;
        public bool Sortable { get; set; }
        public bool Searchable { get; set; }
        public bool Visible { get; set; }
        public bool CanHide { get; set; }
        public string Align { get; set; } = "start";
        public string? Width { get; set; }
        public string? HeaderClass { get; set; }
        public string? CellClass { get; set; }
        public bool HasTemplate { get; set; }
        public string? NullDisplayText { get; set; }
    }
}
