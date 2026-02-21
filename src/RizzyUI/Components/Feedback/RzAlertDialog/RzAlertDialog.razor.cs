using Microsoft.AspNetCore.Components;
using Rizzy.Utility;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// The root component for an alert dialog system that interrupts the user and expects a response.
/// </summary>
public partial class RzAlertDialog : RzComponent<RzAlertDialog.Slots>
{
    /// <summary>
    /// Defines the default styling for the RzAlertDialog component.
    /// </summary>
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new();

    /// <summary>
    /// Gets the identifier used by the title to label the alert dialog content.
    /// </summary>
    internal string AriaLabelId { get; } = IdGenerator.UniqueId("rzalrtttl");

    /// <summary>
    /// Gets the unique identifier for the alert dialog body container.
    /// </summary>
    public string BodyId { get; } = IdGenerator.UniqueId("rzalrtbody");

    /// <summary>
    /// Gets the unique identifier for the alert dialog footer container.
    /// </summary>
    public string FooterId { get; } = IdGenerator.UniqueId("rzalrtfoot");

    /// <summary>
    /// Gets or sets the content of the alert dialog.
    /// </summary>
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = default!;

    /// <summary>
    /// Gets or sets the event name used to open the alert dialog.
    /// </summary>
    [Parameter]
    public string EventTriggerName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the event name used to close the alert dialog.
    /// </summary>
    [Parameter]
    public string CloseEventName { get; set; } = Constants.Events.DialogClose;

    /// <summary>
    /// Gets or sets whether pressing Escape closes the alert dialog.
    /// </summary>
    [Parameter]
    public bool CloseOnEscape { get; set; }

    /// <summary>
    /// Gets or sets whether clicking outside closes the alert dialog.
    /// </summary>
    [Parameter]
    public bool CloseOnClickOutside { get; set; }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (string.IsNullOrEmpty(EventTriggerName))
        {
            EventTriggerName = $"show-alert-dialog-{Id}";
        }
    }

    /// <inheritdoc />
    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.RzAlertDialog;

    /// <summary>
    /// Defines the slots available for styling in the RzAlertDialog component.
    /// </summary>
    public sealed partial class Slots : ISlots
    {
        /// <summary>
        /// The base slot for the component root.
        /// </summary>
        [Slot("alert-dialog")]
        public string? Base { get; set; }
    }
}
