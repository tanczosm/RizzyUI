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
        }
    );

    /// <summary>
    /// The content rendered inside the scroll area viewport.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// When true, renders a default vertical <see cref="ScrollBar"/> within the scroll area.
    /// </summary>
    [Parameter]
    public bool ShowDefaultScrollBar { get; set; } = true;

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
