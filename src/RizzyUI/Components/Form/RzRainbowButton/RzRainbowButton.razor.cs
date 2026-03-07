using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Renders an animated rainbow-styled action button with optional as-child composition support.
/// </summary>
public partial class RzRainbowButton : RzAsChildComponent<RzRainbowButton.Slots>
{
    /// <summary>
    /// Defines the default styling for the <see cref="RzRainbowButton"/> component.
    /// </summary>
    public static readonly TvDescriptor<RzAsChildComponent<Slots>, Slots> DefaultDescriptor = new(
        @base: "relative cursor-pointer group transition-all animate-rainbow inline-flex items-center justify-center gap-2 shrink-0 rounded-sm outline-none focus-visible:ring-[3px] focus-visible:ring-ring/50 aria-invalid:border-destructive text-sm font-medium whitespace-nowrap disabled:pointer-events-none disabled:opacity-50 [&_svg]:pointer-events-none [&_svg:not([class*='size-'])]:size-4 [&_svg]:shrink-0",
        variants: new()
        {
            [b => ((RzRainbowButton)b).Variant] = new Variant<RainbowButtonVariant, Slots>
            {
                [RainbowButtonVariant.Default] = "border-0 bg-[linear-gradient(var(--color-card),var(--color-card)),linear-gradient(var(--color-card)_50%,color-mix(in_oklab,var(--color-card)_60%,transparent)_80%,transparent),linear-gradient(90deg,var(--color-1),var(--color-5),var(--color-3),var(--color-4),var(--color-2))] bg-[length:200%] text-primary-foreground [background-clip:padding-box,border-box,border-box] [background-origin:border-box] [border:calc(0.125rem)_solid_transparent] before:absolute before:bottom-[-20%] before:left-1/2 before:z-0 before:h-1/5 before:w-3/5 before:-translate-x-1/2 before:animate-rainbow before:bg-[linear-gradient(90deg,var(--color-1),var(--color-5),var(--color-3),var(--color-4),var(--color-2))] before:[filter:blur(0.75rem)]",
                [RainbowButtonVariant.Outline] = "border border-input border-b-transparent bg-[linear-gradient(var(--color-background),var(--color-background)),linear-gradient(var(--color-background)_50%,color-mix(in_oklab,var(--color-card)_60%,transparent)_80%,transparent),linear-gradient(90deg,var(--color-1),var(--color-5),var(--color-3),var(--color-4),var(--color-2))] bg-[length:200%] text-accent-foreground [background-clip:padding-box,border-box,border-box] [background-origin:border-box] before:absolute before:bottom-[-20%] before:left-1/2 before:z-0 before:h-1/5 before:w-3/5 before:-translate-x-1/2 before:animate-rainbow before:bg-[linear-gradient(90deg,var(--color-1),var(--color-5),var(--color-3),var(--color-4),var(--color-2))] before:[filter:blur(0.75rem)]"
            },
            [b => ((RzRainbowButton)b).Size] = new Variant<RainbowButtonSize, Slots>
            {
                [RainbowButtonSize.Default] = "h-9 px-4 py-2",
                [RainbowButtonSize.Small] = "h-8 rounded-xl px-3 text-xs",
                [RainbowButtonSize.Large] = "h-11 rounded-xl px-8",
                [RainbowButtonSize.Icon] = "size-9"
            }
        }
    );

    /// <summary>
    /// Gets or sets the content rendered within the button host.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the rainbow visual variant.
    /// </summary>
    [Parameter]
    public RainbowButtonVariant Variant { get; set; } = RainbowButtonVariant.Default;

    /// <summary>
    /// Gets or sets the component size.
    /// </summary>
    [Parameter]
    public RainbowButtonSize Size { get; set; } = RainbowButtonSize.Default;

    /// <summary>
    /// Gets or sets the accessible name for icon-only usage.
    /// </summary>
    [Parameter]
    public string? AriaLabel { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the native button host is disabled.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets the native button type when rendered as a button element.
    /// </summary>
    [Parameter]
    public string Type { get; set; } = "button";

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (string.IsNullOrEmpty(Element))
            Element = "button";
    }

    /// <inheritdoc/>
    protected override RenderFragment? GetAsChildContent() => ChildContent;

    /// <inheritdoc/>
    protected override Dictionary<string, object?> GetComponentAttributes()
    {
        var attributes = new Dictionary<string, object?>(AdditionalAttributes?.ToDictionary(kvp => kvp.Key, kvp => (object?)kvp.Value) ?? new Dictionary<string, object?>(), StringComparer.OrdinalIgnoreCase)
        {
            ["id"] = Id,
            ["class"] = SlotClasses.GetBase(),
            ["data-slot"] = "rainbow-button"
        };

        if (!string.IsNullOrWhiteSpace(AriaLabel))
            attributes["aria-label"] = AriaLabel;

        if (!AsChild && string.Equals(EffectiveElement, "button", StringComparison.OrdinalIgnoreCase))
        {
            attributes["type"] = string.IsNullOrWhiteSpace(Type) ? "button" : Type;
            if (Disabled)
                attributes["disabled"] = true;
        }

        return attributes;
    }

    /// <inheritdoc/>
    protected override TvDescriptor<RzAsChildComponent<Slots>, Slots> GetDescriptor() => Theme.RzRainbowButton;

    /// <summary>
    /// Defines the slots available for styling in the <see cref="RzRainbowButton"/> component.
    /// </summary>
    public sealed partial class Slots : ISlots
    {
        /// <summary>
        /// The base slot for the root element.
        /// </summary>
        [Slot("rainbow-button")]
        public string? Base { get; set; }
    }
}

/// <summary>
/// Defines the visual style variants for <see cref="RzRainbowButton"/>.
/// </summary>
public enum RainbowButtonVariant
{
    /// <summary>
    /// The default filled rainbow treatment.
    /// </summary>
    Default,

    /// <summary>
    /// The outline rainbow treatment.
    /// </summary>
    Outline
}

/// <summary>
/// Defines the size variants for <see cref="RzRainbowButton"/>.
/// </summary>
public enum RainbowButtonSize
{
    /// <summary>
    /// The standard button size.
    /// </summary>
    Default,

    /// <summary>
    /// The compact size.
    /// </summary>
    Small,

    /// <summary>
    /// The large size.
    /// </summary>
    Large,

    /// <summary>
    /// The square icon-only size.
    /// </summary>
    Icon
}
