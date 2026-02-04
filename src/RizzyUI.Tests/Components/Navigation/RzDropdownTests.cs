using AwesomeAssertions;
using Alba;

namespace RizzyUI.Tests.Components.Navigation;

public class RzDropdownTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    private readonly IAlbaHost _host;

    public RzDropdownTests(WebAppFixture fixture) : base(fixture)
    {
        _host = fixture.Host;
    }

    [Fact]
    public void RzDropdownMenu_RendersMenuItems()
    {
        var cut = RenderComponent<RzDropdownMenu>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<DropdownMenuTrigger>(0);
                builder.AddChildContent("Open Menu");
                builder.CloseComponent();
                builder.OpenComponent<DropdownMenuContent>(1);
                builder.AddChildContent(contentBuilder =>
                {
                    contentBuilder.OpenComponent<DropdownMenuLabel>(0);
                    contentBuilder.AddChildContent("Menu Label");
                    contentBuilder.CloseComponent();
                    contentBuilder.OpenComponent<DropdownMenuGroup>(1);
                    contentBuilder.AddChildContent(groupBuilder =>
                    {
                        groupBuilder.OpenComponent<DropdownMenuItem>(0);
                        groupBuilder.AddChildContent("Menu Item");
                        groupBuilder.CloseComponent();
                        groupBuilder.OpenComponent<DropdownMenuShortcut>(1);
                        groupBuilder.AddChildContent("Ctrl+K");
                        groupBuilder.CloseComponent();
                    });
                    contentBuilder.CloseComponent();
                    contentBuilder.OpenComponent<DropdownMenuSeparator>(2);
                    contentBuilder.CloseComponent();
                    contentBuilder.OpenComponent<DropdownMenuSub>(3);
                    contentBuilder.AddChildContent(subBuilder =>
                    {
                        subBuilder.OpenComponent<DropdownMenuSubTrigger>(0);
                        subBuilder.AddChildContent("Sub Trigger");
                        subBuilder.CloseComponent();
                        subBuilder.OpenComponent<DropdownMenuSubContent>(1);
                        subBuilder.AddChildContent("Sub Content");
                        subBuilder.CloseComponent();
                    });
                    contentBuilder.CloseComponent();
                });
                builder.CloseComponent();
            })
        );

        cut.Markup.Should().Contain("Menu Item");
    }
}