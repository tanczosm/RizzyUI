using AwesomeAssertions;
using Alba;

namespace RizzyUI.Tests.Components.Layout;

public class RzSearchButtonTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    private readonly IAlbaHost _host;

    public RzSearchButtonTests(WebAppFixture fixture) : base(fixture)
    {
        _host = fixture.Host;
    }

    [Fact]
    public void RzSearchButton_RendersPlaceholder()
    {
        var cut = RenderComponent<RzSearchButton>(parameters => parameters
            .Add(p => p.Placeholder, "Search")
        );

        cut.Markup.Should().Contain("Search");
    }
}