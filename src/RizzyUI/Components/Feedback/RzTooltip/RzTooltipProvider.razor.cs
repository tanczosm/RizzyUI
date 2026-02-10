using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Provides shared tooltip timing configuration to descendant <see cref="RzTooltip"/> components.
/// </summary>
public partial class RzTooltipProvider : RzComponent<RzTooltipProvider.Slots>
{
    /// <summary>
    /// Defines the default styling for the RzTooltipProvider component.
    /// </summary>
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(
        @base: "contents"
    );

    /// <summary>
    /// Gets or sets the provider content.
    /// </summary>
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = default!;

    /// <summary>
    /// Gets or sets the default delay in milliseconds before a tooltip opens.
    /// </summary>
    [Parameter]
    public int DefaultDelay { get; set; } = 700;

    /// <summary>
    /// Gets or sets the default delay in milliseconds before a tooltip closes.
    /// </summary>
    [Parameter]
    public int DefaultCloseDelay { get; set; } = 150;

    /// <inheritdoc />
    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.RzTooltipProvider;

    /// <summary>
    /// Defines the slots available for styling in the RzTooltipProvider component.
    /// </summary>
    public sealed partial class Slots : ISlots
    {
        /// <summary>
        /// The base slot for the provider root element.
        /// </summary>
        [Slot("tooltip-provider")]
        public string? Base { get; set; }
    }
}
