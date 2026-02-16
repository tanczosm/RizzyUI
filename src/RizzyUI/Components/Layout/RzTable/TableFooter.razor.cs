using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// A semantic table footer section rendered as <c>&lt;tfoot&gt;</c>.
/// </summary>
public partial class TableFooter : RzComponent<TableFooter.Slots>
{
    /// <summary>
    /// Defines the default styling for the TableFooter component.
    /// </summary>
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(
        @base: "bg-muted/50 border-t font-medium [&>tr]:last:border-b-0"
    );

    /// <summary>
    /// Gets or sets the footer row content.
    /// </summary>
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = default!;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (string.IsNullOrEmpty(Element))
            Element = "tfoot";
    }

    /// <inheritdoc />
    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.RzSimpleTableFooter;

    /// <summary>
    /// Defines the slots available for styling in the TableFooter component.
    /// </summary>
    public sealed partial class Slots : ISlots
    {
        /// <summary>
        /// The base slot for the table footer element.
        /// </summary>
        [Slot("table-footer")]
        public string? Base { get; set; }
    }
}
