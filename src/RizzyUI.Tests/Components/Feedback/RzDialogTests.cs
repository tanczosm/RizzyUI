using AwesomeAssertions;
using Alba;

namespace RizzyUI.Tests.Components.Feedback;

public class RzDialogTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    private readonly IAlbaHost _host;

    public RzDialogTests(WebAppFixture fixture) : base(fixture)
    {
        _host = fixture.Host;
    }

    [Fact]
    public void RzDialog_RendersDialogContent()
    {
        var cut = RenderComponent<RzDialog>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<DialogTrigger>(0);
                builder.AddChildContent("Open Dialog");
                builder.CloseComponent();
                builder.OpenComponent<DialogContent>(1);
                builder.AddChildContent(contentBuilder =>
                {
                    contentBuilder.OpenComponent<DialogHeader>(0);
                    contentBuilder.AddChildContent(headerBuilder =>
                    {
                        headerBuilder.OpenComponent<DialogTitle>(0);
                        headerBuilder.AddChildContent("Dialog Title");
                        headerBuilder.CloseComponent();
                        headerBuilder.OpenComponent<DialogDescription>(1);
                        headerBuilder.AddChildContent("Dialog Description");
                        headerBuilder.CloseComponent();
                    });
                    contentBuilder.CloseComponent();
                    contentBuilder.OpenComponent<DialogFooter>(1);
                    contentBuilder.AddChildContent("Dialog Footer");
                    contentBuilder.CloseComponent();
                    contentBuilder.OpenComponent<DialogClose>(2);
                    contentBuilder.AddChildContent("Close");
                    contentBuilder.CloseComponent();
                });
                builder.CloseComponent();
            })
        );

        cut.Markup.Should().Contain("Dialog Title");
        cut.Markup.Should().Contain("Dialog Description");
    }
}