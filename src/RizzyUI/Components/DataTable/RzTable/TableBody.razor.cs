using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Represents the body (<c>tbody</c>) of an <see cref="RzTable{TItem}"/>.
/// </summary>
/// <typeparam name="TItem">The row item type.</typeparam>
public partial class TableBody<TItem> : RzComponent<TableBodySlots>
{
    /// <summary>
    /// Gets or sets the parent table component.
    /// </summary>
    [CascadingParameter(Name = "ParentRzTable")]
    protected RzTable<TItem>? ParentRzTable { get; set; }

    /// <summary>
    /// Gets or sets the items to render.
    /// </summary>
    [Parameter] public IEnumerable<TItem>? Items { get; set; }

    /// <summary>
    /// Gets or sets the row template.
    /// </summary>
    [Parameter, EditorRequired] public RenderFragment<TItem> RowTemplate { get; set; } = default!;

    /// <summary>
    /// Gets or sets the empty-state template.
    /// </summary>
    [Parameter] public RenderFragment? EmptyTemplate { get; set; }

    /// <summary>
    /// Gets the effective items to render.
    /// </summary>
    protected IEnumerable<TItem> EffectiveItems => Items ?? ParentRzTable?.Items ?? Enumerable.Empty<TItem>();

    /// <summary>
    /// Gets the effective table column count.
    /// </summary>
    protected int ColumnCount => ParentRzTable?.ColumnCount ?? 1;

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (string.IsNullOrEmpty(Element))
        {
            Element = "tbody";
        }

        if (ParentRzTable == null)
        {
            throw new InvalidOperationException($"{GetType().Name} must be used within an RzTable.");
        }
    }

    /// <inheritdoc/>
    protected override TvDescriptor<RzComponent<TableBodySlots>, TableBodySlots> GetDescriptor() => Theme.TableBody;
}
