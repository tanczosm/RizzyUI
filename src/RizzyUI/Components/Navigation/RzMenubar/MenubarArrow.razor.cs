using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Renders a decorative arrow for menubar popovers.
/// </summary>
public partial class MenubarArrow : RzComponent<MenubarArrow.Slots>
{
    /// <summary>
    /// Defines the default styling for the <see cref="MenubarArrow"/> component.
    /// </summary>
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(@base: "h-2 w-2 rotate-45 border-l border-t bg-popover");

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (string.IsNullOrEmpty(Element)) Element = "span";
    }

    /// <inheritdoc/>
    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.MenubarArrow;

    /// <summary>
    /// Defines style slots for <see cref="MenubarArrow"/>.
    /// </summary>
    public sealed partial class Slots : ISlots
    {
        /// <summary>
        /// Base slot.
        /// </summary>
        public string? Base { get; set; }
    }
}
