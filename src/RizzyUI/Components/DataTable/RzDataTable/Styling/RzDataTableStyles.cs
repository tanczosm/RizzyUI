using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Styling surface for <see cref="RzDataTable{TItem}"/> descriptor variants.
/// </summary>
public interface IHasRzDataTableStylingProperties
{
    /// <summary>
    /// Gets a value indicating whether hover row styling is enabled.
    /// </summary>
    public bool HoverRows { get; }

    /// <summary>
    /// Gets a value indicating whether striped row styling is enabled.
    /// </summary>
    public bool StripedRows { get; }

    /// <summary>
    /// Gets the current row density value.
    /// </summary>
    public DataTableDensity Density { get; }
}

/// <summary>
/// Defines styling slots for <see cref="RzDataTable{TItem}"/>.
/// </summary>
public sealed partial class RzDataTableSlots : ISlots
{
    /// <summary>Base root slot.</summary>
    [Slot("datatable")]
    public string? Base { get; set; }

    /// <summary>Toolbar container slot.</summary>
    [Slot("toolbar")]
    public string? Toolbar { get; set; }

    /// <summary>Search input slot.</summary>
    [Slot("search-box")]
    public string? SearchBox { get; set; }

    /// <summary>Column visibility menu slot.</summary>
    [Slot("column-menu")]
    public string? ColumnMenu { get; set; }

    /// <summary>Toolbar actions slot.</summary>
    [Slot("toolbar-actions")]
    public string? ToolbarActions { get; set; }

    /// <summary>Outer table shell slot.</summary>
    [Slot("table-shell")]
    public string? TableShell { get; set; }

    /// <summary>Header section slot.</summary>
    [Slot("header")]
    public string? Header { get; set; }

    /// <summary>Header cell slot.</summary>
    [Slot("header-cell")]
    public string? HeaderCell { get; set; }

    /// <summary>Body section slot.</summary>
    [Slot("body")]
    public string? Body { get; set; }

    /// <summary>Body row slot.</summary>
    [Slot("body-row")]
    public string? BodyRow { get; set; }

    /// <summary>Body cell slot.</summary>
    [Slot("body-cell")]
    public string? BodyCell { get; set; }

    /// <summary>Empty-state slot.</summary>
    [Slot("empty")]
    public string? Empty { get; set; }

    /// <summary>Loading-state slot.</summary>
    [Slot("loading")]
    public string? Loading { get; set; }

    /// <summary>Pagination slot.</summary>
    [Slot("pagination")]
    public string? Pagination { get; set; }
}

/// <summary>
/// Default styling descriptor for <see cref="RzDataTable{TItem}"/>.
/// </summary>
public static class RzDataTableStyles
{
    /// <summary>
    /// The default descriptor.
    /// </summary>
    public static readonly TvDescriptor<RzComponent<RzDataTableSlots>, RzDataTableSlots> DefaultDescriptor = new(
        @base: "w-full space-y-4",
        slots: new()
        {
            [s => s.Toolbar] = "flex flex-wrap items-center justify-between gap-3",
            [s => s.SearchBox] = "h-9 w-full max-w-sm rounded-md border border-input bg-transparent px-3 py-1 text-sm shadow-xs outline-none focus-visible:ring-2 focus-visible:ring-ring",
            [s => s.ColumnMenu] = "relative",
            [s => s.ToolbarActions] = "flex items-center gap-2",
            [s => s.TableShell] = "rounded-md border",
            [s => s.Header] = "bg-muted/50",
            [s => s.HeaderCell] = "text-foreground align-middle",
            [s => s.Body] = "align-middle",
            [s => s.BodyRow] = "border-b transition-colors",
            [s => s.BodyCell] = "align-middle",
            [s => s.Empty] = "h-24 text-center text-muted-foreground",
            [s => s.Loading] = "h-24 text-center text-muted-foreground",
            [s => s.Pagination] = "flex items-center justify-between gap-3 pt-2"
        },
        variants: new()
        {
            [c => ((IHasRzDataTableStylingProperties)c).Density] = new Variant<DataTableDensity, RzDataTableSlots>
            {
                [DataTableDensity.Default] = new()
                {
                    [s => s.HeaderCell] = "h-10 px-4",
                    [s => s.BodyCell] = "px-4 py-2"
                },
                [DataTableDensity.Compact] = new()
                {
                    [s => s.HeaderCell] = "h-8 px-3 text-xs",
                    [s => s.BodyCell] = "px-3 py-1.5 text-xs"
                },
                [DataTableDensity.Comfortable] = new()
                {
                    [s => s.HeaderCell] = "h-12 px-5",
                    [s => s.BodyCell] = "px-5 py-3"
                }
            },
            [c => ((IHasRzDataTableStylingProperties)c).HoverRows] = new Variant<bool, RzDataTableSlots>
            {
                [true] = new()
                {
                    [s => s.BodyRow] = "hover:bg-muted/50"
                }
            },
            [c => ((IHasRzDataTableStylingProperties)c).StripedRows] = new Variant<bool, RzDataTableSlots>
            {
                [true] = new()
                {
                    [s => s.Body] = "[&>tr:nth-child(even)]:bg-muted/30"
                }
            }
        });
}
