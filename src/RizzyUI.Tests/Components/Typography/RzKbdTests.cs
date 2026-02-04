using AwesomeAssertions;
using Alba;

namespace RizzyUI.Tests.Components.Typography;

public class RzKbdTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    private readonly IAlbaHost _host;

    public RzKbdTests(WebAppFixture fixture) : base(fixture)
    {
        _host = fixture.Host;
    }

    [Fact]
    public void RzKbdGroup_RendersKeys()
    {
        var cut = RenderComponent<RzKbdGroup>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<RzKbd>(0);
                builder.AddChildContent("Ctrl");
                builder.CloseComponent();
                builder.AddContent(1, "+");
                builder.OpenComponent<RzKbd>(2);
                builder.AddChildContent("K");
                builder.CloseComponent();
            })
        );

        cut.Markup.Should().Contain("Ctrl");
        cut.Markup.Should().Contain("K");
    }
}