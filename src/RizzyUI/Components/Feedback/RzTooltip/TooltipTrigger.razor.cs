using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// The interactive element that opens and closes a <see cref="RzTooltip"/>.
/// </summary>
public partial class TooltipTrigger : RzAsChildComponent<TooltipTrigger.Slots>
{
    /// <summary>
    /// Defines the default styling for the TooltipTrigger component.
    /// </summary>
    public static readonly TvDescriptor<RzAsChildComponent<Slots>, Slots> DefaultDescriptor = new(
        @base: "inline-flex"
    );

    /// <summary>
    /// Gets the parent tooltip container.
    /// </summary>
    [CascadingParameter]
    protected RzTooltip? ParentTooltip { get; set; }

    /// <summary>
    /// Gets or sets the trigger content.
    /// </summary>
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = default!;

    /// <summary>
    /// Gets the trigger element identifier.
    /// </summary>
    protected string TriggerId => $"{ParentTooltip?.Id}-trigger";

    /// <summary>
    /// Gets the content element identifier controlled by this trigger.
    /// </summary>
    protected string ContentId => $"{ParentTooltip?.Id}-content";

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (ParentTooltip == null)
        {
            throw new InvalidOperationException($"{nameof(TooltipTrigger)} must be used within an {nameof(RzTooltip)}.");
        }

        Element = "button";
    }

    /// <inheritdoc />
    protected override RenderFragment? GetAsChildContent() => ChildContent;

    /// <inheritdoc />
    protected override Dictionary<string, object?> GetComponentAttributes()
    {
        var attributes = new Dictionary<string, object?>(AdditionalAttributes?.ToDictionary(kvp => kvp.Key, kvp => (object?)kvp.Value) ?? new Dictionary<string, object?>(), StringComparer.OrdinalIgnoreCase)
        {
            ["id"] = TriggerId,
            ["class"] = SlotClasses.GetBase(),
            ["x-ref"] = "trigger",
            ["x-on:mouseenter"] = "scheduleOpen",
            ["x-on:mouseleave"] = "scheduleClose",
            ["x-on:focusin"] = "openImmediate",
            ["x-on:focusout"] = "scheduleClose",
            ["aria-describedby"] = ContentId,
            ["x-bind:aria-expanded"] = "open.toString()",
            ["data-slot"] = "tooltip-trigger"
        };

        return attributes;
    }

    /// <inheritdoc />
    protected override TvDescriptor<RzAsChildComponent<Slots>, Slots> GetDescriptor() => Theme.TooltipTrigger;

    /// <summary>
    /// Defines the slots available for styling in the TooltipTrigger component.
    /// </summary>
    public sealed partial class Slots : ISlots
    {
        /// <summary>
        /// The base slot for the trigger element.
        /// </summary>
        [Slot("tooltip-trigger")]
        public string? Base { get; set; }
    }
}
