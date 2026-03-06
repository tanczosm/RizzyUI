using Microsoft.AspNetCore.Components;
using RizzyUI.Extensions;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Renders a CSS-only shimmer button with layered decorative effects and native button semantics.
/// </summary>
public partial class RzShimmerButton : RzComponent<RzShimmerButton.Slots>
{
    /// <summary>
    /// Defines the default styling for the <see cref="RzShimmerButton"/> component.
    /// </summary>
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(
        @base: "group relative z-0 inline-flex items-center justify-center overflow-hidden whitespace-nowrap px-6 py-3 border border-border/40 bg-primary text-primary-foreground transition-transform duration-300 ease-in-out active:translate-y-px focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50 [border-radius:var(--rz-shimmer-button-radius)] [background:var(--rz-shimmer-button-bg)]",
        slots: new()
        {
            [s => s.SparkContainer] = "pointer-events-none absolute inset-0 -z-30 overflow-visible blur-[2px] motion-reduce:blur-0",
            [s => s.SparkTrack] = "absolute inset-0 aspect-[1] h-[100cqh] rounded-none [mask:none] motion-safe:animate-shimmer-slide motion-reduce:animate-none",
            [s => s.SparkBeam] = "absolute -inset-full w-auto [translate:0_0] rotate-0 motion-safe:animate-spin-around motion-reduce:animate-none [background:conic-gradient(from_calc(270deg-(var(--rz-shimmer-button-spread)*0.5)),transparent_0,var(--rz-shimmer-button-color)_var(--rz-shimmer-button-spread),transparent_var(--rz-shimmer-button-spread))]",
            [s => s.Content] = "relative z-10 inline-flex items-center justify-center",
            [s => s.Highlight] = "pointer-events-none absolute inset-0 size-full rounded-[inherit] shadow-[inset_0_-8px_10px_#ffffff1f] transition-all duration-300 ease-in-out group-hover:shadow-[inset_0_-6px_10px_#ffffff3f] group-active:shadow-[inset_0_-10px_10px_#ffffff3f]",
            [s => s.Backdrop] = "pointer-events-none absolute -z-20 [inset:var(--rz-shimmer-button-cut)] [border-radius:var(--rz-shimmer-button-radius)] bg-primary [background:var(--rz-shimmer-button-bg)]"
        }
    );

    private const string DefaultShimmerColor = "#ffffff";
    private const string DefaultShimmerSize = "0.05em";
    private const string DefaultBorderRadius = "100px";
    private const string DefaultShimmerDuration = "3s";

    private IReadOnlyDictionary<string, object?>? RootAttributes { get; set; }

    private string? EffectiveAriaLabel => string.IsNullOrWhiteSpace(AriaLabel) ? null : AriaLabel;

    private string RootStyle { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the content rendered inside the shimmer button.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the shimmer beam color.
    /// </summary>
    [Parameter]
    public string? ShimmerColor { get; set; } = DefaultShimmerColor;

    /// <summary>
    /// Gets or sets the shimmer cut size used by the backdrop inset.
    /// </summary>
    [Parameter]
    public string? ShimmerSize { get; set; } = DefaultShimmerSize;

    /// <summary>
    /// Gets or sets the border radius applied to the root and backdrop layers.
    /// </summary>
    [Parameter]
    public string? BorderRadius { get; set; } = DefaultBorderRadius;

    /// <summary>
    /// Gets or sets the shimmer animation duration.
    /// </summary>
    [Parameter]
    public string? ShimmerDuration { get; set; } = DefaultShimmerDuration;

    /// <summary>
    /// Gets or sets an optional background override used by the button face and backdrop.
    /// </summary>
    [Parameter]
    public string? Background { get; set; }

    /// <summary>
    /// Gets or sets the native HTML button type.
    /// </summary>
    [Parameter]
    public ButtonType Type { get; set; } = ButtonType.Button;

    /// <summary>
    /// Gets or sets a value indicating whether the button is disabled.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets an optional accessible name for icon-only or non-text content scenarios.
    /// </summary>
    [Parameter]
    public string? AriaLabel { get; set; }

    private string EffectiveType => Type.ToString().ToLowerInvariant();

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (string.IsNullOrEmpty(Element))
            Element = "button";
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        var userStyle = string.Empty;
        var rootAttributes = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

        if (AdditionalAttributes is not null)
        {
            foreach (var attribute in AdditionalAttributes)
            {
                if (string.Equals(attribute.Key, "style", StringComparison.OrdinalIgnoreCase))
                {
                    userStyle = attribute.Value?.ToString() ?? string.Empty;
                    continue;
                }

                rootAttributes[attribute.Key] = attribute.Value;
            }
        }

        RootAttributes = rootAttributes.Count > 0 ? rootAttributes : null;
        RootStyle = BuildRootStyle(userStyle);
    }

    /// <inheritdoc />
    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.RzShimmerButton;

    private string BuildRootStyle(string userStyle)
    {
        var styleBuilder = new List<string>
        {
            "--rz-shimmer-button-spread:90deg",
            $"--rz-shimmer-button-color:{ResolveCssValue(ShimmerColor, DefaultShimmerColor)}",
            $"--rz-shimmer-button-radius:{ResolveCssValue(BorderRadius, DefaultBorderRadius)}",
            $"--rz-shimmer-button-speed:{ResolveCssValue(ShimmerDuration, DefaultShimmerDuration)}",
            $"--rz-shimmer-button-cut:{ResolveCssValue(ShimmerSize, DefaultShimmerSize)}"
        };

        if (!string.IsNullOrWhiteSpace(Background))
            styleBuilder.Add($"--rz-shimmer-button-bg:{Background}");

        var componentStyle = string.Join(';', styleBuilder);

        if (string.IsNullOrWhiteSpace(userStyle))
            return componentStyle;

        return $"{componentStyle};{userStyle}";
    }

    private static string ResolveCssValue(string? value, string fallback) => string.IsNullOrWhiteSpace(value) ? fallback : value;

    /// <summary>
    /// Defines the slots available for styling in <see cref="RzShimmerButton"/>.
    /// </summary>
    public sealed partial class Slots : ISlots
    {
        /// <summary>
        /// Gets or sets classes for the root shimmer button element.
        /// </summary>
        [Slot("shimmer-button")]
        public string? Base { get; set; }

        /// <summary>
        /// Gets or sets classes for the spark container layer.
        /// </summary>
        [Slot("spark-container")]
        public string? SparkContainer { get; set; }

        /// <summary>
        /// Gets or sets classes for the spark track layer.
        /// </summary>
        [Slot("spark-track")]
        public string? SparkTrack { get; set; }

        /// <summary>
        /// Gets or sets classes for the rotating spark beam layer.
        /// </summary>
        [Slot("spark-beam")]
        public string? SparkBeam { get; set; }

        /// <summary>
        /// Gets or sets classes for the child content wrapper.
        /// </summary>
        [Slot("content")]
        public string? Content { get; set; }

        /// <summary>
        /// Gets or sets classes for the highlight overlay layer.
        /// </summary>
        [Slot("highlight")]
        public string? Highlight { get; set; }

        /// <summary>
        /// Gets or sets classes for the inset backdrop layer.
        /// </summary>
        [Slot("backdrop")]
        public string? Backdrop { get; set; }
    }
}
