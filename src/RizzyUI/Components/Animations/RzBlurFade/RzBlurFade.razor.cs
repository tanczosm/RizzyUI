using System.Globalization;
using Microsoft.AspNetCore.Components;
using RizzyUI.Extensions;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Provides an SSR-safe wrapper that progressively enhances child content with blur-and-offset reveal transitions.
/// </summary>
public partial class RzBlurFade : RzComponent<RzBlurFade.Slots>
{
    /// <summary>
    /// Defines the default styling for <see cref="RzBlurFade"/>.
    /// </summary>
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(
        @base: "block",
        slots: new()
        {
            [s => s.Viewport] = "block transform-gpu will-change-[transform,opacity,filter] transition-[opacity,transform,filter] ease-out motion-reduce:transition-none motion-reduce:transform-none"
        });

    private const double DefaultOffset = 6d;
    private const double DefaultDuration = 0.4d;
    private const double DefaultDelay = 0d;
    private const string DefaultBlur = "6px";
    private const string DefaultInViewMargin = "-50px";
    private string _assets = "[]";

    /// <summary>
    /// Gets or sets the wrapped content to reveal.
    /// </summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the initial reveal direction.
    /// </summary>
    [Parameter] public BlurFadeDirection Direction { get; set; } = BlurFadeDirection.Down;

    /// <summary>
    /// Gets or sets the initial translate offset in CSS pixels.
    /// </summary>
    [Parameter] public double Offset { get; set; } = DefaultOffset;

    /// <summary>
    /// Gets or sets the reveal duration in seconds.
    /// </summary>
    [Parameter] public double Duration { get; set; } = DefaultDuration;

    /// <summary>
    /// Gets or sets the reveal delay in seconds.
    /// </summary>
    [Parameter] public double Delay { get; set; } = DefaultDelay;

    /// <summary>
    /// Gets or sets the initial blur radius as a CSS length.
    /// </summary>
    [Parameter] public string Blur { get; set; } = DefaultBlur;

    /// <summary>
    /// Gets or sets whether reveal should wait for first viewport intersection.
    /// </summary>
    [Parameter] public bool InView { get; set; }

    /// <summary>
    /// Gets or sets the intersection root margin used when <see cref="InView"/> is true.
    /// </summary>
    [Parameter] public string InViewMargin { get; set; } = DefaultInViewMargin;

    /// <summary>
    /// Gets the normalized direction value emitted to Alpine configuration.
    /// </summary>
    protected string DirectionValue => Direction.ToString().ToKebabCase();

    /// <summary>
    /// Gets the normalized offset value emitted to Alpine configuration.
    /// </summary>
    protected string OffsetValue => ClampToZero(Offset).ToString("0.###", CultureInfo.InvariantCulture);

    /// <summary>
    /// Gets the normalized duration value emitted to Alpine configuration.
    /// </summary>
    protected string DurationValue => ClampToZero(Duration).ToString("0.###", CultureInfo.InvariantCulture);

    /// <summary>
    /// Gets the normalized delay value emitted to Alpine configuration.
    /// </summary>
    protected string DelayValue => ClampToZero(Delay).ToString("0.###", CultureInfo.InvariantCulture);

    /// <summary>
    /// Gets the normalized blur value emitted to Alpine configuration.
    /// </summary>
    protected string BlurValue => string.IsNullOrWhiteSpace(Blur) ? DefaultBlur : Blur;

    /// <summary>
    /// Gets the normalized in-view margin value emitted to Alpine configuration.
    /// </summary>
    protected string InViewMarginValue => string.IsNullOrWhiteSpace(InViewMargin) ? DefaultInViewMargin : InViewMargin;

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        _assets = "[]";
    }

    /// <inheritdoc/>
    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.RzBlurFade;

    private static double ClampToZero(double value) => value < 0 ? 0 : value;

    /// <summary>
    /// Defines the slots available for styling in <see cref="RzBlurFade"/>.
    /// </summary>
    public sealed partial class Slots : ISlots
    {
        /// <summary>
        /// Gets or sets classes for the root element.
        /// </summary>
        [Slot("blur-fade")]
        public string? Base { get; set; }

        /// <summary>
        /// Gets or sets classes for the Alpine viewport element.
        /// </summary>
        [Slot("viewport")]
        public string? Viewport { get; set; }
    }
}
