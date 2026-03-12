using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.Options;
using RizzyUI.Extensions;
using System.Linq.Expressions;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// SSR-first data table with progressive TanStack/Alpine enhancement.
/// </summary>
/// <typeparam name="TItem">The row item type.</typeparam>
public partial class RzDataTable<TItem> : RzComponent<RzDataTableSlots>, IHasRzDataTableStylingProperties
{
    private readonly Dictionary<string, RegisteredColumn> _columnsById = new(StringComparer.Ordinal);
    private string _serializedConfig = "{}";
    private string _assets = "[]";

    [Inject]
    private IOptions<RizzyUIConfig> RizzyUIConfig { get; set; } = default!;

    /// <summary>
    /// Gets or sets the item collection.
    /// </summary>
    [Parameter, EditorRequired] public IEnumerable<TItem>? Items { get; set; }

    /// <summary>
    /// Gets or sets the mandatory row-key selector.
    /// </summary>
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
    [Parameter] public string? TableClass { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public RenderFragment? ToolbarActions { get; set; }
    [Parameter] public RenderFragment? EmptyTemplate { get; set; }
    [Parameter] public RenderFragment? LoadingTemplate { get; set; }

    /// <summary>
    /// Optional projected primitive fields used by client templates beyond declared columns.
    /// </summary>
    [Parameter] public Func<TItem, IReadOnlyDictionary<string, object?>>? ClientRowProjection { get; set; }

    /// <summary>
    /// Gets or sets optional component asset keys.
    /// </summary>
    [Parameter] public string[] ComponentAssetKeys { get; set; } = [];

    internal IReadOnlyList<RegisteredColumn> RegisteredColumns => _columnsById.Values.ToList();

    private IEnumerable<TItem> EffectiveItems => Items ?? [];
    private bool HasColumns => _columnsById.Count > 0;
    private int SelectionOffset => SelectionMode == DataTableSelectionMode.Multiple ? 1 : 0;
    private int VisibleColumnCount => _columnsById.Values.Count(c => ResolveInitialVisibility(c)) + SelectionOffset;
    private int FallbackColspan => Math.Max(VisibleColumnCount, 1);
    private string ConfigScriptSelector => $"[data-rz-datatable-config='{Id}']";

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        SearchPlaceholder ??= Localizer["RzDataTable.SearchPlaceholder"];
        UpdateAssets();
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (RowKey is null)
            throw new InvalidOperationException("RzDataTable requires the RowKey parameter.");

        SearchPlaceholder ??= Localizer["RzDataTable.SearchPlaceholder"];
        BuildConfig();
        UpdateAssets();
    }

    internal void RegisterColumn(DataTableColumn<TItem> column)
    {
        if (string.IsNullOrWhiteSpace(column.Id))
            return;

        if (column.Field is null)
            throw new InvalidOperationException($"DataTableColumn '{column.Id}' must provide Field.");

        _columnsById[column.Id] = new RegisteredColumn(column);
    }

    private void UpdateAssets()
    {
        var urls = ComponentAssetKeys
            .Select(key => RizzyUIConfig.Value.AssetUrls.TryGetValue(key, out var url) ? url : null)
            .Where(url => !string.IsNullOrWhiteSpace(url))
            .ToList();
        _assets = JsonSerializer.Serialize(urls);
    }

    private void BuildConfig()
    {
        if (RowKey is null)
            return;

        var keySelector = RowKey.Compile();
        var columns = _columnsById.Values.ToList();
        var rows = new List<Dictionary<string, object?>>();

        foreach (var item in EffectiveItems)
        {
            var row = new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["__key"] = Convert.ToString(keySelector(item)) ?? string.Empty
            };

            foreach (var column in columns)
            {
                row[column.FieldName] = column.ValueSelector(item);
            }

            if (ClientRowProjection is not null)
            {
                foreach (var kvp in ClientRowProjection(item))
                    row[kvp.Key] = kvp.Value;
            }

            rows.Add(row);
        }

