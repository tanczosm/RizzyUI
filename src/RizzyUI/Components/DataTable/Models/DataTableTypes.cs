namespace RizzyUI;

/// <summary>
/// Specifies row selection behavior for <see cref="RzDataTable{TItem}"/>.
/// </summary>
public enum DataTableSelectionMode
{
    /// <summary>
    /// Row selection is disabled.
    /// </summary>
    None,

    /// <summary>
    /// Only one row can be selected at a time.
    /// </summary>
    Single,

    /// <summary>
    /// Multiple rows can be selected.
    /// </summary>
    Multiple
}

/// <summary>
/// Specifies the row density for <see cref="RzDataTable{TItem}"/>.
/// </summary>
public enum DataTableDensity
{
    /// <summary>
    /// Default row spacing.
    /// </summary>
    Default,

    /// <summary>
    /// Compact row spacing.
    /// </summary>
    Compact,

    /// <summary>
    /// Comfortable row spacing.
    /// </summary>
    Comfortable
}

/// <summary>
/// Specifies horizontal alignment for a data table column.
/// </summary>
public enum DataTableColumnAlign
{
    /// <summary>
    /// Align to the start.
    /// </summary>
    Start,

    /// <summary>
    /// Center align.
    /// </summary>
    Center,

    /// <summary>
    /// Align to the end.
    /// </summary>
    End
}

/// <summary>
/// Defines an initial sort entry for <see cref="RzDataTable{TItem}"/>.
/// </summary>
public sealed class DataTableSortDefinition
{
    /// <summary>
    /// Gets or sets the identifier of the sorted column.
    /// </summary>
    public string ColumnId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether sorting is descending.
    /// </summary>
    public bool Descending { get; set; }
}
