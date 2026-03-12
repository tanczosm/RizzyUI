using Microsoft.AspNetCore.Components;
using Rizzy.Utility;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Defines styling properties for a <see cref="TableRow{TItem}"/> component.
/// </summary>
public interface IHasTableRowStylingProperties
{
    /// <summary>
    /// Gets a value indicating whether the row is even-numbered.
    /// </summary>
    public bool IsEven { get; }

    /// <summary>
    /// Gets a value indicating whether hover styling should be applied.
    /// </summary>
    public bool IsHoverable { get; }

    /// <summary>
    /// Gets a value indicating whether the row is selected.
    /// </summary>
    public bool IsSelected { get; }
}

/// <summary>
/// Represents a table row (<c>tr</c>) within an <see cref="RzTable{TItem}"/>.
/// </summary>
/// <typeparam name="TItem">The row item type.</typeparam>
[CascadingTypeParameter(nameof(TItem))]
public partial class TableRow<TItem> : RzComponent<TableRowSlots>, IHasTableRowStylingProperties
{
    /// <summary>
    /// Gets or sets the parent table component.
    /// </summary>
    [CascadingParameter(Name = "ParentRzTable")]
    protected RzTable<TItem>? ParentRzTable { get; set; }

    /// <summary>
    /// Gets or sets the row index.
    /// </summary>
    [CascadingParameter(Name = "RowIndex")]
    protected int? RowIndex { get; set; }

    /// <summary>
    /// Gets or sets the item associated with this row.
    /// </summary>
    [Parameter] public TItem? Item { get; set; }

    /// <summary>
    /// Gets or sets a stable serializable row key.
    /// </summary>
    [Parameter] public string? RowKey { get; set; }

    /// <summary>
    /// Gets or sets the row content.
    /// </summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets a value indicating whether this row is even-numbered for striped styling.
    /// </summary>
    public bool IsEven => ParentRzTable?.Striped == true && RowIndex.HasValue && RowIndex.Value % 2 != 0;

    /// <summary>
    /// Gets a value indicating whether hover styling should be applied.
    /// </summary>
    public bool IsHoverable => ParentRzTable?.Hoverable ?? false;

    /// <summary>
    /// Gets a value indicating whether this row is selected.
    /// </summary>
    public bool IsSelected => !string.IsNullOrWhiteSpace(ResolvedRowKey) && ParentRzTable?.SelectedItems.Contains(ResolvedRowKey) == true;

    /// <summary>
    /// Gets the resolved row key used for event payloads.
    /// </summary>
    protected string ResolvedRowKey => RowKey ?? Id;

    /// <summary>
    /// Gets the selection change dispatch script.
    /// </summary>
    protected string SelectionDispatchScript =>
        "(function(){const host=this.closest('[data-rz-table-id]');if(!host){return;}const table=host.querySelector('[data-slot=table]');const selected=this.dataset.state!=='selected';const detail={tableId:host.id,tableElementId:table?table.id:null,sourceId:this.id,action:'selection-change',rowKey:this.dataset.rowKey,selected,selectionMode:this.dataset.selectionMode};this.dataset.state=selected?'selected':'unselected';this.setAttribute('data-state',this.dataset.state);this.dispatchEvent(new CustomEvent('rz:tablerow:on-selection-change',{detail,bubbles:true,composed:true}));host.dispatchEvent(new CustomEvent('rz:table:on-selection-change',{detail,bubbles:true,composed:true}));host.dispatchEvent(new CustomEvent('rz:table:on-state-change',{detail,bubbles:true,composed:true}));})();";

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (string.IsNullOrEmpty(Element))
        {
            Element = "tr";
        }

        if (ParentRzTable != null)
        {
            Id = $"{ParentRzTable.TableId}-row";

            if (RowIndex != null)
            {
                Id += $"-{RowIndex}";
            }
            else
            {
                Id = IdGenerator.UniqueId(Id);
            }
        }
    }

    /// <inheritdoc/>
    protected override TvDescriptor<RzComponent<TableRowSlots>, TableRowSlots> GetDescriptor() => Theme.TableRow;
}
