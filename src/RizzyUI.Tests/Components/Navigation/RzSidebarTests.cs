using AwesomeAssertions;
using Alba;

namespace RizzyUI.Tests.Components.Navigation;

public class RzSidebarTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    private readonly IAlbaHost _host;

    public RzSidebarTests(WebAppFixture fixture) : base(fixture)
    {
        _host = fixture.Host;
    }

    [Fact]
    public void RzSidebarProvider_RendersSidebar()
    {
        var cut = RenderComponent<RzSidebarProvider>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<Sidebar>(0);
                builder.AddChildContent(sidebarBuilder =>
                {
                    sidebarBuilder.OpenComponent<SidebarHeader>(0);
                    sidebarBuilder.AddChildContent("Sidebar Header");
                    sidebarBuilder.CloseComponent();
                    sidebarBuilder.OpenComponent<SidebarContent>(1);
                    sidebarBuilder.AddChildContent(contentBuilder =>
                    {
                        contentBuilder.OpenComponent<SidebarGroup>(0);
                        contentBuilder.AddChildContent(groupBuilder =>
                        {
                            groupBuilder.OpenComponent<SidebarGroupLabel>(0);
                            groupBuilder.AddChildContent("Group Label");
                            groupBuilder.CloseComponent();
                            groupBuilder.OpenComponent<SidebarGroupContent>(1);
                            groupBuilder.AddChildContent(groupContentBuilder =>
                            {
                                groupContentBuilder.OpenComponent<SidebarMenu>(0);
                                groupContentBuilder.AddChildContent(menuBuilder =>
                                {
                                    menuBuilder.OpenComponent<SidebarMenuItem>(0);
                                    menuBuilder.AddChildContent(itemBuilder =>
                                    {
                                        itemBuilder.OpenComponent<SidebarMenuButton>(0);
                                        itemBuilder.AddChildContent("Menu Button");
                                        itemBuilder.CloseComponent();
                                        itemBuilder.OpenComponent<SidebarMenuAction>(1);
                                        itemBuilder.AddChildContent("Action");
                                        itemBuilder.CloseComponent();
                                        itemBuilder.OpenComponent<SidebarMenuBadge>(2);
                                        itemBuilder.AddChildContent("Badge");
                                        itemBuilder.CloseComponent();
                                        itemBuilder.OpenComponent<SidebarMenuSub>(3);
                                        itemBuilder.AddChildContent(subBuilder =>
                                        {
                                            subBuilder.OpenComponent<SidebarMenuItem>(0);
                                            subBuilder.AddChildContent("Sub Item");
                                            subBuilder.CloseComponent();
                                        });
                                        itemBuilder.CloseComponent();
                                    });
                                    menuBuilder.CloseComponent();
                                });
                                groupContentBuilder.CloseComponent();
                            });
                            groupBuilder.CloseComponent();
                        });
                        contentBuilder.CloseComponent();
                    });
                    sidebarBuilder.CloseComponent();
                    sidebarBuilder.OpenComponent<SidebarFooter>(2);
                    sidebarBuilder.AddChildContent("Sidebar Footer");
                    sidebarBuilder.CloseComponent();
                });
                builder.CloseComponent();
                builder.OpenComponent<SidebarRail>(1);
                builder.CloseComponent();
                builder.OpenComponent<SidebarTrigger>(2);
                builder.AddChildContent("Trigger");
                builder.CloseComponent();
                builder.OpenComponent<SidebarInset>(3);
                builder.AddChildContent("Inset Content");
                builder.CloseComponent();
                builder.OpenComponent<SidebarSeparator>(4);
                builder.CloseComponent();
            })
        );

        cut.Markup.Should().Contain("Menu Button");
        cut.Markup.Should().Contain("Sidebar Footer");
    }
}