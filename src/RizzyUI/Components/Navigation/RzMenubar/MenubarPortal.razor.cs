using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Represents a logical portal wrapper for menubar content.
/// </summary>
public partial class MenubarPortal : RzComponent<MenubarPortal.Slots>
{
    /// <summary>
    /// Defines the default styling for <see cref="MenubarPortal"/>.
    /// </summary>
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(@base: "contents");

    /// <summary>
    /// Gets or sets child content.
    /// </summary>
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = default!;

    /// <inheritdoc/>
    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.MenubarPortal;

    /// <summary>
    /// Defines style slots.
    /// </summary>
    public sealed partial class Slots : ISlots
    {
        /// <summary>
        /// Base slot.
        /// </summary>
        public string? Base { get; set; }
    }
}
