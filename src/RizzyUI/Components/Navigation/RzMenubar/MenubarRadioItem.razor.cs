using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Represents a radio item inside a menubar radio group.
/// </summary>
public partial class MenubarRadioItem : RzComponent<MenubarRadioItem.Slots>
{
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(
        @base: "relative flex cursor-default select-none items-center rounded-sm py-1.5 pl-8 pr-2 text-sm outline-none transition-colors focus:bg-accent focus:text-accent-foreground data-[state=checked]:bg-accent/50 data-[disabled=true]:pointer-events-none data-[disabled=true]:opacity-50",
        slots: new() { [s => s.Indicator] = "absolute left-2 flex size-4 items-center justify-center" }
    );

    [CascadingParameter] protected MenubarRadioGroup? ParentGroup { get; set; }

    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = default!;

    [Parameter, EditorRequired]
    public string Value { get; set; } = default!;

    [Parameter]
    public RenderFragment? ShortcutContent { get; set; }

    [Parameter]
    public bool Disabled { get; set; }

    protected bool IsChecked => string.Equals(ParentGroup?.Value, Value, StringComparison.Ordinal);

    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (string.IsNullOrEmpty(Element))
            Element = "button";
    }

    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.MenubarRadioItem;

    public sealed partial class Slots : ISlots
    {
        [Slot("menubar-radio-item")]
        public string? Base { get; set; }
        [Slot("indicator")]
        public string? Indicator { get; set; }
    }
}
