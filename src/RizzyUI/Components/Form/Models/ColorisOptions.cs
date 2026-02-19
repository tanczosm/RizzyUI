using System.Text.Json.Serialization;

namespace RizzyUI;

/// <summary>
/// Represents configuration values passed to the Coloris color picker library.
/// </summary>
public record ColorisOptions
{
    /// <summary>
    /// Gets or sets the output format for selected values.
    /// </summary>
    [JsonPropertyName("format")]
    public string Format { get; init; } = "hex";

    /// <summary>
    /// Gets or sets a value indicating whether alpha channel support is enabled.
    /// </summary>
    [JsonPropertyName("alpha")]
    public bool Alpha { get; init; }

    /// <summary>
    /// Gets or sets swatches rendered by Coloris.
    /// </summary>
    [JsonPropertyName("swatches")]
    public IEnumerable<string>? Swatches { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether only swatches are displayed.
    /// </summary>
    [JsonPropertyName("swatchesOnly")]
    public bool SwatchesOnly { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether a close button is shown.
    /// </summary>
    [JsonPropertyName("closeButton")]
    public bool CloseButton { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether a clear button is shown.
    /// </summary>
    [JsonPropertyName("clearButton")]
    public bool ClearButton { get; init; }

    /// <summary>
    /// Gets or sets accessibility strings for Coloris.
    /// </summary>
    [JsonPropertyName("a11y")]
    public ColorisA11yOptions A11y { get; init; } = new();
}

/// <summary>
/// Accessibility labels used by Coloris.
/// </summary>
public record ColorisA11yOptions
{
    /// <summary>
    /// Gets or sets the open-picker label.
    /// </summary>
    [JsonPropertyName("open")]
    public string Open { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the close-picker label.
    /// </summary>
    [JsonPropertyName("close")]
    public string Close { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the clear label.
    /// </summary>
    [JsonPropertyName("clear")]
    public string Clear { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the marker label.
    /// </summary>
    [JsonPropertyName("marker")]
    public string Marker { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the hue slider label.
    /// </summary>
    [JsonPropertyName("hueSlider")]
    public string HueSlider { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the alpha slider label.
    /// </summary>
    [JsonPropertyName("alphaSlider")]
    public string AlphaSlider { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the format selector label.
    /// </summary>
    [JsonPropertyName("format")]
    public string Format { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the swatch label.
    /// </summary>
    [JsonPropertyName("swatch")]
    public string Swatch { get; init; } = string.Empty;
}
