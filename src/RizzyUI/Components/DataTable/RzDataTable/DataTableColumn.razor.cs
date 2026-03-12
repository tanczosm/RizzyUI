using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;

namespace RizzyUI;

/// <summary>
/// Represents a declarative column definition consumed by <see cref="RzDataTable{TItem}"/>.
/// </summary>
/// <typeparam name="TItem">The row item type.</typeparam>
public partial class DataTableColumn<TItem> : ComponentBase
{
    [CascadingParameter(Name = "ParentRzDataTable")]
    private RzDataTable<TItem>? ParentDataTable { get; set; }

    /// <summary>
    /// Gets or sets the unique column identifier.
    /// </summary>
    [Parameter, EditorRequired] public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the column header text.
    /// </summary>
    [Parameter, EditorRequired] public string HeaderText { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the field selector used for projection and default rendering.
    /// </summary>
    [Parameter, EditorRequired] public Expression<Func<TItem, object?>>? Field { get; set; }

    /// <summary>
    /// Gets or sets whether this column supports client sorting.
    /// </summary>
    [Parameter] public bool Sortable { get; set; }

    /// <summary>
    /// Gets or sets whether this column participates in global search.
    /// </summary>
    [Parameter] public bool Searchable { get; set; }

    /// <summary>
    /// Gets or sets whether the column is visible by default.
    /// </summary>
    [Parameter] public bool Visible { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the column can be hidden in the column menu.
    /// </summary>
    [Parameter] public bool CanHide { get; set; } = true;

    /// <summary>
    /// Gets or sets the horizontal alignment for both header and cells.
    /// </summary>
    [Parameter] public DataTableColumnAlign Align { get; set; } = DataTableColumnAlign.Start;

    /// <summary>
    /// Gets or sets the optional CSS width value for the column.
    /// </summary>
    [Parameter] public string? Width { get; set; }

    /// <summary>
    /// Gets or sets fallback text rendered when a field value is null.
    /// </summary>
    [Parameter] public string? NullDisplayText { get; set; }

    /// <summary>
    /// Gets or sets additional CSS classes for header cells.
    /// </summary>
    [Parameter] public string? HeaderClass { get; set; }

    /// <summary>
    /// Gets or sets additional CSS classes for body cells.
    /// </summary>
    [Parameter] public string? CellClass { get; set; }

    /// <summary>
    /// Gets or sets optional client-side template content cloned for each rendered cell.
    /// </summary>
    [Parameter] public RenderFragment? ClientTemplate { get; set; }

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        if (ParentDataTable is null)
        {
            throw new InvalidOperationException($"{nameof(DataTableColumn<TItem>)} must be used inside {nameof(RzDataTable<TItem>)}.");
        }

        if (string.IsNullOrWhiteSpace(Id))
        {
            throw new InvalidOperationException($"{nameof(DataTableColumn<TItem>)} requires a non-empty {nameof(Id)}.");
        }

        if (Field is null)
        {
            throw new InvalidOperationException($"{nameof(DataTableColumn<TItem>)} requires {nameof(Field)}.");
        }

        ParentDataTable.RegisterColumn(new DataTableColumnDefinition<TItem>(
            Id,
            HeaderText,
            Field.Compile(),
            Sortable,
            Searchable,
            Visible,
            CanHide,
            Align,
            Width,
            NullDisplayText,
            HeaderClass,
            CellClass,
            ClientTemplate is not null));
    }
}

internal sealed record DataTableColumnDefinition<TItem>(
    string Id,
    string HeaderText,
    Func<TItem, object?> Field,
    bool Sortable,
    bool Searchable,
    bool Visible,
    bool CanHide,
    DataTableColumnAlign Align,
    string? Width,
    string? NullDisplayText,
    string? HeaderClass,
    string? CellClass,
    bool HasTemplate);
