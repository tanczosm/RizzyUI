using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Represents the trigger for opening a menubar menu.
/// </summary>
public partial class MenubarTrigger : RzAsChildComponent<MenubarTrigger.Slots>
{
    /// <summary>
    /// Defines the default styling for the MenubarTrigger component.
    /// </summary>
    public static readonly TvDescriptor<RzAsChildComponent<Slots>, Slots> DefaultDescriptor = new(
        @base: "flex cursor-default select-none items-center rounded-sm px-3 py-1.5 text-sm font-medium outline-none focus:bg-accent data-[state=open]:bg-accent"
    );

    [CascadingParameter] protected MenubarMenu? ParentMenu { get; set; }

    /// <summary>
    /// Gets or sets the trigger content.
    /// </summary>
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = default!;

    protected string TriggerId => $"{ParentMenu?.Id}-trigger";
    protected string ContentId => $"{ParentMenu?.Id}-content";

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (ParentMenu is null)
            throw new InvalidOperationException($"{nameof(MenubarTrigger)} must be used within {nameof(MenubarMenu)}.");
        if (string.IsNullOrEmpty(Element))
            Element = "button";
    }

    /// <inheritdoc/>
    protected override RenderFragment? GetAsChildContent() => ChildContent;

    /// <inheritdoc/>
    protected override Dictionary<string, object?> GetComponentAttributes()
    {
        var attributes = new Dictionary<string, object?>(AdditionalAttributes?.ToDictionary(kvp => kvp.Key, kvp => (object?)kvp.Value) ?? new(), StringComparer.OrdinalIgnoreCase)
        {
            ["id"] = TriggerId,
            ["class"] = SlotClasses.GetBase(),
            ["type"] = "button",
            ["role"] = "menuitem",
            ["tabindex"] = "0",
            ["aria-haspopup"] = "menu",
            ["aria-controls"] = ContentId,
            ["x-bind:aria-expanded"] = "ariaExpanded",
            ["x-on:pointerdown"] = "handleTriggerPointerDown",
            ["x-on:pointerenter"] = "handleTriggerPointerEnter",
            ["x-on:keydown"] = "handleTriggerKeydown",
            ["data-menu-value"] = ParentMenu!.Value,
            ["data-slot"] = "menubar-trigger"
        };
        return attributes;
    }

    /// <inheritdoc/>
    protected override TvDescriptor<RzAsChildComponent<Slots>, Slots> GetDescriptor() => Theme.MenubarTrigger;

    /// <summary>
    /// Defines styling slots for MenubarTrigger.
    /// </summary>
    public sealed partial class Slots : ISlots
    {
        /// <summary>
        /// The base slot.
        /// </summary>
        [Slot("menubar-trigger")]
        public string? Base { get; set; }
    }
}
