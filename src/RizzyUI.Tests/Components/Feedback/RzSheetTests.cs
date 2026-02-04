using AwesomeAssertions;
using Alba;

namespace RizzyUI.Tests.Components.Feedback;

public class RzSheetTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    private readonly IAlbaHost _host;

    public RzSheetTests(WebAppFixture fixture) : base(fixture)
    {
        _host = fixture.Host;
    }

    [Fact]
    public void RzSheet_RendersSheetContent()
    {
        var cut = RenderComponent<RzSheet>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<SheetTrigger>(0);
                builder.AddChildContent("Open Sheet");
                builder.CloseComponent();
                builder.OpenComponent<SheetContent>(1);
                builder.AddChildContent(contentBuilder =>
                {
                    contentBuilder.OpenComponent<SheetHeader>(0);
                    contentBuilder.AddChildContent(headerBuilder =>
                    {
                        headerBuilder.OpenComponent<SheetTitle>(0);
                        headerBuilder.AddChildContent("Sheet Title");
                        headerBuilder.CloseComponent();
                        headerBuilder.OpenComponent<SheetDescription>(1);
                        headerBuilder.AddChildContent("Sheet Description");
                        headerBuilder.CloseComponent();
                    });
                    contentBuilder.CloseComponent();
                    contentBuilder.OpenComponent<SheetFooter>(1);
                    contentBuilder.AddChildContent("Sheet Footer");
                    contentBuilder.CloseComponent();
                    contentBuilder.OpenComponent<SheetClose>(2);
                    contentBuilder.AddChildContent("Close");
                    contentBuilder.CloseComponent();
                });
                builder.CloseComponent();
            })
        );

        cut.Markup.Should().Contain("Sheet Title");
        cut.Markup.Should().Contain("Sheet Description");
    }
}