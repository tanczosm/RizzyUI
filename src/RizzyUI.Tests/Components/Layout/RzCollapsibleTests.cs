using AwesomeAssertions;
using Alba;

namespace RizzyUI.Tests.Components.Layout;

public class RzCollapsibleTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    private readonly IAlbaHost _host;

    public RzCollapsibleTests(WebAppFixture fixture) : base(fixture)
    {
        _host = fixture.Host;
    }

    [Fact]
    public void RzCollapsible_RendersContent()
    {
        var cut = RenderComponent<RzCollapsible>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<CollapsibleTrigger>(0);
                builder.AddChildContent("Toggle");
                builder.CloseComponent();
                builder.OpenComponent<CollapsibleContent>(1);
                builder.AddChildContent("Collapsible Content");
                builder.CloseComponent();
            })
        );

        cut.Markup.Should().Contain("Collapsible Content");
    }
}