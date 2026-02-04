using AwesomeAssertions;
using Alba;

namespace RizzyUI.Tests.Components.Navigation;

public class RzBreadcrumbTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    private readonly IAlbaHost _host;

    public RzBreadcrumbTests(WebAppFixture fixture) : base(fixture)
    {
        _host = fixture.Host;
    }

    [Fact]
    public void RzBreadcrumb_RendersItems()
    {
        var cut = RenderComponent<RzBreadcrumb>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<BreadcrumbList>(0);
                builder.AddChildContent(listBuilder =>
                {
                    listBuilder.OpenComponent<BreadcrumbItem>(0);
                    listBuilder.AddChildContent(itemBuilder =>
                    {
                        itemBuilder.OpenComponent<BreadcrumbLink>(0);
                        itemBuilder.AddAttribute(1, "Href", "#");
                        itemBuilder.AddChildContent("Home");
                        itemBuilder.CloseComponent();
                    });
                    listBuilder.CloseComponent();
                    listBuilder.OpenComponent<BreadcrumbSeparator>(1);
                    listBuilder.CloseComponent();
                    listBuilder.OpenComponent<BreadcrumbItem>(2);
                    listBuilder.AddChildContent(itemBuilder =>
                    {
                        itemBuilder.OpenComponent<BreadcrumbPage>(0);
                        itemBuilder.AddChildContent("Page");
                        itemBuilder.CloseComponent();
                    });
                    listBuilder.CloseComponent();
                    listBuilder.OpenComponent<BreadcrumbEllipsis>(3);
                    listBuilder.CloseComponent();
                });
                builder.CloseComponent();
            })
        );

        cut.Markup.Should().Contain("Home");
        cut.Markup.Should().Contain("Page");
    }
}