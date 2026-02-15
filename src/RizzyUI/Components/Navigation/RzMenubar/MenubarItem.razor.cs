using Blazicons;
using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Represents an item in menubar content.
/// </summary>
public partial class MenubarItem : RzComponent<MenubarItem.Slots>
{
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(
        @base: "relative flex cursor-default select-none items-center rounded-sm px-2 py-1.5 text-sm outline-none transition-colors focus:bg-accent focus:text-accent-foreground data-[disabled=true]:pointer-events-none data-[disabled=true]:opacity-50",
        slots: new() { [s => s.Icon] = "mr-2 size-4" }
    );

    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = default!;

    [Parameter]
    public SvgIcon? Icon { get; set; }

    [Parameter]
    public RenderFragment? ShortcutContent { get; set; }

    [Parameter]
    public bool Disabled { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (string.IsNullOrEmpty(Element))
            Element = "button";
    }

    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.MenubarItem;

    public sealed partial class Slots : ISlots
    {
        [Slot("menubar-item")]
        public string? Base { get; set; }
        [Slot("icon")]
        public string? Icon { get; set; }
    }
}
