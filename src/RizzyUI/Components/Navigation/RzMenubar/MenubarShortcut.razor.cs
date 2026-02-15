
using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Represents a styled text element typically used to display keyboard shortcuts
/// alongside a <see cref="MenubarItem"/>.
/// </summary>
public partial class MenubarShortcut : RzComponent<MenubarShortcut.Slots>
{
    /// <summary>
    /// Defines the default styling for the MenubarShortcut component.
    /// </summary>
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(
        @base: "ml-auto text-xs tracking-widest text-muted-foreground"
    );

    /// <summary>
    /// Gets or sets the content of the shortcut, e.g., "⌘S". Required.
    /// </summary>
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = default!;

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (string.IsNullOrEmpty(Element))
        {
            Element = "span";
        }
    }

    /// <inheritdoc/>
    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.MenubarShortcut;

    /// <summary>
    /// Defines the slots available for styling in the MenubarShortcut component.
    /// </summary>
    public sealed partial class Slots : ISlots
    {
        /// <summary>
        /// The base slot for the component's root element.
        /// </summary>
        public string? Base { get; set; }
    }
}