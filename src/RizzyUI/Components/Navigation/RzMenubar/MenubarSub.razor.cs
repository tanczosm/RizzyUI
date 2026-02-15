using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Represents a nested submenu container inside menubar content.
/// </summary>
public partial class MenubarSub : RzComponent<MenubarSub.Slots>
{
    /// <summary>
    /// Default styling for <see cref="MenubarSub"/>.
    /// </summary>
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(@base: "relative");

    /// <summary>
    /// Gets the parent top-level menu scope.
    /// </summary>
    [CascadingParameter]
    protected MenubarMenu? ParentMenu { get; set; }

    /// <summary>
    /// Gets submenu child content.
    /// </summary>
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = default!;

    /// <summary>
    /// Gets or sets submenu anchor placement.
    /// </summary>
    [Parameter]
    public AnchorPoint Anchor { get; set; } = AnchorPoint.RightStart;

    /// <summary>
    /// Gets or sets submenu offset.
    /// </summary>
    [Parameter]
    public int Offset { get; set; } = -4;

    /// <summary>
    /// Gets parent id to coordinate submenu state.
    /// </summary>
    protected string ParentMenuId => ParentMenu?.MenuId ?? string.Empty;

    /// <inheritdoc/>
    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.MenubarSub;

    /// <summary>
    /// Slot definitions.
    /// </summary>
    public sealed partial class Slots : ISlots
    {
        /// <summary>
        /// Base slot.
        /// </summary>
        public string? Base { get; set; }
    }
}
