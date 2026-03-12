using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Defines styling properties for a <see cref="TableRow"/> component.
/// </summary>
public interface IHasTableRowStylingProperties
{
    /// <summary>
    /// Gets a value indicating whether hover styling should be applied.
    /// </summary>
    public bool IsHoverable { get; }
}

/// <summary>
/// Represents a table row (<c>tr</c>) within an <see cref="RzTable"/>.
/// </summary>
public partial class TableRow : RzComponent<TableRowSlots>, IHasTableRowStylingProperties
{
    /// <summary>
    /// Gets or sets the parent table component.
    /// </summary>
    [CascadingParameter(Name = "ParentRzTable")]
    protected RzTable? ParentRzTable { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether hover styling should be applied for this row.
    /// </summary>
    [Parameter] public bool Hoverable { get; set; } = true;

    /// <summary>
    /// Gets or sets the row content.
    /// </summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets a value indicating whether hover styling should be applied.
    /// </summary>
    public bool IsHoverable => Hoverable;

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (string.IsNullOrEmpty(Element))
        {
            Element = "tr";
        }
    }

    /// <inheritdoc/>
    protected override TvDescriptor<RzComponent<TableRowSlots>, TableRowSlots> GetDescriptor() => Theme.TableRow;
}
