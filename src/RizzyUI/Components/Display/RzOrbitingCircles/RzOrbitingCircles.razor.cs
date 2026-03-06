using System.Globalization;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.RenderTree;
using TailwindVariants.NET;

namespace RizzyUI;

/// <summary>
/// Renders supplied child content as evenly spaced items that orbit around a shared center point.
/// </summary>
[SuppressMessage("Usage", "BL0006:Do not use RenderTree types")]
[SuppressMessage("Usage", "ASP0006:Do not use non-literal sequence numbers")]
public partial class RzOrbitingCircles : RzComponent<RzOrbitingCircles.Slots>
{
    /// <summary>
    /// Defines the default styling for the <see cref="RzOrbitingCircles"/> component.
    /// </summary>
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(
        @base: "relative flex items-center justify-center overflow-hidden",
        slots: new()
        {
            [s => s.PathSvg] = "pointer-events-none absolute inset-0 size-full",
            [s => s.PathCircle] = "fill-none stroke-border/40 stroke-1",
            [s => s.Items] = "absolute inset-0",
            [s => s.Item] = "animate-orbit absolute left-1/2 top-1/2 flex -translate-x-1/2 -translate-y-1/2 transform-gpu items-center justify-center rounded-full"
        },
        variants: new()
        {
            [c => ((RzOrbitingCircles)c).Reverse] = new Variant<bool, Slots>
            {
                [true] = new() { [s => s.Item] = "[animation-direction:reverse]" }
            }
        }
    );

    private readonly List<OrbitItem> _orbitItems = [];

    /// <summary>
    /// Gets or sets the content to orbit around the center point.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether orbiting should run in reverse direction.
    /// </summary>
    [Parameter]
    public bool Reverse { get; set; }

    /// <summary>
    /// Gets or sets the baseline duration in seconds before speed adjustment.
    /// </summary>
    [Parameter]
    public double Duration { get; set; } = 20;

    /// <summary>
    /// Gets or sets the speed multiplier applied to duration.
    /// </summary>
    [Parameter]
    public double Speed { get; set; } = 1;

    /// <summary>
    /// Gets or sets the radius of the orbit path in CSS pixels.
    /// </summary>
    [Parameter]
    public double Radius { get; set; } = 160;

    /// <summary>
    /// Gets or sets a value indicating whether the decorative orbit path is rendered.
    /// </summary>
    [Parameter]
    public bool ShowPath { get; set; } = true;

    /// <summary>
    /// Gets or sets the width and height of each orbit item in CSS pixels.
    /// </summary>
    [Parameter]
    public double ItemSize { get; set; } = 30;

    /// <summary>
    /// Gets or sets the starting angle offset in degrees used before even distribution.
    /// </summary>
    [Parameter]
    public double StartAngle { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the component should be treated as decorative.
    /// </summary>
    [Parameter]
    public bool Decorative { get; set; } = true;

    /// <summary>
    /// Gets or sets an optional accessible label when <see cref="Decorative"/> is <see langword="false"/>.
    /// </summary>
    [Parameter]
    public string? AriaLabel { get; set; }

    private IReadOnlyList<OrbitItem> OrbitItems => _orbitItems;

    private double ClampedDuration { get; set; }

    private double ClampedSpeed { get; set; }

    private double EffectiveDuration { get; set; }

    private double ClampedRadius { get; set; }

    private double ClampedItemSize { get; set; }

    private string? AriaHidden => Decorative ? "true" : null;

    private string? RootRole => Decorative ? null : "group";

    private string? RootAriaLabel => Decorative ? null : AriaLabel;

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        ClampedSpeed = Math.Max(0.0001d, Speed);
        ClampedDuration = Math.Max(0.1d, Duration);
        EffectiveDuration = ClampedDuration / ClampedSpeed;
        ClampedRadius = Math.Max(0d, Radius);
        ClampedItemSize = Math.Max(1d, ItemSize);

        BuildOrbitItems();
    }

    /// <inheritdoc />
    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.RzOrbitingCircles;

    private void BuildOrbitItems()
    {
        _orbitItems.Clear();

        if (ChildContent is null)
        {
            return;
        }

        var topLevelItems = SplitTopLevelContent(ChildContent);

        if (topLevelItems.Count == 0)
        {
            return;
        }

        var step = 360d / topLevelItems.Count;

        for (var i = 0; i < topLevelItems.Count; i++)
        {
            var angle = StartAngle + (step * i);
            var style =
                $"--rz-orbit-duration:{EffectiveDuration.ToString("0.###", CultureInfo.InvariantCulture)}s;" +
                $"--rz-orbit-radius:{ClampedRadius.ToString("0.###", CultureInfo.InvariantCulture)}px;" +
                $"--rz-orbit-angle:{angle.ToString("0.###", CultureInfo.InvariantCulture)}deg;" +
                $"--rz-orbit-item-size:{ClampedItemSize.ToString("0.###", CultureInfo.InvariantCulture)}px;";

            _orbitItems.Add(new OrbitItem(angle, topLevelItems[i], style));
        }
    }

