using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// A semantic table body section rendered as <c>&lt;tbody&gt;</c>.
/// </summary>
public partial class TableBodySection : RzComponent<TableBodySection.Slots>
{
    /// <summary>
    /// Defines the default styling for the TableBodySection component.
    /// </summary>
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(
        @base: "[&_tr:last-child]:border-0"
    );

    /// <summary>
    /// Gets or sets the body row content.
    /// </summary>
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = default!;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (string.IsNullOrEmpty(Element))
            Element = "tbody";
    }

    /// <inheritdoc />
    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.RzSimpleTableBodySection;

    /// <summary>
    /// Defines the slots available for styling in the TableBodySection component.
    /// </summary>
    public sealed partial class Slots : ISlots
    {
        /// <summary>
        /// The base slot for the table body element.
        /// </summary>
        [Slot("table-body")]
        public string? Base { get; set; }
    }
}
