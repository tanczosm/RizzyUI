using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Represents a visual separator line within a <see cref="MenubarContent"/> or <see cref="MenubarGroup"/>.
/// </summary>
public partial class MenubarSeparator : RzComponent<MenubarSeparator.Slots>
{
    /// <summary>
    /// Defines the default styling for the MenubarSeparator component.
    /// </summary>
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(
        @base: "-mx-1 my-1 h-px bg-border"
    );

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (string.IsNullOrEmpty(Element))
        {
            Element = "hr";
        }
    }

    /// <inheritdoc/>
    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.MenubarSeparator;

    /// <summary>
    /// Defines the slots available for styling in the MenubarSeparator component.
    /// </summary>
    public sealed partial class Slots : ISlots
    {
        /// <summary>
        /// The base slot for the component's root element.
        /// </summary>
        public string? Base { get; set; }
    }
}