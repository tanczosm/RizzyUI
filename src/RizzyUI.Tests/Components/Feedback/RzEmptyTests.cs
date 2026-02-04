using AwesomeAssertions;
using Blazicons;
using Alba;

namespace RizzyUI.Tests.Components.Feedback;

public class RzEmptyTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    private readonly IAlbaHost _host;

    public RzEmptyTests(WebAppFixture fixture) : base(fixture)
    {
        _host = fixture.Host;
    }

    [Fact]
    public void RzEmpty_RendersEmptyState()
    {
        var cut = RenderComponent<RzEmpty>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenComponent<EmptyHeader>(0);
                builder.AddChildContent("Empty Header");
                builder.CloseComponent();
                builder.OpenComponent<EmptyMedia>(1);
                builder.AddAttribute(2, "Icon", Ionicon.SearchOutline);
                builder.CloseComponent();
                builder.OpenComponent<EmptyContent>(3);
                builder.AddChildContent("Empty Content");
                builder.CloseComponent();
                builder.OpenComponent<EmptyTitle>(4);
                builder.AddChildContent("Empty Title");
                builder.CloseComponent();
                builder.OpenComponent<EmptyDescription>(5);
                builder.AddChildContent("Empty Description");
                builder.CloseComponent();
            })
        );

        cut.Markup.Should().Contain("Empty Title");
        cut.Markup.Should().Contain("Empty Description");
    }
}