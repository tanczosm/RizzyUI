namespace RizzyUI;

/// <summary>
/// Defines row selection behavior for <see cref="RzDataTable{TItem}"/>.
/// </summary>
public enum DataTableSelectionMode
{
    /// <summary>
    /// Disables row selection.
    /// </summary>
    None,

    /// <summary>
    /// Allows one selected row at a time.
    /// </summary>
    Single,

    /// <summary>
    /// Allows selecting multiple rows.
    /// </summary>
    Multiple
}

/// <summary>
/// Defines row density variants for <see cref="RzDataTable{TItem}"/>.
/// </summary>
public enum DataTableDensity
{
    /// <summary>
    /// Uses the default table row spacing.
    /// </summary>
    Default,

    /// <summary>
    /// Uses compact row spacing.
    /// </summary>
    Compact,

    /// <summary>
    /// Uses comfortable row spacing.
    /// </summary>
    Comfortable
}

/// <summary>
/// Defines horizontal alignment options for a data table column.
/// </summary>
public enum DataTableColumnAlign
{
    /// <summary>
    /// Aligns content to the start.
    /// </summary>
    Start,

    /// <summary>
    /// Aligns content to the center.
    /// </summary>
    Center,

    /// <summary>
    /// Aligns content to the end.
    /// </summary>
    End
}

/// <summary>
/// Defines an initial sort instruction for <see cref="RzDataTable{TItem}"/>.
/// </summary>
public sealed class DataTableSortDefinition
{
    /// <summary>
    /// Gets or sets the column identifier.
    /// </summary>
    public string ColumnId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether sorting is descending.
    /// </summary>
    public bool Descending { get; set; }
}
