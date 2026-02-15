using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Renders custom indicator content for selectable menubar items.
/// </summary>
public partial class MenubarItemIndicator : RzComponent<MenubarItemIndicator.Slots>
{
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(@base: "absolute left-2 flex size-4 items-center justify-center");

    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = default!;

    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.MenubarItemIndicator;

    public sealed partial class Slots : ISlots
    {
        [Slot("menubar-item-indicator")]
        public string? Base { get; set; }
    }
}
