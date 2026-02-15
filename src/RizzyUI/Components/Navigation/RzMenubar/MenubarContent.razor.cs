using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Represents the content panel for a menubar menu.
/// </summary>
public partial class MenubarContent : RzComponent<MenubarContent.Slots>
{
    /// <summary>
    /// Defines the default styling for the MenubarContent component.
    /// </summary>
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(
        @base: "absolute z-50 mt-1 min-w-[12rem] overflow-hidden rounded-md border bg-popover p-1 text-popover-foreground shadow-md"
    );

    [CascadingParameter] protected MenubarMenu? ParentMenu { get; set; }

    /// <summary>
    /// Gets or sets content rendered inside the menu panel.
    /// </summary>
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = default!;

    protected string ContentId => $"{ParentMenu?.Id}-content";
    protected string TriggerId => $"{ParentMenu?.Id}-trigger";

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (ParentMenu is null)
            throw new InvalidOperationException($"{nameof(MenubarContent)} must be used within {nameof(MenubarMenu)}.");
    }

    /// <inheritdoc/>
    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.MenubarContent;

    /// <summary>
    /// Defines styling slots for MenubarContent.
    /// </summary>
    public sealed partial class Slots : ISlots
    {
        [Slot("menubar-content")]
        public string? Base { get; set; }
    }
}
