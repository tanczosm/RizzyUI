using Bunit;
using Microsoft.AspNetCore.Components;
using System.Text.RegularExpressions;

namespace RizzyUI.Tests.Components.SpecialEffects.RzConfetti;

public class ConfettiTriggerTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    public ConfettiTriggerTests(WebAppFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public void RendersDefaultNativeButtonWithRequiredAttributes()
    {
        var cut = Render<global::RizzyUI.ConfettiTrigger>(p => p.AddChildContent("Celebrate"));

        var trigger = cut.Find("[data-slot='confetti-trigger']");
        Assert.Equal("button", trigger.TagName.ToLowerInvariant());
        Assert.Equal("button", trigger.GetAttribute("type"));
        Assert.Equal("true", trigger.GetAttribute("data-confetti-trigger"));
        Assert.Equal("click", trigger.GetAttribute("data-confetti-trigger-event"));
    }

    [Fact]
    public void EmitsOptionalTargetPatternAndRequestAttributes()
    {
        var cut = Render<global::RizzyUI.ConfettiTrigger>(p => p
            .Add(x => x.TargetId, "host-id")
            .Add(x => x.Pattern, global::RizzyUI.ConfettiPattern.Stars)
            .Add(x => x.Options, new global::RizzyUI.ConfettiRequest { ParticleCount = 10 })
            .AddChildContent("Celebrate"));

        var trigger = cut.Find("[data-slot='confetti-trigger']");
        Assert.Equal("host-id", trigger.GetAttribute("data-confetti-target"));
        Assert.Equal("stars", trigger.GetAttribute("data-confetti-pattern"));
        Assert.Contains("particleCount", trigger.GetAttribute("data-confetti-request") ?? string.Empty);
    }

    [Fact]
    public void OmitsTargetWhenNotProvidedAndMergesClass()
    {
        var cut = Render<global::RizzyUI.ConfettiTrigger>(p => p
            .AddUnmatched("class", "extra-style")
            .AddChildContent("Celebrate"));

        var trigger = cut.Find("[data-slot='confetti-trigger']");
        Assert.Null(trigger.GetAttribute("data-confetti-target"));
        Assert.Contains("extra-style", trigger.ClassList);
    }

    [Fact]
    public void DisabledNativeButtonEmitsDisabledAttribute()
    {
        var cut = Render<global::RizzyUI.ConfettiTrigger>(p => p
            .Add(x => x.Disabled, true)
            .AddChildContent("Celebrate"));

        var trigger = cut.Find("[data-slot='confetti-trigger']");
        Assert.NotNull(trigger.GetAttribute("disabled"));
    }

    [Fact]
    public void AsChildDisabledEmitsAriaDisabled()
    {
        RenderFragment child = b =>
        {
            b.OpenElement(0, "a");
            b.AddAttribute(1, "href", "#");
            b.AddContent(2, "Celebrate");
            b.CloseElement();
        };

        var cut = Render<global::RizzyUI.ConfettiTrigger>(p => p
            .Add(x => x.AsChild, true)
            .Add(x => x.Disabled, true)
            .Add(x => x.ChildContent, child));

        var trigger = cut.Find("a[data-slot='confetti-trigger']");
        Assert.Equal("true", trigger.GetAttribute("aria-disabled"));
    }

    [Fact]
    public void AsChildPreservesChildElementAndAttributes()
    {
        RenderFragment child = b =>
        {
            b.OpenElement(0, "button");
            b.AddAttribute(1, "class", "inner-btn");
            b.AddContent(2, "Go");
            b.CloseElement();
        };

        var cut = Render<global::RizzyUI.ConfettiTrigger>(p => p
            .Add(x => x.AsChild, true)
            .Add(x => x.ChildContent, child));

        var trigger = cut.Find("button[data-confetti-trigger='true']");
        Assert.Contains("inner-btn", trigger.ClassList);
        Assert.Equal("confetti-trigger", trigger.GetAttribute("data-slot"));
    }

    [Fact]
    public void ThrowsWhenChildContentMissing()
    {
        Assert.Throws<InvalidOperationException>(() => Render<global::RizzyUI.ConfettiTrigger>());
    }

    [Fact]
    public void NativeButtonRemainsKeyboardFocusableAndSsrSafe()
    {
        var cut = Render<global::RizzyUI.ConfettiTrigger>(p => p.AddChildContent("Celebrate"));
        var trigger = cut.Find("[data-slot='confetti-trigger']");
        Assert.Equal("button", trigger.TagName.ToLowerInvariant());
        Assert.Null(trigger.GetAttribute("tabindex"));

        var markup = cut.Markup;
        Assert.DoesNotContain("blazor:", markup, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("@on", markup, StringComparison.OrdinalIgnoreCase);
        Assert.False(Regex.IsMatch(markup, "on(click|change|submit)=", RegexOptions.IgnoreCase));
    }

    [Fact]
    public void DoesNotRenderAlpineRootContract()
    {
        var cut = Render<global::RizzyUI.ConfettiTrigger>(p => p.AddChildContent("Celebrate"));
        Assert.Empty(cut.FindAll("[data-alpine-root]"));
        Assert.Empty(cut.FindAll("[x-data='rzConfetti']"));
    }
}
