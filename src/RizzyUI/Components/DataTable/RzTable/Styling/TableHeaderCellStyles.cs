using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Defines the slots available for styling in the TableHeaderCell component.
/// </summary>
public sealed partial class TableHeaderCellSlots : ISlots
{
    /// <summary>
    /// The base slot for the <c>th</c> element.
    /// </summary>
    [Slot("table-head")]
    public string? Base { get; set; }
}

/// <summary>
/// Provides the default styling descriptor for the TableHeaderCell component.
/// </summary>
public static class TableHeaderCellStyles
{
    /// <summary>
    /// The default TvDescriptor for the TableHeaderCell component.
    /// </summary>
    public static readonly TvDescriptor<RzComponent<TableHeaderCellSlots>, TableHeaderCellSlots> DefaultDescriptor = new(
        @base: "text-foreground h-10 px-2 text-left align-middle font-medium whitespace-nowrap [&:has([role=checkbox])]:pr-0 [&>[role=checkbox]]:translate-y-[2px]"
    );
}
