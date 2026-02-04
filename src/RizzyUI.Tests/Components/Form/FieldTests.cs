using AwesomeAssertions;
using Alba;

namespace RizzyUI.Tests.Components.Form;

public class FieldTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    private readonly IAlbaHost _host;

    public FieldTests(WebAppFixture fixture) : base(fixture)
    {
        _host = fixture.Host;
    }

    [Fact]
    public void FieldComponents_Render()
    {
        var cut = RenderComponent<RzFieldSet>(parameters => parameters
            .AddChildContent(fieldSetBuilder =>
            {
                fieldSetBuilder.OpenComponent<Field>(0);
                fieldSetBuilder.AddChildContent(fieldBuilder =>
                {
                    fieldBuilder.OpenComponent<FieldLegend>(0);
                    fieldBuilder.AddChildContent("Legend");
                    fieldBuilder.CloseComponent();
                    fieldBuilder.OpenComponent<FieldTitle>(1);
                    fieldBuilder.AddChildContent("Title");
                    fieldBuilder.CloseComponent();
                    fieldBuilder.OpenComponent<FieldLabel<string>>(2);
                    fieldBuilder.AddChildContent("Field Label");
                    fieldBuilder.CloseComponent();
                    fieldBuilder.OpenComponent<FieldContent>(3);
                    fieldBuilder.AddChildContent("Field Content");
                    fieldBuilder.CloseComponent();
                    fieldBuilder.OpenComponent<FieldDescription>(4);
                    fieldBuilder.AddChildContent("Field Description");
                    fieldBuilder.CloseComponent();
                    fieldBuilder.OpenComponent<FieldError>(5);
                    fieldBuilder.AddChildContent("Field Error");
                    fieldBuilder.CloseComponent();
                    fieldBuilder.OpenComponent<FieldSeparator>(6);
                    fieldBuilder.CloseComponent();
                });
                fieldSetBuilder.CloseComponent();
                fieldSetBuilder.OpenComponent<RzFieldGroup>(7);
                fieldSetBuilder.AddChildContent("Field Group");
                fieldSetBuilder.CloseComponent();
            })
        );

        cut.Markup.Should().Contain("Field Label");
        cut.Markup.Should().Contain("Field Description");
    }
}