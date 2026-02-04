using AwesomeAssertions;
using Alba;

namespace RizzyUI.Tests.Components.Document;

public class RzBrowserTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    private readonly IAlbaHost _host;

    public RzBrowserTests(WebAppFixture fixture) : base(fixture)
    {
        _host = fixture.Host;
    }

    [Fact]
    public void RzBrowser_RendersUrl()
    {
        var cut = RenderComponent<RzBrowser>(parameters => parameters
            .Add(p => p.Url, "https://example.com")
        );

        cut.Markup.Should().Contain("example.com");
    }
}