using AwesomeAssertions;
using Alba;

namespace RizzyUI.Tests.Components.Layout;

public class RzCardTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    private readonly IAlbaHost _host;

    public RzCardTests(WebAppFixture fixture) : base(fixture)
    {
        _host = fixture.Host;
    }

    [Fact]
    public void RzCard_RendersContent()
    {
        var cut = RenderComponent<RzCard>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<CardHeader>(0);
                builder.AddChildContent(headerBuilder =>
                {
                    headerBuilder.OpenComponent<CardTitle>(0);
                    headerBuilder.AddChildContent("Card Title");
                    headerBuilder.CloseComponent();
                    headerBuilder.OpenComponent<CardDescription>(1);
                    headerBuilder.AddChildContent("Card Description");
                    headerBuilder.CloseComponent();
                });
                builder.CloseComponent();
                builder.OpenComponent<CardContent>(1);
                builder.AddChildContent("Card Content");
                builder.CloseComponent();
                builder.OpenComponent<CardFooter>(2);
                builder.AddChildContent(footerBuilder =>
                {
                    footerBuilder.OpenComponent<CardAction>(0);
                    footerBuilder.AddChildContent("Card Action");
                    footerBuilder.CloseComponent();
                });
                builder.CloseComponent();
            })
        );

        cut.Markup.Should().Contain("Card Content");
    }
}