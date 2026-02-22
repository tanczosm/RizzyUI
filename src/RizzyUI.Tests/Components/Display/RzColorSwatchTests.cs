using Bunit;

namespace RizzyUI.Tests.Components.Display;

public class RzColorSwatchTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    public RzColorSwatchTests(WebAppFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public void DefaultRender_ShowsCorrectStructure()
    {
        var cut = Render<RzColorSwatch>();

        Assert.NotNull(cut.Find("[data-slot='color-swatch']"));
        Assert.NotNull(cut.Find("[data-slot='swatch']"));
    }

    [Fact]
    public void DisabledAndValue_AppliesExpectedAttributes()
    {
        var cut = Render<RzColorSwatch>(parameters => parameters
            .Add(p => p.Disabled, true)
            .Add(p => p.Value, "rgba(0, 0, 0, 0.5)"));

        var root = cut.Find("[data-slot='color-swatch']");
        var alpineRoot = cut.Find("[data-alpine-root]");

        Assert.Equal("true", root.GetAttribute("aria-disabled"));
        Assert.Equal("rgba(0, 0, 0, 0.5)", alpineRoot.GetAttribute("data-initial-value"));
    }
}
