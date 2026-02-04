using AwesomeAssertions;
using Alba;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace RizzyUI.Tests.Components.Display;

public class RzAvatarTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    private readonly IAlbaHost _host;

    public RzAvatarTests(WebAppFixture fixture) : base(fixture)
    {
        _host = fixture.Host;
    }

    [Fact]
    public void RzAvatar_DefaultRender_ShowsCorrectStructureAndAriaLabel()
    {
        var expectedId = "default-avatar";
        var expectedDefaultAriaLabel = Services.GetRequiredService<IStringLocalizer<RizzyLocalization>>()["RzAvatar.DefaultAriaLabel"].Value;

        var cut = RenderComponent<RzAvatar>(parameters => parameters
            .Add(p => p.Id, expectedId)
        );

        var avatarContainer = cut.Find($"div#{expectedId}");
        avatarContainer.GetAttribute("role").Should().Be("img");
        avatarContainer.GetAttribute("aria-label").Should().Be(expectedDefaultAriaLabel);
    }

    [Theory]
    [InlineData(AvatarShape.Circle, "rounded-full")]
    [InlineData(AvatarShape.Square, "rounded-lg")]
    public void RzAvatar_ShapeParameter_AppliesCorrectShapeClass(AvatarShape shape, string expectedClass)
    {
        var expectedId = $"avatar-shape-{shape.ToString().ToLowerInvariant()}";

        var cut = RenderComponent<RzAvatar>(parameters => parameters
            .Add(p => p.Id, expectedId)
            .Add(p => p.Shape, shape)
        );

        var avatarContainer = cut.Find($"div#{expectedId}");
        avatarContainer.ClassList.Should().Contain(expectedClass);
    }

    [Theory]
    [InlineData(Size.ExtraSmall, "size-6")]
    [InlineData(Size.Small, "size-8")]
    [InlineData(Size.Medium, "size-10")]
    [InlineData(Size.Large, "size-12")]
    [InlineData(Size.ExtraLarge, "size-16")]
    public void RzAvatar_SizeParameter_AppliesCorrectSizeClassToContainer(Size size, string expectedClass)
    {
        var expectedId = $"avatar-size-{size.ToString().ToLowerInvariant()}";

        var cut = RenderComponent<RzAvatar>(parameters => parameters
            .Add(p => p.Id, expectedId)
            .Add(p => p.Size, size)
        );

        var avatarContainer = cut.Find($"div#{expectedId}");
        avatarContainer.ClassList.Should().Contain(expectedClass);
    }

    [Fact]
    public void RzAvatar_BorderParameterTrue_AppliesBorderClass()
    {
        var expectedId = "avatar-border";

        var cut = RenderComponent<RzAvatar>(parameters => parameters
            .Add(p => p.Id, expectedId)
            .Add(p => p.Border, true)
        );

        var avatarContainer = cut.Find($"div#{expectedId}");
        avatarContainer.ClassList.Should().Contain("border-2");
        avatarContainer.ClassList.Should().Contain("ring-2");
    }

    [Fact]
    public void RzAvatar_CustomAriaLabel_OverridesDefault()
    {
        var expectedId = "avatar-custom-aria";
        var customLabel = "Profile picture of Jane Doe";

        var cut = RenderComponent<RzAvatar>(parameters => parameters
            .Add(p => p.Id, expectedId)
            .Add(p => p.AriaLabel, customLabel)
        );

        var avatarContainer = cut.Find($"div#{expectedId}");
        avatarContainer.GetAttribute("aria-label").Should().Be(customLabel);
    }

    [Fact]
    public void AvatarImage_WithValidSource_RendersImageAndSetsParentHasImageTrue()
    {
        var imageUrl = "/images/profile/test.jpg";
        var altText = "Test User";

        var cut = RenderComponent<RzAvatar>(parameters => parameters
            .Add(p => p.AriaLabel, "Parent Container")
            .AddChildContent<AvatarImage>(imageParams => imageParams
                .Add(p => p.ImageSource, imageUrl)
                .Add(p => p.AlternateText, altText)
            )
        );

        var img = cut.Find("img");
        img.GetAttribute("src").Should().Be(imageUrl);
        img.GetAttribute("alt").Should().Be(altText);
        cut.Instance.HasImage.Should().BeTrue();
    }

    [Fact]
    public void AvatarFallback_RendersWhenNoImageIsPresent()
    {
        var cut = RenderComponent<RzAvatar>(parameters => parameters
            .AddChildContent<AvatarImage>(imageParams => imageParams.Add(p => p.ImageSource, null))
            .AddChildContent<AvatarFallback>(fallbackParams => fallbackParams.AddChildContent("JD"))
        );

        cut.Markup.Should().Contain("JD");
        cut.FindAll("img").Should().BeEmpty();
    }
}
