using Bunit;

namespace RizzyUI.Tests.Components.Effects;

public class RzBorderBeamTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    public RzBorderBeamTests(WebAppFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public void Renders_Default_Root_With_Required_DataSlot_And_Id()
    {
        var cut = Render<RzBorderBeam>(parameters => parameters.Add(p => p.Id, "beam-id"));

        var root = cut.Find("#beam-id");
        Assert.Equal("border-beam", root.GetAttribute("data-slot"));
    }

    [Fact]
    public void Renders_Track_And_Beam_Internal_Slots()
    {
        var cut = Render<RzBorderBeam>();

        Assert.NotNull(cut.Find("[data-slot='border-beam-track']"));
        Assert.NotNull(cut.Find("[data-slot='border-beam-beam']"));
    }

    [Fact]
    public void Root_Is_AriaHidden_And_Not_Focusable()
    {
        var cut = Render<RzBorderBeam>();

        var root = cut.Find("[data-slot='border-beam']");
        Assert.Equal("true", root.GetAttribute("aria-hidden"));
        Assert.Null(root.GetAttribute("tabindex"));
        Assert.Empty(root.QuerySelectorAll("a,button,input,select,textarea,[tabindex]"));
    }

    [Fact]
    public void Renders_No_Alpine_Hooks()
    {
        var cut = Render<RzBorderBeam>();

        Assert.DoesNotContain("x-data=", cut.Markup);
        Assert.DoesNotContain("data-alpine-root", cut.Markup);
        Assert.DoesNotContain("data-assets", cut.Markup);
        Assert.DoesNotContain("data-nonce", cut.Markup);
    }

    [Fact]
    public void Default_Classes_Are_Applied_To_Root_Track_And_Beam()
    {
        var cut = Render<RzBorderBeam>();

        var root = cut.Find("[data-slot='border-beam']");
        var track = cut.Find("[data-slot='border-beam-track']");
        var beam = cut.Find("[data-slot='border-beam-beam']");

        Assert.Contains("pointer-events-none", root.ClassList);
        Assert.Contains("absolute", root.ClassList);
        Assert.Contains("rounded-[inherit]", root.ClassList);

        Assert.Contains("absolute", track.ClassList);
        Assert.Contains("mask-intersect", track.ClassList);

        Assert.Contains("absolute", beam.ClassList);
        Assert.Contains("aspect-square", beam.ClassList);
    }

    [Fact]
    public void AdditionalAttributes_Class_Merges_On_Root()
    {
        var cut = Render<RzBorderBeam>(parameters => parameters.AddUnmatched("class", "custom-root"));

        var root = cut.Find("[data-slot='border-beam']");
        var beam = cut.Find("[data-slot='border-beam-beam']");

        Assert.Contains("custom-root", root.ClassList);
        Assert.DoesNotContain("custom-root", beam.ClassList);
    }

    [Fact]
    public void BeamClass_Merges_On_Beam_Slot()
    {
        const string beamClass = "from-transparent via-red-500 to-transparent";

        var cut = Render<RzBorderBeam>(parameters => parameters.Add(p => p.BeamClass, beamClass));

        var beam = cut.Find("[data-slot='border-beam-beam']");
        Assert.Contains("from-transparent", beam.ClassList);
        Assert.Contains("via-red-500", beam.ClassList);
        Assert.Contains("to-transparent", beam.ClassList);
    }

    [Fact]
    public void Default_Css_Custom_Properties_Are_Emitted()
    {
        var cut = Render<RzBorderBeam>();

        var track = cut.Find("[data-slot='border-beam-track']");
        var beam = cut.Find("[data-slot='border-beam-beam']");

        Assert.Contains("--rz-border-beam-width:1px", track.GetAttribute("style"));

        var style = beam.GetAttribute("style");
        Assert.Contains("--rz-border-beam-size:50px", style);
        Assert.Contains("--rz-border-beam-duration:6s", style);
        Assert.Contains("--rz-border-beam-delay:0", style);
        Assert.Contains("--rz-border-beam-offset:0%", style);
        Assert.Contains("--rz-border-beam-color-from:#ffaa40", style);
        Assert.Contains("--rz-border-beam-color-to:#9c40ff", style);
    }

    [Fact]
    public void Reverse_Emits_Reversed_Animation_Contract()
    {
        var cut = Render<RzBorderBeam>(parameters => parameters.Add(p => p.Reverse, true));

        var beam = cut.Find("[data-slot='border-beam-beam']");
        Assert.Contains("--rz-border-beam-direction:reverse", beam.GetAttribute("style"));
    }

    [Theory]
    [InlineData(-10, "--rz-border-beam-offset:0%")]
    [InlineData(130, "--rz-border-beam-offset:100%")]
    public void InitialOffset_Is_Clamped_When_Out_Of_Range(double offset, string expected)
    {
        var cut = Render<RzBorderBeam>(parameters => parameters.Add(p => p.InitialOffset, offset));

        var beam = cut.Find("[data-slot='border-beam-beam']");
        Assert.Contains(expected, beam.GetAttribute("style"));
    }

    [Fact]
    public void BorderWidth_Is_Clamped_When_Negative()
    {
        var cut = Render<RzBorderBeam>(parameters => parameters.Add(p => p.BorderWidth, -5));

        var track = cut.Find("[data-slot='border-beam-track']");
        Assert.Contains("--rz-border-beam-width:0px", track.GetAttribute("style"));
    }

    [Theory]
    [InlineData(0, "--rz-border-beam-duration:0.1s")]
    [InlineData(-2, "--rz-border-beam-duration:0.1s")]
    public void Duration_Is_Clamped_When_Zero_Or_Negative(double duration, string expected)
    {
        var cut = Render<RzBorderBeam>(parameters => parameters.Add(p => p.Duration, duration));

        var beam = cut.Find("[data-slot='border-beam-beam']");
        Assert.Contains(expected, beam.GetAttribute("style"));
    }

    [Fact]
    public void Color_Parameters_Fall_Back_When_Null_Or_Empty()
    {
        var cutNull = Render<RzBorderBeam>(parameters => parameters
            .Add(p => p.ColorFrom, (string?)null)
            .Add(p => p.ColorTo, (string?)null));
        var cutEmpty = Render<RzBorderBeam>(parameters => parameters
            .Add(p => p.ColorFrom, string.Empty)
            .Add(p => p.ColorTo, " "));

        Assert.Contains("--rz-border-beam-color-from:#ffaa40", cutNull.Find("[data-slot='border-beam-beam']").GetAttribute("style"));
        Assert.Contains("--rz-border-beam-color-to:#9c40ff", cutNull.Find("[data-slot='border-beam-beam']").GetAttribute("style"));
        Assert.Contains("--rz-border-beam-color-from:#ffaa40", cutEmpty.Find("[data-slot='border-beam-beam']").GetAttribute("style"));
        Assert.Contains("--rz-border-beam-color-to:#9c40ff", cutEmpty.Find("[data-slot='border-beam-beam']").GetAttribute("style"));
    }

    [Fact]
    public void Renders_Without_Blazor_Event_Attributes()
    {
        var cut = Render<RzBorderBeam>();

        Assert.DoesNotContain("onclick=", cut.Markup, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("onchange=", cut.Markup, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("onsubmit=", cut.Markup, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Omitting_BeamClass_Does_Not_Break_Internal_Beam_Rendering()
    {
        var cut = Render<RzBorderBeam>();

        var beam = cut.Find("[data-slot='border-beam-beam']");
        Assert.NotNull(beam);
        Assert.Contains("bg-linear-to-l", beam.ClassList);
    }
}
