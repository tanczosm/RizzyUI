using AwesomeAssertions;
using Microsoft.AspNetCore.Components.Forms;
using Alba;

namespace RizzyUI.Tests.Components.Form;

public class RzInputNumberTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    private readonly IAlbaHost _host;

    public RzInputNumberTests(WebAppFixture fixture) : base(fixture)
    {
        _host = fixture.Host;
    }

    [Fact]
    public void RzInputNumber_RendersInput()
    {
        var model = new FormModel();
        var editContext = new EditContext(model);

        var cut = RenderComponent<EditForm>(parameters => parameters
            .Add(p => p.EditContext, editContext)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<RzInputNumber<int?>>(0);
                builder.AddAttribute(1, "For", () => model.Number);
                builder.CloseComponent();
            })
        );

        cut.Markup.Should().Contain("input");
    }

    [Fact]
    public void RzInputNumber_SetsPlaceholder()
    {
        var model = new FormModel();
        var editContext = new EditContext(model);

        var cut = RenderComponent<EditForm>(parameters => parameters
            .Add(p => p.EditContext, editContext)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<RzInputNumber<int?>>(0);
                builder.AddAttribute(1, "For", () => model.Number);
                builder.AddAttribute(2, "Placeholder", "0");
                builder.CloseComponent();
            })
        );

        cut.Markup.Should().Contain("placeholder=\"0\"");
    }

    private sealed class FormModel
    {
        public int? Number { get; set; } = 42;
    }
}
