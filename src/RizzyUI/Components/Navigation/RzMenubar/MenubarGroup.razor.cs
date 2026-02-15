using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Groups related menubar items.
/// </summary>
public partial class MenubarGroup : RzComponent<MenubarGroup.Slots>
{
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(@base: "p-1");

    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = default!;

    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.MenubarGroup;

    public sealed partial class Slots : ISlots
    {
        [Slot("menubar-group")]
        public string? Base { get; set; }
    }
}
