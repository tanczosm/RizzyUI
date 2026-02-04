using AwesomeAssertions;
using Alba;

namespace RizzyUI.Tests.Components.Layout;

public class RzAccordionTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    private readonly IAlbaHost _host;

    public RzAccordionTests(WebAppFixture fixture) : base(fixture)
    {
        _host = fixture.Host;
    }

    [Fact]
    public void RzAccordion_RendersItems()
    {
        var cut = RenderComponent<RzAccordion>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<AccordionItem>(0);
                builder.AddAttribute(1, "Title", "Accordion Title");
                builder.AddChildContent("Accordion Body");
                builder.CloseComponent();
            })
        );

        cut.Markup.Should().Contain("Accordion Title");
    }
}