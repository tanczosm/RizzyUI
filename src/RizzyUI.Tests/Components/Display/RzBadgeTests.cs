using AwesomeAssertions;
using Alba;

namespace RizzyUI.Tests.Components.Display;

public class RzBadgeTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    private readonly IAlbaHost _host;

    public RzBadgeTests(WebAppFixture fixture) : base(fixture)
    {
        _host = fixture.Host;
    }

    [Fact]
    public void RzBadge_RendersLabelWhenNoChildContent()
    {
        var cut = RenderComponent<RzBadge>(parameters => parameters
            .Add(p => p.Label, "Label")
        );

        cut.Markup.Should().Contain("Label");
    }

    [Fact]
    public void RzBadge_ChildContentOverridesLabel()
    {
        var cut = RenderComponent<RzBadge>(parameters => parameters
            .Add(p => p.Label, "Label")
            .AddChildContent("Child")
        );

        cut.Markup.Should().Contain("Child");
        cut.Markup.Should().NotContain("Label");
    }

    [Theory]
    [InlineData(ThemeVariant.Default, "bg-input")]
    [InlineData(ThemeVariant.Primary, "bg-primary")]
    [InlineData(ThemeVariant.Secondary, "bg-secondary")]
    [InlineData(ThemeVariant.Destructive, "bg-destructive")]
    [InlineData(ThemeVariant.Accent, "bg-accent")]
    [InlineData(ThemeVariant.Information, "bg-info")]
    [InlineData(ThemeVariant.Success, "bg-success")]
    [InlineData(ThemeVariant.Warning, "bg-warning")]
    [InlineData(ThemeVariant.Inverse, "bg-foreground")]
    [InlineData(ThemeVariant.Ghost, "bg-transparent")]
    public void RzBadge_VariantAppliesExpectedClass(ThemeVariant variant, string expectedClass)
    {
        var cut = RenderComponent<RzBadge>(parameters => parameters
            .Add(p => p.Variant, variant)
            .AddChildContent("Badge")
        );

        cut.Markup.Should().Contain(expectedClass);
    }

    [Theory]
    [InlineData(ThemeVariant.Primary, "bg-primary/10")]
    [InlineData(ThemeVariant.Secondary, "bg-secondary/10")]
    [InlineData(ThemeVariant.Destructive, "bg-destructive/10")]
    public void RzBadge_SoftVariantAppliesExpectedClass(ThemeVariant variant, string expectedClass)
    {
        var cut = RenderComponent<RzBadge>(parameters => parameters
            .Add(p => p.Variant, variant)
            .Add(p => p.Soft, true)
            .AddChildContent("Badge")
        );

        cut.Markup.Should().Contain(expectedClass);
    }
}
