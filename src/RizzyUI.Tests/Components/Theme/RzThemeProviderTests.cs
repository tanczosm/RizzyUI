using AwesomeAssertions;
using Microsoft.AspNetCore.Components;
using Alba;

namespace RizzyUI.Tests.Components.Theme;

public class RzThemeProviderTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    private readonly IAlbaHost _host;

    public RzThemeProviderTests(WebAppFixture fixture) : base(fixture)
    {
        _host = fixture.Host;
    }

    [Fact]
    public void RzThemeProvider_RendersChildContent()
    {
        var cut = RenderComponent<RzThemeProvider>(parameters => parameters
            .Add(p => p.ChildContent, (RenderFragment)(builder => builder.AddContent(0, "Theme Content")))
        );

        cut.Markup.Should().Contain("Theme Content");
    }
}