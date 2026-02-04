using AwesomeAssertions;
using Alba;

namespace RizzyUI.Tests.Components.Form;

public class RzCalendarTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    private readonly IAlbaHost _host;

    public RzCalendarTests(WebAppFixture fixture) : base(fixture)
    {
        _host = fixture.Host;
    }

    [Fact]
    public void RzCalendarProvider_RendersCalendar()
    {
        var cut = RenderComponent<RzCalendarProvider>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<RzCalendar>(0);
                builder.CloseComponent();
            })
        );

        cut.Markup.Should().Contain("data-slot=\"calendar\"");
    }

    [Fact]
    public void RzCalendar_Renders()
    {
        var cut = RenderComponent<RzCalendar>();

        cut.Markup.Should().Contain("data-slot=\"calendar\"");
    }
}