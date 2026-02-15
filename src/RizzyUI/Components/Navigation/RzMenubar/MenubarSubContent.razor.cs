using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Represents content displayed by a menubar submenu.
/// </summary>
public partial class MenubarSubContent : RzComponent<MenubarSubContent.Slots>
{
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(
        @base: "absolute z-50 min-w-[10rem] overflow-hidden rounded-md border bg-popover p-1 text-popover-foreground shadow-lg"
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
            throw new InvalidOperationException($"{nameof(MenubarSubContent)} must be used within {nameof(MenubarSub)}.");
    }

    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.MenubarSubContent;

    public sealed partial class Slots : ISlots
    {
        [Slot("menubar-sub-content")]
        public string? Base { get; set; }
    }
}
