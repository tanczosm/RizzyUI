using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace RizzyUI;

internal sealed class TanStackTableTransport<TItem>
{
    [JsonPropertyName("data")]
    public required IReadOnlyList<TItem> Data { get; init; }

    [JsonPropertyName("columns")]
    public required IReadOnlyList<TanStackColumnConfig> Columns { get; init; }

    [JsonPropertyName("initialState")]
    public required TanStackTableState InitialState { get; init; }

    [JsonPropertyName("options")]
    public required TanStackTableOptions Options { get; init; }

    [JsonPropertyName("rowModelPipeline")]
    public required TanStackRowModelPipeline RowModelPipeline { get; init; }

    [JsonPropertyName("rowStructure")]
    public required TanStackRowStructure RowStructure { get; init; }
}

internal sealed class TanStackColumnConfig
{
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("accessorKey")]
    public string? AccessorKey { get; init; }

    [JsonPropertyName("header")]
    public string? Header { get; init; }

    [JsonPropertyName("footer")]
    public string? Footer { get; init; }

    [JsonPropertyName("cell")]
    public string? Cell { get; init; }

    [JsonPropertyName("columns")]
    public IReadOnlyList<TanStackColumnConfig>? Columns { get; init; }

    [JsonPropertyName("enableSorting")]
    public bool? EnableSorting { get; init; }

    [JsonPropertyName("sortingFn")]
    public string? SortingFn { get; init; }

    [JsonPropertyName("sortDescFirst")]
    public bool? SortDescFirst { get; init; }

    [JsonPropertyName("enableMultiSort")]
    public bool? EnableMultiSort { get; init; }

    [JsonPropertyName("invertSorting")]
    public bool? InvertSorting { get; init; }

    [JsonPropertyName("enableColumnFilter")]
    public bool? EnableColumnFilter { get; init; }

    [JsonPropertyName("filterFn")]
    public string? FilterFn { get; init; }

    [JsonPropertyName("enableGlobalFilter")]
    public bool? EnableGlobalFilter { get; init; }

    [JsonPropertyName("enableHiding")]
    public bool? EnableHiding { get; init; }
}

internal sealed class TanStackTableOptions
{
    [JsonPropertyName("enableSorting")]
    public bool EnableSorting { get; init; }

    [JsonPropertyName("enableFilters")]
    public bool EnableFilters { get; init; }

    [JsonPropertyName("enableColumnFilters")]
    public bool EnableColumnFilters { get; init; }

    [JsonPropertyName("enableGlobalFilter")]
    public bool EnableGlobalFilter { get; init; }

    [JsonPropertyName("enableHiding")]
    public bool EnableHiding { get; init; }

    [JsonPropertyName("enableRowSelection")]
    public bool EnableRowSelection { get; init; }

    [JsonPropertyName("enableMultiRowSelection")]
    public bool EnableMultiRowSelection { get; init; }

    [JsonPropertyName("enableSortingRemoval")]
    public bool EnableSortingRemoval { get; init; }

    [JsonPropertyName("enableMultiSort")]
    public bool EnableMultiSort { get; init; }
}

internal sealed class TanStackRowModelPipeline
{
    [JsonPropertyName("core")]
    public required string Core { get; init; }

    [JsonPropertyName("filtered")]
    public string? Filtered { get; init; }

    [JsonPropertyName("sorted")]
    public string? Sorted { get; init; }

    [JsonPropertyName("paginated")]
    public string? Paginated { get; init; }
}

internal sealed class TanStackTableState
{
    [JsonPropertyName("sorting")]
    public IReadOnlyList<TanStackSortingEntry>? Sorting { get; init; }

    [JsonPropertyName("pagination")]
    public TanStackPaginationState? Pagination { get; init; }

    [JsonPropertyName("columnVisibility")]
    public IReadOnlyDictionary<string, bool>? ColumnVisibility { get; init; }

    [JsonPropertyName("columnFilters")]
    public IReadOnlyList<TanStackColumnFilter>? ColumnFilters { get; init; }

    [JsonPropertyName("globalFilter")]
    public object? GlobalFilter { get; init; }

    [JsonPropertyName("rowSelection")]
    public IReadOnlyDictionary<string, bool>? RowSelection { get; init; }
}

internal sealed class TanStackSortingEntry
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("desc")]
    public bool Desc { get; init; }
}

internal sealed class TanStackPaginationState
{
    [JsonPropertyName("pageIndex")]
    public int PageIndex { get; init; }

    [JsonPropertyName("pageSize")]
    public int PageSize { get; init; }
}

internal sealed class TanStackColumnFilter
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("value")]
    public object? Value { get; init; }
}

internal sealed class TanStackRowStructure
{
    [JsonPropertyName("rowIdPath")]
    public required string RowIdPath { get; init; }
}

internal static class RzDataTableTransportBuilder
{
    public static TanStackTableTransport<TItem> Build<TItem>(
        IReadOnlyList<TItem> items,
        RzDataTableConfig<TItem> config,
        Expression<Func<TItem, object?>> rowIdSelector)
    {
        return new TanStackTableTransport<TItem>
        {
            Data = items,
            Columns = config.Columns.Select(x => x.ToTransport()).ToArray(),
            InitialState = config.InitialState.ToTransport(),
            Options = config.Features.ToTransport(),
            RowModelPipeline = config.Features.ToRowModelPipeline(),
            RowStructure = new TanStackRowStructure
            {
                RowIdPath = ExpressionPathHelper.GetPath(rowIdSelector)
            }
        };
    }
}
