using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Bunit;
using System.Linq.Expressions;
using System.Text.Json;

namespace RizzyUI.Tests.Components.DataTable.RzDataTable;

public class RzDataTableTests : BunitAlbaContext, IClassFixture<WebAppFixture>
{
    public RzDataTableTests(WebAppFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public void Render_EmitsRootAndAlpineHostContract()
    {
        var cut = Render<TestHost>(parameters => parameters
            .Add(p => p.LoadStrategy, "eager"));

        var root = cut.Find("[data-slot='datatable']");
        Assert.NotNull(root);

        var alpineHost = cut.Find("[data-alpine-root='test-datatable']");
        Assert.Equal("rzDataTable", alpineHost.GetAttribute("x-data"));
        Assert.Equal("eager", alpineHost.GetAttribute("x-load"));
        Assert.NotNull(alpineHost.GetAttribute("data-assets"));
        Assert.NotNull(alpineHost.GetAttribute("data-nonce"));
        Assert.NotNull(alpineHost.GetAttribute("data-config-id"));
    }

    [Fact]
    public void Render_EmitsJsonScriptBlock_NotDataAttributesPayload()
    {
        var cut = Render<TestHost>();

        var script = cut.Find("script[type='application/json']");
        Assert.NotNull(script);
        Assert.Contains("\"columns\"", script.TextContent);
        Assert.Contains("\"data\"", script.TextContent);

        var alpineHost = cut.Find("[data-alpine-root='test-datatable']");
        Assert.DoesNotContain("{", alpineHost.GetAttribute("data-config-id"));
        Assert.DoesNotContain("\"columns\"", alpineHost.GetAttribute("data-assets") ?? string.Empty);
    }

    [Fact]
    public void MissingItems_Throws()
    {
        Assert.Throws<InvalidOperationException>(() =>
            Render<RzDataTable<TestRow>>(ps => ps
                .Add(p => p.Items, null!)
                .Add(p => p.Config, BuildConfig())
                .Add(p => p.RowIdSelector, x => x.Id)));
    }

    [Fact]
    public void MissingConfig_Throws()
    {
        Assert.Throws<InvalidOperationException>(() =>
            Render<RzDataTable<TestRow>>(ps => ps
                .Add(p => p.Items, BuildRows())
                .Add(p => p.Config, null!)
                .Add(p => p.RowIdSelector, x => x.Id)));
    }

    [Fact]
    public void MissingRowIdSelector_Throws()
    {
        Assert.Throws<InvalidOperationException>(() =>
            Render<RzDataTable<TestRow>>(ps => ps
                .Add(p => p.Items, BuildRows())
                .Add(p => p.Config, BuildConfig())
                .Add<Expression<Func<TestRow, object?>>>(p => p.RowIdSelector, null!)));
    }

    [Fact]
    public void InvalidRowIdSelector_Throws()
    {
        Assert.Throws<InvalidOperationException>(() =>
            Render<RzDataTable<TestRow>>(ps => ps
                .Add(p => p.Items, BuildRows())
                .Add(p => p.Config, BuildConfig())
                .Add(p => p.RowIdSelector, x => x.Id.ToString())));
    }

    [Fact]
    public void DuplicateColumnIds_Throws()
    {
        var config = new RzDataTableConfig<TestRow>
        {
            Columns =
            [
                new() { Id = "name", Accessor = x => x.Name },
                new() { Id = "name", Accessor = x => x.Id }
            ]
        };

        Assert.Throws<InvalidOperationException>(() =>
            Render<RzDataTable<TestRow>>(ps => ps
                .Add(p => p.Items, BuildRows())
                .Add(p => p.Config, config)
                .Add(p => p.RowIdSelector, x => x.Id)));
    }

    [Fact]
    public void DuplicateRowIds_Throws()
    {
        var items = new List<TestRow>
        {
            new() { Id = "1", Name = "A" },
            new() { Id = "1", Name = "B" }
        };

        Assert.Throws<InvalidOperationException>(() =>
            Render<RzDataTable<TestRow>>(ps => ps
                .Add(p => p.Items, items)
                .Add(p => p.Config, BuildConfig())
                .Add(p => p.RowIdSelector, x => x.Id)));
    }

    [Fact]
    public void TransportSerialization_ContainsNormalizedValues()
    {
        var cut = Render<TestHost>();
        var script = cut.Find("script[type='application/json']");

        using var json = JsonDocument.Parse(script.TextContent);
        var root = json.RootElement;

        Assert.Equal("Id", root.GetProperty("rowStructure").GetProperty("rowIdPath").GetString());
        Assert.Equal("Name", root.GetProperty("columns")[0].GetProperty("id").GetString());
        Assert.Equal("Name", root.GetProperty("columns")[0].GetProperty("accessorKey").GetString());
    }

    private static IReadOnlyList<TestRow> BuildRows() =>
    [
        new() { Id = "1", Name = "Ada" },
        new() { Id = "2", Name = "Grace" }
    ];

    private static RzDataTableConfig<TestRow> BuildConfig() => new()
    {
        Columns =
        [
            new() { Accessor = x => x.Name, Header = "Name" },
            new() { Accessor = x => x.Id, Header = "Id" }
        ]
    };

    private sealed class TestHost : ComponentBase
    {
        [Parameter] public string LoadStrategy { get; set; } = "eager";

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenComponent<RzDataTable<TestRow>>(0);
            builder.AddAttribute(1, "Id", "test-datatable");
            builder.AddAttribute(2, "LoadStrategy", LoadStrategy);
            builder.AddAttribute(3, "Items", BuildRows());
            builder.AddAttribute(4, "Config", BuildConfig());
            builder.AddAttribute(5, "RowIdSelector", (Expression<Func<TestRow, object?>>)(x => x.Id));
            builder.CloseComponent();
        }
    }

    private sealed class TestRow
    {
        public required string Id { get; init; }
        public required string Name { get; init; }
    }
}
