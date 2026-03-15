using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// SSR-first generic host component that normalizes configuration for an Alpine + TanStack data table runtime.
/// </summary>
/// <typeparam name="TItem">The row item type.</typeparam>
public partial class RzDataTable<TItem> : RzComponent<RzDataTableSlots>, IHasRzDataTableStylingProperties
{
    private static readonly JsonSerializerOptions TransportSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private string _assets = "[]";
    private string _configJson = "{}";

    [Inject]
    private IOptions<RizzyUIConfig> RizzyUiConfig { get; set; } = default!;

    /// <summary>
    /// Gets or sets the in-memory items used as table data.
    /// </summary>
    [Parameter, EditorRequired]
    public IReadOnlyList<TItem> Items { get; set; } = Array.Empty<TItem>();

    /// <summary>
    /// Gets or sets the table configuration.
    /// </summary>
    [Parameter, EditorRequired]
    public RzDataTableConfig<TItem> Config { get; set; } = default!;

    /// <summary>
    /// Gets or sets the row-id selector expression used to produce stable row keys.
    /// </summary>
    [Parameter, EditorRequired]
    public Expression<Func<TItem, object?>> RowIdSelector { get; set; } = default!;

    /// <summary>
    /// Gets or sets projected table markup authored by consumers.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets optional runtime asset keys for the host container.
    /// </summary>
    [Parameter]
    public string[] ComponentAssetKeys { get; set; } = [];

    /// <summary>
    /// Gets the configuration script element id used by the Alpine runtime.
    /// </summary>
    protected string ConfigScriptId => $"{Id}-config";

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (string.IsNullOrEmpty(Element))
        {
            Element = "div";
        }
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        ValidateParameters();
        UpdateAssets();

        var transport = RzDataTableTransportBuilder.Build(Items, Config, RowIdSelector);
        ValidateNormalizedColumns(transport.Columns);
        ValidateRowIds(transport.Data, transport.RowStructure.RowIdPath);
        _configJson = JsonSerializer.Serialize(transport, TransportSerializerOptions);
    }

    private void ValidateParameters()
    {
        if (Items is null)
        {
            throw new InvalidOperationException($"{nameof(Items)} must be provided.");
        }

        if (Config is null)
        {
            throw new InvalidOperationException($"{nameof(Config)} must be provided.");
        }

        if (RowIdSelector is null)
        {
            throw new InvalidOperationException($"{nameof(RowIdSelector)} must be provided.");
        }

        if (Config.Columns.Count == 0)
        {
            throw new InvalidOperationException("At least one column definition is required.");
        }
    }

    private static void ValidateNormalizedColumns(IReadOnlyList<TanStackColumnConfig> columns)
    {
        var ids = new HashSet<string>(StringComparer.Ordinal);

        void Visit(IReadOnlyList<TanStackColumnConfig> currentColumns)
        {
            foreach (var column in currentColumns)
            {
                var hasChildren = column.Columns is { Count: > 0 };

                if (hasChildren)
                {
                    Visit(column.Columns!);
                    continue;
                }

                if (string.IsNullOrWhiteSpace(column.Id))
                {
                    throw new InvalidOperationException("Leaf columns must define an Id or Accessor member path.");
                }

                if (!ids.Add(column.Id))
                {
                    throw new InvalidOperationException($"Duplicate normalized column id '{column.Id}' is not allowed.");
                }
            }
        }

        Visit(columns);
    }

    private static void ValidateRowIds(IReadOnlyList<TItem> data, string rowIdPath)
    {
        var segments = rowIdPath.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var rowIds = new HashSet<string>(StringComparer.Ordinal);

        for (var i = 0; i < data.Count; i++)
        {
            var item = data[i];
            var value = ResolvePathValue(item, segments);
            var rowId = value?.ToString();

            if (string.IsNullOrWhiteSpace(rowId))
            {
                throw new InvalidOperationException($"Row id resolved to null or empty for item index {i} using path '{rowIdPath}'.");
            }

            if (!rowIds.Add(rowId))
            {
                throw new InvalidOperationException($"Duplicate row id '{rowId}' is not allowed.");
            }
        }
    }

    private static object? ResolvePathValue(object? instance, IReadOnlyList<string> segments)
    {
        object? current = instance;

        foreach (var segment in segments)
        {
            if (current is null)
            {
                return null;
            }

            var property = current.GetType().GetProperty(segment);
            if (property is null)
            {
                throw new InvalidOperationException($"Could not resolve property '{segment}' on type '{current.GetType().Name}'.");
            }

            current = property.GetValue(current);
        }

        return current;
    }

    private void UpdateAssets()
    {
        var assetUrls = ComponentAssetKeys
            .Select(key => RizzyUiConfig.Value.AssetUrls.TryGetValue(key, out var url) ? url : null)
            .Where(url => !string.IsNullOrWhiteSpace(url))
            .ToArray();

        _assets = JsonSerializer.Serialize(assetUrls, TransportSerializerOptions);
    }

    /// <inheritdoc />
    protected override TvDescriptor<RzComponent<RzDataTableSlots>, RzDataTableSlots> GetDescriptor() => Theme.RzDataTable;
}
