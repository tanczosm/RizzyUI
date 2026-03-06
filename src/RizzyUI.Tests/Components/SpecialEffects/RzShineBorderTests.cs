using Bunit;

namespace RizzyUI.Tests.Components.SpecialEffects;

public class RzShineBorderTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    public RzShineBorderTests(WebAppFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public void Renders_Default_Decorative_Overlay_Contract()
    {
        var cut = Render<RzShineBorder>(p => p.Add(c => c.Id, "shine-1"));

        var overlay = cut.Find("div#shine-1");
        Assert.Equal("shine-border", overlay.GetAttribute("data-slot"));
        Assert.Equal("true", overlay.GetAttribute("aria-hidden"));
        Assert.Null(overlay.GetAttribute("role"));
        Assert.Null(overlay.GetAttribute("tabindex"));

        Assert.Contains("pointer-events-none", overlay.ClassList);
        Assert.Contains("absolute", overlay.ClassList);
        Assert.Contains("inset-0", overlay.ClassList);
        Assert.Contains("size-full", overlay.ClassList);
        Assert.Contains("rounded-[inherit]", overlay.ClassList);
        Assert.Contains("will-change-[background-position]", overlay.ClassList);
        Assert.Contains("motion-safe:animate-shine", overlay.ClassList);
    }

    [Fact]
    public void Does_Not_Emit_Alpine_Hooks_Because_Component_Is_Css_Only()
    {
        var cut = Render<RzShineBorder>();

        var overlay = cut.Find("[data-slot='shine-border']");
        Assert.Null(overlay.GetAttribute("x-data"));
        Assert.Null(overlay.GetAttribute("data-alpine-root"));
        Assert.Null(overlay.GetAttribute("data-assets"));
        Assert.Null(overlay.GetAttribute("data-nonce"));
    }

    [Fact]
    public void Merges_User_Class_And_Passes_Additional_Attributes()
    {
        var cut = Render<RzShineBorder>(p => p
            .AddUnmatched("class", "custom-border")
            .AddUnmatched("data-test-id", "shine-border-custom"));

        var overlay = cut.Find("[data-slot='shine-border']");
        Assert.Contains("custom-border", overlay.ClassList);
        Assert.Contains("absolute", overlay.ClassList);
        Assert.Equal("shine-border-custom", overlay.GetAttribute("data-test-id"));
    }

    [Fact]
    public void BorderWidth_And_Duration_Are_Reflected_In_Style()
    {
        var cut = Render<RzShineBorder>(p => p
            .Add(c => c.BorderWidth, 2.5)
            .Add(c => c.Duration, 9));

        var style = cut.Find("[data-slot='shine-border']").GetAttribute("style");
        Assert.NotNull(style);
        Assert.Contains("--border-width:2.5px", style);
        Assert.Contains("--duration:9s", style);
    }

    [Fact]
    public void ShineColor_Renders_Single_Color_Gradient()
    {
        var cut = Render<RzShineBorder>(p => p.Add(c => c.ShineColor, "rebeccapurple"));

        var style = cut.Find("[data-slot='shine-border']").GetAttribute("style");
        Assert.NotNull(style);
        Assert.Contains("radial-gradient(transparent,transparent,rebeccapurple,transparent,transparent)", style);
    }

    [Fact]
    public void ShineColors_Takes_Precedence_Over_ShineColor_When_Both_Are_Set()
    {
        var cut = Render<RzShineBorder>(p => p
            .Add(c => c.ShineColor, "black")
            .Add(c => c.ShineColors, new[] { "#A07CFE", "#FE8FB5", "#FFBE7B" }));

        var style = cut.Find("[data-slot='shine-border']").GetAttribute("style");
        Assert.NotNull(style);
        Assert.Contains("#A07CFE,#FE8FB5,#FFBE7B", style);
        Assert.DoesNotContain("transparent,transparent,black,transparent,transparent", style);
    }

    [Fact]
    public void Empty_ShineColors_Falls_Back_To_ShineColor()
    {
        var cut = Render<RzShineBorder>(p => p
            .Add(c => c.ShineColor, "#333333")
            .Add(c => c.ShineColors, Array.Empty<string>()));

        var style = cut.Find("[data-slot='shine-border']").GetAttribute("style");
        Assert.NotNull(style);
        Assert.Contains("radial-gradient(transparent,transparent,#333333,transparent,transparent)", style);
    }

    [Fact]
    public void Missing_Color_Configuration_Falls_Back_To_Default_Black()
    {
        var cut = Render<RzShineBorder>();

        var style = cut.Find("[data-slot='shine-border']").GetAttribute("style");
        Assert.NotNull(style);
        Assert.Contains("radial-gradient(transparent,transparent,#000000,transparent,transparent)", style);
    }

    [Fact]
    public void Negative_BorderWidth_And_NonPositive_Duration_Are_Normalized()
    {
        var cut = Render<RzShineBorder>(p => p
            .Add(c => c.BorderWidth, -4)
            .Add(c => c.Duration, 0));

        var style = cut.Find("[data-slot='shine-border']").GetAttribute("style");
        Assert.NotNull(style);
        Assert.Contains("--border-width:0px", style);
        Assert.Contains("--duration:14s", style);
    }

    [Fact]
    public void Invalid_Color_String_Still_Renders_Without_Throwing()
    {
        var cut = Render<RzShineBorder>(p => p.Add(c => c.ShineColor, "not-a-real-color("));

        var style = cut.Find("[data-slot='shine-border']").GetAttribute("style");
        Assert.NotNull(style);
        Assert.Contains("not-a-real-color(", style);
    }

    [Fact]
    public void Appends_User_Style_After_Generated_Style()
    {
        var cut = Render<RzShineBorder>(p => p.AddUnmatched("style", "outline:1px solid red"));

        var style = cut.Find("[data-slot='shine-border']").GetAttribute("style");
        Assert.NotNull(style);
        Assert.Contains("padding:var(--border-width)", style);
        Assert.Contains("outline:1px solid red", style);
    }

    [Fact]
    public void Can_Disable_Reduced_Motion_Respect_Class()
    {
        var cut = Render<RzShineBorder>(p => p.Add(c => c.RespectReducedMotion, false));

        var overlay = cut.Find("[data-slot='shine-border']");
        Assert.Contains("animate-shine", overlay.ClassList);
        Assert.DoesNotContain("motion-safe:animate-shine", overlay.ClassList);
    }

    [Fact]
    public void Does_Not_Emit_Blazor_Event_Attributes_For_Ssr_Only_Usage()
    {
        var cut = Render<RzShineBorder>();

        var markup = cut.Markup;
        Assert.DoesNotContain("@onclick", markup);
        Assert.DoesNotContain("@onchange", markup);
        Assert.DoesNotContain("@bind", markup);
    }
}
