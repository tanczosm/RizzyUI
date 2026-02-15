using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Renders shortcut text in a menubar item.
/// </summary>
public partial class MenubarShortcut : RzComponent<MenubarShortcut.Slots>
{
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(@base: "ml-auto text-xs tracking-widest text-muted-foreground");

    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = default!;

    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.MenubarShortcut;

    public sealed partial class Slots : ISlots
    {
        [Slot("menubar-shortcut")]
        public string? Base { get; set; }
    }
}
