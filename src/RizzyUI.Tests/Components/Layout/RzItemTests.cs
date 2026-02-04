using AwesomeAssertions;
using Alba;

namespace RizzyUI.Tests.Components.Layout;

public class RzItemTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    private readonly IAlbaHost _host;

    public RzItemTests(WebAppFixture fixture) : base(fixture)
    {
        _host = fixture.Host;
    }

    [Fact]
    public void RzItemGroup_RendersItems()
    {
        var cut = RenderComponent<RzItemGroup>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<RzItem>(0);
                builder.AddChildContent(itemBuilder =>
                {
                    itemBuilder.OpenComponent<ItemHeader>(0);
                    itemBuilder.AddChildContent(headerBuilder =>
                    {
                        headerBuilder.OpenComponent<ItemTitle>(0);
                        headerBuilder.AddChildContent("Item Title");
                        headerBuilder.CloseComponent();
                        headerBuilder.OpenComponent<ItemDescription>(1);
                        headerBuilder.AddChildContent("Item Description");
                        headerBuilder.CloseComponent();
                    });
                    itemBuilder.CloseComponent();
                    itemBuilder.OpenComponent<ItemContent>(1);
                    itemBuilder.AddChildContent("Item Content");
                    itemBuilder.CloseComponent();
                    itemBuilder.OpenComponent<ItemMedia>(2);
                    itemBuilder.AddChildContent("Item Media");
                    itemBuilder.CloseComponent();
                    itemBuilder.OpenComponent<ItemActions>(3);
                    itemBuilder.AddChildContent("Item Actions");
                    itemBuilder.CloseComponent();
                    itemBuilder.OpenComponent<ItemSeparator>(4);
                    itemBuilder.CloseComponent();
                    itemBuilder.OpenComponent<ItemFooter>(5);
                    itemBuilder.AddChildContent("Item Footer");
                    itemBuilder.CloseComponent();
                });
                builder.CloseComponent();
            })
        );

        cut.Markup.Should().Contain("Item Title");
    }
}