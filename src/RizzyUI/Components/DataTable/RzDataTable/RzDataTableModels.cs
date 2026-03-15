using System.Linq.Expressions;

namespace RizzyUI;

/// <summary>
/// Public configuration contract for <see cref="RzDataTable{TItem}"/>.
/// </summary>
/// <typeparam name="TItem">The item type represented by each table row.</typeparam>
public sealed class RzDataTableConfig<TItem>
{
    /// <summary>
    /// Gets the table column definitions.
    /// </summary>
    public required IReadOnlyList<RzDataTableColumn<TItem>> Columns { get; init; } = Array.Empty<RzDataTableColumn<TItem>>();

    /// <summary>
    /// Gets the feature flags used to configure TanStack table behavior.
    /// </summary>
    public RzDataTableFeatures Features { get; init; } = new();

    /// <summary>
    /// Gets the optional initial table state.
    /// </summary>
    public RzDataTableInitialState InitialState { get; init; } = new();
}

/// <summary>
/// Represents a table column or header group.
/// </summary>
/// <typeparam name="TItem">The item type represented by each table row.</typeparam>
public sealed class RzDataTableColumn<TItem>
{
    /// <summary>
    /// Gets the optional explicit column identifier.
    /// </summary>
    public string? Id { get; init; }

    /// <summary>
    /// Gets the optional strongly typed member accessor used to derive accessor keys.
    /// </summary>
    public Expression<Func<TItem, object?>>? Accessor { get; init; }

    /// <summary>
    /// Gets the optional plain-text column header.
    /// </summary>
    public string? Header { get; init; }

    /// <summary>
    /// Gets the optional plain-text column footer.
    /// </summary>
    public string? Footer { get; init; }

    /// <summary>
    /// Gets the optional client-side cell renderer key.
    /// </summary>
    public string? CellRenderer { get; init; }

    /// <summary>
    /// Gets optional child columns for grouped header definitions.
    /// </summary>
    public IReadOnlyList<RzDataTableColumn<TItem>>? Columns { get; init; }

    /// <summary>
    /// Gets optional per-column sorting enablement.
    /// </summary>
    public bool? EnableSorting { get; init; }

    /// <summary>
    /// Gets optional TanStack sorting function key.
    /// </summary>
    public string? SortingFn { get; init; }

    /// <summary>
    /// Gets optional descending-first sort behavior.
    /// </summary>
    public bool? SortDescFirst { get; init; }

    /// <summary>
    /// Gets optional per-column multi-sort enablement.
    /// </summary>
    public bool? EnableMultiSort { get; init; }

    /// <summary>
    /// Gets optional inversion behavior for sort ranking.
    /// </summary>
    public bool? InvertSorting { get; init; }

    /// <summary>
    /// Gets optional per-column filter enablement.
    /// </summary>
    public bool? EnableColumnFilter { get; init; }

    /// <summary>
    /// Gets optional TanStack filter function key.
    /// </summary>
    public string? FilterFn { get; init; }

    /// <summary>
    /// Gets optional per-column global filter participation.
    /// </summary>
    public bool? EnableGlobalFilter { get; init; }

    /// <summary>
    /// Gets optional per-column visibility toggle enablement.
    /// </summary>
    public bool? EnableHiding { get; init; }

    internal TanStackColumnConfig ToTransport()
    {
        var accessorPath = Accessor is null ? null : ExpressionPathHelper.GetPath(Accessor);

        return new TanStackColumnConfig
        {
            Id = Id ?? accessorPath,
            AccessorKey = accessorPath,
            Header = Header,
            Footer = Footer,
            Cell = CellRenderer,
            Columns = Columns?.Select(x => x.ToTransport()).ToArray(),
            EnableSorting = EnableSorting,
            SortingFn = SortingFn,
            SortDescFirst = SortDescFirst,
            EnableMultiSort = EnableMultiSort,
            InvertSorting = InvertSorting,
            EnableColumnFilter = EnableColumnFilter,
            FilterFn = FilterFn,
            EnableGlobalFilter = EnableGlobalFilter,
            EnableHiding = EnableHiding
        };
    }
}

/// <summary>
/// Feature flags supported by <see cref="RzDataTable{TItem}"/>.
/// </summary>
public sealed class RzDataTableFeatures
{
    /// <summary>
    /// Gets whether sorting is enabled.
    /// </summary>
    public bool EnableSorting { get; init; } = true;

    /// <summary>
    /// Gets whether filtering is enabled.
    /// </summary>
    public bool EnableFiltering { get; init; } = true;

    /// <summary>
    /// Gets whether column filters are enabled.
    /// </summary>
    public bool EnableColumnFilters { get; init; } = true;

    /// <summary>
    /// Gets whether global filtering is enabled.
    /// </summary>
    public bool EnableGlobalFilter { get; init; } = true;

    /// <summary>
    /// Gets whether pagination is enabled.
    /// </summary>
    public bool EnablePagination { get; init; } = true;

    /// <summary>
    /// Gets whether column hiding is enabled.
    /// </summary>
    public bool EnableHiding { get; init; } = true;

