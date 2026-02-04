using AwesomeAssertions;
using Alba;

namespace RizzyUI.Tests.Components.Navigation;

public class RzCommandTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    private readonly IAlbaHost _host;

    public RzCommandTests(WebAppFixture fixture) : base(fixture)
    {
        _host = fixture.Host;
    }

    [Fact]
    public void RzCommand_RendersItems()
    {
        var cut = RenderComponent<RzCommand>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<CommandInput>(0);
                builder.AddAttribute(1, "Placeholder", "Search");
                builder.CloseComponent();
                builder.OpenComponent<CommandList>(2);
                builder.AddChildContent(listBuilder =>
                {
                    listBuilder.OpenComponent<CommandEmpty>(0);
                    listBuilder.AddChildContent("No results");
                    listBuilder.CloseComponent();
                    listBuilder.OpenComponent<CommandGroup>(1);
                    listBuilder.AddAttribute(2, "Heading", "Group");
                    listBuilder.AddChildContent(groupBuilder =>
                    {
                        groupBuilder.OpenComponent<CommandItem>(0);
                        groupBuilder.AddAttribute(1, "Value", "item");
                        groupBuilder.AddChildContent("Command Item");
                        groupBuilder.CloseComponent();
                        groupBuilder.OpenComponent<CommandItemTemplate>(2);
                        groupBuilder.AddChildContent("Template Item");
                        groupBuilder.CloseComponent();
                        groupBuilder.OpenComponent<CommandShortcut>(3);
                        groupBuilder.AddChildContent("Ctrl+P");
                        groupBuilder.CloseComponent();
                        groupBuilder.OpenComponent<CommandSeparator>(4);
                        groupBuilder.CloseComponent();
                    });
                    listBuilder.CloseComponent();
                });
                builder.CloseComponent();
            })
        );

        cut.Markup.Should().Contain("Command Item");
    }

    [Fact]
    public void RzCommandDialog_RendersContent()
    {
        var cut = RenderComponent<RzCommandDialog>(parameters => parameters
            .Add(p => p.Open, true)
            .AddChildContent("Dialog content")
        );

        cut.Markup.Should().Contain("Dialog content");
    }
}