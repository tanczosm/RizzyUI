using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Represents a table pagination wrapper region where consumer pagination markup can be rendered.
/// </summary>
public partial class TablePagination : RzComponent<TablePaginationSlots>
{
    /// <summary>
    /// Gets or sets the content rendered inside the pagination wrapper.
    /// </summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (string.IsNullOrEmpty(Element))
        {
            Element = "div";
        }
    }

    /// <inheritdoc/>
    protected override TvDescriptor<RzComponent<TablePaginationSlots>, TablePaginationSlots> GetDescriptor() => Theme.TablePagination;
}
