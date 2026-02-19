using System.Runtime.Serialization;

namespace RizzyUI;

/// <summary>
/// Defines supported output formats for <see cref="RzColorPicker"/>.
/// </summary>
public enum ColorFormat
{
    /// <summary>
    /// Hexadecimal format (for example, #3b82f6).
    /// </summary>
    [EnumMember(Value = "hex")]
    Hex,

    /// <summary>
    /// RGB format.
    /// </summary>
    [EnumMember(Value = "rgb")]
    Rgb,

    /// <summary>
    /// HSL format.
    /// </summary>
    [EnumMember(Value = "hsl")]
    Hsl,

    /// <summary>
    /// Automatically chooses the output format.
    /// </summary>
    [EnumMember(Value = "auto")]
    Auto,

    /// <summary>
    /// Mixed format mode.
    /// </summary>
    [EnumMember(Value = "mixed")]
    Mixed
}
