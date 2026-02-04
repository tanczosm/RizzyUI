using AwesomeAssertions;
using Microsoft.AspNetCore.Components.Forms;
using Alba;

namespace RizzyUI.Tests.Components.Form;

public class RzComboboxTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    private readonly IAlbaHost _host;

    public RzComboboxTests(WebAppFixture fixture) : base(fixture)
    {
        _host = fixture.Host;
    }

    [Fact]
    public void RzCombobox_Renders()
    {
        var model = new FormModel();
        var editContext = new EditContext(model);

        var cut = RenderComponent<EditForm>(parameters => parameters
            .Add(p => p.EditContext, editContext)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<RzCombobox<string>>(0);
                builder.AddAttribute(1, "For", () => model.Selection);
                builder.AddAttribute(2, "Options", new ComboboxOptions());
                builder.CloseComponent();
            })
        );

        cut.Markup.Should().Contain("data-config-id");
    }

    [Fact]
    public void RzCombobox_PlaceholderAndMultiple_AppliesAttributes()
    {
        var model = new FormModel();
        var editContext = new EditContext(model);

        var cut = RenderComponent<EditForm>(parameters => parameters
            .Add(p => p.EditContext, editContext)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<RzCombobox<string>>(0);
                builder.AddAttribute(1, "For", () => model.Selection);
                builder.AddAttribute(2, "Options", new ComboboxOptions());
                builder.AddAttribute(3, "Placeholder", "Choose");
                builder.AddAttribute(4, "Multiple", true);
                builder.CloseComponent();
            })
        );

        cut.Markup.Should().Contain("multiple");
        cut.Markup.Should().Contain("Choose");
    }

    [Fact]
    public void RzCombobox_Disabled_AppliesAttribute()
    {
        var model = new FormModel();
        var editContext = new EditContext(model);

        var cut = RenderComponent<EditForm>(parameters => parameters
            .Add(p => p.EditContext, editContext)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<RzCombobox<string>>(0);
                builder.AddAttribute(1, "For", () => model.Selection);
                builder.AddAttribute(2, "Options", new ComboboxOptions());
                builder.AddAttribute(3, "Disabled", true);
                builder.CloseComponent();
            })
        );

        cut.Markup.Should().Contain("disabled");
    }

    private sealed class FormModel
    {
        public string Selection { get; set; } = string.Empty;
    }
}
