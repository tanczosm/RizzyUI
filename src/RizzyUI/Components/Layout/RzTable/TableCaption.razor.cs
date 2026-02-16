using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// A semantic table caption rendered as <c>&lt;caption&gt;</c>.
/// </summary>
public partial class TableCaption : RzComponent<TableCaption.Slots>
{
    /// <summary>
    /// Defines the default styling for the TableCaption component.
    /// </summary>
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(
        @base: "text-muted-foreground mt-4 text-sm"
    );

    /// <summary>
    /// Gets or sets the caption content.
    /// </summary>
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = default!;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (string.IsNullOrEmpty(Element))
            Element = "caption";
    }

    /// <inheritdoc />
    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.RzSimpleTableCaption;

    /// <summary>
    /// Defines the slots available for styling in the TableCaption component.
    /// </summary>
    public sealed partial class Slots : ISlots
    {
        /// <summary>
        /// The base slot for the table caption element.
        /// </summary>
        [Slot("table-caption")]
        public string? Base { get; set; }
    }
}