        var config = new
        {
            id = Id,
            selectionMode = SelectionMode.ToString().ToLowerInvariant(),
            search = Search,
            searchDebounceMs = Math.Max(SearchDebounceMs, 0),
            enablePagination = EnablePagination,
            pageSizeOptions = PageSizeOptions,
            initialState = new
            {
                globalFilter = InitialSearch ?? string.Empty,
                pagination = new { pageIndex = Math.Max(InitialPageIndex, 0), pageSize = Math.Max(PageSize, 1) },
                sorting = InitialSorting.Select(s => new { id = s.ColumnId, desc = s.Descending }),
                columnVisibility = columns.ToDictionary(c => c.Id, ResolveInitialVisibility),
                selectedKeys = InitialSelectedKeys ?? []
            },
            columns = columns.Select(c => new
            {
                id = c.Id,
                headerText = c.HeaderText,
                field = c.FieldName,
                sortable = c.Sortable,
                searchable = c.Searchable,
                visible = c.Visible,
                canHide = c.CanHide,
                align = c.Align.ToString().ToLowerInvariant(),
                width = c.Width,
                hasTemplate = c.ClientTemplate is not null,
                nullDisplayText = c.NullDisplayText,
                headerClass = c.HeaderClass,
                cellClass = c.CellClass
            }),
            rows
        };

        _serializedConfig = JsonSerializer.Serialize(config, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });
    }

    private bool ResolveInitialVisibility(RegisteredColumn column)
    {
        if (InitialColumnVisibility is not null && InitialColumnVisibility.TryGetValue(column.Id, out var isVisible))
            return isVisible;

        return column.Visible;
    }

    private string RenderFallbackCell(RegisteredColumn column, TItem row)
    {
        var value = column.ValueSelector(row);
        return value?.ToString() ?? column.NullDisplayText ?? string.Empty;
    }

    private string GetTextAlignClass(DataTableColumnAlign align) => align switch
    {
        DataTableColumnAlign.Center => "text-center",
        DataTableColumnAlign.End => "text-right",
        _ => "text-left"
    };

    /// <inheritdoc />
    protected override TvDescriptor<RzComponent<RzDataTableSlots>, RzDataTableSlots> GetDescriptor() => Theme.RzDataTable;

    internal sealed class RegisteredColumn
    {
        public RegisteredColumn(DataTableColumn<TItem> column)
        {
            Id = column.Id;
            HeaderText = column.HeaderText;
            Sortable = column.Sortable;
            Searchable = column.Searchable;
            Visible = column.Visible;
            CanHide = column.CanHide;
            Align = column.Align;
            Width = column.Width;
            NullDisplayText = column.NullDisplayText;
            HeaderClass = column.HeaderClass;
            CellClass = column.CellClass;
            ClientTemplate = column.ClientTemplate;
            FieldName = ResolveFieldName(column.Field!);
            ValueSelector = column.Field!.Compile();
        }

        public string Id { get; }
        public string HeaderText { get; }
        public bool Sortable { get; }
        public bool Searchable { get; }
        public bool Visible { get; }
        public bool CanHide { get; }
        public DataTableColumnAlign Align { get; }
        public string? Width { get; }
        public string? NullDisplayText { get; }
        public string? HeaderClass { get; }
        public string? CellClass { get; }
        public RenderFragment? ClientTemplate { get; }
        public string FieldName { get; }
        public Func<TItem, object?> ValueSelector { get; }

        private static string ResolveFieldName(Expression<Func<TItem, object?>> expression)
        {
            Expression current = expression.Body;
            if (current is UnaryExpression unary && unary.NodeType == ExpressionType.Convert)
                current = unary.Operand;

            if (current is MemberExpression member)
            {
                var name = member.Member.Name;
                return string.IsNullOrEmpty(name) ? name : char.ToLowerInvariant(name[0]) + name[1..];
            }

            return "value";
        }
    }
}
