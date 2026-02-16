using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Defines the slots available for styling in the <see cref="RzSimpleTable"/> component.
/// </summary>
public sealed partial class RzSimpleTableSlots : ISlots
{
    /// <summary>
    /// The base slot for the table container element.
    /// </summary>
    [Slot("table-container")]
    public string? Base { get; set; }

    /// <summary>
    /// The slot for the rendered <c>&lt;table&gt;</c> element.
    /// </summary>
    [Slot("table")]
    public string? Table { get; set; }
}
