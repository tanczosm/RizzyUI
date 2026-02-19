using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// A themed, SSR-first color input that integrates with Coloris for interactive color selection.
/// </summary>
public partial class RzColorPicker : InputBase<string, RzColorPicker.Slots>
{
    /// <summary>
    /// Defines the default styling for the <see cref="RzColorPicker"/> component.
    /// </summary>
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(
        @base: "w-full",
        slots: new()
        {
            [s => s.InputGroup] = "w-full",
            [s => s.Input] = "",
            [s => s.ThumbnailContainer] = "px-3",
            [s => s.Thumbnail] = "size-5 rounded-md border border-border shadow-sm"
        }
    );

    private string _assets = "[]";
    private string _serializedConfig = "{}";
    private readonly string _inputId = $"rz-color-picker-{Guid.NewGuid():N}";

    [Inject]
    private IOptions<RizzyUIConfig> RizzyUIConfig { get; set; } = default!;

    /// <summary>
    /// Gets or sets the output format emitted by Coloris.
    /// </summary>
    [Parameter]
    public ColorFormat Format { get; set; } = ColorFormat.Hex;

    /// <summary>
    /// Gets or sets a value indicating whether alpha channel editing is enabled.
    /// </summary>
    [Parameter]
    public bool Alpha { get; set; }

    /// <summary>
    /// Gets or sets the set of preset swatches shown by Coloris.
    /// </summary>
    [Parameter]
    public IEnumerable<string>? Swatches { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the picker is limited to swatch selection.
    /// </summary>
    [Parameter]
    public bool SwatchesOnly { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the picker close button is visible.
    /// </summary>
    [Parameter]
    public bool CloseButton { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the picker clear button is visible.
    /// </summary>
    [Parameter]
    public bool ClearButton { get; set; }

    /// <summary>
    /// Gets or sets placeholder text rendered inside the text input.
    /// </summary>
    [Parameter]
    public string? Placeholder { get; set; }

    /// <summary>
    /// Gets or sets the position of the color thumbnail relative to the input.
    /// </summary>
    [Parameter]
    public ColorPickerThumbnailPosition ThumbnailPosition { get; set; } = ColorPickerThumbnailPosition.Start;

    /// <summary>
    /// Gets or sets the ARIA label for the text input.
    /// </summary>
    [Parameter]
    public string? AriaLabel { get; set; }

    /// <summary>
    /// Gets or sets the logical asset keys required to load Coloris.
    /// </summary>
    [Parameter]
    public string[] ComponentAssetKeys { get; set; } = ["Coloris", "ColorisCss"];

    private RenderFragment ThumbnailAddon => builder =>
    {
        builder.OpenComponent<InputGroupAddon>(0);
        builder.AddAttribute(1, "Align", ThumbnailPosition == ColorPickerThumbnailPosition.Start ? InputGroupAddonAlign.InlineStart : InputGroupAddonAlign.InlineEnd);
        builder.AddAttribute(2, "class", SlotClasses.GetThumbnailContainer());
        builder.AddAttribute(3, "data-slot", SlotNames.NameOf(SlotTypes.ThumbnailContainer));
        builder.AddAttribute(4, "ChildContent", (RenderFragment)(addonBuilder =>
        {
            addonBuilder.OpenElement(5, "div");
            addonBuilder.AddAttribute(6, "class", SlotClasses.GetThumbnail());
            addonBuilder.AddAttribute(7, "data-slot", SlotNames.NameOf(SlotTypes.Thumbnail));
            addonBuilder.AddAttribute(8, "x-bind:style", "thumbnailStyle");
            addonBuilder.AddAttribute(9, "aria-hidden", "true");
            addonBuilder.CloseElement();
        }));
        builder.CloseComponent();
    };

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (string.IsNullOrEmpty(Element))
            Element = "div";

        ApplyLocalizedDefaults();
        UpdateConfigAndAssets();
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        ApplyLocalizedDefaults();
        UpdateConfigAndAssets();
    }

    private void ApplyLocalizedDefaults()
    {
        AriaLabel ??= Localizer["RzColorPicker.A11y.Open"];
    }

    private void UpdateConfigAndAssets()
    {
        var options = new ColorisOptions
        {
            Format = Format,
            Alpha = Alpha,
            SwatchesOnly = SwatchesOnly,
            CloseButton = CloseButton,
            ClearButton = ClearButton,
            Swatches = Swatches?.ToList(),
            A11y = new ColorisA11yOptions
            {
                Open = Localizer["RzColorPicker.A11y.Open"],
                Close = Localizer["RzColorPicker.A11y.Close"],
                Clear = Localizer["RzColorPicker.A11y.Clear"],
                Marker = Localizer["RzColorPicker.A11y.Marker"],
                HueSlider = Localizer["RzColorPicker.A11y.HueSlider"],
                AlphaSlider = Localizer["RzColorPicker.A11y.AlphaSlider"],
                Format = Localizer["RzColorPicker.A11y.Format"],
                Swatch = Localizer["RzColorPicker.A11y.Swatch"]
            }
        };

        _serializedConfig = JsonSerializer.Serialize(options, new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var assetUrls = ComponentAssetKeys
            .Select(key => RizzyUIConfig.Value.AssetUrls.TryGetValue(key, out var url) ? url : null)
            .Where(url => !string.IsNullOrEmpty(url))
            .ToList();

        _assets = JsonSerializer.Serialize(assetUrls);
    }

    /// <inheritdoc />
    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.RzColorPicker;

    /// <summary>
    /// Defines the slots available for styling in the <see cref="RzColorPicker"/> component.
    /// </summary>
    public sealed partial class Slots : ISlots
    {
        /// <summary>
        /// The root element slot.
        /// </summary>
        [Slot("color-picker")]
        public string? Base { get; set; }

        /// <summary>
        /// The input group container slot.
        /// </summary>
        [Slot("input-group")]
        public string? InputGroup { get; set; }

        /// <summary>
        /// The text input slot.
        /// </summary>
        [Slot("input")]
        public string? Input { get; set; }

        /// <summary>
        /// The thumbnail addon container slot.
        /// </summary>
        [Slot("thumbnail-container")]
        public string? ThumbnailContainer { get; set; }

        /// <summary>
        /// The color preview thumbnail slot.
        /// </summary>
        [Slot("thumbnail")]
        public string? Thumbnail { get; set; }
    }
}
