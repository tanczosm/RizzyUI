using System.Globalization;
using Microsoft.AspNetCore.Components;
using RizzyUI.Extensions;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// A decorative overlay that renders progressive backdrop blur layers at the top, bottom, or both edges of a region.
/// </summary>
public partial class RzProgressiveBlur : RzComponent<RzProgressiveBlur.Slots>
{
    private static readonly double[] DefaultBlurLevels = [0.5, 1, 2, 4, 8, 16, 32, 64];

    /// <summary>
    /// Defines the default styling for the RzProgressiveBlur component.
    /// </summary>
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(
        @base: "pointer-events-none absolute inset-x-0",
        slots: new()
        {
            [s => s.Layer] = "absolute inset-0"
        },
        variants: new()
        {
            [c => ((RzProgressiveBlur)c).Position] = new Variant<ProgressiveBlurPosition, Slots>
            {
                [ProgressiveBlurPosition.Top] = "top-0",
                [ProgressiveBlurPosition.Bottom] = "bottom-0",
                [ProgressiveBlurPosition.Both] = "inset-y-0"
            }
        }
    );

    /// <summary>
    /// Gets or sets the position of the blur overlay.
    /// </summary>
    [Parameter] public ProgressiveBlurPosition Position { get; set; } = ProgressiveBlurPosition.Bottom;

    /// <summary>
    /// Gets or sets the CSS height of the overlay when <see cref="Position"/> is Top or Bottom.
    /// </summary>
    [Parameter] public string? Height { get; set; } = "30%";

    /// <summary>
    /// Gets or sets the ordered blur levels in pixels from weakest to strongest.
    /// </summary>
    [Parameter] public IReadOnlyList<double>? BlurLevels { get; set; }

    /// <summary>
    /// Gets or sets an optional z-index value for the overlay root.
    /// </summary>
    [Parameter] public int? ZIndex { get; set; } = 10;

    /// <summary>
    /// Gets or sets an optional inset-inline-start override.
    /// </summary>
    [Parameter] public string? InsetStart { get; set; }

