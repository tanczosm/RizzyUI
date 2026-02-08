using Bunit;

namespace RizzyUI.Tests.Components.Navigation;

public class RzScrollAreaTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    public RzScrollAreaTests(WebAppFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public void RzScrollArea_DefaultRender_RendersViewportAndDefaultVerticalScrollBar()
    {
        // Act
        var cut = RenderComponent<RzScrollArea>(parameters => parameters
            .AddChildContent("Scrollable content")
        );

        // Assert
        cut.Find("[data-slot='scroll-area']");
        var viewport = cut.Find("[data-slot='scroll-area-viewport']");
        Assert.Equal("0", viewport.GetAttribute("tabindex"));
        Assert.Contains("Scrollable content", viewport.TextContent);

        var scrollbar = cut.Find("[data-slot='scroll-area-scrollbar']");
        Assert.Equal("vertical", scrollbar.GetAttribute("data-orientation"));
        cut.Find("[data-slot='scroll-area-thumb']");
    }

    [Fact]
    public void RzScrollArea_ShowDefaultScrollBarFalse_DoesNotRenderDefaultScrollbar()
    {
        // Act
        var cut = RenderComponent<RzScrollArea>(parameters => parameters
            .Add(p => p.ShowDefaultScrollBar, false)
        );

        // Assert
        Assert.Empty(cut.FindAll("[data-slot='scroll-area-scrollbar']"));
    }

    [Fact]
    public void ScrollBar_OrientationParameter_UpdatesDataOrientation()
    {
        // Act
        var cut = RenderComponent<ScrollBar>(parameters => parameters
            .Add(p => p.Orientation, Orientation.Horizontal)
        );

        // Assert
        var scrollbar = cut.Find("[data-slot='scroll-area-scrollbar']");
        Assert.Equal("horizontal", scrollbar.GetAttribute("data-orientation"));
    }
}
