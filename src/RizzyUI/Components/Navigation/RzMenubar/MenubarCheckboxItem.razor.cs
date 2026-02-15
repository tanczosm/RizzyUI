using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Represents a checkable menu item in a menubar content panel.
/// </summary>
public partial class MenubarCheckboxItem : RzComponent<MenubarCheckboxItem.Slots>
{
    /// <summary>
    /// Defines default styles for <see cref="MenubarCheckboxItem"/>.
    /// </summary>
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(
        @base: "relative flex cursor-default select-none w-full items-center rounded-sm py-1.5 pl-8 pr-2 text-sm outline-none transition-colors focus:bg-accent hover:bg-accent",
        slots: new() { [s => s.Indicator] = "absolute left-2 flex size-3.5 items-center justify-center" });

    /// <summary>
    /// Gets or sets item content.
    /// </summary>
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = default!;

    /// <summary>
    /// Gets or sets whether the item is checked.
    /// </summary>
    [Parameter]
    public bool Checked { get; set; }

    /// <summary>
    /// Gets or sets optional shortcut content.
    /// </summary>
    [Parameter]
    public RenderFragment? ShortcutContent { get; set; }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (string.IsNullOrEmpty(Element)) Element = "button";
        if (Element.Equals("button", StringComparison.OrdinalIgnoreCase) && (AdditionalAttributes == null || !AdditionalAttributes.ContainsKey("type")))
        {
            AdditionalAttributes ??= new Dictionary<string, object>();
            AdditionalAttributes["type"] = "button";
        }
    }

    /// <inheritdoc/>
    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.MenubarCheckboxItem;

    /// <summary>
    /// Defines style slots.
    /// </summary>
    public sealed partial class Slots : ISlots
    {
        /// <summary>
        /// Base slot.
        /// </summary>
        public string? Base { get; set; }

        /// <summary>
        /// Indicator slot.
        /// </summary>
        public string? Indicator { get; set; }
    }
}
