using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Groups radio items within menubar content.
/// </summary>
public partial class MenubarRadioGroup : RzComponent<MenubarRadioGroup.Slots>
{
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(@base: "p-0");

    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = default!;

    [Parameter]
    public string? Value { get; set; }

    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.MenubarRadioGroup;

    public sealed partial class Slots : ISlots
    {
        [Slot("menubar-radio-group")]
        public string? Base { get; set; }
    }
}
