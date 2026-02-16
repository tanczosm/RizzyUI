using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// A semantic table row rendered as <c>&lt;tr&gt;</c>.
/// </summary>
public partial class TableRowItem : RzComponent<TableRowItem.Slots>
{
    /// <summary>
    /// Defines the default styling for the TableRowItem component.
    /// </summary>
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(
        @base: "border-b transition-colors hover:bg-muted/50 data-[state=selected]:bg-muted"
    );

    /// <summary>
    /// Gets or sets the row content.
    /// </summary>
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = default!;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (string.IsNullOrEmpty(Element))
            Element = "tr";
    }

    /// <inheritdoc />
    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.RzSimpleTableRowItem;

    /// <summary>
    /// Defines the slots available for styling in the TableRowItem component.
    /// </summary>
    public sealed partial class Slots : ISlots
    {
        /// <summary>
        /// The base slot for the table row element.
        /// </summary>
        [Slot("table-row")]
        public string? Base { get; set; }
    }
}
