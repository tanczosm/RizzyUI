using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Represents the floating content panel for a <see cref="MenubarMenu"/>.
/// </summary>
public partial class MenubarContent : RzComponent<MenubarContent.Slots>
{
    /// <summary>
    /// Default styles for <see cref="MenubarContent"/>.
    /// </summary>
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(
        @base: "absolute left-0 top-full z-50 mt-1 min-w-[12rem] overflow-hidden rounded-md border bg-popover p-1 text-popover-foreground shadow-md animate-in data-[state=closed]:animate-out data-[state=closed]:fade-out-0 data-[state=open]:fade-in-0 data-[state=closed]:zoom-out-95 data-[state=open]:zoom-in-95",
        slots: new()
        {
            [s => s.InnerContainer] = ""
        }
    );

    /// <summary>
    /// Gets parent menu.
    /// </summary>
    [CascadingParameter]
    protected MenubarMenu? ParentMenu { get; set; }

    /// <summary>
    /// Gets content fragment.
    /// </summary>
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = default!;

    /// <summary>
    /// Gets content id.
    /// </summary>
    protected string ContentId => $"{ParentMenu?.MenuId}-content";

    /// <summary>
    /// Gets trigger id.
    /// </summary>
    protected string TriggerId => $"{ParentMenu?.MenuId}-trigger";

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (ParentMenu == null)
            throw new InvalidOperationException($"{nameof(MenubarContent)} must be used within a {nameof(MenubarMenu)}.");
    }

    /// <inheritdoc/>
    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.MenubarContent;

    /// <summary>
    /// Slot definitions.
    /// </summary>
    public sealed partial class Slots : ISlots
    {
        /// <summary>
        /// Base slot.
        /// </summary>
        public string? Base { get; set; }

        /// <summary>
        /// Inner container slot.
        /// </summary>
        public string? InnerContainer { get; set; }
    }
}
