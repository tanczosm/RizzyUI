using System.Text.Json.Serialization;

namespace RizzyUI;

/// <summary>
/// Represents a serializable confetti fire request.
/// </summary>
public sealed class ConfettiRequest
{
    /// <summary>Gets or sets the number of particles.</summary>
    public int? ParticleCount { get; set; }

    /// <summary>Gets or sets the launch angle in degrees.</summary>
    public double? Angle { get; set; }

    /// <summary>Gets or sets the spread in degrees.</summary>
    public double? Spread { get; set; }

    /// <summary>Gets or sets the initial velocity.</summary>
    public double? StartVelocity { get; set; }

    /// <summary>Gets or sets the decay ratio.</summary>
    public double? Decay { get; set; }

    /// <summary>Gets or sets the gravity factor.</summary>
    public double? Gravity { get; set; }

    /// <summary>Gets or sets the horizontal drift.</summary>
    public double? Drift { get; set; }

    /// <summary>Gets or sets the particle scale factor.</summary>
    public double? Scalar { get; set; }

    /// <summary>Gets or sets the ticks duration.</summary>
    public int? Ticks { get; set; }

    /// <summary>Gets or sets the normalized origin x coordinate.</summary>
    public double? OriginX { get; set; }

    /// <summary>Gets or sets the normalized origin y coordinate.</summary>
    public double? OriginY { get; set; }

    /// <summary>Gets or sets the z-index for rendered particles.</summary>
    public int? ZIndex { get; set; }

    /// <summary>Gets or sets a value indicating whether reduced-motion suppression should be disabled for this request.</summary>
    public bool? DisableForReducedMotion { get; set; }

    /// <summary>Gets or sets a value indicating whether flat confetti rendering is used.</summary>
    public bool? Flat { get; set; }

    /// <summary>Gets or sets the particle color palette.</summary>
    public IReadOnlyList<string>? Colors { get; set; }

    /// <summary>Gets or sets the list of particle shapes.</summary>
    public IReadOnlyList<ConfettiShape>? Shapes { get; set; }
}

/// <summary>
/// Represents global configuration options used when creating a confetti canvas instance.
/// </summary>
public sealed class ConfettiGlobalOptions
{
    /// <summary>Gets or sets a value indicating whether the canvas should automatically resize.</summary>
    public bool Resize { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether a worker should be used when possible.</summary>
    public bool UseWorker { get; set; } = true;
}

/// <summary>
/// Defines supported built-in confetti patterns.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ConfettiPattern
{
    /// <summary>Default one-shot burst pattern.</summary>
    Burst,
    /// <summary>One-shot burst with a randomized launch angle.</summary>
    RandomDirection,
    /// <summary>Repeating fireworks-style dual-origin burst.</summary>
    Fireworks,
    /// <summary>Continuous side-cannon streams from both viewport edges.</summary>
    SideCannons,
    /// <summary>Layered star and circle celebratory burst.</summary>
    Stars
}

/// <summary>
/// Defines supported trigger activation events.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ConfettiTriggerEvent
{
    /// <summary>Trigger on click.</summary>
    Click,
    /// <summary>Trigger on pointer entry.</summary>
    MouseEnter,
    /// <summary>Trigger on focus.</summary>
    Focus
}

/// <summary>
/// Defines supported particle shapes.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ConfettiShape
{
    /// <summary>Square confetti shape.</summary>
    Square,
    /// <summary>Circle confetti shape.</summary>
    Circle,
    /// <summary>Star confetti shape.</summary>
    Star
}
