using AwesomeAssertions;
using Microsoft.AspNetCore.Components.Forms;
using Alba;

namespace RizzyUI.Tests.Components.Form;

public class RzNativeSelectTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    private readonly IAlbaHost _host;

    public RzNativeSelectTests(WebAppFixture fixture) : base(fixture)
    {
        _host = fixture.Host;
    }

    [Fact]
    public void RzNativeSelect_RendersOptions()
    {
        var model = new FormModel();
        var editContext = new EditContext(model);

        var cut = RenderComponent<EditForm>(parameters => parameters
            .Add(p => p.EditContext, editContext)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<RzNativeSelect<string>>(0);
                builder.AddAttribute(1, "For", () => model.Selected);
                builder.AddChildContent(selectBuilder =>
                {
                    selectBuilder.OpenComponent<RzNativeSelectOption>(0);
                    selectBuilder.AddAttribute(1, "Value", "Option");
                    selectBuilder.AddChildContent("Option");
                    selectBuilder.CloseComponent();
                    selectBuilder.OpenComponent<RzNativeSelectOptGroup>(2);
                    selectBuilder.AddAttribute(3, "Label", "Group");
                    selectBuilder.AddChildContent(groupBuilder =>
                    {
                        groupBuilder.OpenComponent<RzNativeSelectOption>(0);
                        groupBuilder.AddAttribute(1, "Value", "Grouped");
                        groupBuilder.AddChildContent("Grouped Option");
                        groupBuilder.CloseComponent();
                    });
                    selectBuilder.CloseComponent();
                });
                builder.CloseComponent();
            })
        );

        cut.Markup.Should().Contain("Grouped Option");
    }

    private sealed class FormModel
    {
        public string Selected { get; set; } = string.Empty;
    }
}