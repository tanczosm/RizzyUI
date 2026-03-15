namespace RizzyUI;

/// <summary>
/// Defines available row-selection modes for <see cref="RzDataTable{TItem}"/>.
/// </summary>
public enum DataTableSelectionMode
{
    /// <summary>
    /// Selection is disabled.
    /// </summary>
    None,

    /// <summary>
    /// A single row can be selected at a time.
    /// </summary>
    Single,

    /// <summary>
    /// Multiple rows can be selected.
    /// </summary>
    Multiple
}

/// <summary>
/// Defines density presets for <see cref="RzDataTable{TItem}"/> rows.
/// </summary>
public enum DataTableDensity
{
    /// <summary>
    /// Default density.
    /// </summary>
    Default,

    /// <summary>
    /// Compact density.
    /// </summary>
    Compact,

    /// <summary>
    /// Comfortable density.
    /// </summary>
    Comfortable
}

/// <summary>
/// Defines horizontal alignment for a data table column.
/// </summary>
public enum DataTableColumnAlign
{
    /// <summary>
    /// Start alignment.
    /// </summary>
    Start,

    /// <summary>
    /// Center alignment.
    /// </summary>
    Center,

    /// <summary>
    /// End alignment.
    /// </summary>
    End
}

/// <summary>
/// Describes an initial sorting rule for a data table column.
/// </summary>
public sealed class DataTableSortDefinition
{
    /// <summary>
    /// Gets or sets the column identifier.
    /// </summary>
    public string ColumnId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether descending sort should be applied.
    /// </summary>
    public bool Descending { get; set; }
}
