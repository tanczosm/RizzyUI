using Microsoft.AspNetCore.Components;
using RizzyUI.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Trigger primitive that dispatches confetti requests to a target <see cref="RzConfetti"/> host.
/// </summary>
public partial class ConfettiTrigger : RzAsChildComponent<ConfettiTrigger.Slots>
{
    /// <summary>
    /// Defines the default styling for <see cref="ConfettiTrigger"/>.
    /// </summary>
    public static readonly TvDescriptor<RzAsChildComponent<Slots>, Slots> DefaultDescriptor = new(
        @base: "inline-flex items-center justify-center whitespace-nowrap rounded-md focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring disabled:pointer-events-none disabled:opacity-50"
    );

    /// <summary>
    /// Gets or sets optional visible trigger content.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets an optional explicit target host ID.
    /// </summary>
    [Parameter]
    public string? TargetId { get; set; }

    /// <summary>
    /// Gets or sets an optional pattern override.
    /// </summary>
    [Parameter]
    public ConfettiPattern? Pattern { get; set; }

    /// <summary>
    /// Gets or sets optional request options merged onto host defaults.
    /// </summary>
    [Parameter]
    public ConfettiRequest? Options { get; set; }

    /// <summary>
    /// Gets or sets the event that should fire confetti.
    /// </summary>
    [Parameter]
    public ConfettiTriggerEvent Trigger { get; set; } = ConfettiTriggerEvent.Click;

    /// <summary>
    /// Gets or sets a value indicating whether the trigger is disabled.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets an optional aria-label for icon-only trigger content.
    /// </summary>
    [Parameter]
    public string? AriaLabel { get; set; }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (string.IsNullOrWhiteSpace(Element))
            Element = "button";
    }

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (ChildContent is null)
        {
            throw new InvalidOperationException($"{nameof(ConfettiTrigger)} requires visible child content for accessibility.");
        }
    }

    /// <inheritdoc/>
    protected override RenderFragment? GetAsChildContent() => ChildContent;

    /// <inheritdoc/>
    protected override Dictionary<string, object?> GetComponentAttributes()
    {
        var attributes = new Dictionary<string, object?>(
            AdditionalAttributes?.ToDictionary(kvp => kvp.Key, kvp => (object?)kvp.Value) ?? new Dictionary<string, object?>(),
            StringComparer.OrdinalIgnoreCase)
        {
            ["id"] = Id,
            ["class"] = AsChild ? null : SlotClasses.GetBase(),
            ["data-slot"] = "confetti-trigger",
            ["data-confetti-trigger"] = "true",
            ["data-confetti-trigger-event"] = Trigger.ToString().ToKebabCase(),
            ["data-confetti-target"] = string.IsNullOrWhiteSpace(TargetId) ? null : TargetId,
            ["data-confetti-pattern"] = Pattern?.ToString().ToKebabCase(),
            ["data-confetti-request"] = Options is null ? null : JsonSerializer.Serialize(SerializeRequest(Options), SerializerOptions),
            ["disabled"] = !AsChild && Disabled ? true : null,
            ["aria-disabled"] = AsChild && Disabled ? "true" : null,
            ["aria-label"] = AriaLabel,
            ["type"] = !AsChild && string.Equals(EffectiveElement, "button", StringComparison.OrdinalIgnoreCase) ? "button" : null
        };

        return attributes;
    }

    /// <inheritdoc/>
    protected override TvDescriptor<RzAsChildComponent<Slots>, Slots> GetDescriptor() => Theme.ConfettiTrigger;

    private static object SerializeRequest(ConfettiRequest request)
    {
        return new
        {
            request.ParticleCount,
            request.Angle,
            request.Spread,
            request.StartVelocity,
            request.Decay,
            request.Gravity,
            request.Drift,
            request.Scalar,
            request.Ticks,
            origin = request.OriginX.HasValue || request.OriginY.HasValue
                ? new { x = request.OriginX, y = request.OriginY }
                : null,
            request.ZIndex,
            disableForReducedMotion = request.DisableForReducedMotion,
            request.Flat,
            request.Colors,
            shapes = request.Shapes?.Select(s => s.ToString().ToKebabCase()).ToArray()
        };
    }

    private static JsonSerializerOptions SerializerOptions => new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    /// <summary>
    /// Defines the slots available for styling in <see cref="ConfettiTrigger"/>.
    /// </summary>
    public sealed partial class Slots : ISlots
    {
        /// <summary>
        /// Gets or sets classes for the root element.
        /// </summary>
        [Slot("confetti-trigger")]
        public string? Base { get; set; }
    }
}
