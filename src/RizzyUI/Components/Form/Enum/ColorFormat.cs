using System.Text.Json.Serialization;

namespace RizzyUI;

/// <summary>
/// Defines the output format used by <see cref="RzColorPicker"/>.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ColorFormat
{
    /// <summary>
    /// Hexadecimal output (for example, #3b82f6).
    /// </summary>
    Hex,

    /// <summary>
    /// RGB functional output (for example, rgb(59,130,246)).
    /// </summary>
    Rgb,

    /// <summary>
    /// HSL functional output (for example, hsl(217, 91%, 60%)).
    /// </summary>
    Hsl,

    /// <summary>
    /// Lets Coloris infer the output format.
    /// </summary>
    Auto,

    /// <summary>
    /// Allows mixed format output values.
    /// </summary>
    Mixed
}
