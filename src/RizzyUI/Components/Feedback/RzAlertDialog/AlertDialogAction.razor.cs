using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// A primary action button for confirming an alert dialog operation.
/// </summary>
public partial class AlertDialogAction : RzAsChildComponent<AlertDialogAction.Slots>
{
    /// <summary>
    /// Defines the default styling for the AlertDialogAction component.
    /// </summary>
    public static readonly TvDescriptor<RzAsChildComponent<Slots>, Slots> DefaultDescriptor = new(
        @base: "inline-flex items-center justify-center whitespace-nowrap rounded-md text-sm font-medium transition-all cursor-pointer disabled:pointer-events-none disabled:opacity-50 bg-primary text-primary-foreground shadow-xs hover:bg-primary/90 h-9 px-4 py-2"
    );

    /// <summary>
    /// Gets or sets the action content.
    /// </summary>
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = default!;

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (string.IsNullOrEmpty(Element))
            Element = "button";
    }

    /// <inheritdoc/>
    protected override RenderFragment? GetAsChildContent() => ChildContent;

    /// <inheritdoc/>
    protected override Dictionary<string, object?> GetComponentAttributes()
    {
        var attributes = new Dictionary<string, object?>(AdditionalAttributes?.ToDictionary(kvp => kvp.Key, kvp => (object?)kvp.Value) ?? new Dictionary<string, object?>(), StringComparer.OrdinalIgnoreCase)
        {
            ["id"] = Id,
            ["class"] = SlotClasses.GetBase(),
            ["x-on:click"] = "closeModal",
            ["data-slot"] = "alert-dialog-action"
        };
        return attributes;
    }

    /// <inheritdoc />
    protected override TvDescriptor<RzAsChildComponent<Slots>, Slots> GetDescriptor() => Theme.AlertDialogAction;

    /// <summary>
    /// Defines the slots available for styling in the AlertDialogAction component.
    /// </summary>
    public sealed partial class Slots : ISlots
    {
        /// <summary>
        /// The base slot for the component.
        /// </summary>
        [Slot("alert-dialog-action")]
        public string? Base { get; set; }
    }
}
