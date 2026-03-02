using System.Globalization;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Provides a server-rendered, Alpine-enhanced slider supporting single-value and range selection.
/// </summary>
public partial class RzSlider : RzComponent<RzSlider.Slots>
{
    /// <summary>
    /// Defines the default styling for the <see cref="RzSlider"/> component.
    /// </summary>
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(
        @base: "relative flex w-full touch-none select-none items-center data-[disabled=true]:pointer-events-none data-[disabled=true]:opacity-50 data-[orientation=vertical]:h-44 data-[orientation=vertical]:w-3 data-[orientation=vertical]:justify-center",
        slots: new()
        {
            [s => s.Track] = "bg-muted relative grow overflow-hidden rounded-full data-[orientation=horizontal]:h-2 data-[orientation=vertical]:h-full data-[orientation=vertical]:w-2",
            [s => s.Range] = "bg-primary absolute data-[orientation=horizontal]:h-full data-[orientation=vertical]:w-full",
            [s => s.Thumb] = "border-primary bg-background ring-offset-background absolute block size-5 rounded-full border shadow-sm transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:pointer-events-none disabled:opacity-50",
            [s => s.Input] = "sr-only"
        },
        variants: new()
        {
            [c => ((RzSlider)c).Orientation] = new Variant<SliderOrientation, Slots>
            {
                [SliderOrientation.Horizontal] = new()
                {
                    [s => s.Base] = "h-5",
                    [s => s.Thumb] = "top-1/2 -translate-y-1/2"
                },
                [SliderOrientation.Vertical] = new()
                {
                    [s => s.Base] = "flex-col",
                    [s => s.Thumb] = "left-1/2 -translate-x-1/2"
                }
            },
            [c => ((RzSlider)c).Disabled] = new Variant<bool, Slots>
            {
                [true] = new()
                {
                    [s => s.Base] = "data-[disabled=true]:cursor-not-allowed",
                    [s => s.Thumb] = "pointer-events-none"
                }
            }
        }
    );

    /// <summary>
    /// Gets or sets the controlled slider values.
    /// </summary>
    [Parameter] public double[]? Value { get; set; }

    /// <summary>
    /// Gets or sets the fallback slider values when <see cref="Value"/> is not supplied.
    /// </summary>
    [Parameter] public double[]? DefaultValue { get; set; }

    /// <summary>
    /// Gets or sets the minimum selectable value.
    /// </summary>
    [Parameter] public double Min { get; set; }

    /// <summary>
    /// Gets or sets the maximum selectable value.
    /// </summary>
    [Parameter] public double Max { get; set; } = 100;

    /// <summary>
    /// Gets or sets the step interval used for value snapping.
    /// </summary>
    [Parameter] public double Step { get; set; } = 1;

    /// <summary>
    /// Gets or sets the slider orientation.
    /// </summary>
    [Parameter] public SliderOrientation Orientation { get; set; } = SliderOrientation.Horizontal;

    /// <summary>
    /// Gets or sets a value indicating whether the slider is disabled.
    /// </summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the visual mapping should be inverted.
    /// </summary>
    [Parameter] public bool Inverted { get; set; }

    /// <summary>
    /// Gets or sets the minimum distance between adjacent thumbs in value space.
    /// </summary>
    [Parameter] public double MinStepsBetweenThumbs { get; set; }

    /// <summary>
    /// Gets or sets the base form name used for hidden input fields.
    /// </summary>
    [Parameter] public string? Name { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether hidden input fields should use indexed names.
    /// </summary>
    [Parameter] public bool UseIndexedNames { get; set; } = true;

    /// <summary>
    /// Gets or sets the default aria-label for single-thumb mode.
    /// </summary>
    [Parameter] public string? AriaLabel { get; set; }

    /// <summary>
    /// Gets or sets per-thumb aria-labels for multi-thumb mode.
    /// </summary>
    [Parameter] public string[]? AriaLabels { get; set; }

    /// <summary>
    /// Gets or sets the ID reference for aria-labelledby.
    /// </summary>
    [Parameter] public string? AriaLabelledBy { get; set; }

    /// <summary>
    /// Gets or sets the ID reference for aria-describedby.
    /// </summary>
    [Parameter] public string? AriaDescribedBy { get; set; }

    /// <summary>
    /// Gets or sets an optional format hint for value display.
    /// </summary>
    [Parameter] public string? ValueFormat { get; set; }

    /// <summary>
    /// Gets or sets the tabindex value applied to each thumb.
    /// </summary>
    [Parameter] public int? TabIndex { get; set; }

    /// <summary>
    /// Gets the effective values after sanitization.
    /// </summary>
    protected IReadOnlyList<double> EffectiveValues { get; private set; } = [0];

    /// <summary>
    /// Gets the JSON serialized value array used by Alpine initialization.
    /// </summary>
    protected string SerializedValues { get; private set; } = "[0]";

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        AriaLabel ??= Localizer["RzSlider.DefaultAriaLabel"];
        ResolveAndSanitizeValues();
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        AriaLabel ??= Localizer["RzSlider.DefaultAriaLabel"];
        ResolveAndSanitizeValues();
    }

