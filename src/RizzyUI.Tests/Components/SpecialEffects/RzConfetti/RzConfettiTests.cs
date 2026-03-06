using Bunit;
using System.Text.RegularExpressions;

namespace RizzyUI.Tests.Components.SpecialEffects.RzConfetti;

public class RzConfettiTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    public RzConfettiTests(WebAppFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public void RendersRootWithDataSlotAndProvidedId()
    {
        var cut = Render<global::RizzyUI.RzConfetti>(p => p.Add(x => x.Id, "celebrate-host"));

        var root = cut.Find("[data-slot='confetti']");
        Assert.Equal("celebrate-host", root.Id);
    }

    [Fact]
    public void RendersRequiredAlpineContainerContract()
    {
        var cut = Render<global::RizzyUI.RzConfetti>(p => p.Add(x => x.Id, "host-a"));

        var alpine = cut.Find("[data-alpine-root='host-a']");
        Assert.Equal("rzConfetti", alpine.GetAttribute("x-data"));
        Assert.NotNull(alpine.GetAttribute("data-assets"));
        Assert.NotNull(alpine.GetAttribute("data-nonce"));
        Assert.Equal("host-a", alpine.GetAttribute("data-confetti-host"));
        Assert.NotNull(alpine.GetAttribute("data-confetti-options"));
        Assert.NotNull(alpine.GetAttribute("data-confetti-global-options"));
        Assert.Equal("burst", alpine.GetAttribute("data-confetti-pattern"));
    }

    [Fact]
    public void RendersCanvasWithExpectedAccessibilityAndSlot()
    {
        var cut = Render<global::RizzyUI.RzConfetti>();

        var canvas = cut.Find("canvas[data-slot='canvas']");
        Assert.Equal("true", canvas.GetAttribute("aria-hidden"));
        Assert.Equal("-1", canvas.GetAttribute("tabindex"));
    }

    [Fact]
    public void RendersContentWrapperOnlyWhenChildContentExists()
    {
        var withContent = Render<global::RizzyUI.RzConfetti>(p => p.AddChildContent("<p>hello</p>"));
        Assert.Single(withContent.FindAll("[data-slot='content']"));

        var withoutContent = Render<global::RizzyUI.RzConfetti>();
        Assert.Empty(withoutContent.FindAll("[data-slot='content']"));
    }

    [Fact]
    public void AppliesDefaultClassesAndMergesUserClass()
    {
        var cut = Render<global::RizzyUI.RzConfetti>(p => p.AddUnmatched("class", "custom-host"));

        var root = cut.Find("[data-slot='confetti']");
        var viewport = cut.Find("[data-slot='viewport']");
        var canvas = cut.Find("[data-slot='canvas']");

        Assert.Contains("relative", root.ClassList);
        Assert.Contains("custom-host", root.ClassList);
        Assert.Contains("absolute", viewport.ClassList);
        Assert.Contains("size-full", canvas.ClassList);
    }

    [Fact]
    public void EmitsManualStartDefaultsAndOverrides()
    {
        var cutDefault = Render<global::RizzyUI.RzConfetti>();
        Assert.Equal("false", cutDefault.Find("[data-slot='viewport']").GetAttribute("data-confetti-manual-start"));

        var cutManual = Render<global::RizzyUI.RzConfetti>(p => p.Add(x => x.ManualStart, true));
        Assert.Equal("true", cutManual.Find("[data-slot='viewport']").GetAttribute("data-confetti-manual-start"));
    }

    [Fact]
    public void EmitsKebabCasePatternAndReducedMotionAttribute()
    {
        var cut = Render<global::RizzyUI.RzConfetti>(p => p
            .Add(x => x.Pattern, global::RizzyUI.ConfettiPattern.RandomDirection)
            .Add(x => x.RespectReducedMotion, false));

        var viewport = cut.Find("[data-slot='viewport']");
        Assert.Equal("random-direction", viewport.GetAttribute("data-confetti-pattern"));
        Assert.Equal("false", viewport.GetAttribute("data-confetti-respect-reduced-motion"));
    }

    [Fact]
    public void EmitsSerializedOptionsAndGlobalOptionsAndHostOverride()
    {
        var cut = Render<global::RizzyUI.RzConfetti>(p => p
            .Add(x => x.HostId, "logical-host")
            .Add(x => x.Options, new global::RizzyUI.ConfettiRequest { ParticleCount = 25, OriginX = 0.2 })
            .Add(x => x.GlobalOptions, new global::RizzyUI.ConfettiGlobalOptions { Resize = false, UseWorker = false }));

        var viewport = cut.Find("[data-slot='viewport']");
        Assert.Equal("logical-host", viewport.GetAttribute("data-confetti-host"));

        var options = viewport.GetAttribute("data-confetti-options") ?? string.Empty;
        var globals = viewport.GetAttribute("data-confetti-global-options") ?? string.Empty;
        Assert.Contains("particleCount", options);
        Assert.Contains("origin", options);
        Assert.Contains("\"resize\":false", globals);
        Assert.Contains("\"useWorker\":false", globals);
    }

    [Fact]
    public void DoesNotRenderBlazorInteractiveArtifacts()
    {
        var cut = Render<global::RizzyUI.RzConfetti>();
        var markup = cut.Markup;

        Assert.DoesNotContain("blazor:", markup, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("@on", markup, StringComparison.OrdinalIgnoreCase);
        Assert.False(Regex.IsMatch(markup, "on(click|change|submit)=", RegexOptions.IgnoreCase));
    }
}