    /// <summary>
    /// Gets whether row selection is enabled.
    /// </summary>
    public bool EnableRowSelection { get; init; }

    /// <summary>
    /// Gets whether multiple rows can be selected.
    /// </summary>
    public bool EnableMultiRowSelection { get; init; } = true;

    /// <summary>
    /// Gets whether removing a sort state is allowed.
    /// </summary>
    public bool EnableSortingRemoval { get; init; } = true;

    /// <summary>
    /// Gets whether multi-column sorting is enabled.
    /// </summary>
    public bool EnableMultiSort { get; init; } = true;

    internal TanStackTableOptions ToTransport()
    {
        return new TanStackTableOptions
        {
            EnableSorting = EnableSorting,
            EnableFilters = EnableFiltering,
            EnableColumnFilters = EnableColumnFilters,
            EnableGlobalFilter = EnableGlobalFilter,
            EnableHiding = EnableHiding,
            EnableRowSelection = EnableRowSelection,
            EnableMultiRowSelection = EnableMultiRowSelection,
            EnableSortingRemoval = EnableSortingRemoval,
            EnableMultiSort = EnableMultiSort
        };
    }

    internal TanStackRowModelPipeline ToRowModelPipeline()
    {
        return new TanStackRowModelPipeline
        {
            Core = "getCoreRowModel",
            Filtered = EnableFiltering ? "getFilteredRowModel" : null,
            Sorted = EnableSorting ? "getSortedRowModel" : null,
            Paginated = EnablePagination ? "getPaginationRowModel" : null
        };
    }
}

/// <summary>
/// Initial state contract for supported table features.
/// </summary>
public sealed class RzDataTableInitialState
{
    /// <summary>
    /// Gets the initial sorting entries.
    /// </summary>
    public IReadOnlyList<RzSortDescriptor>? Sorting { get; init; }

    /// <summary>
    /// Gets the initial pagination settings.
    /// </summary>
    public RzPaginationState? Pagination { get; init; }

    /// <summary>
    /// Gets the initial column visibility map.
    /// </summary>
    public Dictionary<string, bool>? ColumnVisibility { get; init; }

    /// <summary>
    /// Gets the initial column filter entries.
    /// </summary>
    public IReadOnlyList<RzColumnFilter>? ColumnFilters { get; init; }

    /// <summary>
    /// Gets the initial global filter value.
    /// </summary>
    public object? GlobalFilter { get; init; }

    /// <summary>
    /// Gets the initial row selection map.
    /// </summary>
    public Dictionary<string, bool>? RowSelection { get; init; }

    internal TanStackTableState ToTransport()
    {
        return new TanStackTableState
        {
            Sorting = Sorting?.Select(x => new TanStackSortingEntry { Id = x.Id, Desc = x.Desc }).ToArray(),
            Pagination = Pagination is null ? null : new TanStackPaginationState { PageIndex = Pagination.PageIndex, PageSize = Pagination.PageSize },
            ColumnVisibility = ColumnVisibility,
            ColumnFilters = ColumnFilters?.Select(x => new TanStackColumnFilter { Id = x.Id, Value = x.Value }).ToArray(),
            GlobalFilter = GlobalFilter,
            RowSelection = RowSelection
        };
    }
}

/// <summary>
/// Represents a single sorting instruction.
/// </summary>
public sealed class RzSortDescriptor
{
    /// <summary>
    /// Gets the column identifier.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// Gets whether sorting is descending.
    /// </summary>
    public bool Desc { get; init; }
}

/// <summary>
/// Represents pagination state.
/// </summary>
public sealed class RzPaginationState
{
    /// <summary>
    /// Gets the initial page index.
    /// </summary>
    public int PageIndex { get; init; }

    /// <summary>
    /// Gets the initial page size.
    /// </summary>
    public int PageSize { get; init; } = 10;
}

/// <summary>
/// Represents a single column filter.
/// </summary>
public sealed class RzColumnFilter
{
    /// <summary>
    /// Gets the column identifier.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// Gets the filter value.
    /// </summary>
    public object? Value { get; init; }
}

internal static class ExpressionPathHelper
{
    public static string GetPath(LambdaExpression expression)
    {
        var members = new Stack<string>();
        Expression current = expression.Body;

        while (true)
        {
            if (current is UnaryExpression unary &&
                (unary.NodeType == ExpressionType.Convert || unary.NodeType == ExpressionType.ConvertChecked))
            {
                current = unary.Operand;
                continue;
            }

            if (current is MemberExpression member)
            {
                members.Push(member.Member.Name);
                current = member.Expression!;
                continue;
            }

            if (current is ParameterExpression)
            {
                break;
            }

            throw new InvalidOperationException(
                $"Expression '{expression}' must be a simple member access expression such as x => x.Id or x => x.Customer.Id.");
        }

        if (members.Count == 0)
        {
            throw new InvalidOperationException($"Expression '{expression}' did not resolve to a member path.");
        }

        return string.Join('.', members);
    }
}
