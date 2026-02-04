using AwesomeAssertions;
using Alba;

namespace RizzyUI.Tests.Components.Display;

public class RzSeparatorTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    private readonly IAlbaHost _host;

    public RzSeparatorTests(WebAppFixture fixture) : base(fixture)
    {
        _host = fixture.Host;
    }

    [Fact]
    public void RzSeparator_Renders()
    {
        var cut = RenderComponent<RzSeparator>();

        cut.Markup.Should().NotBeNullOrWhiteSpace();
    }

    [Theory]
    [InlineData(SeparatorStyle.Solid, "border-solid")]
    [InlineData(SeparatorStyle.Dashed, "border-dashed")]
    [InlineData(SeparatorStyle.Dotted, "border-dotted")]
    public void RzSeparator_StyleAppliesExpectedClass(SeparatorStyle style, string expectedClass)
    {
        var cut = RenderComponent<RzSeparator>(parameters => parameters
            .Add(p => p.Style, style)
        );

        cut.Markup.Should().Contain(expectedClass);
    }

    [Theory]
    [InlineData(Orientation.Horizontal, "w-full")]
    [InlineData(Orientation.Vertical, "h-full")]
    public void RzSeparator_OrientationAppliesExpectedClass(Orientation orientation, string expectedClass)
    {
        var cut = RenderComponent<RzSeparator>(parameters => parameters
            .Add(p => p.Orientation, orientation)
        );

        cut.Markup.Should().Contain(expectedClass);
    }

    [Theory]
    [InlineData(Align.Start, "after:flex-1")]
    [InlineData(Align.Center, "before:flex-1")]
    [InlineData(Align.End, "before:flex-1")]
    public void RzSeparator_WithContent_AlignsLabel(Align alignment, string expectedClass)
    {
        var cut = RenderComponent<RzSeparator>(parameters => parameters
            .Add(p => p.LabelAlignment, alignment)
            .AddChildContent("Label")
        );

        cut.Markup.Should().Contain("Label");
        cut.Markup.Should().Contain(expectedClass);
    }
}
