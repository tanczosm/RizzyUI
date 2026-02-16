using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// A semantic table data cell rendered as <c>&lt;td&gt;</c>.
/// </summary>
public partial class TableCellItem : RzComponent<TableCellItem.Slots>
{
    /// <summary>
    /// Defines the default styling for the TableCellItem component.
    /// </summary>
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(
        @base: "p-2 align-middle whitespace-nowrap [&:has([role=checkbox])]:pr-0 [&>[role=checkbox]]:translate-y-[2px]"
    );

    /// <summary>
    /// Gets or sets the cell content.
    /// </summary>
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = default!;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (string.IsNullOrEmpty(Element))
            Element = "td";
    }

    /// <inheritdoc />
    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.RzSimpleTableCellItem;

    /// <summary>
    /// Defines the slots available for styling in the TableCellItem component.
    /// </summary>
    public sealed partial class Slots : ISlots
    {
        /// <summary>
        /// The base slot for the table data cell element.
        /// </summary>
        [Slot("table-cell")]
        public string? Base { get; set; }
    }
}
