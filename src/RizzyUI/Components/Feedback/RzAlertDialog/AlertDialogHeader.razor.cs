using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// A header container for alert dialog title and description content.
/// </summary>
public partial class AlertDialogHeader : RzComponent<AlertDialogHeader.Slots>
{
    /// <summary>
    /// Defines the default styling for the AlertDialogHeader component.
    /// </summary>
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(
        @base: "flex flex-col gap-2 text-center sm:text-left"
    );

    /// <summary>
    /// Gets or sets the header content.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <inheritdoc />
    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.AlertDialogHeader;

    /// <summary>
    /// Defines the slots available for styling in the AlertDialogHeader component.
    /// </summary>
    public sealed partial class Slots : ISlots
    {
        /// <summary>
        /// The base slot for the component.
        /// </summary>
        [Slot("alert-dialog-header")]
        public string? Base { get; set; }
    }
}
