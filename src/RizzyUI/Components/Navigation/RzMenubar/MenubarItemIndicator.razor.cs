using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Renders an indicator region for check and radio items.
/// </summary>
public partial class MenubarItemIndicator : RzComponent<MenubarItemIndicator.Slots>
{
    /// <summary>
    /// Defines the default styling for <see cref="MenubarItemIndicator"/>.
    /// </summary>
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(@base: "absolute left-2 flex size-3.5 items-center justify-center");

    /// <summary>
    /// Gets or sets content rendered inside the indicator.
    /// </summary>
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = default!;

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (string.IsNullOrEmpty(Element)) Element = "span";
    }

    /// <inheritdoc/>
    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.MenubarItemIndicator;

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
