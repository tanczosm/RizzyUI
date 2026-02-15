using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Renders a decorative arrow for menubar content.
/// </summary>
public partial class MenubarArrow : RzComponent<MenubarArrow.Slots>
{
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(@base: "fill-popover");

    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.MenubarArrow;

    public sealed partial class Slots : ISlots
    {
        [Slot("menubar-arrow")]
        public string? Base { get; set; }
    }
}
