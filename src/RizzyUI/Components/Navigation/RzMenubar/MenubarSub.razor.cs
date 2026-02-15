using Microsoft.AspNetCore.Components;
using RizzyUI.Extensions;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Represents a nested submenu in menubar content.
/// </summary>
public partial class MenubarSub : RzComponent<MenubarSub.Slots>
{
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(@base: "relative");

    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = default!;

    [Parameter]
    public AnchorPoint Anchor { get; set; } = AnchorPoint.RightStart;

    [Parameter]
    public int Offset { get; set; } = 4;

    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.MenubarSub;

    public sealed partial class Slots : ISlots
    {
        [Slot("menubar-sub")]
        public string? Base { get; set; }
    }
}
