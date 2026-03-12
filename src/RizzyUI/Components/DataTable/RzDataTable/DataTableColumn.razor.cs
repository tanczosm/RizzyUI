using Microsoft.AspNetCore.Components;
using System.Linq.Expressions;

namespace RizzyUI;

/// <summary>
/// Declarative column definition used inside <see cref="RzDataTable{TItem}"/>.
/// </summary>
/// <typeparam name="TItem">The row item type.</typeparam>
public partial class DataTableColumn<TItem> : ComponentBase, IDisposable
{
    [CascadingParameter] private RzDataTable<TItem>? ParentTable { get; set; }

    private Func<TItem, object?>? _fieldAccessor;

    /// <summary>
    /// Gets or sets the column identifier.
    /// </summary>
    [Parameter, EditorRequired] public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the column header text.
    /// </summary>
    [Parameter, EditorRequired] public string HeaderText { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the field selector expression.
    /// </summary>
    [Parameter, EditorRequired] public Expression<Func<TItem, object?>>? Field { get; set; }

    /// <summary>
    /// Gets or sets whether the column can be sorted.
    /// </summary>
    [Parameter] public bool Sortable { get; set; }

    /// <summary>
    /// Gets or sets whether the column participates in global search.
    /// </summary>
    [Parameter] public bool Searchable { get; set; }

    /// <summary>
    /// Gets or sets whether the column is visible by default.
    /// </summary>
    [Parameter] public bool Visible { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the column can be hidden from the column menu.
    /// </summary>
    [Parameter] public bool CanHide { get; set; } = true;

    /// <summary>
    /// Gets or sets column alignment.
    /// </summary>
    [Parameter] public DataTableColumnAlign Align { get; set; } = DataTableColumnAlign.Start;

    /// <summary>
    /// Gets or sets optional width CSS value.
    /// </summary>
    [Parameter] public string? Width { get; set; }

    /// <summary>
    /// Gets or sets the text used when field value is null.
    /// </summary>
    [Parameter] public string? NullDisplayText { get; set; }

    /// <summary>
    /// Gets or sets custom header class names.
    /// </summary>
    [Parameter] public string? HeaderClass { get; set; }

    /// <summary>
    /// Gets or sets custom body-cell class names.
    /// </summary>
    [Parameter] public string? CellClass { get; set; }

    /// <summary>
    /// Gets or sets the optional inert client template to clone per cell.
    /// </summary>
    [Parameter] public RenderFragment? ClientTemplate { get; set; }

    internal string FieldName => ToCamelCase(GetMemberName(Field) ?? Id);

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (ParentTable is null)
            throw new InvalidOperationException($"{nameof(DataTableColumn<TItem>)} must be nested in {nameof(RzDataTable<TItem>)}.");

        ParentTable.RegisterColumn(this);
    }

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (Field is null)
            throw new InvalidOperationException($"{nameof(DataTableColumn<TItem>)} requires {nameof(Field)}.");

        _fieldAccessor = Field.Compile();
    }

    /// <summary>
    /// Evaluates the field value for an item.
    /// </summary>
    /// <param name="item">The row item.</param>
    /// <returns>The projected value.</returns>
    internal object? Evaluate(TItem item) => _fieldAccessor?.Invoke(item);

    /// <inheritdoc/>
    public void Dispose()
        => ParentTable?.UnregisterColumn(this);

    private static string? GetMemberName(Expression<Func<TItem, object?>>? expression)
    {
        if (expression?.Body is MemberExpression member)
            return member.Member.Name;

        if (expression?.Body is UnaryExpression unary && unary.Operand is MemberExpression unaryMember)
            return unaryMember.Member.Name;

        return null;
    }

    private static string ToCamelCase(string input)
        => string.IsNullOrEmpty(input) ? input : char.ToLowerInvariant(input[0]) + input[1..];
}
