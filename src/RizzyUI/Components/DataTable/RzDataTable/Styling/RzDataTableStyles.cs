using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Defines style-driving properties for <see cref="RzDataTable{TItem}"/>.
/// </summary>
public interface IHasRzDataTableStylingProperties
{
    /// <summary>
    /// Gets a value indicating whether rows should show hover feedback.
    /// </summary>
    public bool HoverRows { get; }

    /// <summary>
    /// Gets a value indicating whether rows should be striped.
    /// </summary>
    public bool StripedRows { get; }

    /// <summary>
    /// Gets the configured row density.
    /// </summary>
    public DataTableDensity Density { get; }
}

/// <summary>
/// Defines slots for <see cref="RzDataTable{TItem}"/> styling.
/// </summary>
public sealed partial class RzDataTableSlots : ISlots
{
    /// <summary>Root container slot.</summary>
    [Slot("datatable")]
    public string? Base { get; set; }

    /// <summary>Toolbar slot.</summary>
    [Slot("toolbar")]
    public string? Toolbar { get; set; }

    /// <summary>Search box slot.</summary>
    [Slot("search-box")]
    public string? SearchBox { get; set; }

    /// <summary>Column visibility menu slot.</summary>
    [Slot("column-menu")]
    public string? ColumnMenu { get; set; }

    /// <summary>Toolbar actions slot.</summary>
    [Slot("toolbar-actions")]
    public string? ToolbarActions { get; set; }

    /// <summary>Enhanced table shell slot.</summary>
    [Slot("table-shell")]
    public string? TableShell { get; set; }

    /// <summary>Header region slot.</summary>
    [Slot("header")]
    public string? Header { get; set; }

    /// <summary>Header cell slot.</summary>
    [Slot("header-cell")]
    public string? HeaderCell { get; set; }

    /// <summary>Body region slot.</summary>
    [Slot("body")]
    public string? Body { get; set; }

    /// <summary>Body row slot.</summary>
    [Slot("body-row")]
    public string? BodyRow { get; set; }

    /// <summary>Body cell slot.</summary>
    [Slot("body-cell")]
    public string? BodyCell { get; set; }

    /// <summary>Empty state slot.</summary>
    [Slot("empty")]
    public string? Empty { get; set; }

    /// <summary>Loading state slot.</summary>
    [Slot("loading")]
    public string? Loading { get; set; }

    /// <summary>Pagination slot.</summary>
    [Slot("pagination")]
    public string? Pagination { get; set; }
}

/// <summary>
/// Provides default styles for <see cref="RzDataTable{TItem}"/>.
/// </summary>
public static class RzDataTableStyles
{
    /// <summary>
    /// The default style descriptor.
    /// </summary>
    public static readonly TvDescriptor<RzComponent<RzDataTableSlots>, RzDataTableSlots> DefaultDescriptor = new(
        @base: "w-full space-y-3",
        slots: new()
        {
            [s => s.Toolbar] = "flex flex-wrap items-center justify-between gap-2",
            [s => s.SearchBox] = "h-9 w-full max-w-sm rounded-md border border-input bg-background px-3 py-1 text-sm shadow-xs",
            [s => s.ColumnMenu] = "relative",
            [s => s.ToolbarActions] = "flex items-center gap-2",
            [s => s.TableShell] = "rounded-md border border-border",
            [s => s.Header] = "bg-muted/40",
            [s => s.HeaderCell] = "whitespace-nowrap",
            [s => s.Body] = "",
            [s => s.BodyRow] = "",
            [s => s.BodyCell] = "",
            [s => s.Empty] = "text-muted-foreground py-8 text-center text-sm",
            [s => s.Loading] = "text-muted-foreground py-8 text-center text-sm",
            [s => s.Pagination] = "flex flex-wrap items-center justify-between gap-2"
        },
        variants: new()
        {
            [c => ((IHasRzDataTableStylingProperties)c).HoverRows] = new Variant<bool, RzDataTableSlots>
            {
                [true] = new() { [s => s.BodyRow] = "hover:bg-muted/50" }
            },
            [c => ((IHasRzDataTableStylingProperties)c).StripedRows] = new Variant<bool, RzDataTableSlots>
            {
                [true] = new() { [s => s.BodyRow] = "odd:bg-muted/20" }
            },
            [c => ((IHasRzDataTableStylingProperties)c).Density] = new Variant<DataTableDensity, RzDataTableSlots>
            {
                [DataTableDensity.Default] = new() { [s => s.BodyCell] = "py-3", [s => s.HeaderCell] = "py-3" },
                [DataTableDensity.Compact] = new() { [s => s.BodyCell] = "py-2", [s => s.HeaderCell] = "py-2" },
                [DataTableDensity.Comfortable] = new() { [s => s.BodyCell] = "py-4", [s => s.HeaderCell] = "py-4" }
            }
        }
    );
}
