using AwesomeAssertions;
using Microsoft.AspNetCore.Components;
using Alba;

namespace RizzyUI.Tests.Components;

public class FragmentComponentTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    private readonly IAlbaHost _host;

    public FragmentComponentTests(WebAppFixture fixture) : base(fixture)
    {
        _host = fixture.Host;
    }

    [Fact]
    public void FragmentComponent_RendersFragment()
    {
        var cut = RenderComponent<FragmentComponent>(parameters => parameters
            .Add(p => p.Fragment, (RenderFragment)(builder => builder.AddContent(0, "Fragment Content")))
        );

        cut.Markup.Should().Contain("Fragment Content");
    }
}