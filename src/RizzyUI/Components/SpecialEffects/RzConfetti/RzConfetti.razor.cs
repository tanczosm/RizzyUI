using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using RizzyUI.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Canvas-backed confetti host that coordinates Alpine-driven confetti effects and trigger targeting.
/// </summary>
public partial class RzConfetti : RzComponent<RzConfetti.Slots>
{
    /// <summary>
    /// Defines the default styling for the <see cref="RzConfetti"/> component.
    /// </summary>
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(
        @base: "relative isolate overflow-hidden",
        slots: new()
        {
            [s => s.Viewport] = "absolute inset-0 pointer-events-none",
            [s => s.Canvas] = "size-full",
            [s => s.Content] = "relative z-10"
        }
    );

    private string _assets = "[]";

    [Inject]
    private IOptions<RizzyUIConfig> RizzyUIConfig { get; set; } = default!;

    /// <summary>
    /// Gets or sets optional content rendered above the confetti canvas.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets default confetti request options merged with trigger-level requests.
    /// </summary>
    [Parameter]
    public ConfettiRequest? Options { get; set; }

    /// <summary>
    /// Gets or sets global options used when creating the confetti canvas instance.
    /// </summary>
    [Parameter]
    public ConfettiGlobalOptions? GlobalOptions { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether automatic first-fire on initialization is disabled.
    /// </summary>
    [Parameter]
    public bool ManualStart { get; set; }

    /// <summary>
    /// Gets or sets the default named pattern for auto-fire and trigger fallbacks.
    /// </summary>
    [Parameter]
    public ConfettiPattern Pattern { get; set; } = ConfettiPattern.Burst;

    /// <summary>
    /// Gets or sets a value indicating whether reduced-motion preferences should suppress effects by default.
    /// </summary>
    [Parameter]
    public bool RespectReducedMotion { get; set; } = true;

    /// <summary>
    /// Gets or sets an optional logical host identifier used by triggers.
    /// </summary>
    [Parameter]
    public string? HostId { get; set; }

    /// <summary>
    /// Gets or sets logical asset keys used for optional runtime asset loading.
    /// </summary>
    [Parameter]
    public string[] ComponentAssetKeys { get; set; } = [];

    /// <summary>
    /// Gets the effective host identifier used for trigger routing.
    /// </summary>
    protected string EffectiveHostId => string.IsNullOrWhiteSpace(HostId) ? Id : HostId;

    /// <summary>
    /// Gets the serialized default request payload.
    /// </summary>
    protected string SerializedOptions => SerializeRequest(Options);

    /// <summary>
    /// Gets the serialized global options payload.
    /// </summary>
    protected string SerializedGlobalOptions => JsonSerializer.Serialize(GlobalOptions ?? new ConfettiGlobalOptions(), SerializerOptions);

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        UpdateAssets();
    }

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        UpdateAssets();
    }

    /// <inheritdoc/>
    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.RzConfetti;

    private static JsonSerializerOptions SerializerOptions => new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private static string SerializeRequest(ConfettiRequest? request)
    {
        if (request is null)
        {
            return "{}";
        }

        var payload = new
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

        return JsonSerializer.Serialize(payload, SerializerOptions);
    }

    private void UpdateAssets()
    {
        var assetUrls = ComponentAssetKeys
            .Select(key => RizzyUIConfig.Value.AssetUrls.TryGetValue(key, out var url) ? url : null)
            .Where(url => !string.IsNullOrEmpty(url))
            .ToList();

        _assets = JsonSerializer.Serialize(assetUrls, SerializerOptions);
    }

    /// <summary>
    /// Defines the slots available for styling in <see cref="RzConfetti"/>.
    /// </summary>
    public sealed partial class Slots : ISlots
    {
        /// <summary>
        /// Gets or sets classes for the root element.
        /// </summary>
        [Slot("confetti")]
        public string? Base { get; set; }

        /// <summary>
        /// Gets or sets classes for the Alpine viewport element.
        /// </summary>
        [Slot("viewport")]
        public string? Viewport { get; set; }

        /// <summary>
        /// Gets or sets classes for the canvas element.
        /// </summary>
        [Slot("canvas")]
        public string? Canvas { get; set; }

        /// <summary>
        /// Gets or sets classes for the visible content wrapper.
        /// </summary>
        [Slot("content")]
        public string? Content { get; set; }
    }
}
