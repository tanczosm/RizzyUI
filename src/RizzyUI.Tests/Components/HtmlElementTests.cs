using AwesomeAssertions;
using Alba;

namespace RizzyUI.Tests.Components;

public class HtmlElementTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    private readonly IAlbaHost _host;

    public HtmlElementTests(WebAppFixture fixture) : base(fixture)
    {
        _host = fixture.Host;
    }

    [Fact]
    public void HtmlElement_RendersCustomElement()
    {
        var cut = RenderComponent<HtmlElement>(parameters => parameters
            .Add(p => p.Element, "section")
            .Add(p => p.AdditionalAttributes, new Dictionary<string, object> { ["data-test"] = "value" })
            .AddChildContent("Section Content")
        );

        cut.Markup.Should().Contain("section");
        cut.Markup.Should().Contain("data-test=\"value\"");
        cut.Markup.Should().Contain("Section Content");
    }
}