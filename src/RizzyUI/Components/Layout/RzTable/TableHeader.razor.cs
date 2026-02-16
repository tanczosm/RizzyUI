using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// A semantic table header section rendered as <c>&lt;thead&gt;</c>.
/// </summary>
public partial class TableHeader : RzComponent<TableHeader.Slots>
{
    /// <summary>
    /// Defines the default styling for the TableHeader component.
    /// </summary>
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(
        @base: "[&_tr]:border-b"
    );

    /// <summary>
    /// Gets or sets the header row content.
    /// </summary>
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = default!;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (string.IsNullOrEmpty(Element))
            Element = "thead";
    }

    /// <inheritdoc />
    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.RzSimpleTableHeader;

    /// <summary>
    /// Defines the slots available for styling in the TableHeader component.
    /// </summary>
    public sealed partial class Slots : ISlots
    {
        /// <summary>
        /// The base slot for the table header element.
        /// </summary>
        [Slot("table-header")]
        public string? Base { get; set; }
    }
}
