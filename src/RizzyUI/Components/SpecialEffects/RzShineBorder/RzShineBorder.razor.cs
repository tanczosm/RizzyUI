using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Renders a decorative animated shine overlay that can be layered over a relatively positioned container.
/// </summary>
public partial class RzShineBorder : RzComponent<RzShineBorder.Slots>
{
    /// <summary>
    /// Defines the default styling for the <see cref="RzShineBorder"/> component.
    /// </summary>
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(
        @base: "pointer-events-none absolute inset-0 size-full rounded-[inherit] will-change-[background-position]",
        variants: new()
        {
            [c => ((RzShineBorder)c).RespectReducedMotion] = new Variant<bool, Slots>
            {
                [true] = new() { [s => s.Base] = "motion-safe:animate-shine" },
                [false] = new() { [s => s.Base] = "animate-shine" }
            }
        }
    );

    /// <summary>
    /// Gets or sets the border width, in pixels.
    /// </summary>
    [Parameter]
    public double BorderWidth { get; set; } = 1;

    /// <summary>
    /// Gets or sets the animation duration, in seconds.
    /// </summary>
    [Parameter]
    public double Duration { get; set; } = 14;

    /// <summary>
    /// Gets or sets a single CSS color used by the shine effect.
    /// </summary>
    [Parameter]
    public string? ShineColor { get; set; }

    /// <summary>
    /// Gets or sets a sequence of CSS colors used for a multi-color shine effect.
    /// When provided with at least one value, this takes precedence over <see cref="ShineColor"/>.
    /// </summary>
    [Parameter]
    public IEnumerable<string>? ShineColors { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether motion-safe animation utility classes should be used.
    /// </summary>
    [Parameter]
    public bool RespectReducedMotion { get; set; } = true;

    /// <summary>
    /// Gets the computed inline style for the shine border effect.
    /// </summary>
    protected string ComputedStyle { get; private set; } = string.Empty;

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        ComputedStyle = BuildStyle();
    }

    /// <inheritdoc />
    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.RzShineBorder;

    private string BuildStyle()
    {
        var normalizedBorderWidth = Math.Max(0, BorderWidth);
        var normalizedDuration = Duration <= 0 ? 14 : Duration;

        var multiColors = ShineColors?.Where(color => !string.IsNullOrWhiteSpace(color)).ToArray();
        var gradientColors = multiColors is { Length: > 0 }
            ? string.Join(",", multiColors)
            : !string.IsNullOrWhiteSpace(ShineColor)
                ? ShineColor!
                : "#000000";

        var builder = new StringBuilder();
        builder.Append($"--border-width:{normalizedBorderWidth.ToString(CultureInfo.InvariantCulture)}px;");
        builder.Append($"--duration:{normalizedDuration.ToString(CultureInfo.InvariantCulture)}s;");
        builder.Append($"background-image:radial-gradient(transparent,transparent,{gradientColors},transparent,transparent);");
        builder.Append("background-size:300% 300%;");
        builder.Append("mask:linear-gradient(#fff 0 0) content-box,linear-gradient(#fff 0 0);");
        builder.Append("-webkit-mask:linear-gradient(#fff 0 0) content-box,linear-gradient(#fff 0 0);");
        builder.Append("-webkit-mask-composite:xor;");
        builder.Append("mask-composite:exclude;");
        builder.Append("padding:var(--border-width);");

        var userStyle = AdditionalAttributes?.TryGetValue("style", out var styleValue) == true
            ? styleValue?.ToString()
            : null;

        if (!string.IsNullOrWhiteSpace(userStyle))
        {
            if (!userStyle!.TrimEnd().EndsWith(';'))
            {
                builder.Append(';');
            }

            builder.Append(userStyle);
        }

        return builder.ToString();
    }

    /// <summary>
    /// Defines the slots available for styling in the <see cref="RzShineBorder"/> component.
    /// </summary>
    public sealed partial class Slots : ISlots
    {
        /// <summary>
        /// The base slot for the component's root element.
        /// </summary>
        [Slot("shine-border")]
        public string? Base { get; set; }
    }
}
