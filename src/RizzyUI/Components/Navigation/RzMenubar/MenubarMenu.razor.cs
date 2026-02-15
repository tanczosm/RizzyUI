using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Defines a single menu within a <see cref="RzMenubar"/>.
/// </summary>
public partial class MenubarMenu : RzComponent<MenubarMenu.Slots>
{
    /// <summary>
    /// Defines the default styling for the MenubarMenu component.
    /// </summary>
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(@base: "contents");

    /// <summary>
    /// Gets the parent menubar.
    /// </summary>
    [CascadingParameter]
    protected RzMenubar? ParentMenubar { get; set; }

    /// <summary>
    /// Gets or sets the menu content.
    /// </summary>
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = default!;

    /// <summary>
    /// Gets or sets a stable menu value. If omitted, a generated value is used.
    /// </summary>
    [Parameter]
    public string? Value { get; set; }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (ParentMenubar is null)
            throw new InvalidOperationException($"{nameof(MenubarMenu)} must be used within {nameof(RzMenubar)}.");
        if (string.IsNullOrWhiteSpace(Value))
            Value = Id;
        if (string.IsNullOrEmpty(Element))
            Element = "div";
    }

    /// <inheritdoc/>
    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.MenubarMenu;

    /// <summary>
    /// Defines the slots available for styling in the MenubarMenu component.
    /// </summary>
    public sealed partial class Slots : ISlots
    {
        /// <summary>
        /// The base slot for the component root.
        /// </summary>
        [Slot("menubar-menu")]
        public string? Base { get; set; }
    }
}
