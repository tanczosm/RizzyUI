using Bunit;

namespace RizzyUI.Tests.Components.Layout;

public class RzSimpleTableTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    public RzSimpleTableTests(WebAppFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public void RzSimpleTable_RendersContainerAndTable()
    {
        var cut = RenderComponent<RzSimpleTable>(parameters => parameters
            .AddChildContent<TableBodySection>(body => body
                .AddChildContent<TableRowItem>(row => row
                    .AddChildContent<TableCellItem>(cell => cell.AddChildContent("Cell")))));

        var container = cut.Find("[data-slot='table-container']");
        var table = cut.Find("[data-slot='table']");

        Assert.NotNull(container);
        Assert.NotNull(table);
        Assert.Contains("overflow-x-auto", container.ClassList);
        Assert.Contains("caption-bottom", table.ClassList);
    }

    [Fact]
    public void TableFamily_RendersExpectedDataSlots()
    {
        var cut = RenderComponent<RzSimpleTable>(parameters => parameters
            .AddChildContent<TableCaption>(caption => caption.AddChildContent("Caption"))
            .AddChildContent<TableHeader>(header => header
                .AddChildContent<TableRowItem>(row => row
                    .AddChildContent<TableHead>(head => head.AddChildContent("Header"))))
            .AddChildContent<TableBodySection>(body => body
                .AddChildContent<TableRowItem>(row => row
                    .AddChildContent<TableCellItem>(cell => cell.AddChildContent("Value"))))
            .AddChildContent<TableFooter>(footer => footer
                .AddChildContent<TableRowItem>(row => row
                    .AddChildContent<TableCellItem>(cell => cell.AddChildContent("Footer")))));

        Assert.NotNull(cut.Find("[data-slot='table-caption']"));
        Assert.NotNull(cut.Find("[data-slot='table-header']"));
        Assert.NotNull(cut.Find("[data-slot='table-row']"));
        Assert.NotNull(cut.Find("[data-slot='table-head']"));
        Assert.NotNull(cut.Find("[data-slot='table-body']"));
        Assert.NotNull(cut.Find("[data-slot='table-cell']"));
        Assert.NotNull(cut.Find("[data-slot='table-footer']"));
    }
}
