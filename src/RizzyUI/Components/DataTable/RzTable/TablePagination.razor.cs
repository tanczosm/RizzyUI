using Microsoft.AspNetCore.Components;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Renders pagination controls for an <see cref="RzTable{TItem}"/>.
/// </summary>
/// <typeparam name="TItem">The row item type.</typeparam>
[CascadingTypeParameter(nameof(TItem))]
public partial class TablePagination<TItem> : RzComponent<TablePaginationSlots>
{
    /// <summary>
    /// Gets or sets the parent table component.
    /// </summary>
    [CascadingParameter(Name = "ParentRzTable")]
    protected RzTable<TItem>? ParentRzTable { get; set; }

    /// <summary>
    /// Gets or sets the current page.
    /// </summary>
    [Parameter] public int? CurrentPage { get; set; }

    /// <summary>
    /// Gets or sets the page size.
    /// </summary>
    [Parameter] public int? PageSize { get; set; }

    /// <summary>
    /// Gets or sets the total item count.
    /// </summary>
    [Parameter] public long? TotalItems { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of page links.
    /// </summary>
    [Parameter] public int MaxVisiblePageLinks { get; set; } = 7;

    /// <summary>
    /// Gets or sets the previous button label.
    /// </summary>
    [Parameter] public string? PreviousButtonLabel { get; set; }

    /// <summary>
    /// Gets or sets the next button label.
    /// </summary>
    [Parameter] public string? NextButtonLabel { get; set; }

    /// <summary>
    /// Gets or sets the pagination navigation aria-label.
    /// </summary>
    [Parameter] public string? NavigationAriaLabel { get; set; }

    /// <summary>
    /// Gets the effective current page.
    /// </summary>
    protected int EffectiveCurrentPage => CurrentPage ?? ParentRzTable?.CurrentPage ?? 1;
    /// <summary>
    /// Gets the effective page size.
    /// </summary>
    protected int EffectivePageSize => PageSize ?? ParentRzTable?.PageSize ?? 10;
    /// <summary>
    /// Gets the effective total item count.
    /// </summary>
    protected long EffectiveTotalItems => TotalItems ?? ParentRzTable?.TotalItems ?? 0;
    /// <summary>
    /// Gets the total number of pages.
    /// </summary>
    protected int EffectiveTotalPages => EffectivePageSize <= 0 ? 0 : (int)Math.Ceiling(EffectiveTotalItems / (double)EffectivePageSize);
    /// <summary>
    /// Gets a value indicating whether previous navigation is available.
    /// </summary>
    protected bool CanGoPrevious => EffectiveCurrentPage > 1;
    /// <summary>
    /// Gets a value indicating whether next navigation is available.
    /// </summary>
    protected bool CanGoNext => EffectiveCurrentPage < EffectiveTotalPages;

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (string.IsNullOrEmpty(Element))
        {
            Element = "nav";
        }

        if (ParentRzTable == null && (CurrentPage == null || PageSize == null || TotalItems == null))
        {
            throw new InvalidOperationException($"{GetType().Name} requires either to be within an RzTable, or to have CurrentPage, PageSize and TotalItems provided.");
        }

        EnsureParameterDefaults();
    }

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (MaxVisiblePageLinks < 5)
        {
            MaxVisiblePageLinks = 5;
        }

        EnsureParameterDefaults();
    }

    private void EnsureParameterDefaults()
    {
        PreviousButtonLabel ??= Localizer["RzPagination.PreviousButtonLabel"];
        NextButtonLabel ??= Localizer["RzPagination.NextButtonLabel"];
        NavigationAriaLabel ??= Localizer["RzPagination.NavigationAriaLabel"];
    }

    /// <summary>
    /// Builds the inline script that dispatches pagination events for a target page.
    /// </summary>
    /// <param name="targetPage">The requested page number.</param>
    /// <returns>A JavaScript snippet for the click handler.</returns>
    protected string BuildPageChangeScript(int targetPage) =>
        $"(function(){{const host=this.closest('[data-rz-table-id]');if(!host){{return;}}const table=host.querySelector('[data-slot=table]');const detail={{tableId:host.id,tableElementId:table?table.id:null,sourceId:this.id,action:'page-change',targetPage:{targetPage},request:{{page:{targetPage},pageSize:{EffectivePageSize},sortBy:{ToJsString(ParentRzTable?.SortBy)},sortDirection:{ToJsString(ToRequestSortDirection(ParentRzTable?.SortDirection ?? SortDirection.Unset))}}}}};this.dispatchEvent(new CustomEvent('rz:tablepagination:on-page-change',{{detail,bubbles:true,composed:true}}));host.dispatchEvent(new CustomEvent('rz:table:on-state-change',{{detail,bubbles:true,composed:true}}));}})();";

    private static string? ToRequestSortDirection(SortDirection direction) => direction switch
    {
        SortDirection.Ascending => "asc",
        SortDirection.Descending => "desc",
        _ => null
    };

    private static string ToJsString(string? value) => value is null ? "null" : $"\"{value.Replace("\\", "\\\\").Replace("\"", "\\\"")}\"";

    /// <summary>
    /// Builds the visible set of page links.
    /// </summary>
    /// <returns>A list of page links and ellipsis placeholders.</returns>
    protected List<PageLink> GetPageLinks()
    {
        var links = new List<PageLink>();
        var totalPages = EffectiveTotalPages;
        var currentPage = EffectiveCurrentPage;

        if (totalPages <= 0)
        {
            return links;
        }

        if (totalPages <= MaxVisiblePageLinks)
        {
            for (var i = 1; i <= totalPages; i++)
            {
                links.Add(new PageLink(i.ToString(), i, i == currentPage, false));
            }

            return links;
        }

        links.Add(new PageLink("1", 1, currentPage == 1, false));

        var window = MaxVisiblePageLinks - 2;
        var start = Math.Max(2, currentPage - (window / 2));
        var end = Math.Min(totalPages - 1, start + window - 1);

        if (end - start + 1 < window)
        {
            start = Math.Max(2, end - window + 1);
        }

        if (start > 2)
        {
            links.Add(new PageLink("...", -1, false, true));
        }

        for (var i = start; i <= end; i++)
        {
            links.Add(new PageLink(i.ToString(), i, i == currentPage, false));
        }

        if (end < totalPages - 1)
        {
            links.Add(new PageLink("...", -2, false, true));
        }

        links.Add(new PageLink(totalPages.ToString(), totalPages, totalPages == currentPage, false));
        return links;
    }

    /// <inheritdoc/>
    protected override TvDescriptor<RzComponent<TablePaginationSlots>, TablePaginationSlots> GetDescriptor() => Theme.TablePagination;

    /// <summary>
    /// Represents a link or ellipsis entry in the pagination control.
    /// </summary>
    /// <param name="Text">The display text.</param>
    /// <param name="PageNumber">The represented page number.</param>
    /// <param name="IsCurrent">Indicates whether this is the active page.</param>
    /// <param name="IsEllipsis">Indicates whether this item is an ellipsis placeholder.</param>
    protected record PageLink(string Text, int PageNumber, bool IsCurrent, bool IsEllipsis);
}
