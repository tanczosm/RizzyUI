using AwesomeAssertions;
using Microsoft.AspNetCore.Components.Forms;
using Alba;

namespace RizzyUI.Tests.Components.Form;

public class RzInputTextAreaTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    private readonly IAlbaHost _host;

    public RzInputTextAreaTests(WebAppFixture fixture) : base(fixture)
    {
        _host = fixture.Host;
    }

    [Fact]
    public void RzInputTextArea_RendersTextArea()
    {
        var model = new FormModel();
        var editContext = new EditContext(model);

        var cut = RenderComponent<EditForm>(parameters => parameters
            .Add(p => p.EditContext, editContext)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<RzInputTextArea>(0);
                builder.AddAttribute(1, "For", () => model.Text);
                builder.CloseComponent();
            })
        );

        cut.Markup.Should().Contain("textarea");
    }

    [Fact]
    public void RzInputTextArea_SetsPlaceholder()
    {
        var model = new FormModel();
        var editContext = new EditContext(model);

        var cut = RenderComponent<EditForm>(parameters => parameters
            .Add(p => p.EditContext, editContext)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<RzInputTextArea>(0);
                builder.AddAttribute(1, "For", () => model.Text);
                builder.AddAttribute(2, "Placeholder", "Notes");
                builder.CloseComponent();
            })
        );

        cut.Markup.Should().Contain("placeholder=\"Notes\"");
    }

    private sealed class FormModel
    {
        public string Text { get; set; } = string.Empty;
    }
}
