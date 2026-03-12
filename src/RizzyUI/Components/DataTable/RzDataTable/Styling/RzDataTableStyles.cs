using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Exposes styling variant properties used by <see cref="RzDataTable{TItem}"/>.
/// </summary>
public interface IHasRzDataTableStylingProperties
{
    /// <summary>
    /// Gets a value indicating whether row hover styling is enabled.
    /// </summary>
    public bool HoverRows { get; }

    /// <summary>
    /// Gets a value indicating whether striped row styling is enabled.
    /// </summary>
    public bool StripedRows { get; }

    /// <summary>
    /// Gets the table density mode.
    /// </summary>
    public DataTableDensity Density { get; }
}

/// <summary>
/// Defines slots for <see cref="RzDataTable{TItem}"/>.
/// </summary>
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

/// <summary>
/// Default style descriptor for <see cref="RzDataTable{TItem}"/>.
/// </summary>
public static class RzDataTableStyles
{
    /// <summary>
    /// Gets the default descriptor.
    /// </summary>
    public static readonly TvDescriptor<RzComponent<RzDataTableSlots>, RzDataTableSlots> DefaultDescriptor = new(
        @base: "space-y-3",
        slots: new()
        {
            [s => s.Toolbar] = "flex items-center justify-between gap-3",
            [s => s.SearchBox] = "h-9 w-full max-w-sm rounded-md border border-input bg-background px-3 py-1 text-sm",
            [s => s.ColumnMenu] = "relative",
            [s => s.ToolbarActions] = "flex items-center gap-2",
            [s => s.TableShell] = "rounded-md border border-border",
            [s => s.Header] = "bg-muted/40",
            [s => s.HeaderCell] = "text-muted-foreground",
            [s => s.Body] = "",
            [s => s.BodyRow] = "",
            [s => s.BodyCell] = "",
            [s => s.Empty] = "h-24 text-center text-sm text-muted-foreground",
            [s => s.Loading] = "h-24 text-center text-sm text-muted-foreground",
            [s => s.Pagination] = "flex items-center justify-between gap-2"
        },
        variants: new()
        {
            [c => ((IHasRzDataTableStylingProperties)c).HoverRows] = new Variant<bool, RzDataTableSlots>
            {
                [true] = new() { [s => s.BodyRow] = "transition-colors hover:bg-muted/50" }
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
