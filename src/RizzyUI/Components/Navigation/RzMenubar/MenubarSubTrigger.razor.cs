using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Represents the trigger for opening a menubar submenu.
/// </summary>
public partial class MenubarSubTrigger : RzComponent<MenubarSubTrigger.Slots>
{
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(
        @base: "flex cursor-default select-none items-center rounded-sm px-2 py-1.5 text-sm outline-none focus:bg-accent",
        slots: new() { [s => s.Chevron] = "ml-auto size-4" }
    );

    [CascadingParameter] protected MenubarSub? ParentSub { get; set; }

    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = default!;

    protected string TriggerId => $"{ParentSub?.Id}-trigger";
    protected string ContentId => $"{ParentSub?.Id}-content";

    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (ParentSub is null)
            throw new InvalidOperationException($"{nameof(MenubarSubTrigger)} must be used within {nameof(MenubarSub)}.");
        if (string.IsNullOrEmpty(Element))
            Element = "button";
    }

    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.MenubarSubTrigger;

    public sealed partial class Slots : ISlots
    {
        [Slot("menubar-sub-trigger")]
        public string? Base { get; set; }
        [Slot("chevron")]
        public string? Chevron { get; set; }
    }
}
