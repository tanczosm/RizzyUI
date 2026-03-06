using Bunit;
using Microsoft.AspNetCore.Components;

namespace RizzyUI.Tests.Components.Animations.RzBlurFade;

public class RzBlurFadeTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    public RzBlurFadeTests(WebAppFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public void DefaultRender_UsesDivRootIdAndRequiredDataSlot()
    {
        var cut = Render<RizzyUI.RzBlurFade>(p => p
            .Add(x => x.Id, "blur-fade-root")
            .AddChildContent("Hello"));

        var root = cut.Find("#blur-fade-root");
        Assert.Equal("div", root.TagName.ToLowerInvariant());
        Assert.Equal("blur-fade", root.GetAttribute("data-slot"));
        Assert.Contains("Hello", cut.Markup);
    }

    [Fact]
    public void InternalSlots_EmitViewportClassAndSlotMarker()
    {
        var cut = Render<RizzyUI.RzBlurFade>(p => p.Add(x => x.Id, "bf-1"));

        var root = cut.Find("#bf-1");
        var viewport = cut.Find("[data-slot='viewport']");

        Assert.False(string.IsNullOrWhiteSpace(root.GetAttribute("class")));
        Assert.False(string.IsNullOrWhiteSpace(viewport.GetAttribute("class")));
        Assert.NotNull(viewport.GetAttribute("data-slot"));
    }

    [Fact]
    public void AlpineHooks_RenderExpectedAttributesAndDefaults()
    {
        var cut = Render<RizzyUI.RzBlurFade>(p => p.Add(x => x.Id, "bf-2"));

        var viewport = cut.Find("[data-slot='viewport']");

        Assert.Equal("bf-2", viewport.GetAttribute("data-alpine-root"));
        Assert.Equal("rzBlurFade", viewport.GetAttribute("x-data"));
        Assert.Equal("[]", viewport.GetAttribute("data-assets"));
        Assert.NotNull(viewport.GetAttribute("data-nonce"));
        Assert.Equal("down", viewport.GetAttribute("data-direction"));
        Assert.Equal("6", viewport.GetAttribute("data-offset"));
        Assert.Equal("0.4", viewport.GetAttribute("data-duration"));
        Assert.Equal("0", viewport.GetAttribute("data-delay"));
        Assert.Equal("6px", viewport.GetAttribute("data-blur"));
        Assert.Equal("false", viewport.GetAttribute("data-in-view"));
        Assert.Equal("-50px", viewport.GetAttribute("data-in-view-margin"));
    }

    [Fact]
    public void ParameterRendering_ReflectsProvidedValues()
    {
        var cut = Render<RizzyUI.RzBlurFade>(p => p
            .Add(x => x.Direction, BlurFadeDirection.Left)
            .Add(x => x.Offset, 12)
            .Add(x => x.Duration, 1.25)
            .Add(x => x.Delay, 0.15)
            .Add(x => x.Blur, "10px")
            .Add(x => x.InView, true)
            .Add(x => x.InViewMargin, "-20px"));

        var viewport = cut.Find("[data-slot='viewport']");
        Assert.Equal("left", viewport.GetAttribute("data-direction"));
        Assert.Equal("12", viewport.GetAttribute("data-offset"));
        Assert.Equal("1.25", viewport.GetAttribute("data-duration"));
        Assert.Equal("0.15", viewport.GetAttribute("data-delay"));
        Assert.Equal("10px", viewport.GetAttribute("data-blur"));
        Assert.Equal("true", viewport.GetAttribute("data-in-view"));
        Assert.Equal("-20px", viewport.GetAttribute("data-in-view-margin"));
    }

    [Fact]
    public void Normalization_ClampsNumericValuesAndFallbacksStrings()
    {
        var cut = Render<RizzyUI.RzBlurFade>(p => p
            .Add(x => x.Offset, -1)
            .Add(x => x.Duration, -0.2)
            .Add(x => x.Delay, -0.1)
            .Add(x => x.Blur, "")
            .Add(x => x.InViewMargin, ""));

        var viewport = cut.Find("[data-slot='viewport']");
        Assert.Equal("0", viewport.GetAttribute("data-offset"));
        Assert.Equal("0", viewport.GetAttribute("data-duration"));
        Assert.Equal("0", viewport.GetAttribute("data-delay"));
        Assert.Equal("6px", viewport.GetAttribute("data-blur"));
        Assert.Equal("-50px", viewport.GetAttribute("data-in-view-margin"));
    }

    [Fact]
    public void SsrMarkup_DoesNotEmitServerHiddenStyleOrBlazorEventAttributes()
    {
        var cut = Render<RizzyUI.RzBlurFade>(p => p.AddChildContent("Content"));

        var viewport = cut.Find("[data-slot='viewport']");
        Assert.Null(viewport.GetAttribute("style"));
        Assert.DoesNotContain("@on", cut.Markup, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("blazor:on", cut.Markup, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void RootClassAndAdditionalAttributes_AreMergedAndPassedThrough()
    {
        var cut = Render<RizzyUI.RzBlurFade>(p => p
            .AddUnmatched("class", "custom-class")
            .AddUnmatched("data-test-id", "blur-fade-test"));

        var root = cut.Find("[data-slot='blur-fade']");
        Assert.Contains("custom-class", root.GetAttribute("class"));
        Assert.Equal("blur-fade-test", root.GetAttribute("data-test-id"));
    }

    [Fact]
    public void Accessibility_DefaultMarkupHasNoRoleOrAriaLabel_ChildOwnsSemantics()
    {
        var cut = Render<RizzyUI.RzBlurFade>(p => p.AddChildContent("A11y"));

        var root = cut.Find("[data-slot='blur-fade']");
        Assert.Null(root.GetAttribute("role"));
        Assert.Null(root.GetAttribute("aria-label"));
    }

    [Fact]
    public void ElementOverride_RendersRequestedTag()
    {
        var cut = Render<RizzyUI.RzBlurFade>(p => p.Add(x => x.Element, "section"));

        var root = cut.Find("[data-slot='blur-fade']");
        Assert.Equal("section", root.TagName.ToLowerInvariant());
    }

    [Fact]
    public void NullChildContent_RendersWithoutException()
    {
        var cut = Render<RizzyUI.RzBlurFade>();

        var root = cut.Find("[data-slot='blur-fade']");
        var viewport = cut.Find("[data-slot='viewport']");

        Assert.NotNull(root);
        Assert.NotNull(viewport);
    }
}
