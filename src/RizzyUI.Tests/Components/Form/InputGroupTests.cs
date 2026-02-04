using AwesomeAssertions;
using Alba;

namespace RizzyUI.Tests.Components.Form;

public class InputGroupTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    private readonly IAlbaHost _host;

    public InputGroupTests(WebAppFixture fixture) : base(fixture)
    {
        _host = fixture.Host;
    }

    [Fact]
    public void RzInputGroup_RendersAddonAndInput()
    {
        var model = new FormModel();

        var cut = RenderComponent<RzInputGroup>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<InputGroupAddon>(0);
                builder.AddChildContent("@");
                builder.CloseComponent();
                builder.OpenComponent<InputGroupInput>(1);
                builder.AddAttribute(2, "For", () => model.Text);
                builder.CloseComponent();
                builder.OpenComponent<InputGroupButton>(3);
                builder.AddChildContent("Go");
                builder.CloseComponent();
                builder.OpenComponent<InputGroupText>(4);
                builder.AddChildContent("Text");
                builder.CloseComponent();
                builder.OpenComponent<InputGroupTextarea>(5);
                builder.AddAttribute(6, "For", () => model.Text);
                builder.CloseComponent();
            })
        );

        cut.Markup.Should().Contain("@");
        cut.Markup.Should().Contain("Go");
    }

    private sealed class FormModel
    {
        public string Text { get; set; } = string.Empty;
    }
}