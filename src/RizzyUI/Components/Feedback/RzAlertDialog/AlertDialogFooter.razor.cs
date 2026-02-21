using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// A footer container for alert dialog actions.
/// </summary>
public partial class AlertDialogFooter : RzComponent<AlertDialogFooter.Slots>
{
    /// <summary>
    /// Defines the default styling for the AlertDialogFooter component.
    /// </summary>
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(
        @base: "flex flex-col-reverse gap-2 sm:flex-row sm:justify-end"
    );

    /// <summary>
    /// Gets or sets the footer content.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <inheritdoc />
    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.AlertDialogFooter;

    /// <summary>
    /// Defines the slots available for styling in the AlertDialogFooter component.
    /// </summary>
    public sealed partial class Slots : ISlots
    {
        /// <summary>
        /// The base slot for the component.
        /// </summary>
        [Slot("alert-dialog-footer")]
        public string? Base { get; set; }
    }
}
