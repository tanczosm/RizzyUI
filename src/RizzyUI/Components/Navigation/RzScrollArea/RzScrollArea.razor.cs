using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// A container that provides a styled viewport for scrollable content.
/// </summary>
public partial class RzScrollArea : RzComponent<RzScrollArea.Slots>
{
    /// <summary>
    /// Defines the default styling for the RzScrollArea component.
    /// </summary>
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(
        @base: "relative",
        slots: new()
        {
            [s => s.Viewport] = "focus-visible:ring-ring/50 size-full rounded-[inherit] transition-[color,box-shadow] outline-none focus-visible:ring-[3px] focus-visible:outline-1"
        },
        variants: new()
        {
            [s => ((RzScrollArea)s).ScrollAreaOrientation] = new Variant<ScrollAreaOrientation, Slots>
            {
                [ScrollAreaOrientation.Vertical] = new() { [s => s.Viewport] = "overflow-y-auto overflow-x-hidden" },
                [ScrollAreaOrientation.Horizontal] = new() { [s => s.Viewport] = "overflow-x-auto overflow-y-hidden whitespace-nowrap" }
            }
        }
    );

    /// <summary>
    /// The content rendered inside the scroll area viewport.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// The primary scrolling orientation for the viewport.
    /// </summary>
    [Parameter]
    public ScrollAreaOrientation ScrollAreaOrientation { get; set; } = ScrollAreaOrientation.Vertical;

    /// <summary>
    /// When true, renders a default <see cref="ScrollBar"/> matching <see cref="ScrollAreaOrientation"/>.
    /// </summary>
    [Parameter]
    public bool ShowDefaultScrollBar { get; set; } = true;

    private Orientation DefaultScrollBarOrientation => ScrollAreaOrientation == ScrollAreaOrientation.Horizontal
        ? Orientation.Horizontal
        : Orientation.Vertical;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        Element = "div";
    }

    /// <inheritdoc />
    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.RzScrollArea;

    /// <summary>
    /// Defines the slots available for styling in the RzScrollArea component.
    /// </summary>
    public sealed partial class Slots : ISlots
    {
        /// <summary>
        /// The base slot for the root element.
        /// </summary>
        [Slot("scroll-area")]
        public string? Base { get; set; }

        /// <summary>
        /// The slot for the viewport wrapper.
        /// </summary>
        [Slot("scroll-area-viewport")]
        public string? Viewport { get; set; }
    }
}
