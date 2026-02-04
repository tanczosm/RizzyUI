using AwesomeAssertions;
using Alba;

namespace RizzyUI.Tests.Components.Typography;

public class TypographyTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    private readonly IAlbaHost _host;

    public TypographyTests(WebAppFixture fixture) : base(fixture)
    {
        _host = fixture.Host;
    }

    [Fact]
    public void HeadingAndParagraph_Render()
    {
        var heading = RenderComponent<RzHeading>(parameters => parameters
            .Add(p => p.Level, HeadingLevel.H2)
            .AddChildContent("Heading")
        );

        heading.Markup.Should().Contain("Heading");

        var paragraph = RenderComponent<RzParagraph>(parameters => parameters
            .AddChildContent("Paragraph")
        );

        paragraph.Markup.Should().Contain("Paragraph");
    }
}