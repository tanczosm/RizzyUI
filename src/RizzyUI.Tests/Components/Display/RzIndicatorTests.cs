using AwesomeAssertions;
using Alba;

namespace RizzyUI.Tests.Components.Display;

public class RzIndicatorTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    private readonly IAlbaHost _host;

    public RzIndicatorTests(WebAppFixture fixture) : base(fixture)
    {
        _host = fixture.Host;
    }

    [Fact]
    public void RzIndicator_RendersWithDefaultAriaLabel()
    {
        var cut = RenderComponent<RzIndicator>();

        cut.Markup.Should().Contain("data-slot=\"indicator\"");
        cut.Markup.Should().Contain("aria-label=");
    }

    [Theory]
    [InlineData(true, "data-visible=\"true\"")]
    [InlineData(false, "data-visible=\"false\"")]
    public void RzIndicator_VisibleFlag_RendersDataAttribute(bool visible, string expectedAttribute)
    {
        var cut = RenderComponent<RzIndicator>(parameters => parameters
            .Add(p => p.Visible, visible)
        );

        cut.Markup.Should().Contain(expectedAttribute);
    }

    [Theory]
    [InlineData(IndicatorPosition.TopEnd, "top-0 right-0")]
    [InlineData(IndicatorPosition.BottomStart, "bottom-0 left-0")]
    [InlineData(IndicatorPosition.Center, "top-1/2 left-1/2")]
    public void RzIndicator_PositionAppliesExpectedClass(IndicatorPosition position, string expectedClass)
    {
        var cut = RenderComponent<RzIndicator>(parameters => parameters
            .Add(p => p.Position, position)
        );

        cut.Markup.Should().Contain(expectedClass);
    }

    [Theory]
    [InlineData(Size.ExtraSmall, "size-2")]
    [InlineData(Size.Medium, "size-3")]
    [InlineData(Size.ExtraLarge, "size-4")]
    public void RzIndicator_SizeAppliesExpectedClass(Size size, string expectedClass)
    {
        var cut = RenderComponent<RzIndicator>(parameters => parameters
            .Add(p => p.Size, size)
        );

        cut.Markup.Should().Contain(expectedClass);
    }
}
