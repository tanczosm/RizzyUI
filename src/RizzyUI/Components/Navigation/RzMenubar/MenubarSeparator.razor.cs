using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Renders a separator between menubar menu sections.
/// </summary>
public partial class MenubarSeparator : RzComponent<MenubarSeparator.Slots>
{
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(@base: "-mx-1 my-1 h-px bg-muted");

    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (string.IsNullOrEmpty(Element))
            Element = "div";
    }

    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.MenubarSeparator;

    public sealed partial class Slots : ISlots
    {
        [Slot("menubar-separator")]
        public string? Base { get; set; }
    }
}
