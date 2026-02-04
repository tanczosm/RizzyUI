using AwesomeAssertions;
using Alba;

namespace RizzyUI.Tests.Components.Theme;

public class RzDarkModeToggleTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    private readonly IAlbaHost _host;

    public RzDarkModeToggleTests(WebAppFixture fixture) : base(fixture)
    {
        _host = fixture.Host;
    }

    [Fact]
    public void RzDarkModeToggle_RendersButton()
    {
        var cut = RenderComponent<RzDarkModeToggle>();

        cut.Markup.Should().Contain("button");
    }
}