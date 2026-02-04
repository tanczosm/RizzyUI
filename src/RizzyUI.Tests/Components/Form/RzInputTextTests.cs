using AwesomeAssertions;
using Microsoft.AspNetCore.Components.Forms;
using Alba;

namespace RizzyUI.Tests.Components.Form;

public class RzInputTextTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    private readonly IAlbaHost _host;

    public RzInputTextTests(WebAppFixture fixture) : base(fixture)
    {
        _host = fixture.Host;
    }

    [Fact]
    public void RzInputText_RendersInput()
    {
        var model = new FormModel();
        var editContext = new EditContext(model);

        var cut = RenderComponent<EditForm>(parameters => parameters
            .Add(p => p.EditContext, editContext)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<RzInputText>(0);
                builder.AddAttribute(1, "For", () => model.Text);
                builder.CloseComponent();
            })
        );

        cut.Markup.Should().Contain("input");
    }

    [Fact]
    public void RzInputText_SetsPlaceholderAndRole()
    {
        var model = new FormModel();
        var editContext = new EditContext(model);

        var cut = RenderComponent<EditForm>(parameters => parameters
            .Add(p => p.EditContext, editContext)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<RzInputText>(0);
                builder.AddAttribute(1, "For", () => model.Text);
                builder.AddAttribute(2, "Placeholder", "Enter email");
                builder.AddAttribute(3, "Role", TextRole.Email);
                builder.CloseComponent();
            })
        );

        cut.Markup.Should().Contain("placeholder=\"Enter email\"");
        cut.Markup.Should().Contain("type=\"email\"");
    }

    private sealed class FormModel
    {
        public string Text { get; set; } = string.Empty;
    }
}
