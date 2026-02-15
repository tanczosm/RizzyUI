using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Defines a single top-level menu scope within <see cref="RzMenubar"/>.
/// </summary>
public partial class MenubarMenu : RzComponent<MenubarMenu.Slots>
{
    /// <summary>
    /// Default styles for <see cref="MenubarMenu"/>.
    /// </summary>
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(@base: "relative");

    /// <summary>
    /// Gets or sets menu children, typically <see cref="MenubarTrigger"/> and <see cref="MenubarContent"/>.
    /// </summary>
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = default!;

    /// <summary>
    /// Gets or sets an optional stable value for this menu.
    /// </summary>
    [Parameter]
    public string? Value { get; set; }

    /// <summary>
    /// Gets the resolved menu id used by trigger and content.
    /// </summary>
    public string MenuId => string.IsNullOrWhiteSpace(Value) ? Id : Value!;

    /// <inheritdoc/>
    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.MenubarMenu;

    /// <summary>
    /// Defines style slots for <see cref="MenubarMenu"/>.
    /// </summary>
    public sealed partial class Slots : ISlots
    {
        /// <summary>
        /// Base slot.
        /// </summary>
        public string? Base { get; set; }
    }
}
