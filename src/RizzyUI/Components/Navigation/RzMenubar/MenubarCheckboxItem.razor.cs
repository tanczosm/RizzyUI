using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Represents a checkbox item in a menubar.
/// </summary>
public partial class MenubarCheckboxItem : RzComponent<MenubarCheckboxItem.Slots>
{
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(
        @base: "relative flex cursor-default select-none items-center rounded-sm py-1.5 pl-8 pr-2 text-sm outline-none transition-colors focus:bg-accent focus:text-accent-foreground data-[state=checked]:bg-accent/50 data-[disabled=true]:pointer-events-none data-[disabled=true]:opacity-50",
        slots: new() { [s => s.Indicator] = "absolute left-2 flex size-4 items-center justify-center" }
    );

    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = default!;

    [Parameter]
    public RenderFragment? ShortcutContent { get; set; }

    [Parameter]
    public bool Checked { get; set; }

    [Parameter]
    public bool Disabled { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (string.IsNullOrEmpty(Element))
            Element = "button";
    }

    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.MenubarCheckboxItem;

    public sealed partial class Slots : ISlots
    {
        [Slot("menubar-checkbox-item")]
        public string? Base { get; set; }
        [Slot("indicator")]
        public string? Indicator { get; set; }
    }
}
