using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Provides the root container and Alpine state for a menubar.
/// </summary>
public partial class RzMenubar : RzComponent<RzMenubar.Slots>
{
    /// <summary>
    /// Defines the default styling for the RzMenubar component.
    /// </summary>
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(
        @base: "flex h-10 items-center space-x-1 rounded-md border bg-background p-1"
    );

    /// <summary>
    /// Gets or sets the content of the menubar.
    /// </summary>
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = default!;

    /// <summary>
    /// Gets or sets a value indicating whether left and right navigation loops.
    /// </summary>
    [Parameter]
    public bool Loop { get; set; } = true;

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
    /// Defines the slots available for styling in the RzMenubar component.
    /// </summary>
    public sealed partial class Slots : ISlots
    {
        /// <summary>
        /// The base slot for the component root.
        /// </summary>
        [Slot("menubar")]
        public string? Base { get; set; }
    }
}
