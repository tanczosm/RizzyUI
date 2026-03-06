using Bunit;

namespace RizzyUI.Tests.Components.Display;

public class RzProgressiveBlurTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    public RzProgressiveBlurTests(WebAppFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public void DefaultRender_ShowsDecorativeRootAndExpectedLayerCount()
    {
        var cut = Render<RzProgressiveBlur>(parameters => parameters.Add(p => p.Id, "blur-default"));

        var root = cut.Find("#blur-default");
        Assert.Equal("progressive-blur", root.GetAttribute("data-slot"));
        Assert.Equal("bottom", root.GetAttribute("data-position"));
        Assert.Equal("true", root.GetAttribute("aria-hidden"));
        Assert.Null(root.GetAttribute("role"));
        Assert.Null(root.GetAttribute("tabindex"));
        Assert.Contains("absolute", root.ClassList);
        Assert.Contains("pointer-events-none", root.ClassList);
        Assert.Contains("bottom-0", root.ClassList);

        var layers = cut.FindAll("[data-slot='layer']");
        Assert.Equal(8, layers.Count);
        Assert.Equal("first", layers[0].GetAttribute("data-layer-kind"));
        Assert.Equal("last", layers[^1].GetAttribute("data-layer-kind"));
    }

    [Fact]
    public void DefaultRender_DoesNotEmitAlpineHooksOrInteractiveDescendants()
    {
        var cut = Render<RzProgressiveBlur>();

        Assert.Empty(cut.FindAll("[x-data]"));
        Assert.Empty(cut.FindAll("[data-alpine-root]"));
        Assert.Empty(cut.FindAll("button, input, select, textarea, a[href]") );
        Assert.DoesNotContain("x-data", cut.Markup);
    }

    [Fact]
    public void LayerMarkers_AreRenderedForEachLayer()
    {
        var cut = Render<RzProgressiveBlur>(parameters => parameters
            .Add(p => p.BlurLevels, new double[] { 1, 2, 3, 4 })
        );

        var layers = cut.FindAll("[data-slot='layer']");
        Assert.Equal(4, layers.Count);
        Assert.Equal("first", layers[0].GetAttribute("data-layer-kind"));
        Assert.Equal("0", layers[0].GetAttribute("data-layer-index"));
        Assert.Equal("middle", layers[1].GetAttribute("data-layer-kind"));
        Assert.Equal("1", layers[1].GetAttribute("data-layer-index"));
        Assert.Equal("middle", layers[2].GetAttribute("data-layer-kind"));
        Assert.Equal("2", layers[2].GetAttribute("data-layer-index"));
        Assert.Equal("last", layers[3].GetAttribute("data-layer-kind"));
        Assert.Equal("3", layers[3].GetAttribute("data-layer-index"));
    }

    [Fact]
    public void AriaHidden_False_IsReflectedInMarkup()
    {
        var cut = Render<RzProgressiveBlur>(parameters => parameters.Add(p => p.AriaHidden, false));

        var root = cut.Find("[data-slot='progressive-blur']");
        Assert.Equal("false", root.GetAttribute("aria-hidden"));
    }

    [Fact]
    public void PositionVariants_EmitExpectedPlacementAndHeightBehavior()
    {
        var topCut = Render<RzProgressiveBlur>(parameters => parameters
            .Add(p => p.Position, ProgressiveBlurPosition.Top)
            .Add(p => p.Height, "25%")
        );
        var topRoot = topCut.Find("[data-slot='progressive-blur']");
        Assert.Contains("top-0", topRoot.ClassList);
        Assert.Contains("height: 25%;", topRoot.GetAttribute("style"));

        var bothCut = Render<RzProgressiveBlur>(parameters => parameters
            .Add(p => p.Position, ProgressiveBlurPosition.Both)
            .Add(p => p.Height, "10%")
        );
        var bothRoot = bothCut.Find("[data-slot='progressive-blur']");
        Assert.Contains("inset-y-0", bothRoot.ClassList);
        Assert.Contains("height: 100%;", bothRoot.GetAttribute("style"));
        Assert.Equal("both", bothRoot.GetAttribute("data-position"));
    }

    [Fact]
    public void AdditionalAttributes_ClassMergesAndStyleOptionsRender()
    {
        var cut = Render<RzProgressiveBlur>(parameters => parameters
            .AddUnmatched("class", "custom-class")
            .Add(p => p.ZIndex, 40)
            .Add(p => p.InsetStart, "1rem")
            .Add(p => p.InsetEnd, "2rem")
        );

        var root = cut.Find("[data-slot='progressive-blur']");
        var style = root.GetAttribute("style");
        Assert.Contains("custom-class", root.ClassList);
        Assert.Contains("z-index: 40;", style);
        Assert.Contains("inset-inline-start: 1rem;", style);
        Assert.Contains("inset-inline-end: 2rem;", style);
    }

    [Fact]
    public void BlurLevels_NullOrEmpty_FallsBackToDefaults()
    {
        var nullCut = Render<RzProgressiveBlur>(parameters => parameters
            .Add(p => p.BlurLevels, null)
        );
        Assert.Equal(8, nullCut.FindAll("[data-slot='layer']").Count);

        var emptyCut = Render<RzProgressiveBlur>(parameters => parameters
            .Add(p => p.BlurLevels, Array.Empty<double>())
        );
        Assert.Equal(8, emptyCut.FindAll("[data-slot='layer']").Count);
    }

    [Fact]
    public void MinimalBlurArrays_RenderWithoutMiddleLayers_WhenApplicable()
    {
        var singleCut = Render<RzProgressiveBlur>(parameters => parameters
            .Add(p => p.BlurLevels, new double[] { 12 })
        );
        var singleLayers = singleCut.FindAll("[data-slot='layer']");
        Assert.Single(singleLayers);
        Assert.Equal("single", singleLayers[0].GetAttribute("data-layer-kind"));

        var twoCut = Render<RzProgressiveBlur>(parameters => parameters
            .Add(p => p.BlurLevels, new double[] { 4, 20 })
        );
        var twoLayers = twoCut.FindAll("[data-slot='layer']");
        Assert.Equal(2, twoLayers.Count);
        Assert.Equal("first", twoLayers[0].GetAttribute("data-layer-kind"));
        Assert.Equal("last", twoLayers[1].GetAttribute("data-layer-kind"));
    }

    [Fact]
    public void NegativeBlurLevels_AreClampedToZero()
    {
        var cut = Render<RzProgressiveBlur>(parameters => parameters
            .Add(p => p.BlurLevels, new double[] { -5, 2 })
        );

        var firstLayer = cut.FindAll("[data-slot='layer']")[0];
        Assert.Contains("blur(0px)", firstLayer.GetAttribute("style"));
    }

    [Fact]
    public void PartialConfiguration_UsesDefaultHeightAndPassesThroughInvalidCss()
    {
        var defaultHeightCut = Render<RzProgressiveBlur>(parameters => parameters
            .Add(p => p.Height, null)
            .Add(p => p.Position, ProgressiveBlurPosition.Bottom)
        );
        var defaultRoot = defaultHeightCut.Find("[data-slot='progressive-blur']");
        Assert.Contains("height: 30%;", defaultRoot.GetAttribute("style"));

        var invalidHeightCut = Render<RzProgressiveBlur>(parameters => parameters
            .Add(p => p.Height, "definitely-not-css")
        );
        var invalidRoot = invalidHeightCut.Find("[data-slot='progressive-blur']");
        Assert.Contains("height: definitely-not-css;", invalidRoot.GetAttribute("style"));
    }

    [Fact]
    public void RendersSsrOnlyMarkupWithoutBlazorEventAttributes()
    {
        var cut = Render<RzProgressiveBlur>();

        Assert.DoesNotContain("@onclick", cut.Markup);
        Assert.DoesNotContain("@onchange", cut.Markup);
        Assert.DoesNotContain("@bind", cut.Markup);
    }
}
