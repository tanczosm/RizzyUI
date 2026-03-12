using Microsoft.AspNetCore.Components;
using System.Linq.Expressions;
using TailwindVariants.NET;

namespace RizzyUI;

public partial class DataTableColumn<TItem> : RzComponent<DataTableColumnSlots>
{
    

    [CascadingParameter] internal RzDataTable<TItem>? ParentDataTable { get; set; }

    [Parameter, EditorRequired] public string Id { get; set; } = string.Empty;
    [Parameter, EditorRequired] public string HeaderText { get; set; } = string.Empty;
    [Parameter, EditorRequired] public Expression<Func<TItem, object?>>? Field { get; set; }

    [Parameter] public bool Sortable { get; set; }
    [Parameter] public bool Searchable { get; set; }
    [Parameter] public bool Visible { get; set; } = true;
    [Parameter] public bool CanHide { get; set; } = true;
    [Parameter] public DataTableColumnAlign Align { get; set; } = DataTableColumnAlign.Start;
    [Parameter] public string? Width { get; set; }
    [Parameter] public string? NullDisplayText { get; set; }
    [Parameter] public string? HeaderClass { get; set; }
    [Parameter] public string? CellClass { get; set; }
    [Parameter] public RenderFragment? ClientTemplate { get; set; }

    internal string TemplateId => $"{ParentDataTable?.Id}-{Id}-template";

    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (ParentDataTable is null)
            throw new InvalidOperationException($"{nameof(DataTableColumn<TItem>)} must be used inside {nameof(RzDataTable<TItem>)}.");

        ParentDataTable.RegisterColumn(this);
    }

    protected override TvDescriptor<RzComponent<DataTableColumnSlots>, DataTableColumnSlots> GetDescriptor() => Theme.DataTableColumn;
}
