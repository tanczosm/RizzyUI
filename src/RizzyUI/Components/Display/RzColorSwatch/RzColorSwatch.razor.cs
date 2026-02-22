using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Displays a theme-aware color swatch with optional transparency visualization.
/// </summary>
public partial class RzColorSwatch : RzComponent<RzColorSwatch.Slots>
{
    /// <summary>
    /// Defines the default styling for the <see cref="RzColorSwatch"/> component.
    /// </summary>
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(
        @base: "inline-flex align-middle",
        slots: new()
        {
            [s => s.Swatch] = "box-border rounded-sm border shadow-sm [background-clip:padding-box] forced-colors:[forced-color-adjust:none]"
        },
        variants: new()
        {
            [s => ((RzColorSwatch)s).Size] = new Variant<Size, Slots>
            {
                [Size.Small] = new() { [slot => slot.Swatch] = "size-6" },
                [Size.Medium] = new() { [slot => slot.Swatch] = "size-8" },
                [Size.Large] = new() { [slot => slot.Swatch] = "size-12" }
            },
            [s => ((RzColorSwatch)s).Disabled] = new Variant<bool, Slots>
            {
                [true] = new() { [slot => slot.Swatch] = "pointer-events-none opacity-50" }
            }
        }
    );

    /// <summary>
    /// Gets or sets the color value to render in the swatch.
    /// </summary>
    [Parameter] public string? Value { get; set; }

    /// <summary>
    /// Gets or sets the swatch size.
    /// </summary>
    [Parameter] public Size Size { get; set; } = Size.Medium;

    /// <summary>
    /// Gets or sets a value indicating whether the swatch is disabled.
    /// </summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether transparency visualization should be removed.
    /// </summary>
    [Parameter] public bool WithoutTransparency { get; set; }

    /// <summary>
    /// Gets or sets the ARIA label announced for the component.
    /// </summary>
    [Parameter] public string? AriaLabel { get; set; }

    /// <summary>
    /// Gets the effective ARIA label value used by the swatch.
    /// </summary>
    protected string EffectiveAriaLabel => string.IsNullOrWhiteSpace(AriaLabel)
        ? (string.IsNullOrWhiteSpace(Value)
            ? Localizer["RzColorSwatch.A11y.NoColorSelected"]
            : string.Format(Localizer["RzColorSwatch.A11y.ColorSwatch"], Value))
        : AriaLabel;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (string.IsNullOrEmpty(Element))
            Element = "div";
    }

    /// <inheritdoc />
    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.RzColorSwatch;

    /// <summary>
    /// Defines the slots available for styling in the <see cref="RzColorSwatch"/> component.
    /// </summary>
    public sealed partial class Slots : ISlots
    {
        /// <summary>
        /// Gets or sets classes for the root element.
        /// </summary>
        [Slot("color-swatch")]
        public string? Base { get; set; }

        /// <summary>
        /// Gets or sets classes for the swatch visual element.
        /// </summary>
        [Slot("swatch")]
        public string? Swatch { get; set; }
    }
}
