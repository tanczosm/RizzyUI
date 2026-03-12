using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Provides an SSR-first, composable table container with shared table state.
/// </summary>
/// <typeparam name="TItem">The type of data item rendered by the table.</typeparam>
[CascadingTypeParameter(nameof(TItem))]
public partial class RzTable<TItem> : RzComponent<RzTableSlots>, IHasTableStylingProperties
{
    private readonly List<ColumnDefinition<TItem>> _columnDefinitions = new();

    /// <summary>
    /// Gets or sets the collection of items rendered by the table.
    /// </summary>
    [Parameter, EditorRequired] public IEnumerable<TItem> Items { get; set; } = Enumerable.Empty<TItem>();

    /// <summary>
    /// Gets or sets the current page number.
    /// </summary>
    [Parameter] public int CurrentPage { get; set; } = 1;

    /// <summary>
    /// Gets or sets the current page size.
    /// </summary>
    [Parameter] public int PageSize { get; set; } = 10;

    /// <summary>
    /// Gets or sets the total number of items available for pagination.
    /// </summary>
    [Parameter] public long TotalItems { get; set; }

    /// <summary>
    /// Gets or sets the column key currently used for sorting.
    /// </summary>
    [Parameter] public string? SortBy { get; set; }

    /// <summary>
    /// Gets or sets the current sort direction.
    /// </summary>
    [Parameter] public SortDirection SortDirection { get; set; } = SortDirection.Unset;

    /// <summary>
    /// Gets or sets a value indicating whether the table is currently loading.
    /// </summary>
    [Parameter] public bool IsLoading { get; set; }

    /// <summary>
    /// Gets or sets the content rendered inside the <c>table</c> element.
    /// </summary>
    [Parameter, EditorRequired] public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to apply striped row styling.
    /// </summary>
    [Parameter] public bool Striped { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether rows should display hover styling.
    /// </summary>
    [Parameter] public bool Hoverable { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether table chrome (border and rounded corners) should be applied.
    /// </summary>
    [Parameter] public bool Border { get; set; }

    /// <summary>
    /// Gets or sets the row selection mode.
    /// </summary>
    [Parameter] public TableSelectionMode SelectionMode { get; set; } = TableSelectionMode.None;

    /// <summary>
    /// Gets or sets the selected row keys used for SSR-selected state.
    /// </summary>
    [Parameter] public HashSet<string> SelectedItems { get; set; } = new(StringComparer.Ordinal);

    /// <summary>
    /// Gets or sets a value indicating whether the table header should be sticky.
    /// </summary>
    [Parameter] public bool FixedHeader { get; set; }

    /// <summary>
    /// Gets or sets the body height class used when <see cref="FixedHeader"/> is enabled.
    /// </summary>
    [Parameter] public string TableBodyHeightClass { get; set; } = "h-96";

    /// <summary>
    /// Gets the unique identifier for the inner table element.
    /// </summary>
    public string TableId => $"{Id}-table";

    /// <summary>
    /// Gets the total number of registered columns.
    /// </summary>
    public int ColumnCount => _columnDefinitions.Count > 0 ? _columnDefinitions.Count : 1;

    /// <summary>
    /// Gets the total page count based on table pagination state.
    /// </summary>
    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalItems / (double)PageSize);

    internal void AddColumnDefinition(ColumnDefinition<TItem> columnDefinition)
    {
        if (_columnDefinitions.Any(cd => cd.Key == columnDefinition.Key))
        {
            return;
        }

        _columnDefinitions.Add(columnDefinition);
        StateHasChanged();
    }

    internal IReadOnlyList<ColumnDefinition<TItem>> GetColumnDefinitions() => _columnDefinitions.AsReadOnly();

    /// <inheritdoc/>
    protected override TvDescriptor<RzComponent<RzTableSlots>, RzTableSlots> GetDescriptor() => Theme.RzTable;
}
