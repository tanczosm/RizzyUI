using TailwindVariants.NET;

namespace RizzyUI;

public interface IHasRzDataTableStylingProperties
{
    public DataTableDensity Density { get; }
    public bool HoverRows { get; }
    public bool StripedRows { get; }
    public bool ShowToolbar { get; }
}

public sealed partial class RzDataTableSlots : ISlots
{
    [Slot("datatable")]
    public string? Base { get; set; }

    [Slot("toolbar")]
    public string? Toolbar { get; set; }

    [Slot("search-box")]
    public string? SearchBox { get; set; }

    [Slot("column-menu")]
    public string? ColumnMenu { get; set; }

    [Slot("toolbar-actions")]
    public string? ToolbarActions { get; set; }

    [Slot("table-shell")]
    public string? TableShell { get; set; }

    [Slot("header")]
    public string? Header { get; set; }

    [Slot("header-cell")]
    public string? HeaderCell { get; set; }

    [Slot("body")]
    public string? Body { get; set; }

    [Slot("body-row")]
    public string? BodyRow { get; set; }

    [Slot("body-cell")]
    public string? BodyCell { get; set; }

    [Slot("empty")]
    public string? Empty { get; set; }

    [Slot("loading")]
    public string? Loading { get; set; }

    [Slot("pagination")]
    public string? Pagination { get; set; }
}

public static class RzDataTableStyles
{
    public static readonly TvDescriptor<RzComponent<RzDataTableSlots>, RzDataTableSlots> DefaultDescriptor = new(
        @base: "w-full space-y-3",
        slots: new()
        {
            [s => s.Toolbar] = "flex flex-wrap items-center gap-2",
            [s => s.SearchBox] = "h-9 w-full max-w-sm rounded-md border border-input bg-background px-3 text-sm",
            [s => s.ColumnMenu] = "relative",
            [s => s.ToolbarActions] = "ml-auto flex items-center gap-2",
            [s => s.TableShell] = "rounded-md border",
            [s => s.Header] = "text-muted-foreground",
            [s => s.HeaderCell] = "h-10 px-2 text-left align-middle font-medium",
            [s => s.Body] = "",
            [s => s.BodyRow] = "border-b",
            [s => s.BodyCell] = "p-2 align-middle",
            [s => s.Empty] = "h-24 text-center text-sm text-muted-foreground",
            [s => s.Loading] = "h-24 text-center text-sm text-muted-foreground",
            [s => s.Pagination] = "flex items-center justify-between gap-2 text-sm"
        },
        variants: new()
        {
            [c => ((IHasRzDataTableStylingProperties)c).Density] = new Variant<DataTableDensity, RzDataTableSlots>
            {
                [DataTableDensity.Compact] = new() { [s => s.HeaderCell] = "h-8 py-1", [s => s.BodyCell] = "py-1" },
                [DataTableDensity.Default] = new() { [s => s.HeaderCell] = "h-10 py-2", [s => s.BodyCell] = "py-2" },
                [DataTableDensity.Comfortable] = new() { [s => s.HeaderCell] = "h-12 py-3", [s => s.BodyCell] = "py-3" }
            },
            [c => ((IHasRzDataTableStylingProperties)c).HoverRows] = new Variant<bool, RzDataTableSlots>
            {
                [true] = new() { [s => s.BodyRow] = "hover:bg-muted/40" }
            },
            [c => ((IHasRzDataTableStylingProperties)c).StripedRows] = new Variant<bool, RzDataTableSlots>
            {
                [true] = new() { [s => s.BodyRow] = "odd:bg-muted/20" }
            },
            [c => ((IHasRzDataTableStylingProperties)c).ShowToolbar] = new Variant<bool, RzDataTableSlots>
            {
                [false] = new() { [s => s.Toolbar] = "hidden" }
            }
        }
    );
}
