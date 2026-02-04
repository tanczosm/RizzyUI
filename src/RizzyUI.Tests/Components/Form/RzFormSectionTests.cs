using AwesomeAssertions;
using Microsoft.AspNetCore.Components;
using Alba;

namespace RizzyUI.Tests.Components.Form;

public class RzFormSectionTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    private readonly IAlbaHost _host;

    public RzFormSectionTests(WebAppFixture fixture) : base(fixture)
    {
        _host = fixture.Host;
    }

    [Fact]
    public void RzFormSection_RendersContent()
    {
        var cut = RenderComponent<RzFormSection>(parameters => parameters
            .Add(p => p.Title, "Section")
            .Add(p => p.Content, (RenderFragment)(builder => builder.AddContent(0, "Section Content")))
        );

        cut.Markup.Should().Contain("Section");
        cut.Markup.Should().Contain("Section Content");
    }

    [Theory]
    [InlineData(SectionLayout.TwoColumn, "md:flex")]
    [InlineData(SectionLayout.Stacked, "border-b")]
    public void RzFormSection_LayoutAppliesExpectedClass(SectionLayout layout, string expectedClass)
    {
        var cut = RenderComponent<RzFormSection>(parameters => parameters
            .Add(p => p.Title, "Section")
            .Add(p => p.Layout, layout)
            .Add(p => p.Content, (RenderFragment)(builder => builder.AddContent(0, "Section Content")))
        );

        cut.Markup.Should().Contain(expectedClass);
    }
}
