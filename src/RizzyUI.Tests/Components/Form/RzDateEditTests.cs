using AwesomeAssertions;
using Microsoft.AspNetCore.Components.Forms;
using Alba;
using Blazicons;

namespace RizzyUI.Tests.Components.Form;

public class RzDateEditTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    private readonly IAlbaHost _host;

    public RzDateEditTests(WebAppFixture fixture) : base(fixture)
    {
        _host = fixture.Host;
    }

    [Fact]
    public void RzDateEdit_RendersInput()
    {
        var model = new FormModel();
        var editContext = new EditContext(model);

        var cut = RenderComponent<EditForm>(parameters => parameters
            .Add(p => p.EditContext, editContext)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<RzDateEdit>(0);
                builder.AddAttribute(1, "For", () => model.SelectedDate);
                builder.CloseComponent();
            })
        );

        cut.Markup.Should().Contain("input");
    }

    [Fact]
    public void RzDateEdit_SetsPlaceholderAndPrependText()
    {
        var model = new FormModel();
        var editContext = new EditContext(model);

        var cut = RenderComponent<EditForm>(parameters => parameters
            .Add(p => p.EditContext, editContext)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<RzDateEdit>(0);
                builder.AddAttribute(1, "For", () => model.SelectedDate);
                builder.AddAttribute(2, "Placeholder", "Pick a date");
                builder.AddAttribute(3, "PrependText", "Date");
                builder.CloseComponent();
            })
        );

        cut.Markup.Should().Contain("placeholder=\"Pick a date\"");
        cut.Markup.Should().Contain("Date");
    }

    [Fact]
    public void RzDateEdit_PrependIcon_RendersIcon()
    {
        var model = new FormModel();
        var editContext = new EditContext(model);

        var cut = RenderComponent<EditForm>(parameters => parameters
            .Add(p => p.EditContext, editContext)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<RzDateEdit>(0);
                builder.AddAttribute(1, "For", () => model.SelectedDate);
                builder.AddAttribute(2, "PrependIcon", Ionicon.CalendarOutline);
                builder.CloseComponent();
            })
        );

        cut.Markup.Should().Contain("svg");
    }

    private sealed class FormModel
    {
        public DateTime? SelectedDate { get; set; } = DateTime.Today;
    }
}
