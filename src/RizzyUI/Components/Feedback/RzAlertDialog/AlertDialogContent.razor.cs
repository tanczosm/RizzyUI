using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// The modal content container for an <see cref="RzAlertDialog"/>.
/// </summary>
public partial class AlertDialogContent : RzComponent<AlertDialogContent.Slots>
{
    /// <summary>
    /// Defines the default styling for the AlertDialogContent component.
    /// </summary>
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(
        @base: "fixed top-[50%] left-[50%] z-50 grid w-full max-w-[calc(100%-2rem)] translate-x-[-50%] translate-y-[-50%] gap-4 rounded-lg border bg-background p-6 shadow-lg duration-200 sm:max-w-lg",
        slots: new()
        {
            [s => s.Overlay] = "fixed inset-0 z-50 bg-black/80"
        }
    );

    /// <summary>
    /// Gets the parent <see cref="RzAlertDialog"/> component.
    /// </summary>
    [CascadingParameter]
    protected RzAlertDialog? ParentAlertDialog { get; set; }

    /// <summary>
    /// Gets or sets the content rendered inside the alert dialog panel.
    /// </summary>
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = default!;

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (ParentAlertDialog == null)
        {
            throw new InvalidOperationException($"{nameof(AlertDialogContent)} must be used within an {nameof(RzAlertDialog)}.");
        }
    }

    /// <inheritdoc />
    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.AlertDialogContent;

    /// <summary>
    /// Defines the slots available for styling in the AlertDialogContent component.
    /// </summary>
    public sealed partial class Slots : ISlots
    {
        /// <summary>
        /// The base slot for the alert dialog panel.
        /// </summary>
        [Slot("alert-dialog-content")]
        public string? Base { get; set; }

        /// <summary>
        /// The slot for the backdrop overlay.
        /// </summary>
        [Slot("alert-dialog-overlay")]
        public string? Overlay { get; set; }
    }
}
