using AwesomeAssertions;
using Microsoft.AspNetCore.Components.Forms;
using Alba;

namespace RizzyUI.Tests.Components.Form;

public class RzCheckboxGroupTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    private readonly IAlbaHost _host;

    public RzCheckboxGroupTests(WebAppFixture fixture) : base(fixture)
    {
        _host = fixture.Host;
    }

    [Fact]
    public void RzCheckboxComponents_Render()
    {
        var model = new FormModel();
        var editContext = new EditContext(model);

        var cut = RenderComponent<EditForm>(parameters => parameters
            .Add(p => p.EditContext, editContext)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<RzInputCheckbox>(0);
                builder.AddAttribute(1, "For", () => model.IsEnabled);
                builder.CloseComponent();

                builder.OpenComponent<RzCheckboxGroup<string>>(2);
                builder.AddAttribute(3, "For", () => model.SelectedOptions);
                builder.AddAttribute(4, "DisplayName", "Checkboxes");
                builder.AddChildContent(groupBuilder =>
                {
                    groupBuilder.OpenComponent<RzCheckboxGroupItem<string>>(0);
                    groupBuilder.AddAttribute(1, "Value", "Item");
                    groupBuilder.AddChildContent(itemBuilder =>
                    {
                        itemBuilder.AddContent(0, "Item");
                        itemBuilder.OpenComponent<CheckboxGroupItemIndicator>(1);
                        itemBuilder.CloseComponent();
                    });
                    groupBuilder.CloseComponent();
                });
                builder.CloseComponent();
            })
        );

        cut.Markup.Should().Contain("Item");
    }

    private sealed class FormModel
    {
        public bool IsEnabled { get; set; }
        public IList<string> SelectedOptions { get; set; } = new List<string>();
    }
}