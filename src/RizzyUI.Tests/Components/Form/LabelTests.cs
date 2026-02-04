using AwesomeAssertions;
using Alba;

namespace RizzyUI.Tests.Components.Form;

public class LabelTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    private readonly IAlbaHost _host;

    public LabelTests(WebAppFixture fixture) : base(fixture)
    {
        _host = fixture.Host;
    }

    [Fact]
    public void Label_RendersContent()
    {
        var cut = RenderComponent<Label>(parameters => parameters
            .AddChildContent("Label")
        );

        cut.Markup.Should().Contain("Label");
    }
}