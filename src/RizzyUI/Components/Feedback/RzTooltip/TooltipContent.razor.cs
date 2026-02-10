using Microsoft.AspNetCore.Components;
using RizzyUI.Extensions;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// The floating panel rendered for a <see cref="RzTooltip"/>.
/// </summary>
public partial class TooltipContent : RzComponent<TooltipContent.Slots>
{
    /// <summary>
    /// Defines the default styling for the TooltipContent component.
    /// </summary>
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(
        @base: "z-50 overflow-hidden rounded-md bg-primary px-3 py-1.5 text-xs text-primary-foreground shadow-md"
    );

    /// <summary>
    /// Gets the parent tooltip container.
    /// </summary>
    [CascadingParameter]
    protected RzTooltip? ParentTooltip { get; set; }

    /// <summary>
    /// Gets or sets the tooltip content.
    /// </summary>
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = default!;

    /// <summary>
    /// Gets or sets the preferred side of the trigger.
    /// </summary>
    [Parameter]
    public AnchorPoint Side { get; set; } = AnchorPoint.Top;

    /// <summary>
    /// Gets or sets the alignment of the tooltip.
    /// </summary>
    [Parameter]
    public Alignment Align { get; set; } = Alignment.Center;

    /// <summary>
    /// Gets or sets the distance in pixels from the trigger.
    /// </summary>
    [Parameter]
    public int Offset { get; set; } = 6;

    /// <summary>
    /// Gets the identifier for the tooltip content element.
    /// </summary>
    protected string ContentId => $"{ParentTooltip?.Id}-content";

    /// <summary>
    /// Gets the selector for the trigger element used by anchoring.
    /// </summary>
    protected string TriggerSelector => $"#{ParentTooltip?.Id}-trigger";

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (ParentTooltip == null)
        {
            throw new InvalidOperationException($"{nameof(TooltipContent)} must be used within an {nameof(RzTooltip)}.");
        }

        if (string.IsNullOrEmpty(Element))
        {
            Element = "div";
        }
    }

    /// <inheritdoc />
    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.TooltipContent;

    /// <summary>
    /// Defines the slots available for styling in the TooltipContent component.
    /// </summary>
    public sealed partial class Slots : ISlots
    {
        /// <summary>
        /// The base slot for the tooltip panel.
        /// </summary>
        [Slot("tooltip-content")]
        public string? Base { get; set; }
    }

    /// <summary>
    /// Supported tooltip content alignment values.
    /// </summary>
    public enum Alignment
    {
        /// <summary>
        /// Aligns the tooltip to the start edge.
        /// </summary>
        Start,

        /// <summary>
        /// Centers the tooltip.
        /// </summary>
        Center,

        /// <summary>
        /// Aligns the tooltip to the end edge.
        /// </summary>
        End
    }
}
