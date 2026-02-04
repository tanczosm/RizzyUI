using AwesomeAssertions;
using Alba;

namespace RizzyUI.Tests.Components.Navigation;

public class RzNavigationMenuTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    private readonly IAlbaHost _host;

    public RzNavigationMenuTests(WebAppFixture fixture) : base(fixture)
    {
        _host = fixture.Host;
    }

    [Fact]
    public void RzNavigationMenu_RendersContent()
    {
        var cut = RenderComponent<RzNavigationMenu>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<NavigationMenuList>(0);
                builder.AddChildContent(listBuilder =>
                {
                    listBuilder.OpenComponent<NavigationMenuItem>(0);
                    listBuilder.AddChildContent(itemBuilder =>
                    {
                        itemBuilder.OpenComponent<NavigationMenuTrigger>(0);
                        itemBuilder.AddChildContent("Trigger");
                        itemBuilder.CloseComponent();
                        itemBuilder.OpenComponent<NavigationMenuContent>(1);
                        itemBuilder.AddChildContent("Nav Content");
                        itemBuilder.CloseComponent();
                        itemBuilder.OpenComponent<NavigationMenuLink>(2);
                        itemBuilder.AddAttribute(3, "Href", "#");
                        itemBuilder.AddChildContent("Nav Link");
                        itemBuilder.CloseComponent();
                    });
                    listBuilder.CloseComponent();
                });
                builder.CloseComponent();
            })
        );

        cut.Markup.Should().Contain("Nav Content");
    }
}