    /// <summary>
    /// Generates the inline style for the range fill element.
    /// </summary>
    /// <returns>The style string for range positioning.</returns>
    protected string GetRangeStyle()
    {
        if (EffectiveValues.Count == 0)
            return Orientation == SliderOrientation.Vertical ? "bottom:0;height:0%;" : "left:0;width:0%;";

        var end = GetPercent(EffectiveValues[^1]);
        var start = EffectiveValues.Count > 1 ? GetPercent(EffectiveValues[0]) : (Inverted ? end : 0d);
        var min = Math.Min(start, end);
        var span = Math.Abs(end - start);

        if (Orientation == SliderOrientation.Vertical)
            return $"bottom:{min.ToString("0.###", CultureInfo.InvariantCulture)}%;height:{span.ToString("0.###", CultureInfo.InvariantCulture)}%;";

        return $"left:{min.ToString("0.###", CultureInfo.InvariantCulture)}%;width:{span.ToString("0.###", CultureInfo.InvariantCulture)}%;";
    }

    /// <summary>
    /// Generates the inline style for a thumb element.
    /// </summary>
    /// <param name="index">The thumb index.</param>
    /// <returns>The style string for thumb positioning.</returns>
    protected string GetThumbStyle(int index)
    {
        var percent = GetPercent(EffectiveValues[index]).ToString("0.###", CultureInfo.InvariantCulture);
        return Orientation == SliderOrientation.Vertical ? $"bottom:{percent}%;" : $"left:{percent}%;";
    }

    /// <summary>
    /// Computes the hidden input name for the specified thumb.
    /// </summary>
    /// <param name="index">The thumb index.</param>
    /// <returns>The hidden input name.</returns>
    protected string BuildInputName(int index)
    {
        if (string.IsNullOrWhiteSpace(Name))
            return string.Empty;

        return UseIndexedNames ? $"{Name}[{index}]" : Name;
    }

    /// <summary>
    /// Resolves the accessible label for a thumb.
    /// </summary>
    /// <param name="index">The thumb index.</param>
    /// <returns>The resolved thumb label.</returns>
    protected string GetThumbAriaLabel(int index)
    {
        if (AriaLabels is not null && index < AriaLabels.Length && !string.IsNullOrWhiteSpace(AriaLabels[index]))
            return AriaLabels[index];

        if (EffectiveValues.Count == 1)
            return AriaLabel ?? Localizer["RzSlider.DefaultAriaLabel"];

        if (index == 0)
            return Localizer["RzSlider.MinimumThumbAriaLabel"];

        if (index == EffectiveValues.Count - 1)
            return Localizer["RzSlider.MaximumThumbAriaLabel"];

        return $"{Localizer["RzSlider.DefaultAriaLabel"]} {index + 1}";
    }

    /// <inheritdoc />
    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.RzSlider;

    private void ResolveAndSanitizeValues()
    {
        var candidateValues = (Value is { Length: > 0 } ? Value : DefaultValue) ?? [Min];
        if (candidateValues.Length == 0)
            candidateValues = [Min];

        var sanitized = candidateValues.Select(SanitizeSingleValue).ToList();

        if (sanitized.Count > 1)
            sanitized.Sort();

        EnforceMinimumDistance(sanitized);

        EffectiveValues = sanitized;
        SerializedValues = JsonSerializer.Serialize(sanitized);
    }

    private double SanitizeSingleValue(double rawValue)
    {
        var effectiveStep = Step <= 0 ? 1 : Step;
        var clamped = Math.Clamp(rawValue, Min, Max);
        var snapped = Math.Round((clamped - Min) / effectiveStep, MidpointRounding.AwayFromZero) * effectiveStep + Min;
        return Math.Clamp(snapped, Min, Max);
    }

    private void EnforceMinimumDistance(List<double> values)
    {
        if (values.Count < 2 || MinStepsBetweenThumbs <= 0)
            return;

        for (var i = 1; i < values.Count; i++)
        {
            var minimumAllowed = values[i - 1] + MinStepsBetweenThumbs;
            if (values[i] < minimumAllowed)
                values[i] = Math.Min(Max, minimumAllowed);
        }

        for (var i = values.Count - 2; i >= 0; i--)
        {
            var maximumAllowed = values[i + 1] - MinStepsBetweenThumbs;
            if (values[i] > maximumAllowed)
                values[i] = Math.Max(Min, maximumAllowed);
        }

        for (var i = 0; i < values.Count; i++)
            values[i] = SanitizeSingleValue(values[i]);
    }

    private double GetPercent(double value)
    {
        if (Math.Abs(Max - Min) < double.Epsilon)
            return 0;

        var ratio = (value - Min) / (Max - Min);
        var percent = Math.Clamp(ratio * 100d, 0d, 100d);
        return Inverted ? 100d - percent : percent;
    }

    /// <summary>
    /// Defines the available styling slots for the <see cref="RzSlider"/> component.
    /// </summary>
    public sealed partial class Slots : ISlots
    {
        /// <summary>
        /// Gets or sets base classes for the root slider element.
        /// </summary>
        [Slot("slider")]
        public string? Base { get; set; }

        /// <summary>
        /// Gets or sets classes for the slider track element.
        /// </summary>
        [Slot("slider-track")]
        public string? Track { get; set; }

        /// <summary>
        /// Gets or sets classes for the slider range element.
        /// </summary>
        [Slot("slider-range")]
        public string? Range { get; set; }

        /// <summary>
        /// Gets or sets classes for slider thumb elements.
        /// </summary>
        [Slot("slider-thumb")]
        public string? Thumb { get; set; }

        /// <summary>
        /// Gets or sets classes for hidden input elements.
        /// </summary>
        [Slot("slider-input")]
        public string? Input { get; set; }
    }
}

/// <summary>
/// Specifies the orientation options for <see cref="RzSlider"/>.
/// </summary>
public enum SliderOrientation
{
    /// <summary>
    /// Renders the slider horizontally.
    /// </summary>
    Horizontal,

    /// <summary>
    /// Renders the slider vertically.
    /// </summary>
    Vertical
}
