using AwesomeAssertions;
using Microsoft.AspNetCore.Components;
using Alba;

namespace RizzyUI.Tests.Components.DataTable;

public class RzTableTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    private readonly IAlbaHost _host;

    public RzTableTests(WebAppFixture fixture) : base(fixture)
    {
        _host = fixture.Host;
    }

    [Fact]
    public void RzTable_RendersHeaderAndBody()
    {
        var cut = RenderComponent<RzTable<string>>(parameters => parameters
            .Add(p => p.Items, new[] { "Row" })
            .Add(p => p.HxControllerUrl, "/table")
            .Add(p => p.Header, table => builder =>
            {
                builder.OpenElement(0, "tr");
                builder.OpenComponent<TableHeaderCell<string>>(1);
                builder.AddAttribute(2, "ChildContent", (RenderFragment)(contentBuilder => contentBuilder.AddContent(0, "Header")));
                builder.CloseComponent();
                builder.CloseElement();
            })
            .Add(p => p.Body, table => builder =>
            {
                builder.OpenComponent<TableBody<string>>(0);
                builder.AddAttribute(1, "RowTemplate", (RenderFragment<string>)(item => rowBuilder =>
                {
                    rowBuilder.OpenComponent<TableRow<string>>(0);
                    rowBuilder.AddChildContent(cellBuilder =>
                    {
                        cellBuilder.OpenComponent<TableCell<string>>(0);
                        cellBuilder.AddChildContent(item);
                        cellBuilder.CloseComponent();
                    });
                    rowBuilder.CloseComponent();
                }));
                builder.CloseComponent();
            })
        );

        cut.Markup.Should().Contain("Header");
        cut.Markup.Should().Contain("Row");
    }

    [Fact]
    public void TablePagination_RendersNavigation()
    {
        var cut = RenderComponent<TablePagination<string>>(parameters => parameters
            .Add(p => p.PaginationState, new PaginationState(1, 10, 5, 2))
            .Add(p => p.HxControllerUrl, "/table")
        );

        cut.Markup.Should().Contain("nav");
    }
}