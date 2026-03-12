using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Represents the body (<c>tbody</c>) of an <see cref="RzTable"/>.
/// </summary>
public partial class TableBody : RzComponent<TableBodySlots>
{
    /// <summary>
    /// Gets or sets the body content, typically one or more <see cref="TableRow"/> components.
    /// </summary>
    [Parameter, EditorRequired] public RenderFragment? ChildContent { get; set; }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (string.IsNullOrEmpty(Element))
        {
            Element = "tbody";
        }
    }

    /// <inheritdoc/>
    protected override TvDescriptor<RzComponent<TableBodySlots>, TableBodySlots> GetDescriptor() => Theme.TableBody;
}
