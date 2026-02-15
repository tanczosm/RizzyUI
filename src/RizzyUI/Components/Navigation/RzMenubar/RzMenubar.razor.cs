using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Provides the root menubar container and Alpine.js controller for grouped menus.
/// </summary>
public partial class RzMenubar : RzComponent<RzMenubar.Slots>
{
    /// <summary>
    /// Defines the default styling for the <see cref="RzMenubar"/> component.
    /// </summary>
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(
        @base: "flex h-10 items-center space-x-1 rounded-md border bg-background p-1"
    );

    /// <summary>
    /// Gets or sets the menu definitions rendered inside the menubar.
    /// </summary>
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = default!;

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (string.IsNullOrEmpty(Element))
            Element = "div";
    }

    /// <inheritdoc/>
    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.RzMenubar;

    /// <summary>
    /// Defines style slots for <see cref="RzMenubar"/>.
    /// </summary>
    public sealed partial class Slots : ISlots
    {
        /// <summary>
        /// Base slot applied to the root menubar element.
        /// </summary>
        public string? Base { get; set; }
    }
}
