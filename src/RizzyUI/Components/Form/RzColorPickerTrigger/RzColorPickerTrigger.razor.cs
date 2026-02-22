using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Provides an accessible, headless trigger that opens an enclosing <see cref="RzColorPickerProvider"/>.
/// </summary>
public partial class RzColorPickerTrigger : RzComponent<RzColorPickerTrigger.Slots>
{
    /// <summary>
    /// Defines the default styling for <see cref="RzColorPickerTrigger"/>.
    /// </summary>
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(
        @base: "contents"
    );

    /// <summary>
    /// Gets or sets the content rendered inside the trigger.
    /// </summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether trigger interaction is disabled.
    /// </summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets the accessible label used by assistive technologies.
    /// </summary>
    [Parameter] public string? AriaLabel { get; set; }

    [CascadingParameter] private RzColorPickerProvider.Context? ProviderContext { get; set; }

    private int TabIndex => Disabled ? -1 : 0;

    private string? OpenExpression => Disabled ? null : "colorPicker.open";

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        Element = "div";
        AriaLabel ??= Localizer["RzColorPickerTrigger.DefaultAriaLabel"];

        if (ProviderContext is null)
        {
            throw new InvalidOperationException($"{nameof(RzColorPickerTrigger)} must be used within an {nameof(RzColorPickerProvider)}.");
        }
    }

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        AriaLabel ??= Localizer["RzColorPickerTrigger.DefaultAriaLabel"];
    }

    /// <inheritdoc />
    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.RzColorPickerTrigger;

    /// <summary>
    /// Defines the slots available for styling in <see cref="RzColorPickerTrigger"/>.
    /// </summary>
    public sealed partial class Slots : ISlots
    {
        /// <summary>
        /// Gets or sets classes for the root element.
        /// </summary>
        [Slot("color-picker-trigger")]
        public string? Base { get; set; }
    }
}
