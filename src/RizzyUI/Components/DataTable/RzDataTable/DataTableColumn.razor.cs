using Microsoft.AspNetCore.Components;
using System.Linq.Expressions;

namespace RizzyUI;

/// <summary>
/// Declares a column for <see cref="RzDataTable{TItem}"/>.
/// </summary>
public partial class DataTableColumn<TItem> : ComponentBase
{
    [CascadingParameter]
    private RzDataTable<TItem>? ParentTable { get; set; }

    /// <summary>
    /// Gets or sets the unique column id.
    /// </summary>
    [Parameter, EditorRequired] public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the column header text.
    /// </summary>
    [Parameter, EditorRequired] public string HeaderText { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the field selector used for raw value projection.
    /// </summary>
    [Parameter, EditorRequired] public Expression<Func<TItem, object?>>? Field { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the column is sortable.
    /// </summary>
    [Parameter] public bool Sortable { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the column participates in global search.
    /// </summary>
    [Parameter] public bool Searchable { get; set; }

    /// <summary>
    /// Gets or sets the default visibility for the column when not overridden by table-level initial visibility.
    /// </summary>
    [Parameter] public bool Visible { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether users can hide the column.
    /// </summary>
    [Parameter] public bool CanHide { get; set; } = true;

    /// <summary>
    /// Gets or sets the cell alignment.
    /// </summary>
    [Parameter] public DataTableColumnAlign Align { get; set; } = DataTableColumnAlign.Start;

    /// <summary>
    /// Gets or sets an optional column width.
    /// </summary>
    [Parameter] public string? Width { get; set; }

    /// <summary>
    /// Gets or sets an optional null-display text.
    /// </summary>
    [Parameter] public string? NullDisplayText { get; set; }

    /// <summary>
    /// Gets or sets optional additional classes for the header cell.
    /// </summary>
    [Parameter] public string? HeaderClass { get; set; }

    /// <summary>
    /// Gets or sets optional additional classes for body cells.
    /// </summary>
    [Parameter] public string? CellClass { get; set; }

    /// <summary>
    /// Gets or sets an inert client template rendered once and cloned per cell by Alpine.
    /// </summary>
    [Parameter] public RenderFragment? ClientTemplate { get; set; }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        ParentTable?.RegisterColumn(this);
    }
}
