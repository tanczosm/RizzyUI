namespace RizzyUI;

/// <summary>
/// Defines the native HTML button <c>type</c> values supported by <see cref="RzShimmerButton"/>.
/// </summary>
public enum ButtonType
{
    /// <summary>
    /// A clickable button with no default form submission behavior.
    /// </summary>
    Button,

    /// <summary>
    /// A submit button that submits the nearest parent form.
    /// </summary>
    Submit,

    /// <summary>
    /// A reset button that resets the nearest parent form's controls.
    /// </summary>
    Reset
}
