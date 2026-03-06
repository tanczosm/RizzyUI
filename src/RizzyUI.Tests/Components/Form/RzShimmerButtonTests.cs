using Bunit;

namespace RizzyUI.Tests.Components.Form;

public class RzShimmerButtonTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    public RzShimmerButtonTests(WebAppFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public void Renders_RootAndAllExpectedDataSlots()
    {
        var cut = Render<global::RizzyUI.RzShimmerButton>(p => p
            .Add(x => x.ChildContent, "Shimmer"));

        Assert.NotNull(cut.Find("button[data-slot='shimmer-button']"));
        Assert.NotNull(cut.Find("[data-slot='spark-container']"));
        Assert.NotNull(cut.Find("[data-slot='spark-track']"));
        Assert.NotNull(cut.Find("[data-slot='spark-beam']"));
        Assert.NotNull(cut.Find("[data-slot='content']"));
        Assert.NotNull(cut.Find("[data-slot='highlight']"));
        Assert.NotNull(cut.Find("[data-slot='backdrop']"));
    }

    [Fact]
    public void Renders_NativeButtonSemantics_AndSupportsTypeDisabledAndAriaLabel()
    {
        var cut = Render<global::RizzyUI.RzShimmerButton>(p => p
            .Add(x => x.Type, global::RizzyUI.ButtonType.Submit)
            .Add(x => x.Disabled, true)
            .Add(x => x.AriaLabel, "Submit form"));

        var button = cut.Find("button[data-slot='shimmer-button']");
        Assert.Equal("submit", button.GetAttribute("type"));
        Assert.Equal("", button.GetAttribute("disabled"));
        Assert.Equal("Submit form", button.GetAttribute("aria-label"));
        Assert.Null(button.GetAttribute("role"));
    }

    [Fact]
    public void DoesNotRender_AlpineMarkers_ForCssOnlyComponent()
    {
        var cut = Render<global::RizzyUI.RzShimmerButton>();
        var markup = cut.Markup;

        Assert.DoesNotContain("x-data", markup);
        Assert.DoesNotContain("data-alpine-root", markup);
        Assert.DoesNotContain("data-assets", markup);
        Assert.DoesNotContain("data-nonce", markup);
    }

    [Fact]
    public void Maps_ParametersAndUserStyle_ToRootCssVariables()
    {
        var cut = Render<global::RizzyUI.RzShimmerButton>(p => p
            .Add(x => x.ShimmerColor, "#22d3ee")
            .Add(x => x.ShimmerDuration, "2s")
            .Add(x => x.BorderRadius, "8px")
            .Add(x => x.ShimmerSize, "0.1em")
            .Add(x => x.Background, "linear-gradient(to right, #111827, #1f2937)")
            .AddUnmatched("style", "--rz-shimmer-button-color:#f43f5e;outline:1px solid red"));

        var style = cut.Find("button").GetAttribute("style");
        Assert.NotNull(style);
        Assert.Contains("--rz-shimmer-button-spread:90deg", style);
        Assert.Contains("--rz-shimmer-button-speed:2s", style);
        Assert.Contains("--rz-shimmer-button-radius:8px", style);
        Assert.Contains("--rz-shimmer-button-cut:0.1em", style);
        Assert.Contains("--rz-shimmer-button-bg:linear-gradient(to right, #111827, #1f2937)", style);
        Assert.EndsWith("--rz-shimmer-button-color:#f43f5e;outline:1px solid red", style, StringComparison.Ordinal);
    }

    [Fact]
    public void PassesThrough_HtmxDataAndClassAttributes()
    {
        var cut = Render<global::RizzyUI.RzShimmerButton>(p => p
            .AddUnmatched("hx-post", "/api/demo")
            .AddUnmatched("data-qa", "cta")
            .AddUnmatched("class", "shadow-2xl"));

        var button = cut.Find("button[data-slot='shimmer-button']");
        Assert.Equal("/api/demo", button.GetAttribute("hx-post"));
        Assert.Equal("cta", button.GetAttribute("data-qa"));
        Assert.Contains("shadow-2xl", button.ClassList);
    }

    [Fact]
    public void Uses_DefaultStyleValues_WhenCssValueParametersAreNullOrEmpty()
    {
        var cut = Render<global::RizzyUI.RzShimmerButton>(p => p
            .Add(x => x.ShimmerColor, string.Empty)
            .Add(x => x.ShimmerSize, string.Empty)
            .Add(x => x.BorderRadius, string.Empty)
            .Add(x => x.ShimmerDuration, string.Empty));

        var style = cut.Find("button").GetAttribute("style");
        Assert.NotNull(style);
        Assert.Contains("--rz-shimmer-button-color:#ffffff", style);
        Assert.Contains("--rz-shimmer-button-cut:0.05em", style);
        Assert.Contains("--rz-shimmer-button-radius:100px", style);
        Assert.Contains("--rz-shimmer-button-speed:3s", style);
        Assert.DoesNotContain("--rz-shimmer-button-bg:", style);
    }
}
