
using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Represents a group of related <see cref="MenubarItem"/>s within <see cref="MenubarContent"/>.
/// </summary>
public partial class MenubarGroup : RzComponent<MenubarGroup.Slots>
{
    /// <summary>
    /// Defines the default styling for the MenubarGroup component.
    /// </summary>
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(
        @base: "py-1"
    );

    /// <summary>
    /// Gets or sets the content of the group, typically <see cref="MenubarItem"/>s. Required.
    /// </summary>
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = default!;

    /// <inheritdoc/>
    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.MenubarGroup;

    /// <summary>
    /// Defines the slots available for styling in the MenubarGroup component.
    /// </summary>
    public sealed partial class Slots : ISlots
    {
        /// <summary>
        /// The base slot for the component's root element.
        /// </summary>
        public string? Base { get; set; }
    }
}