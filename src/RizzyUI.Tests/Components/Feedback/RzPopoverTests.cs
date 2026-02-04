using AwesomeAssertions;
using Alba;

namespace RizzyUI.Tests.Components.Feedback;

public class RzPopoverTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    private readonly IAlbaHost _host;

    public RzPopoverTests(WebAppFixture fixture) : base(fixture)
    {
        _host = fixture.Host;
    }

    [Fact]
    public void RzPopover_RendersContent()
    {
        var cut = RenderComponent<RzPopover>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<PopoverTrigger>(0);
                builder.AddChildContent("Open Popover");
                builder.CloseComponent();
                builder.OpenComponent<PopoverContent>(1);
                builder.AddChildContent("Popover Content");
                builder.CloseComponent();
            })
        );

        cut.Markup.Should().Contain("Popover Content");
    }
}