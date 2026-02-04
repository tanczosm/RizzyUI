using AwesomeAssertions;
using Microsoft.AspNetCore.Components.Forms;
using Alba;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace RizzyUI.Tests.Components.Form;

public class RzSwitchTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    private readonly IAlbaHost _host;

    public RzSwitchTests(WebAppFixture fixture) : base(fixture)
    {
        _host = fixture.Host;
    }

    [Fact]
    public void RzSwitch_RendersSwitch()
    {
        var model = new FormModel();
        var editContext = new EditContext(model);

        var cut = RenderComponent<EditForm>(parameters => parameters
            .Add(p => p.EditContext, editContext)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<RzSwitch>(0);
                builder.AddAttribute(1, "For", () => model.IsEnabled);
                builder.CloseComponent();
            })
        );

        cut.Markup.Should().Contain("role=\"switch\"");
    }

    [Fact]
    public void RzSwitch_DefaultAriaLabel_IsLocalized()
    {
        var model = new FormModel();
        var editContext = new EditContext(model);
        var expectedLabel = Services.GetRequiredService<IStringLocalizer<RizzyLocalization>>()["RzSwitch.DefaultAriaLabel"].Value;

        var cut = RenderComponent<EditForm>(parameters => parameters
            .Add(p => p.EditContext, editContext)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<RzSwitch>(0);
                builder.AddAttribute(1, "For", () => model.IsEnabled);
                builder.CloseComponent();
            })
        );

        cut.Markup.Should().Contain($"aria-label=\"{expectedLabel}\"");
    }

    [Fact]
    public void RzSwitch_CustomAriaLabel_OverridesDefault()
    {
        var model = new FormModel();
        var editContext = new EditContext(model);

        var cut = RenderComponent<EditForm>(parameters => parameters
            .Add(p => p.EditContext, editContext)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<RzSwitch>(0);
                builder.AddAttribute(1, "For", () => model.IsEnabled);
                builder.AddAttribute(2, "AriaLabel", "Toggle setting");
                builder.CloseComponent();
            })
        );

        cut.Markup.Should().Contain("aria-label=\"Toggle setting\"");
    }

    private sealed class FormModel
    {
        public bool IsEnabled { get; set; }
    }
}
