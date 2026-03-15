using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Marker interface for <see cref="RzDataTable{TItem}"/> styling compatibility.
/// </summary>
public interface IHasRzDataTableStylingProperties
{
}

/// <summary>
/// Defines the slots available for <see cref="RzDataTable{TItem}"/> styling.
/// </summary>
public sealed partial class RzDataTableSlots : ISlots
{
    /// <summary>
    /// The root slot for the data table host.
    /// </summary>
    [Slot("datatable")]
    public string? Base { get; set; }
}

/// <summary>
/// Provides minimal default styles for <see cref="RzDataTable{TItem}"/>.
/// </summary>
public static class RzDataTableStyles
{
    /// <summary>
    /// The default descriptor for <see cref="RzDataTable{TItem}"/>.
    /// </summary>
    public static readonly TvDescriptor<RzComponent<RzDataTableSlots>, RzDataTableSlots> DefaultDescriptor = new(
        @base: string.Empty
    );
}
