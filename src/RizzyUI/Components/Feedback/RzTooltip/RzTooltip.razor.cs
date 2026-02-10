using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// A container that coordinates tooltip trigger and content behavior.
/// </summary>
public partial class RzTooltip : RzComponent<RzTooltip.Slots>
{
    /// <summary>
    /// Defines the default styling for the RzTooltip component.
    /// </summary>
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(
        @base: "relative inline-flex"
    );

    /// <summary>
    /// Gets or sets the tooltip structure, typically including <see cref="TooltipTrigger"/> and <see cref="TooltipContent"/>.
    /// </summary>
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = default!;

    /// <summary>
    /// Gets or sets the delay in milliseconds before opening.
    /// If null, the provider default is used.
    /// </summary>
    [Parameter]
    public int? OpenDelay { get; set; }

    /// <summary>
    /// Gets or sets the delay in milliseconds before closing.
    /// If null, the provider default is used.
    /// </summary>
    [Parameter]
    public int? CloseDelay { get; set; }

    /// <summary>
    /// Gets or sets whether hovering over content should keep the tooltip open.
    /// </summary>
    [Parameter]
    public bool DisableHoverableContent { get; set; }

    /// <summary>
    /// Gets the parent tooltip provider.
    /// </summary>
    [CascadingParameter]
    protected RzTooltipProvider? TooltipProvider { get; set; }

    /// <summary>
    /// Gets the resolved open delay.
    /// </summary>
    protected int EffectiveOpenDelay => OpenDelay ?? TooltipProvider?.DefaultDelay ?? 700;

    /// <summary>
    /// Gets the resolved close delay.
    /// </summary>
    protected int EffectiveCloseDelay => CloseDelay ?? TooltipProvider?.DefaultCloseDelay ?? 150;

    /// <summary>
    /// Gets the Alpine.js data object used by the tooltip root scope.
    /// </summary>
    protected string AlpineData => $"{{ open: false, openTimer: null, closeTimer: null, openDelay: {EffectiveOpenDelay}, closeDelay: {EffectiveCloseDelay}, scheduleOpen() {{ clearTimeout(this.closeTimer); clearTimeout(this.openTimer); this.openTimer = setTimeout(() => this.open = true, this.openDelay); }}, scheduleClose() {{ clearTimeout(this.openTimer); clearTimeout(this.closeTimer); this.closeTimer = setTimeout(() => this.open = false, this.closeDelay); }}, openImmediate() {{ clearTimeout(this.openTimer); clearTimeout(this.closeTimer); this.open = true; }}, closeImmediate() {{ clearTimeout(this.openTimer); clearTimeout(this.closeTimer); this.open = false; }} }}";

    /// <inheritdoc />
    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.RzTooltip;

    /// <summary>
    /// Defines the slots available for styling in the RzTooltip component.
    /// </summary>
    public sealed partial class Slots : ISlots
    {
        /// <summary>
        /// The base slot for the tooltip root element.
        /// </summary>
        [Slot("tooltip")]
        public string? Base { get; set; }
    }
}
