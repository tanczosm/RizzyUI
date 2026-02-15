using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Renders a label for a menubar section.
/// </summary>
public partial class MenubarLabel : RzComponent<MenubarLabel.Slots>
{
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(@base: "px-2 py-1.5 text-sm font-semibold");

    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = default!;

    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.MenubarLabel;

    public sealed partial class Slots : ISlots
    {
        [Slot("menubar-label")]
        public string? Base { get; set; }
    }
}
