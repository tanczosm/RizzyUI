using AwesomeAssertions;
using Alba;

namespace RizzyUI.Tests.Components.Layout;

public class RzStepTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    private readonly IAlbaHost _host;

    public RzStepTests(WebAppFixture fixture) : base(fixture)
    {
        _host = fixture.Host;
    }

    [Fact]
    public void RzSteps_RendersStepLabels()
    {
        var cut = RenderComponent<RzSteps>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<RzStep>(0);
                builder.AddAttribute(1, "Label", "Step 1");
                builder.AddAttribute(2, "Status", StepStatus.Current);
                builder.CloseComponent();
            })
        );

        cut.Markup.Should().Contain("Step 1");
    }
}