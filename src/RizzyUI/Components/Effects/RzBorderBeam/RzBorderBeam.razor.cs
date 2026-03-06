using System.Globalization;
using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Renders a decorative animated beam that traces around a host element's border.
/// </summary>
public partial class RzBorderBeam : RzComponent<RzBorderBeam.Slots>
{
    private const double MinBeamSize = 1;
    private const double MinDuration = 0.1;
    private const string DefaultColorFrom = "#ffaa40";
    private const string DefaultColorTo = "#9c40ff";

    private static readonly CultureInfo InvariantCulture = CultureInfo.InvariantCulture;

    /// <summary>
    /// Defines the default styling for the <see cref="RzBorderBeam"/> component.
    /// </summary>
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(
        @base: "pointer-events-none absolute inset-0 rounded-[inherit]",
        slots: new()
        {
            [s => s.Track] = "absolute inset-0 rounded-[inherit] border-(length:--rz-border-beam-width) border-transparent mask-[linear-gradient(transparent,transparent),linear-gradient(#000,#000)] mask-intersect [mask-clip:padding-box,border-box]",
            [s => s.Beam] = "absolute aspect-square bg-linear-to-l from-(--rz-border-beam-color-from) via-(--rz-border-beam-color-to) to-transparent [will-change:offset-distance]"
        }
    );

    /// <summary>
    /// Gets or sets the width and height, in pixels, of the animated beam element.
    /// </summary>
    [Parameter]
    public double Size { get; set; } = 50;

    /// <summary>
    /// Gets or sets the animation cycle duration in seconds.
    /// </summary>
    [Parameter]
    public double Duration { get; set; } = 6;

    /// <summary>
    /// Gets or sets the animation delay in seconds.
    /// </summary>
    [Parameter]
    public double Delay { get; set; }

    /// <summary>
    /// Gets or sets the starting gradient color for the beam.
    /// </summary>
    [Parameter]
    public string? ColorFrom { get; set; } = DefaultColorFrom;

    /// <summary>
    /// Gets or sets the middle gradient color for the beam.
    /// </summary>
    [Parameter]
    public string? ColorTo { get; set; } = DefaultColorTo;

    /// <summary>
    /// Gets or sets whether the perimeter animation should run in reverse.
    /// </summary>
    [Parameter]
    public bool Reverse { get; set; }

    /// <summary>
    /// Gets or sets the initial animation offset percentage.
    /// </summary>
    [Parameter]
    public double InitialOffset { get; set; }

    /// <summary>
    /// Gets or sets the visible border width in pixels.
    /// </summary>
    [Parameter]
    public double BorderWidth { get; set; } = 1;

    /// <summary>
    /// Gets or sets additional classes applied to the animated beam element.
    /// </summary>
    [Parameter]
    public string? BeamClass { get; set; }

    private double ClampedSize { get; set; }

    private double ClampedDuration { get; set; }

    private double ClampedOffset { get; set; }

    private double ClampedBorderWidth { get; set; }

    private string EffectiveColorFrom { get; set; } = DefaultColorFrom;

    private string EffectiveColorTo { get; set; } = DefaultColorTo;

    private string BeamClasses => string.IsNullOrWhiteSpace(BeamClass)
        ? (SlotClasses.GetBeam() ?? string.Empty)
        : $"{SlotClasses.GetBeam() ?? string.Empty} {BeamClass}".Trim();

    private string TrackStyle => $"--rz-border-beam-width:{ClampedBorderWidth.ToString("0.###", InvariantCulture)}px;";

    private string BeamStyle => string.Join(
        string.Empty,
        $"--rz-border-beam-size:{ClampedSize.ToString("0.###", InvariantCulture)}px;",
        $"--rz-border-beam-duration:{ClampedDuration.ToString("0.###", InvariantCulture)}s;",
        $"--rz-border-beam-delay:{Delay.ToString("0.###", InvariantCulture)};",
        $"--rz-border-beam-offset:{ClampedOffset.ToString("0.###", InvariantCulture)}%;",
        $"--rz-border-beam-color-from:{EffectiveColorFrom};",
        $"--rz-border-beam-color-to:{EffectiveColorTo};",
        $"--rz-border-beam-direction:{(Reverse ? "reverse" : "normal")};",
        "width:var(--rz-border-beam-size);",
        "offset-path:rect(0 auto auto 0 round var(--rz-border-beam-size));",
        "offset-distance:var(--rz-border-beam-offset);",
        "animation:rz-border-beam var(--rz-border-beam-duration) linear infinite;",
        "animation-delay:calc(var(--rz-border-beam-delay) * -1s);",
        "animation-direction:var(--rz-border-beam-direction);"
    );

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        ClampedSize = Math.Max(MinBeamSize, Size);
        ClampedDuration = Math.Max(MinDuration, Duration);
        ClampedBorderWidth = Math.Max(0, BorderWidth);
        ClampedOffset = Math.Clamp(InitialOffset, 0, 100);
        EffectiveColorFrom = string.IsNullOrWhiteSpace(ColorFrom) ? DefaultColorFrom : ColorFrom!;
        EffectiveColorTo = string.IsNullOrWhiteSpace(ColorTo) ? DefaultColorTo : ColorTo!;
    }

    /// <inheritdoc/>
    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.RzBorderBeam;

    /// <summary>
    /// Defines the slots available for styling in the <see cref="RzBorderBeam"/> component.
    /// </summary>
    public sealed partial class Slots : ISlots
    {
        /// <summary>
        /// Gets or sets classes for the root overlay element.
        /// </summary>
        [Slot("border-beam")]
        public string? Base { get; set; }

        /// <summary>
        /// Gets or sets classes for the masked border track.
        /// </summary>
        [Slot("border-beam-track")]
        public string? Track { get; set; }

        /// <summary>
        /// Gets or sets classes for the animated beam element.
        /// </summary>
        [Slot("border-beam-beam")]
        public string? Beam { get; set; }
    }
}
