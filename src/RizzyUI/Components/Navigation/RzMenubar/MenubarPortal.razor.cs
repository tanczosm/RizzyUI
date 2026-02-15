using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Provides API parity for portal semantics in SSR menubar compositions.
/// </summary>
public partial class MenubarPortal : RzComponent<MenubarPortal.Slots>
{
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(@base: "contents");

    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = default!;

    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.MenubarPortal;

    public sealed partial class Slots : ISlots
    {
        [Slot("menubar-portal")]
        public string? Base { get; set; }
    }
}
