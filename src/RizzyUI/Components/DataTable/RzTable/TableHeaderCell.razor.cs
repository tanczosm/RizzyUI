using Blazicons;
using Microsoft.AspNetCore.Components;
using Rizzy.Utility;
using System.Linq.Expressions;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Represents a header cell in an <see cref="RzTable{TItem}"/>.
/// </summary>
/// <typeparam name="TItem">The table row item type.</typeparam>
public partial class TableHeaderCell<TItem> : RzComponent<TableHeaderCellSlots>, IHasTableHeaderCellStylingProperties
{
    private string? _columnKeyInternal;
    private SortDirection _currentSortDirection = SortDirection.Unset;
    private SortDirection _nextSortDirection = SortDirection.Ascending;
    private string _ariaSortValue = "none";
    private string _sortButtonAriaLabel = string.Empty;

    /// <summary>
    /// Gets or sets the parent table component.
    /// </summary>
    [CascadingParameter(Name = "ParentRzTable")]
    protected RzTable<TItem>? ParentRzTable { get; set; }

    /// <summary>
    /// Gets or sets the expression used to derive a column key.
    /// </summary>
    [Parameter] public Expression<Func<TItem, object?>>? For { get; set; }

    /// <summary>
    /// Gets or sets the explicit column key.
    /// </summary>
    [Parameter] public string? ColumnKey { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the header is sortable.
    /// </summary>
    [Parameter] public bool Sortable { get; set; }

    /// <summary>
    /// Gets or sets the initial sort direction used when the table is unsorted.
    /// </summary>
    [Parameter] public SortDirection InitialSortDirection { get; set; } = SortDirection.Unset;

    /// <summary>
    /// Gets or sets the content displayed in the header cell.
    /// </summary>
    [Parameter, EditorRequired] public RenderFragment ChildContent { get; set; } = default!;

    /// <summary>
    /// Gets the resolved column key.
    /// </summary>
    public string EffectiveColumnKey => _columnKeyInternal ?? "unknown_column";

    /// <summary>
    /// Gets the current column sort direction.
    /// </summary>
    public SortDirection CurrentSortDirection => _currentSortDirection;

    /// <summary>
    /// Gets the icon displayed for the current sort direction.
    /// </summary>
    protected SvgIcon? SortIndicatorIcon => _currentSortDirection switch
    {
        SortDirection.Ascending => MdiIcon.ArrowUp,
        SortDirection.Descending => MdiIcon.ArrowDown,
        _ => MdiIcon.ArrowUpDownBoldOutline
    };

    /// <summary>
    /// Gets the serialized request payload for emitted sort events.
    /// </summary>
    protected string RequestPayload => $"{{\"page\":1,\"pageSize\":{ParentRzTable?.PageSize ?? 10},\"sortBy\":{GetJsStringValue(_nextSortDirection == SortDirection.Unset ? null : EffectiveColumnKey)},\"sortDirection\":{GetJsStringValue(ToRequestSortDirection(_nextSortDirection))}}}";

    /// <summary>
    /// Gets the script used to dispatch sort events.
    /// </summary>
    protected string SortDispatchScript =>
        "(function(){const host=this.closest('[data-rz-table-id]');if(!host){return;}const table=host.querySelector('[data-slot=table]');const detail={tableId:host.id,tableElementId:table?table.id:null,sourceId:this.id,action:'sort-change',columnKey:this.dataset.columnKey,sortDirection:this.dataset.sortDirection,previousSortDirection:this.dataset.previousSortDirection,request:JSON.parse(this.dataset.request)};this.dispatchEvent(new CustomEvent('rz:tableheadercell:on-sort-change',{detail,bubbles:true,composed:true}));host.dispatchEvent(new CustomEvent('rz:table:on-state-change',{detail,bubbles:true,composed:true}));})();";

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (ParentRzTable == null)
        {
            throw new InvalidOperationException($"{GetType().Name} must be used within an RzTable.");
        }

        if (string.IsNullOrEmpty(Element))
        {
            Element = "th";
        }

        AdditionalAttributes ??= new Dictionary<string, object>();
        AdditionalAttributes.TryAdd("scope", "col");

        ResolveColumnKeyAndRegister();
    }

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        UpdateSortState();
    }

    private void ResolveColumnKeyAndRegister()
    {
        _columnKeyInternal = ResolveColumnKey();

        ParentRzTable!.AddColumnDefinition(new ColumnDefinition<TItem>(
            EffectiveColumnKey,
            ChildContent,
            Sortable,
            InitialSortDirection,
            For
        ));
    }

    private string ResolveColumnKey()
    {
        if (!string.IsNullOrWhiteSpace(ColumnKey))
        {
            return ColumnKey;
        }

        if (For?.Body is MemberExpression memberExpression)
        {
            return memberExpression.Member.Name;
        }

        if (For?.Body is UnaryExpression { Operand: MemberExpression unaryMemberExpression })
        {
            return unaryMemberExpression.Member.Name;
        }

        var resolvedKey = ChildContent.AsMarkupString().Trim().Replace(" ", "_") + "_" + IdGenerator.UniqueId("col");
        return string.IsNullOrWhiteSpace(resolvedKey) || resolvedKey.StartsWith("_") ? IdGenerator.UniqueId("col_anon_") : resolvedKey;
    }

    private void UpdateSortState()
    {
        _currentSortDirection = SortDirection.Unset;
        _nextSortDirection = Sortable
            ? InitialSortDirection is not SortDirection.Unset ? InitialSortDirection : SortDirection.Ascending
            : SortDirection.Unset;

        _ariaSortValue = "none";
        _sortButtonAriaLabel = string.Format(Localizer["RzTable.SortButtonAriaLabelFormat"], ChildContent.AsMarkupString(), Localizer["RzTable.SortDirectionNone"]);

        if (!Sortable || ParentRzTable == null || ParentRzTable.SortBy != EffectiveColumnKey)
        {
            return;
        }

        if (ParentRzTable.SortDirection == SortDirection.Ascending)
        {
            _currentSortDirection = SortDirection.Ascending;
            _nextSortDirection = SortDirection.Descending;
            _ariaSortValue = "ascending";
            _sortButtonAriaLabel = string.Format(Localizer["RzTable.SortButtonAriaLabelFormat"], ChildContent.AsMarkupString(), Localizer["RzTable.SortDirectionAscendingLong"]);
            return;
        }

        if (ParentRzTable.SortDirection == SortDirection.Descending)
        {
            _currentSortDirection = SortDirection.Descending;
            _nextSortDirection = SortDirection.Unset;
            _ariaSortValue = "descending";
            _sortButtonAriaLabel = string.Format(Localizer["RzTable.SortButtonAriaLabelFormat"], ChildContent.AsMarkupString(), Localizer["RzTable.SortDirectionDescendingLong"]);
        }
    }

    private static string? ToRequestSortDirection(SortDirection direction) => direction switch
    {
        SortDirection.Ascending => "asc",
        SortDirection.Descending => "desc",
        _ => null
    };

    private static string GetJsStringValue(string? value) => value is null ? "null" : $"\"{value.Replace("\\", "\\\\").Replace("\"", "\\\"")}\"";

    /// <inheritdoc/>
    protected override TvDescriptor<RzComponent<TableHeaderCellSlots>, TableHeaderCellSlots> GetDescriptor() => Theme.TableHeaderCell;
}