    private static List<RenderFragment> SplitTopLevelContent(RenderFragment fragment)
    {
        var probe = new RenderTreeBuilder();
        fragment(probe);

        var range = probe.GetFrames();
        if (range.Count == 0)
        {
            return [];
        }

        var frames = range.Array;
        var fragments = new List<RenderFragment>();

        var index = 0;
        while (index < range.Count)
        {
            var subtreeLength = GetSubtreeLength(frames[index]);
            if (subtreeLength <= 0)
            {
                index++;
                continue;
            }

            var start = index;
            var length = subtreeLength;
            fragments.Add(builder => ReplayChildren(builder, frames, start, length));
            index += subtreeLength;
        }

        return fragments;
    }

    private static void ReplayChildren(RenderTreeBuilder builder, RenderTreeFrame[] frames, int start, int length)
    {
        var i = start;
        var end = start + length;

        while (i < end)
        {
            var frame = frames[i];
            switch (frame.FrameType)
            {
                case RenderTreeFrameType.Element:
                {
                    var elementIndex = i;
                    var subtreeLength = frame.ElementSubtreeLength;

                    builder.OpenElement(frame.Sequence, frame.ElementName);
                    if (frame.ElementKey is not null)
                        builder.SetKey(frame.ElementKey);

                    CopyAttributesVerbatim(builder, frames, ref i);
                    var childCount = subtreeLength - (i - elementIndex);

                    ReplayChildren(builder, frames, i, childCount);
                    builder.CloseElement();

                    i = elementIndex + subtreeLength;
                    break;
                }
                case RenderTreeFrameType.Component:
                {
                    var componentIndex = i;
                    var subtreeLength = frame.ComponentSubtreeLength;

                    builder.OpenComponent(frame.Sequence, frame.ComponentType);
                    if (frame.ComponentKey is not null)
                        builder.SetKey(frame.ComponentKey);

                    CopyAttributesVerbatim(builder, frames, ref i);
                    var childCount = subtreeLength - (i - componentIndex);

                    ReplayChildren(builder, frames, i, childCount);
                    builder.CloseComponent();

                    i = componentIndex + subtreeLength;
                    break;
                }
                case RenderTreeFrameType.Region:
                {
                    var regionIndex = i;
                    var subtreeLength = frame.RegionSubtreeLength;

                    ReplayChildren(builder, frames, i + 1, subtreeLength - 1);
                    i = regionIndex + subtreeLength;
                    break;
                }
                case RenderTreeFrameType.Text:
                    builder.AddContent(frame.Sequence, frame.TextContent);
                    i++;
                    break;
                case RenderTreeFrameType.Markup:
                    builder.AddMarkupContent(frame.Sequence, frame.MarkupContent);
                    i++;
                    break;
                case RenderTreeFrameType.ElementReferenceCapture:
                    builder.AddElementReferenceCapture(frame.Sequence, frame.ElementReferenceCaptureAction);
                    i++;
                    break;
                case RenderTreeFrameType.ComponentReferenceCapture:
                    builder.AddComponentReferenceCapture(frame.Sequence, frame.ComponentReferenceCaptureAction);
                    i++;
                    break;
                default:
                    i++;
                    break;
            }
        }
    }

    private static void CopyAttributesVerbatim(RenderTreeBuilder builder, RenderTreeFrame[] frames, ref int index)
    {
        var attributeIndex = index + 1;
        while (attributeIndex < frames.Length && frames[attributeIndex].FrameType == RenderTreeFrameType.Attribute)
        {
            var frame = frames[attributeIndex];
            builder.AddAttribute(frame.Sequence, frame.AttributeName, frame.AttributeValue);
            attributeIndex++;
        }

        index = attributeIndex;
    }

    private static int GetSubtreeLength(in RenderTreeFrame frame) => frame.FrameType switch
    {
        RenderTreeFrameType.Element => frame.ElementSubtreeLength,
        RenderTreeFrameType.Component => frame.ComponentSubtreeLength,
        RenderTreeFrameType.Region => frame.RegionSubtreeLength,
        RenderTreeFrameType.Text => 1,
        RenderTreeFrameType.Markup => 1,
        _ => 1
    };

    /// <summary>
    /// Defines the slots available for styling in the <see cref="RzOrbitingCircles"/> component.
    /// </summary>
    public sealed partial class Slots : ISlots
    {
        /// <summary>
        /// The base slot for the component root element.
        /// </summary>
        [Slot("orbiting-circles")]
        public string? Base { get; set; }

        /// <summary>
        /// The slot for the orbit path SVG container.
        /// </summary>
        [Slot("path-svg")]
        public string? PathSvg { get; set; }

        /// <summary>
        /// The slot for the orbit path circle.
        /// </summary>
        [Slot("path-circle")]
        public string? PathCircle { get; set; }

        /// <summary>
        /// The slot for the items wrapper.
        /// </summary>
        [Slot("items")]
        public string? Items { get; set; }

        /// <summary>
        /// The slot for each orbiting item wrapper.
        /// </summary>
        [Slot("item")]
        public string? Item { get; set; }
    }

    private sealed record OrbitItem(double Angle, RenderFragment Content, string Style);
}
