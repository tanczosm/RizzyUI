using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Groups related radio items in a menubar menu.
/// </summary>
public partial class MenubarRadioGroup : RzComponent<MenubarRadioGroup.Slots>
{
    /// <summary>
    /// Defines default styles for <see cref="MenubarRadioGroup"/>.
    /// </summary>
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(@base: "py-1");

    /// <summary>
    /// Gets or sets child content.
    /// </summary>
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = default!;

    /// <summary>
    /// Gets or sets the current group value.
    /// </summary>
    [Parameter]
    public string? Value { get; set; }

    /// <inheritdoc/>
    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.MenubarRadioGroup;

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
