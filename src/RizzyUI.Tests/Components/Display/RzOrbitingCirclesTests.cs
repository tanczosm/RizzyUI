using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Bunit;
using Microsoft.AspNetCore.Components;

namespace RizzyUI.Tests.Components.Display;

public class RzOrbitingCirclesTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    public RzOrbitingCirclesTests(WebAppFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public void RootContract_RendersExpectedRootAndMergedClass()
    {
        var cut = Render<RzOrbitingCircles>(parameters => parameters
            .Add(p => p.Id, "orbit-root")
            .AddUnmatched("class", "custom-root"));

        var root = cut.Find("#orbit-root");
        Assert.Equal("orbiting-circles", root.GetAttribute("data-slot"));
        Assert.Contains("relative", root.ClassList);
        Assert.Contains("custom-root", root.ClassList);
    }

    [Fact]
    public void PathRendering_ShowPathTrue_RendersSvgAndCircleWithRadius()
    {
        var cut = Render<RzOrbitingCircles>(parameters => parameters
            .Add(p => p.ShowPath, true)
            .Add(p => p.Radius, 95));

        var svg = cut.Find("svg[data-slot='path-svg']");
        var circle = cut.Find("circle[data-slot='path-circle']");

        Assert.Equal("true", svg.GetAttribute("aria-hidden"));
        Assert.Equal("95", circle.GetAttribute("r"));
    }

    [Fact]
    public void PathRendering_ShowPathFalse_OmitsPathMarkup()
    {
        var cut = Render<RzOrbitingCircles>(parameters => parameters
            .Add(p => p.ShowPath, false));

        Assert.Empty(cut.FindAll("svg[data-slot='path-svg']"));
        Assert.Empty(cut.FindAll("circle[data-slot='path-circle']"));
    }

    [Fact]
    public void ItemRendering_RendersOneWrapperPerTopLevelChildWithEvenAngles()
    {
        var cut = Render<RzOrbitingCircles>(parameters => parameters
            .Add(p => p.ChildContent, BuildChildren(4)));

        var items = cut.FindAll("div[data-slot='item']");
        Assert.Equal(4, items.Count);

        Assert.Equal("0", items[0].GetAttribute("data-angle"));
        Assert.Equal("90", items[1].GetAttribute("data-angle"));
        Assert.Equal("180", items[2].GetAttribute("data-angle"));
        Assert.Equal("270", items[3].GetAttribute("data-angle"));

        Assert.All(items, item => Assert.Contains("--rz-orbit-duration", item.GetAttribute("style")));
        Assert.All(items, item => Assert.Contains("--rz-orbit-radius", item.GetAttribute("style")));
        Assert.All(items, item => Assert.Contains("--rz-orbit-angle", item.GetAttribute("style")));
        Assert.All(items, item => Assert.Contains("--rz-orbit-item-size", item.GetAttribute("style")));
    }

    [Fact]
    public void ItemRendering_NullChildContent_RendersZeroItems()
    {
        var cut = Render<RzOrbitingCircles>();

        Assert.Empty(cut.FindAll("div[data-slot='item']"));
    }

    [Fact]
    public void ReverseAndDurationSpeed_EmitExpectedMetadataAndClampedDuration()
    {
        var cut = Render<RzOrbitingCircles>(parameters => parameters
            .Add(p => p.Reverse, true)
            .Add(p => p.Duration, 20)
            .Add(p => p.Speed, 2)
            .Add(p => p.ChildContent, BuildChildren(1)));

        var item = cut.Find("div[data-slot='item']");
        Assert.Equal("true", item.GetAttribute("data-reverse"));
        Assert.Contains("[animation-direction:reverse]", item.ClassName);
        Assert.Contains("--rz-orbit-duration:10s", item.GetAttribute("style"));
    }

    [Fact]
    public void ClampBehavior_ClampsInvalidDurationSpeedRadiusAndItemSize()
    {
        var cut = Render<RzOrbitingCircles>(parameters => parameters
            .Add(p => p.Duration, 0)
            .Add(p => p.Speed, 0)
            .Add(p => p.Radius, -100)
            .Add(p => p.ItemSize, 0)
            .Add(p => p.ChildContent, BuildChildren(1)));

        var circle = cut.Find("circle[data-slot='path-circle']");
        var item = cut.Find("div[data-slot='item']");

        Assert.Equal("0", circle.GetAttribute("r"));
        Assert.Equal("0", item.GetAttribute("data-radius"));
        Assert.Contains("--rz-orbit-duration:1000s", item.GetAttribute("style"));
        Assert.Contains("--rz-orbit-item-size:1px", item.GetAttribute("style"));
    }

    [Fact]
    public void Accessibility_DefaultDecorative_RendersAriaHidden()
    {
        var cut = Render<RzOrbitingCircles>();

        var root = cut.Find("[data-slot='orbiting-circles']");
        Assert.Equal("true", root.GetAttribute("aria-hidden"));
        Assert.Null(root.GetAttribute("role"));
        Assert.Null(root.GetAttribute("aria-label"));
    }

    [Fact]
    public void Accessibility_NonDecorativeWithLabel_RendersRoleAndAriaLabel()
    {
        var cut = Render<RzOrbitingCircles>(parameters => parameters
            .Add(p => p.Decorative, false)
            .Add(p => p.AriaLabel, "Orbit group"));

        var root = cut.Find("[data-slot='orbiting-circles']");
        Assert.Equal("group", root.GetAttribute("role"));
        Assert.Equal("Orbit group", root.GetAttribute("aria-label"));
        Assert.Null(root.GetAttribute("aria-hidden"));
    }

    [Fact]
    public void SsrOnly_NoAlpineHooksEmitted()
    {
        var cut = Render<RzOrbitingCircles>(parameters => parameters
            .Add(p => p.ChildContent, BuildChildren(2)));

        Assert.Empty(cut.FindAll("[x-data]"));
        Assert.Empty(cut.FindAll("[data-alpine-root]"));
        Assert.Empty(cut.FindAll("[data-assets]"));
        Assert.Empty(cut.FindAll("[data-nonce]"));
    }

    [SuppressMessage("Usage", "ASP0006:Do not use non-literal sequence numbers")]
    private static RenderFragment BuildChildren(int count)
    {
        return builder =>
        {
            for (var i = 0; i < count; i++)
            {
                var seq = i * 3;
                builder.OpenElement(seq, "span");
                builder.AddAttribute(seq + 1, "class", "inline-block");
                builder.AddContent(seq + 2, i.ToString(CultureInfo.InvariantCulture));
                builder.CloseElement();
            }
        };
    }
}
