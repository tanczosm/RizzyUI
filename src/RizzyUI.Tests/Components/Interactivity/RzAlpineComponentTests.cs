using AwesomeAssertions;
using Alba;

namespace RizzyUI.Tests.Components.Interactivity;

public class RzAlpineComponentTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    private readonly IAlbaHost _host;

    public RzAlpineComponentTests(WebAppFixture fixture) : base(fixture)
    {
        _host = fixture.Host;
    }

    [Fact]
    public void RzAlpineComponent_RendersChildContent()
    {
        var cut = RenderComponent<RzAlpineComponent>(parameters => parameters
            .Add(p => p.For, new RzButton())
            .Add(p => p.Name, "rz-alpine")
            .AddChildContent("Alpine Content")
        );

        cut.Markup.Should().Contain("Alpine Content");
    }
}