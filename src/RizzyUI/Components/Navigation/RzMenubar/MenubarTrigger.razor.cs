using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Represents a top-level trigger button inside a <see cref="MenubarMenu"/>.
/// </summary>
public partial class MenubarTrigger : RzAsChildComponent<MenubarTrigger.Slots>
{
    /// <summary>
    /// Default styling for <see cref="MenubarTrigger"/>.
    /// </summary>
    public static readonly TvDescriptor<RzAsChildComponent<Slots>, Slots> DefaultDescriptor = new(
        @base: "flex cursor-default select-none items-center rounded-sm px-3 py-1.5 text-sm font-medium outline-none focus:bg-accent data-[state=open]:bg-accent"
    );

    /// <summary>
    /// Gets the parent menu scope.
    /// </summary>
    [CascadingParameter]
    protected MenubarMenu? ParentMenu { get; set; }

    /// <summary>
    /// Gets trigger content.
    /// </summary>
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = default!;

    /// <summary>
    /// Gets the trigger id.
    /// </summary>
    protected string TriggerId => $"{ParentMenu?.MenuId}-trigger";

    /// <summary>
    /// Gets the controlled content id.
    /// </summary>
    protected string ContentId => $"{ParentMenu?.MenuId}-content";

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (ParentMenu == null)
            throw new InvalidOperationException($"{nameof(MenubarTrigger)} must be used within a {nameof(MenubarMenu)}.");
        Element = "button";
    }

    /// <inheritdoc/>
    protected override RenderFragment? GetAsChildContent() => ChildContent;

    /// <inheritdoc/>
    protected override Dictionary<string, object?> GetComponentAttributes()
    {
        var attributes = new Dictionary<string, object?>(AdditionalAttributes?.ToDictionary(kvp => kvp.Key, kvp => (object?)kvp.Value) ?? new Dictionary<string, object?>(), StringComparer.OrdinalIgnoreCase)
        {
            ["id"] = TriggerId,
            ["class"] = SlotClasses.GetBase(),
            ["role"] = "menuitem",
            ["type"] = "button",
            ["x-ref"] = "trigger",
            ["aria-haspopup"] = "menu",
            ["aria-controls"] = ContentId,
            ["x-bind:aria-expanded"] = "ariaExpanded",
            ["x-on:click"] = "toggle",
            ["x-on:mouseenter"] = "handleTriggerMouseEnter",
            ["x-on:keydown.enter.prevent"] = "handleTriggerKeydown",
            ["x-on:keydown.space.prevent"] = "handleTriggerKeydown",
            ["x-on:keydown.down.prevent"] = "handleTriggerKeydown",
            ["x-on:keydown.right.prevent"] = "focusNextMenubarTrigger",
            ["x-on:keydown.left.prevent"] = "focusPreviousMenubarTrigger",
            ["data-slot"] = "menubar-trigger"
        };
        return attributes;
    }

    /// <inheritdoc/>
    protected override TvDescriptor<RzAsChildComponent<Slots>, Slots> GetDescriptor() => Theme.MenubarTrigger;

    /// <summary>
    /// Slot definitions.
    /// </summary>
    public sealed partial class Slots : ISlots
    {
        /// <summary>
        /// Base slot.
        /// </summary>
        public string? Base { get; set; }
    }
}
