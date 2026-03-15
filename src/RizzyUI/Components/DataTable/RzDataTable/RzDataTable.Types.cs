namespace RizzyUI;

public enum DataTableSelectionMode
{
    None,
    Single,
    Multiple
}

public enum DataTableDensity
{
    Default,
    Compact,
    Comfortable
}

public enum DataTableColumnAlign
{
    Start,
    Center,
    End
}

public sealed class DataTableSortDefinition
{
    public string ColumnId { get; set; } = string.Empty;
    public bool Descending { get; set; }
}
