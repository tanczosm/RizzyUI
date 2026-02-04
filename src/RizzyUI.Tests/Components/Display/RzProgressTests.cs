using AwesomeAssertions;
using Alba;

namespace RizzyUI.Tests.Components.Display;

public class RzProgressTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    private readonly IAlbaHost _host;

    public RzProgressTests(WebAppFixture fixture) : base(fixture)
    {
        _host = fixture.Host;
    }

    [Fact]
    public void RzProgress_RendersProgressBar()
    {
        var cut = RenderComponent<RzProgress>(parameters => parameters
            .Add(p => p.CurrentValue, 50)
        );

        cut.Markup.Should().Contain("data-slot=\"progress\"");
    }

    [Fact]
    public void RzProgress_SetsAriaAttributes()
    {
        var cut = RenderComponent<RzProgress>(parameters => parameters
            .Add(p => p.CurrentValue, 25)
            .Add(p => p.MinValue, 0)
            .Add(p => p.MaxValue, 200)
            .Add(p => p.AriaLabel, "Loading")
        );

        cut.Markup.Should().Contain("aria-valuenow=\"25\"");
        cut.Markup.Should().Contain("aria-valuemin=\"0\"");
        cut.Markup.Should().Contain("aria-valuemax=\"200\"");
        cut.Markup.Should().Contain("aria-label=\"Loading\"");
    }

    [Theory]
    [InlineData(ProgressLabelPosition.Outside, "progress-outside-label-container")]
    [InlineData(ProgressLabelPosition.Inside, "progress-inside-label-container")]
    public void RzProgress_LabelPosition_RendersExpectedSlot(ProgressLabelPosition position, string expectedSlot)
    {
        var cut = RenderComponent<RzProgress>(parameters => parameters
            .Add(p => p.CurrentValue, 10)
            .Add(p => p.Label, "10%")
            .Add(p => p.LabelPosition, position)
        );

        cut.Markup.Should().Contain(expectedSlot);
    }

    [Theory]
    [InlineData(StatusColor.Primary, "bg-primary")]
    [InlineData(StatusColor.Success, "bg-success")]
    [InlineData(StatusColor.Warning, "bg-warning")]
    public void RzProgress_VariantAppliesExpectedClass(StatusColor variant, string expectedClass)
    {
        var cut = RenderComponent<RzProgress>(parameters => parameters
            .Add(p => p.CurrentValue, 40)
            .Add(p => p.Variant, variant)
        );

        cut.Markup.Should().Contain(expectedClass);
    }
}
