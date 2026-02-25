using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using RizzyUI.Charts;

namespace RizzyUI.Components.Display.RzChart;

public class ChartEnumSerializationTests
{
    [Fact]
    public void AllChartEnumsSerializeUsingEnumMemberValues()
    {
        var enumTypes = typeof(JoinStyle).Assembly
            .GetTypes()
            .Where(t => t.Namespace == "RizzyUI.Charts" && t.IsEnum)
            .ToList();

        var failures = new List<string>();

        foreach (var enumType in enumTypes)
        {
            var values = Enum.GetValues(enumType).Cast<Enum>();
            foreach (var value in values)
            {
                var name = value.ToString();
                var field = enumType.GetField(name)!;
                var enumMember = field.GetCustomAttribute<EnumMemberAttribute>();
                var expected = enumMember?.Value ?? name.ToLowerInvariant();

                var json = JsonSerializer.Serialize(value, enumType);
                var actual = JsonSerializer.Deserialize<string>(json)!;

                if (!string.Equals(expected, actual, StringComparison.Ordinal))
                    failures.Add($"{enumType.Name}.{name}: expected '{expected}' but was '{actual}'");
            }
        }

        Assert.True(failures.Count == 0, string.Join(Environment.NewLine, failures));
    }

    [Fact]
    public void AllChartEnumsSerializeUsingEnumMemberValuesWhenBoxedAsObject()
    {
        var enumTypes = typeof(JoinStyle).Assembly
            .GetTypes()
            .Where(t => t.Namespace == "RizzyUI.Charts" && t.IsEnum)
            .ToList();

        var failures = new List<string>();

        foreach (var enumType in enumTypes)
        {
            var values = Enum.GetValues(enumType).Cast<Enum>();
            foreach (var value in values)
            {
                var name = value.ToString();
                var field = enumType.GetField(name)!;
                var enumMember = field.GetCustomAttribute<EnumMemberAttribute>();
                var expected = enumMember?.Value ?? name.ToLowerInvariant();

                var json = JsonSerializer.Serialize(new BoxedEnumHolder { Value = value });
                using var doc = JsonDocument.Parse(json);
                var actual = doc.RootElement.GetProperty("Value").GetString();

                if (!string.Equals(expected, actual, StringComparison.Ordinal))
                    failures.Add($"{enumType.Name}.{name}: expected '{expected}' but was '{actual}'");
            }
        }

        Assert.True(failures.Count == 0, string.Join(Environment.NewLine, failures));
    }


    [Fact]
    public void EnumBackedBuilderMethodsSerializeExpectedStringValues()
    {
        var builder = new ChartBuilder();
        builder.Options(options => options
            .Plugins(plugins => plugins.Tooltip(tooltip => tooltip.Position(TooltipPosition.Average)))
            .Animation(animation => animation.Easing(AnimationEasing.EaseInOutBounce))
            .Interaction(interaction => interaction.Mode(InteractionMode.Index).Axis(Axis.XY)));

        var json = JsonSerializer.Serialize(builder.Chart, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        Assert.Equal("average", root.GetProperty("options").GetProperty("plugins").GetProperty("tooltip").GetProperty("position").GetString());
        Assert.Equal("easeInOutBounce", root.GetProperty("options").GetProperty("animation").GetProperty("easing").GetString());
        Assert.Equal("index", root.GetProperty("options").GetProperty("interaction").GetProperty("mode").GetString());
        Assert.Equal("xy", root.GetProperty("options").GetProperty("interaction").GetProperty("axis").GetString());
    }
    private sealed class BoxedEnumHolder
    {
        public object? Value { get; set; }
    }
}
