using AwesomeAssertions;
using Alba;

namespace RizzyUI.Tests.Components.Layout;

public class RzCarouselTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    private readonly IAlbaHost _host;

    public RzCarouselTests(WebAppFixture fixture) : base(fixture)
    {
        _host = fixture.Host;
    }

    [Fact]
    public void RzCarousel_RendersSlides()
    {
        var cut = RenderComponent<RzCarousel>(parameters => parameters
            .Add(p => p.Options, new CarouselOptions())
            .AddChildContent(builder =>
            {
                builder.OpenComponent<CarouselContent>(0);
                builder.AddChildContent(contentBuilder =>
                {
                    contentBuilder.OpenComponent<CarouselItem>(0);
                    contentBuilder.AddChildContent("Slide 1");
                    contentBuilder.CloseComponent();
                });
                builder.CloseComponent();
                builder.OpenComponent<CarouselPrevious>(1);
                builder.AddChildContent("Prev");
                builder.CloseComponent();
                builder.OpenComponent<CarouselNext>(2);
                builder.AddChildContent("Next");
                builder.CloseComponent();
            })
        );

        cut.Markup.Should().Contain("Slide 1");
    }
}