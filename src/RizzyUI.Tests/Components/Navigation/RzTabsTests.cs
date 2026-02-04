using AwesomeAssertions;
using Alba;

namespace RizzyUI.Tests.Components.Navigation;

public class RzTabsTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    private readonly IAlbaHost _host;

    public RzTabsTests(WebAppFixture fixture) : base(fixture)
    {
        _host = fixture.Host;
    }

    [Fact]
    public void RzTabs_RendersTabContent()
    {
        var cut = RenderComponent<RzTabs>(parameters => parameters
            .Add(p => p.DefaultValue, "tab1")
            .AddChildContent(builder =>
            {
                builder.OpenComponent<TabsList>(0);
                builder.AddChildContent(listBuilder =>
                {
                    listBuilder.OpenComponent<TabsTrigger>(0);
                    listBuilder.AddAttribute(1, "Value", "tab1");
                    listBuilder.AddChildContent("Tab 1");
                    listBuilder.CloseComponent();
                });
                builder.CloseComponent();
                builder.OpenComponent<TabsContent>(1);
                builder.AddAttribute(2, "Value", "tab1");
                builder.AddChildContent("Tab Content");
                builder.CloseComponent();
            })
        );

        cut.Markup.Should().Contain("Tab Content");
    }
}