    /// <summary>
    /// Gets or sets an optional inset-inline-end override.
    /// </summary>
    [Parameter] public string? InsetEnd { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether assistive technology should ignore this decorative overlay.
    /// </summary>
    [Parameter] public bool AriaHidden { get; set; } = true;

    /// <summary>
    /// Gets the CSS style string used by the root element.
    /// </summary>
    protected string RootStyle => BuildRootStyle();

    /// <summary>
    /// Gets the normalized and clamped blur values used for rendering.
    /// </summary>
    protected IReadOnlyList<double> EffectiveBlurLevels => NormalizeBlurLevels();

    /// <inheritdoc />
    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.RzProgressiveBlur;

    /// <summary>
    /// Builds the layer styles and identity metadata for rendering.
    /// </summary>
    protected IReadOnlyList<BlurLayer> BuildLayers()
    {
        var blurLevels = EffectiveBlurLevels;
        var layers = new List<BlurLayer>(blurLevels.Count);

        if (blurLevels.Count == 1)
        {
            layers.Add(new BlurLayer(0, "single", BuildLayerStyle(blurLevels[0], BuildBothMaskGradient(), 1)));
            return layers;
        }

        layers.Add(new BlurLayer(0, "first", BuildLayerStyle(blurLevels[0], BuildFirstMaskGradient(), 1)));

        for (var blurIndex = 1; blurIndex < blurLevels.Count - 1; blurIndex++)
        {
            layers.Add(new BlurLayer(blurIndex, "middle", BuildLayerStyle(blurLevels[blurIndex], BuildMiddleMaskGradient(blurIndex), blurIndex + 1)));
        }

        var lastIndex = blurLevels.Count - 1;
        layers.Add(new BlurLayer(lastIndex, "last", BuildLayerStyle(blurLevels[lastIndex], BuildLastMaskGradient(), blurLevels.Count)));

        return layers;
    }

    private IReadOnlyList<double> NormalizeBlurLevels()
    {
        if (BlurLevels is null || BlurLevels.Count == 0)
            return DefaultBlurLevels;

        return BlurLevels.Select(level => Math.Max(0, level)).ToArray();
    }

    private string BuildRootStyle()
    {
        var styles = new List<string>
        {
            $"height: {(Position == ProgressiveBlurPosition.Both ? "100%" : Height ?? "30%")};"
        };

        if (ZIndex.HasValue)
            styles.Add($"z-index: {ZIndex.Value.ToString(CultureInfo.InvariantCulture)};");

        if (!string.IsNullOrWhiteSpace(InsetStart))
            styles.Add($"inset-inline-start: {InsetStart};");

        if (!string.IsNullOrWhiteSpace(InsetEnd))
            styles.Add($"inset-inline-end: {InsetEnd};");

        return string.Join(' ', styles);
    }

    private string BuildLayerStyle(double blurValue, string maskGradient, int zIndex)
    {
        var blur = blurValue.ToString(CultureInfo.InvariantCulture);
        return $"z-index: {zIndex.ToString(CultureInfo.InvariantCulture)}; backdrop-filter: blur({blur}px); -webkit-backdrop-filter: blur({blur}px); mask-image: {maskGradient}; -webkit-mask-image: {maskGradient};";
    }

    private string BuildFirstMaskGradient()
    {
        if (Position == ProgressiveBlurPosition.Both)
            return BuildBothMaskGradient();

        var direction = Position == ProgressiveBlurPosition.Bottom ? "to bottom" : "to top";
        return $"linear-gradient({direction}, rgba(0,0,0,0) 0%, rgba(0,0,0,1) 12.5%, rgba(0,0,0,1) 25%, rgba(0,0,0,0) 37.5%)";
    }

    private string BuildMiddleMaskGradient(int blurIndex)
    {
        if (Position == ProgressiveBlurPosition.Both)
            return BuildBothMaskGradient();

        var direction = Position == ProgressiveBlurPosition.Bottom ? "to bottom" : "to top";
        var startPercent = blurIndex * 12.5;
        var midPercent = (blurIndex + 1) * 12.5;
        var endPercent = (blurIndex + 2) * 12.5;
        var tailPercent = endPercent + 12.5;

        return $"linear-gradient({direction}, rgba(0,0,0,0) {FormatPercent(startPercent)}%, rgba(0,0,0,1) {FormatPercent(midPercent)}%, rgba(0,0,0,1) {FormatPercent(endPercent)}%, rgba(0,0,0,0) {FormatPercent(tailPercent)}%)";
    }

    private string BuildLastMaskGradient()
    {
        if (Position == ProgressiveBlurPosition.Both)
            return BuildBothMaskGradient();

        var direction = Position == ProgressiveBlurPosition.Bottom ? "to bottom" : "to top";
        return $"linear-gradient({direction}, rgba(0,0,0,0) 87.5%, rgba(0,0,0,1) 100%)";
    }

    private static string BuildBothMaskGradient() =>
        "linear-gradient(rgba(0,0,0,0) 0%, rgba(0,0,0,1) 5%, rgba(0,0,0,1) 95%, rgba(0,0,0,0) 100%)";

    private static string FormatPercent(double value) => value.ToString("0.###", CultureInfo.InvariantCulture);

    /// <summary>
    /// Represents a rendered blur layer with index, kind, and inline style.
    /// </summary>
    protected sealed record BlurLayer(int Index, string Kind, string Style);

    /// <summary>
    /// Defines the slots available for styling in the RzProgressiveBlur component.
    /// </summary>
    public sealed partial class Slots : ISlots
    {
        /// <summary>
        /// The base slot for the progressive blur root.
        /// </summary>
        [Slot("progressive-blur")]
        public string? Base { get; set; }

        /// <summary>
        /// The slot for each generated blur layer.
        /// </summary>
        [Slot("layer")]
        public string? Layer { get; set; }
    }
}
