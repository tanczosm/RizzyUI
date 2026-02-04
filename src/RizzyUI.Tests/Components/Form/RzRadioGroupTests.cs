using AwesomeAssertions;
using Microsoft.AspNetCore.Components.Forms;
using Alba;

namespace RizzyUI.Tests.Components.Form;

public class RzRadioGroupTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    private readonly IAlbaHost _host;

    public RzRadioGroupTests(WebAppFixture fixture) : base(fixture)
    {
        _host = fixture.Host;
    }

    [Fact]
    public void RzRadioGroup_RendersItems()
    {
        var model = new FormModel();
        var editContext = new EditContext(model);

        var cut = RenderComponent<EditForm>(parameters => parameters
            .Add(p => p.EditContext, editContext)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<RzRadioGroup<string>>(0);
                builder.AddAttribute(1, "For", () => model.SelectedOption);
                builder.AddAttribute(2, "DisplayName", "Choices");
                builder.AddChildContent(radioBuilder =>
                {
                    radioBuilder.OpenComponent<RadioGroupItem<string>>(0);
                    radioBuilder.AddAttribute(1, "Value", "A");
                    radioBuilder.AddChildContent(itemBuilder =>
                    {
                        itemBuilder.AddContent(0, "Option A");
                        itemBuilder.OpenComponent<RadioGroupItemIndicator>(1);
                        itemBuilder.CloseComponent();
                    });
                    radioBuilder.CloseComponent();
                });
                builder.CloseComponent();
            })
        );

        cut.Markup.Should().Contain("Option A");
    }

    [Fact]
    public void RzRadioGroup_HidesIndicators_WhenDisabled()
    {
        var model = new FormModel();
        var editContext = new EditContext(model);

        var cut = RenderComponent<EditForm>(parameters => parameters
            .Add(p => p.EditContext, editContext)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<RzRadioGroup<string>>(0);
                builder.AddAttribute(1, "For", () => model.SelectedOption);
                builder.AddAttribute(2, "DisplayName", "Choices");
                builder.AddAttribute(3, "ShowIndicators", false);
                builder.AddChildContent(radioBuilder =>
                {
                    radioBuilder.OpenComponent<RadioGroupItem<string>>(0);
                    radioBuilder.AddAttribute(1, "Value", "A");
                    radioBuilder.AddChildContent("Option A");
                    radioBuilder.CloseComponent();
                });
                builder.CloseComponent();
            })
        );

        cut.Markup.Should().NotContain("indicator-wrapper");
    }

    private sealed class FormModel
    {
        public string SelectedOption { get; set; } = "A";
    }
}
