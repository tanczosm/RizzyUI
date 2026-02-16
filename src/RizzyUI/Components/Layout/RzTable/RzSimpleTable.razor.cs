using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// A responsive table container that renders a semantic <c>&lt;table&gt;</c> element.
/// </summary>
public partial class RzSimpleTable : RzComponent<RzSimpleTableSlots>
{
    /// <summary>
    /// Defines the default styling for the RzSimpleTable component.
    /// </summary>
    public static readonly TvDescriptor<RzComponent<RzSimpleTableSlots>, RzSimpleTableSlots> DefaultDescriptor = new(
        @base: "relative w-full overflow-x-auto",
        slots: new()
        {
            [s => s.Table] = "w-full caption-bottom text-sm"
        }
    );

    /// <summary>
    /// Gets or sets the table content, typically composed of <see cref="TableHeader"/>, <see cref="TableBodySection"/>, <see cref="TableFooter"/>, and <see cref="TableCaption"/>.
    /// </summary>
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = default!;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (string.IsNullOrEmpty(Element))
            Element = "div";
    }

    /// <inheritdoc />
    protected override TvDescriptor<RzComponent<RzSimpleTableSlots>, RzSimpleTableSlots> GetDescriptor() => Theme.RzSimpleTableLayout;
}
