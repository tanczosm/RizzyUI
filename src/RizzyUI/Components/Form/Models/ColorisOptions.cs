using System.Text.Json.Serialization;

namespace RizzyUI;

/// <summary>
/// Represents Coloris configuration for a single <see cref="RzColorPicker"/> instance.
/// </summary>
public sealed record ColorisOptions
{
    /// <summary>
    /// Gets the output color format.
    /// </summary>
    [JsonPropertyName("format")]
    public ColorFormat Format { get; init; } = ColorFormat.Hex;

    /// <summary>
    /// Gets a value indicating whether alpha channel support is enabled.
    /// </summary>
    [JsonPropertyName("alpha")]
    public bool Alpha { get; init; }

    /// <summary>
    /// Gets a value indicating whether only swatches are allowed.
    /// </summary>
    [JsonPropertyName("swatchesOnly")]
    public bool SwatchesOnly { get; init; }

    /// <summary>
    /// Gets a value indicating whether the close button is shown.
    /// </summary>
    [JsonPropertyName("closeButton")]
    public bool CloseButton { get; init; }

    /// <summary>
    /// Gets a value indicating whether the clear button is shown.
    /// </summary>
    [JsonPropertyName("clearButton")]
    public bool ClearButton { get; init; }

    /// <summary>
    /// Gets the list of preset swatches.
    /// </summary>
    [JsonPropertyName("swatches")]
    public IReadOnlyList<string>? Swatches { get; init; }

    /// <summary>
    /// Gets localized accessibility labels used by Coloris.
    /// </summary>
    [JsonPropertyName("a11y")]
    public ColorisA11yOptions A11y { get; init; } = new();
}

/// <summary>
/// Represents localized accessibility labels consumed by Coloris.
/// </summary>
public sealed record ColorisA11yOptions
{
    /// <summary>
    /// Gets the localized "open" label.
    /// </summary>
    [JsonPropertyName("open")]
    public string Open { get; init; } = string.Empty;

    /// <summary>
    /// Gets the localized "close" label.
    /// </summary>
    [JsonPropertyName("close")]
    public string Close { get; init; } = string.Empty;

    /// <summary>
    /// Gets the localized "clear" label.
    /// </summary>
    [JsonPropertyName("clear")]
    public string Clear { get; init; } = string.Empty;

    /// <summary>
    /// Gets the localized marker guidance text.
    /// </summary>
    [JsonPropertyName("marker")]
    public string Marker { get; init; } = string.Empty;

    /// <summary>
    /// Gets the localized hue slider label.
    /// </summary>
    [JsonPropertyName("hueSlider")]
    public string HueSlider { get; init; } = string.Empty;

    /// <summary>
    /// Gets the localized alpha slider label.
    /// </summary>
    [JsonPropertyName("alphaSlider")]
    public string AlphaSlider { get; init; } = string.Empty;

    /// <summary>
    /// Gets the localized format label.
    /// </summary>
    [JsonPropertyName("format")]
    public string Format { get; init; } = string.Empty;

    /// <summary>
    /// Gets the localized swatch label.
    /// </summary>
    [JsonPropertyName("swatch")]
    public string Swatch { get; init; } = string.Empty;
}
