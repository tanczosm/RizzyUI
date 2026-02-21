using Microsoft.AspNetCore.Components;

namespace RizzyUI.Components.Internal;

/// <summary>
/// Provides the shared Alpine.js modal primitive used by higher-level dialog components.
/// </summary>
public partial class RzModalPrimitive : RzComponent
{
    /// <summary>
    /// Gets or sets the content rendered inside the modal panel.
    /// </summary>
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = default!;

    /// <summary>
    /// Gets or sets the role applied to the modal content.
    /// </summary>
    [Parameter]
    public string Role { get; set; } = "dialog";

    /// <summary>
    /// Gets or sets whether the Escape key should close the modal.
    /// </summary>
    [Parameter]
    public bool CloseOnEscape { get; set; } = true;

    /// <summary>
    /// Gets or sets whether clicking outside the panel should close the modal.
    /// </summary>
    [Parameter]
    public bool CloseOnClickOutside { get; set; } = true;

    /// <summary>
    /// Gets or sets the event name used to open the modal.
    /// </summary>
    [Parameter]
    public string EventTriggerName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the event name used to close the modal.
    /// </summary>
    [Parameter]
    public string CloseEventName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the identifier published in modal lifecycle events.
    /// </summary>
    [Parameter]
    public string ModalId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the body region identifier published in modal lifecycle events.
    /// </summary>
    [Parameter]
    public string BodyId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the footer region identifier published in modal lifecycle events.
    /// </summary>
    [Parameter]
    public string FooterId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the classes applied to the backdrop element.
    /// </summary>
    [Parameter]
    public string? OverlayClass { get; set; }

    /// <summary>
    /// Gets or sets the classes applied to the content element.
    /// </summary>
    [Parameter]
    public string? ContentClass { get; set; }

    /// <summary>
    /// Gets or sets additional attributes applied to the content element.
    /// </summary>
    [Parameter]
    public Dictionary<string, object>? ContentAttributes { get; set; }

    /// <summary>
    /// Gets or sets the slot name for the overlay element.
    /// </summary>
    [Parameter]
    public string OverlayDataSlot { get; set; } = "dialog-overlay";

    /// <summary>
    /// Gets or sets the slot name for the content element.
    /// </summary>
    [Parameter]
    public string ContentDataSlot { get; set; } = "dialog-content";

    /// <summary>
    /// Gets or sets the ARIA label for the dialog content.
    /// </summary>
    [Parameter]
    public string? AriaLabel { get; set; }

    /// <summary>
    /// Gets or sets the ARIA labelledby value for the dialog content.
    /// </summary>
    [Parameter]
    public string? AriaLabelledBy { get; set; }
}
