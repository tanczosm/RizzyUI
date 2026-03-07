using Bunit;
using Microsoft.AspNetCore.Components;

namespace RizzyUI.Tests.Components.Form;

public class RzRainbowButtonTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    public RzRainbowButtonTests(WebAppFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public void Renders_Default_Button_With_RainbowButton_DataSlot()
    {
        var cut = Render<RzRainbowButton>(parameters => parameters
            .Add(p => p.ChildContent, "Rainbow"));

        var root = cut.Find("[data-slot='rainbow-button']");

        Assert.Equal("button", root.TagName.ToLowerInvariant());
        Assert.False(string.IsNullOrWhiteSpace(root.GetAttribute("id")));
        Assert.Null(root.GetAttribute("x-data"));
        Assert.Null(root.GetAttribute("data-alpine-root"));
    }

    [Fact]
    public void Renders_Default_Classes_For_Base_Component()
    {
        var cut = Render<RzRainbowButton>(parameters => parameters
            .Add(p => p.ChildContent, "Rainbow"));

        var root = cut.Find("button");

        Assert.Contains("relative", root.ClassList);
        Assert.Contains("inline-flex", root.ClassList);
        Assert.Contains("items-center", root.ClassList);
        Assert.Contains("justify-center", root.ClassList);
        Assert.Contains("animate-rainbow", root.ClassList);
    }

    [Fact]
    public void Renders_Default_Variant_When_No_Variant_Is_Provided()
    {
        var cut = Render<RzRainbowButton>(parameters => parameters
            .Add(p => p.ChildContent, "Rainbow"));

        var root = cut.Find("button");

        Assert.Contains("text-primary-foreground", root.ClassList);
        Assert.Contains("border-0", root.ClassList);
    }

    [Fact]
    public void Renders_Outline_Variant_Classes_When_Requested()
    {
        var cut = Render<RzRainbowButton>(parameters => parameters
            .Add(p => p.Variant, RainbowButtonVariant.Outline)
            .Add(p => p.ChildContent, "Rainbow"));

        var root = cut.Find("button");

        Assert.Contains("border", root.ClassList);
        Assert.Contains("border-input", root.ClassList);
        Assert.Contains("text-accent-foreground", root.ClassList);
    }

    [Theory]
    [InlineData(RainbowButtonSize.Default, "h-9")]
    [InlineData(RainbowButtonSize.Small, "h-8")]
    [InlineData(RainbowButtonSize.Large, "h-11")]
    [InlineData(RainbowButtonSize.Icon, "size-9")]
    public void Renders_Size_Classes(RainbowButtonSize size, string expectedClass)
    {
        var cut = Render<RzRainbowButton>(parameters => parameters
            .Add(p => p.Size, size)
            .Add(p => p.ChildContent, "Rainbow"));

        var root = cut.Find("button");

        Assert.Contains(expectedClass, root.ClassList);
    }

    [Fact]
    public void Passes_Through_AdditionalAttributes()
    {
        var cut = Render<RzRainbowButton>(parameters => parameters
            .Add(p => p.ChildContent, "Rainbow")
            .AddUnmatched("data-test-id", "cta")
            .AddUnmatched("aria-describedby", "desc")
            .AddUnmatched("hx-post", "/submit"));

        var root = cut.Find("button");

        Assert.Equal("cta", root.GetAttribute("data-test-id"));
        Assert.Equal("desc", root.GetAttribute("aria-describedby"));
        Assert.Equal("/submit", root.GetAttribute("hx-post"));
    }

    [Fact]
    public void Merges_User_Class_With_Default_Classes()
    {
        var cut = Render<RzRainbowButton>(parameters => parameters
            .Add(p => p.ChildContent, "Rainbow")
            .AddUnmatched("class", "my-custom-class"));

        var root = cut.Find("button");

        Assert.Contains("my-custom-class", root.ClassList);
        Assert.Contains("animate-rainbow", root.ClassList);
    }

    [Fact]
    public void Renders_Disabled_Attribute_When_Disabled()
    {
        var cut = Render<RzRainbowButton>(parameters => parameters
            .Add(p => p.Disabled, true)
            .Add(p => p.ChildContent, "Rainbow"));

        var root = cut.Find("button");

        Assert.True(root.HasAttribute("disabled"));
        Assert.Contains("disabled:pointer-events-none", root.ClassList);
        Assert.Contains("disabled:opacity-50", root.ClassList);
    }

    [Fact]
    public void Renders_AriaLabel_When_Provided()
    {
        var cut = Render<RzRainbowButton>(parameters => parameters
            .Add(p => p.AriaLabel, "Create item")
            .Add(p => p.ChildContent, "Rainbow"));

        var root = cut.Find("button");

        Assert.Equal("Create item", root.GetAttribute("aria-label"));
    }

    [Fact]
    public void Renders_Type_Button_By_Default()
    {
        var cut = Render<RzRainbowButton>(parameters => parameters
            .Add(p => p.ChildContent, "Rainbow"));

        var root = cut.Find("button");

        Assert.Equal("button", root.GetAttribute("type"));
    }

    [Fact]
    public void Allows_Type_Override()
    {
        var cut = Render<RzRainbowButton>(parameters => parameters
            .Add(p => p.Type, "submit")
            .Add(p => p.ChildContent, "Rainbow"));

        var root = cut.Find("button");

        Assert.Equal("submit", root.GetAttribute("type"));
    }

    [Fact]
    public void Renders_ChildContent()
    {
        var cut = Render<RzRainbowButton>(parameters => parameters
            .Add(p => p.ChildContent, "Get Unlimited Access"));

        Assert.Contains("Get Unlimited Access", cut.Markup);
    }

    [Fact]
    public void AsChild_Merges_Base_Classes_Onto_Child_Host()
    {
        RenderFragment child = b =>
        {
            b.OpenElement(0, "a");
            b.AddAttribute(1, "href", "/pricing");
            b.AddContent(2, "Pricing");
            b.CloseElement();
        };

        var cut = Render<RzRainbowButton>(parameters => parameters
            .Add(p => p.AsChild, true)
            .Add(p => p.ChildContent, child));

        var root = cut.Find("a[data-slot='rainbow-button']");

        Assert.Equal("/pricing", root.GetAttribute("href"));
        Assert.Contains("animate-rainbow", root.ClassList);
    }

    [Fact]
    public void IconOnly_Usage_With_AriaLabel_Renders_Accessibly()
    {
        RenderFragment iconOnly = b =>
        {
            b.OpenElement(0, "svg");
            b.AddAttribute(1, "aria-hidden", "true");
            b.CloseElement();
        };

        var cut = Render<RzRainbowButton>(parameters => parameters
            .Add(p => p.Size, RainbowButtonSize.Icon)
            .Add(p => p.AriaLabel, "Share")
            .Add(p => p.ChildContent, iconOnly));

        var root = cut.Find("button");

        Assert.Equal("Share", root.GetAttribute("aria-label"));
        Assert.NotNull(root.QuerySelector("svg"));
    }

    [Fact]
    public void Does_Not_Emit_Blazor_Interactivity_Markers()
    {
        var cut = Render<RzRainbowButton>(parameters => parameters
            .Add(p => p.ChildContent, "Rainbow"));

        Assert.DoesNotContain("blazor:", cut.Markup, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("@on", cut.Markup, StringComparison.OrdinalIgnoreCase);
    }
}
