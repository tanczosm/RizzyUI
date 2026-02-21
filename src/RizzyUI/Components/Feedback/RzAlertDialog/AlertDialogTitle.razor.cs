using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Displays the title for an alert dialog.
/// </summary>
public partial class AlertDialogTitle : RzComponent<AlertDialogTitle.Slots>
{
    /// <summary>
    /// Defines the default styling for the AlertDialogTitle component.
    /// </summary>
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(
        @base: "text-lg font-semibold"
    );

    /// <summary>
    /// Gets the parent alert dialog.
    /// </summary>
    [CascadingParameter]
    protected RzAlertDialog? ParentAlertDialog { get; set; }

    /// <summary>
    /// Gets or sets the title content.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (string.IsNullOrEmpty(Element))
            Element = "h2";
    }

    /// <inheritdoc />
    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.AlertDialogTitle;

    /// <summary>
    /// Defines the slots available for styling in the AlertDialogTitle component.
    /// </summary>
    public sealed partial class Slots : ISlots
    {
        /// <summary>
        /// The base slot for the component.
        /// </summary>
        [Slot("alert-dialog-title")]
        public string? Base { get; set; }
    }
}
