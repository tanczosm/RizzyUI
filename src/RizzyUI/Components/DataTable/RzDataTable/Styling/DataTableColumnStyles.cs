using TailwindVariants.NET;

namespace RizzyUI;

public sealed partial class DataTableColumnSlots : ISlots
{
    [Slot("datatable-column")]
    public string? Base { get; set; }
}

public static class DataTableColumnStyles
{
    public static readonly TvDescriptor<RzComponent<DataTableColumnSlots>, DataTableColumnSlots> DefaultDescriptor = new(
        @base: string.Empty
    );
}
