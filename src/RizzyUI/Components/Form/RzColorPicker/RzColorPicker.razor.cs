using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using RizzyUI.Extensions;
using System.Text.Json;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Input component that integrates Coloris for accessible color selection.
/// </summary>
public partial class RzColorPicker : InputBase<string, RzColorPicker.Slots>
{
    /// <summary>
    /// Defines the default styling descriptor for the color picker.
    /// </summary>
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(
        @base: "w-full",
        slots: new()
        {
            [s => s.InputGroup] = "w-full",
            [s => s.ThumbnailContainer] = "px-3",
            [s => s.Thumbnail] = "size-5 rounded-md border border-border shadow-sm ring-offset-background"
        }
    );

    private string _assets = "[]";
    private readonly string _inputId = $"rzcp-{Guid.NewGuid():N}";
    private string _serializedOptions = "{}";

    [Inject]
    private IOptions<RizzyUIConfig> RizzyUIConfig { get; set; } = default!;

    /// <summary>
    /// Gets or sets the output format used by Coloris.
    /// </summary>
    [Parameter] public ColorFormat Format { get; set; } = ColorFormat.Hex;

    /// <summary>
    /// Gets or sets a value indicating whether alpha channel selection is enabled.
    /// </summary>
    [Parameter] public bool Alpha { get; set; }

    /// <summary>
    /// Gets or sets predefined swatches displayed in the picker.
    /// </summary>
    [Parameter] public IEnumerable<string>? Swatches { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether only swatches should be shown.
    /// </summary>
    [Parameter] public bool SwatchesOnly { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the popup renders a close button.
    /// </summary>
    [Parameter] public bool CloseButton { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the popup renders a clear button.
    /// </summary>
    [Parameter] public bool ClearButton { get; set; }

    /// <summary>
    /// Gets or sets placeholder text for the color input.
    /// </summary>
    [Parameter] public string? Placeholder { get; set; }

    /// <summary>
    /// Gets or sets an accessible label for the input.
    /// </summary>
    [Parameter] public string? AriaLabel { get; set; }

    /// <summary>
    /// Gets or sets where the color swatch thumbnail is displayed.
    /// </summary>
    [Parameter] public AddonPosition ThumbnailPosition { get; set; } = AddonPosition.Start;

    /// <summary>
    /// Gets or sets asset keys resolved via <see cref="RizzyUIConfig"/>.
    /// </summary>
    [Parameter] public string[] ComponentAssetKeys { get; set; } = ["Coloris", "ColorisCss"];

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (string.IsNullOrEmpty(Element))
            Element = "div";

        AriaLabel ??= Localizer["RzColorPicker.DefaultAriaLabel"];
        UpdateSerializedOptions();
        UpdateAssets();
    }

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        AriaLabel ??= Localizer["RzColorPicker.DefaultAriaLabel"];
        UpdateSerializedOptions();
        UpdateAssets();
    }

    private void UpdateAssets()
    {
        var assetUrls = ComponentAssetKeys
            .Select(key => RizzyUIConfig.Value.AssetUrls.TryGetValue(key, out var url) ? url : null)
            .Where(url => !string.IsNullOrEmpty(url))
            .ToList();

        _assets = JsonSerializer.Serialize(assetUrls);
    }

    private void UpdateSerializedOptions()
    {
        var options = new ColorisOptions
        {
            Format = Format.ToString().ToLowerInvariant(),
            Alpha = Alpha,
            Swatches = Swatches,
            SwatchesOnly = SwatchesOnly,
            CloseButton = CloseButton,
            ClearButton = ClearButton,
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

        _serializedOptions = JsonSerializer.Serialize(options);
    }

    /// <inheritdoc />
    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.RzColorPicker;

    /// <summary>
    /// Slot definitions for <see cref="RzColorPicker"/>.
    /// </summary>
    public sealed partial class Slots : ISlots
    {
        /// <summary>
        /// Root slot classes.
        /// </summary>
        [Slot("color-picker")]
        public string? Base { get; set; }

        /// <summary>
        /// Input group wrapper classes.
        /// </summary>
        [Slot("input-group")]
        public string? InputGroup { get; set; }

        /// <summary>
        /// Swatch addon container classes.
        /// </summary>
        [Slot("thumbnail-container")]
        public string? ThumbnailContainer { get; set; }

        /// <summary>
        /// Swatch thumbnail classes.
        /// </summary>
        [Slot("thumbnail")]
        public string? Thumbnail { get; set; }
    }
}
