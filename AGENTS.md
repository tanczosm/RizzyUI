### **The Definitive Guide to Authoring a RizzyUI Component (Version 3.5 - Documentation Standards)**

**Fully expanded specification for code-generation models — Feb 2026, rev 3.5**

---

## Up-Front Non-Negotiables (READ FIRST)

1. **RizzyUI is SSR (server-side rendered) ONLY.**

   * **Do not** write components that require Blazor interactivity.
   * **Do not** introduce Blazor event-based behaviors (`@onclick`, `@onchange`, `@onsubmit`, `@bind`, `EventCallback`, interactive input components that require a Blazor circuit, etc.).
   * All client-side interactivity must be implemented via **Alpine.js** and/or **HTMX** patterns already used in RizzyUI (see §10).

2. **AGENTS ONLY — Node install rule**

   * Run `npm install` in **any directory that contains a `packages.json`**.
     *(If the repository uses `package.json` in practice, treat it as the same intent; do not skip installs.)*

3. **Documentation is Mandatory**

4. **SSR interaction event policy (mandatory)**

   * For browser-observable interactions in SSR components, prefer Alpine `$dispatch(...)` or `CustomEvent` helpers that emit `rz:` namespaced events.
   * Do **not** use `EventCallback` for browser-only post-render interaction flows.
   * Event payloads must use serializable primitives and stable identifiers.
   * Never emit full `TItem` instances or server object graphs in browser event payloads.
   * For table-like stateful primitives, emit granular events and a table-level aggregate state event when useful (for example `rz:table:on-state-change`).

   * **Every** new component must have a corresponding documentation page in `src/RizzyUI.Docs/Components/Pages/Components/`.
   * **Every** modified component must have its documentation page updated to reflect API, parameter, or behavior changes.
   * **Every** new component must be added to the navigation menu in `src/RizzyUI.Docs/Components/Layout/ComponentList.razor`.

---

## Repository Directory Structure

The repository is organized as a .NET solution containing the core Razor Class Library, a documentation site, and a companion NPM package for client-side asset generation.

### `src/RizzyUI` (Core Library)

The main Razor Class Library (RCL) containing all UI components and logic.

* **`Components/`**: The UI components, organized by category (e.g., `Display`, `Form`, `Layout`, `Navigation`).

  * *Structure*: Most components use a split-file pattern: `RzComponent.razor` (markup) and `RzComponent.razor.cs` (logic/styling). Generic components may also have a separate `Styling/` folder.
* **`RzTheme.cs` & `RzTheme.StyleProviders.cs`**: The central registry for component styling definitions (`TvDescriptor`) and theme configuration.
* **`Resources/`**: Contains `.resx` files for localization (e.g., `RizzyLocalization.en.resx`).
* **`Extensions/`**: Service collection extensions and helper methods.
* **`Attributes/`**: Custom attributes used for Alpine code-behind discovery.

### `packages/rizzyui` (Client Assets)

The NPM package responsible for building the CSS (Tailwind) and JavaScript bundles distributed with the library.

* **`src/js/lib/components/`**: Individual Alpine component factories. Each file usually maps to a single `x-data` name.
* **`src/js/bundles/`**: Bundle entry modules that re-export owned Alpine components as a feature cluster.
* **`src/js/runtime/componentBundleManifest.js`**: Canonical Alpine component-to-bundle ownership map.
* **`src/js/runtime/bundleLoaderRegistry.js`**: Dynamic import registry for bundle loading.
* **`src/js/runtime/asyncBundleRegistrar.js`**: Async Alpine integration that resolves a component name to its owning bundle.
* **`src/js/rizzyui.js`**: Standard shell runtime entrypoint.
* **`src/js/rizzyui-csp.js`**: CSP-safe shell runtime entrypoint.
* **`src/css/`**: Tailwind CSS source files.

**Do not directly alter files in `packages/rizzyui/dist` or `src/RizzyUI/wwwroot`.** Those are build outputs produced from the `packages/rizzyui` source tree.

### `src/RizzyUI.Docs` (Documentation)

A Blazor Web App that acts as the documentation site and component playground.

* **`Components/Pages/Components/`**: Contains the documentation pages for specific components (e.g., `ButtonInfo.razor`). These pages serve as the primary source of usage examples.
* **`Components/Layout/ComponentList.razor`**: The side navigation menu listing all available components.

### `src/RizzyUI.Tests` (Unit Tests)

Contains bUnit tests to verify component rendering and logic.

* **`Components/`**: Mirrors the folder structure of `src/RizzyUI/Components` for component-specific tests.

### `src/RizzyUI.Tasks` (Build Tools)

Contains MSBuild tasks used for build-time operations, such as computing source paths for co-located JavaScript modules.

---

## 0. Why this file exists

This guide is a **contract for any large-language model**—ChatGPT, Claude, Gemini, Azure OpenAI, etc.—that emits source code destined for the **RizzyUI** repository.

If the model follows every rule, a maintainer (or an automated agent) can:

### If working via copy/paste (chat-style generation)

1. Paste the generated files into `src/RizzyUI/…` and `src/RizzyUI.Docs/…`.
2. Apply the indicated cross-file edits (see § 15).
3. Run `dotnet build`.

### If working as an agent with direct repository access (agent-based edits)

1. Apply the file additions/edits directly in the working tree (including any cross-file edits described in §15).
2. Ensure any required Node dependencies are installed where applicable (**AGENTS ONLY — see “Up-Front Non-Negotiables”**).
3. Run `dotnet build` (and any requested tests), and ensure repository conventions remain intact.

In both modes, the solution should compile, pass unit tests (when applicable/requested), and conform to RizzyUI’s conventions **without manual tweaks**.

SPECIAL NOTE: @* *@ comments in this document are used to provide LLM guidance only. They are **not** to be included in the generated code.

---

## 1. Output-file syntax (mandatory for new or replacement files)

When the user requests code, wrap each file in a single **`output` block** so automation scripts know where to save it.

````markdown
```output
<files>
  <file path="src/RizzyUI/Components/Fancy/RzFancyThing/RzFancyThing.razor">
  <!-- Razor markup -->
  </file>

  <file path="src/RizzyUI/Components/Fancy/RzFancyThing/RzFancyThing.razor.cs">
  <!-- C# code-behind -->
  </file>

  <file path="src/RizzyUI.Docs/Components/Pages/Components/FancyThingInfo.razor">
  <!-- Documentation Page -->
  </file>
</files>
```
`````

* Never nest `<files>` elements.
* Always close every `<file>` tag.
* If no new files are needed, **omit** the `output` block entirely.
* Wrap the entire files response in a single markdown `output` block when generating multiple files.

**Agent-mode note (direct repo edits):**

* If you are applying changes directly (not generating copy/paste blocks), you must still adhere to **all** file paths, patterns, conventions, and cross-file edit rules in this document.
* When reporting results back, enumerate which files were changed/added and summarize the edits precisely (a diff-style explanation is acceptable).

---

## 2. Root element pattern (in every `.razor` file)

```razor
<HtmlElement Element="@EffectiveElement"
             id="@Id"
             @attributes="@AdditionalAttributes"
             class="@SlotClasses.GetBase()"
             data-slot="fancy-thing">
    @* Optional content here, like an Alpine child-container *@
</HtmlElement>
```

* `Element="@EffectiveElement"` keeps the tag overridable (default "div" in `RzComponent`).
* `@Id` is required for HTMX, Alpine, and tests.
* `@SlotClasses.GetBase()` is supplied by the `TailwindVariants.NET` system in the code-behind.
* `data-slot="component-name"` is a **mandatory** attribute. The value should be the kebab-case version of the component's name (e.g., `RzFancyThing` becomes `fancy-thing`).
* **Always** convert enum values used as data- attributes into kebab-case (e.g. MyEnumProperty.ToString().ToKebabCase()).
* **Always** write `@attributes="@AdditionalAttributes"` (note the leading `@`).
* **Always place `@attributes` before `class`** on component root elements.
* **Never** use @* *@ comments inside Razor markup for elements that will be rendered.
* To change the `Element` type, override `OnInitialized()` in the code-behind. Set `Element` to the new type only if `Element` is empty or null.

  ```csharp
          if (string.IsNullOrEmpty(Element))
              Element = "nav";
  ```

---

## 3. `.razor` File Guidelines

* **Namespace:** Add `@namespace RizzyUI`.
* **Inheritance:** Add `@inherits RzComponent<ComponentName.Slots>` for non-generic components or `@inherits RzComponent<ComponentNameSlots>` for generic components.
* **Component Naming Convention:**

  * For **root-level components**, the component name MUST be prefixed with `Rz` (e.g., `RzDropdownMenu`).
  * For **nested components**, the `Rz` prefix MUST be omitted (e.g., `DropdownMenuLabel`).
* **Root Element:** Use the pattern in §2.
* **CSS Classes and `data-slot` for Internal Elements:**

  * Every HTML element *inside* the root element that receives styling from a slot **MUST** have both a `class` attribute and a `data-slot` attribute.
  * The `class` attribute MUST use the source-generated `SlotClasses.Get...()` accessor (e.g., `class="@SlotClasses.GetIcon()"`).
  * The `data-slot` attribute's value MUST be retrieved using the source-generated `SlotNames.NameOf(...)` helper. The syntax depends on whether the component is generic.

    * **Non-Generic Components** (nested `Slots` class): `data-slot="@RzComponentName.SlotNames.NameOf(SlotTypes.SlotPropertyName)"`

      * Example: `data-slot="@RzFancyThing.SlotNames.NameOf(SlotTypes.Icon)"`
    * **Generic Components** (external `Slots` class): `data-slot="@ComponentNameSlotNames.NameOf(ComponentNameSlotTypes.SlotPropertyName)"`

      * Example: `data-slot="@TableHeaderCellSlotNames.NameOf(TableHeaderCellSlotTypes.SortIndicator)"`
* **Alpine Integration:** See §10 for the Alpine child-container convention if Alpine is used.
* **Accessibility:** Refer to §8 for accessibility guidelines.
* **Localization:** Refer to §9 for localization guidelines.

**SSR-only reminder (CRITICAL):**

* Do not add Blazor interactivity constructs to `.razor` markup (no `@on...`, no `@bind`, no interactive component state that depends on a Blazor circuit). Use Alpine/HTMX approaches per §10.

---

## 4. Code-behind skeleton (`.razor.cs`)

```csharp
// src/RizzyUI/Components/Fancy/RzFancyThing/RzFancyThing.razor.cs
using Microsoft.AspNetCore.Components;
using RizzyUI.Extensions;
using Blazicons;
using TailwindVariants.NET;
// Add other necessary using statements

namespace RizzyUI;

/// <summary>
/// Brief description of RzFancyThing and its purpose.
/// </summary>
public partial class RzFancyThing : RzComponent<RzFancyThing.Slots>
{
    /// <summary>
    /// Defines the default styling for the RzFancyThing component.
    /// </summary>
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(
        @base: "inline-flex items-center font-medium rounded",
        slots: new()
        {
            [s => s.Icon] = "size-5"
        },
        variants: new()
        {
            [c => ((RzFancyThing)c).Size] = new Variant<Size, Slots>
            {
                [Size.Small] = new() { [s => s.Base] = "text-xs py-1 px-2" },
                [Size.Medium] = new() { [s => s.Base] = "text-sm py-2 px-3" }
            },
            [c => ((RzFancyThing)c).IsActive] = new Variant<bool, Slots>
            {
                [true] = new() { [s => s.Base] = "ring-2 ring-primary" }
            }
        }
    );

    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public SvgIcon? Icon { get; set; }
    [Parameter] public Size Size { get; set; } = Size.Medium;
    [Parameter] public bool IsActive { get; set; }
    [Parameter] public string? AriaLabel { get; set; }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        AriaLabel ??= Localizer["RzFancyThing.DefaultAriaLabel"];
    }

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        AriaLabel ??= Localizer["RzFancyThing.DefaultAriaLabel"];
    }

    /// <inheritdoc/>
    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.RzFancyThing;

    /// <summary>
    /// Defines the slots available for styling in the RzFancyThing component.
    /// </summary>
    public sealed partial class Slots : ISlots
    {
        [Slot("fancy-thing")]
        public string? Base { get; set; }
        [Slot("icon")]
        public string? Icon { get; set; }
    }
}
```

Important for the LLM:

* Start with `/// <summary>...</summary>` for the class.
* **All** public members get `<summary>` XML docs.
* Inherit from `RzComponent<TSlots>` or `RzAsChildComponent<TSlots>`.
* Define a `public sealed partial class Slots : ISlots` inside the component class for non-generic components.
* **All properties in the `Slots` class MUST be decorated with the `[Slot("kebab-case-name")]` attribute.** The name should match `shadcn/ui` conventions where applicable.
* For components inheriting from `RzAsChildComponent`, the `Base` slot's classes are merged onto the child element. The `[Slot(...)]` attribute is still required on the `Base` property in the `Slots` class.
* Define a `public static readonly TvDescriptor<...>` with all base, slot, and variant styles.
* Implement `protected override TvDescriptor<...> GetDescriptor() => Theme.ComponentName;`.
* Handle default localized strings for parameters like `AriaLabel` (§9).

**SSR-only reminder (CRITICAL):**

* Do not introduce Blazor-interactivity-oriented APIs (e.g., `EventCallback` meant for client-side interaction) as part of RizzyUI component design. Prefer semantic HTML + Alpine/HTMX patterns.

---

## 5. Icon-passing protocol

```razor
@* Inside RzFancyThing.razor *@
@if (Icon is not null)
{
    <Blazicon Svg="@Icon"
              class="@SlotClasses.GetIcon()"
              data-slot="@RzFancyThing.SlotNames.NameOf(SlotTypes.Icon)"
              aria-hidden="true" />
}
```

* Accept icons as `SvgIcon?`.
* Style the `<Blazicon>` using the source-generated `SlotClasses.Get...()` accessor.
* Add the mandatory `data-slot` attribute using the `SlotNames.NameOf(...)` helper.
* Add `aria-hidden="true"` if the icon is purely decorative.

---

## 6. Styling with TailwindVariants.NET

RizzyUI uses its own **Tailwind 4 plugin** that defines a palette of **CSS custom-property tokens** (e.g., `--background`, `--primary`).

### 6.1 Semantic tokens, not raw palette colors

You never hard-code `text-blue-600` or `bg-slate-900`. Instead, you write utilities that reference the semantic tokens:

```
bg-background
text-foreground
hover:bg-accent
ring-primary
```

Because the token names stay identical across light and dark modes, you do **not** need `dark:` prefixes for these semantic tokens.

### 6.2 Where utilities live

All styling logic—base classes, slot-specific classes, and conditional variants—belongs in the `TvDescriptor` defined within the component's `.razor.cs` or `Styling/ComponentNameStyles.cs` file.

### 6.3 Automatic Class Merging

The `RzComponent<TSlots>` base class automatically handles merging classes. The `TwVariants.Invoke` method combines classes from the `TvDescriptor` with any `class` attribute provided by the user on the component tag. You do not need to manually call `TwMerge`.

### 6.4 Defining Variants in the Descriptor

Variants are defined within the `variants` property of the `TvDescriptor`. They map a component parameter's value to a set of CSS classes.

```csharp
// In the TvDescriptor
variants: new()
{
    [c => ((RzFancyThing)c).Size] = new Variant<Size, Slots>
    {
        [Size.Small] = new() { [s => s.Base] = "text-xs py-1 px-2" },
        [Size.Medium] = new() { [s => s.Base] = "text-sm py-2 px-3" }
    }
}
```

### 6.5 Handling Nullable Enum Parameters in Variants

When a component `[Parameter]` is a **nullable enum** (e.g., `public SemanticColor? TextColor { get; set; }`), the corresponding `Variant` definition in the `TvDescriptor` **MUST** use the **non-nullable** enum type for its generic parameter. The library automatically handles `null` values by not applying a variant class.

**❌ WRONG:** The generic type `Variant<SemanticColor?, Slots>` is nullable.

```csharp
// This is incorrect and will cause issues.
[c => ((RzHeading)c).TextColor] = new Variant<SemanticColor?, Slots> { ... }
```

**✅ CORRECT:** The generic type `Variant<SemanticColor, Slots>` is non-nullable.

```csharp
// This is the correct pattern.
[c => ((RzHeading)c).TextColor] = new Variant<SemanticColor, Slots> { ... }
```

---

## 7. The TvDescriptor Pattern (Styling Providers)

The styling system is based on the `TvDescriptor`, which encapsulates all styling logic for a component. The implementation pattern differs depending on whether the component is generic.

### 7.1 For Non-Generic Components (Self-Contained Pattern)

For a non-generic component like `RzButton`, the `Slots` class and the `DefaultDescriptor` are defined directly within the component's `.razor.cs` file.

```csharp
// src/RizzyUI/Components/Form/RzButton/RzButton.razor.cs
public partial class RzButton : RzComponent<RzButton.Slots>
{
    public static readonly TvDescriptor<RzComponent<Slots>, Slots> DefaultDescriptor = new(/* ... */);

    // ... parameters ...

    protected override TvDescriptor<RzComponent<Slots>, Slots> GetDescriptor() => Theme.RzButton;

    public sealed partial class Slots : ISlots
    {
        [Slot("button")]
        public string? Base { get; set; }
    }
}
```

### 7.2 For Generic Components (External Styling Pattern)

If a component is generic (e.g., `TableHeaderCell<TItem>`) or requires decoupled styling, you **MUST** NOT define the `Slots` or `TvDescriptor` in the main `.razor.cs` file. Instead, use the External Styling Pattern.

**1. Create the Styles File:**
Create a file named `Styling/{ComponentName}Styles.cs` within the component's directory.

**2. Define Three Types in the Styles File:**
This file MUST contain exactly these three types:

* **A Styling Interface:** `public interface IHas{ComponentName}StylingProperties` containing only the properties needed for variants.
* **The Slots Class:** `public sealed partial class {ComponentName}Slots : ISlots` with all `[Slot]` decorated properties.
* **A Static Styles Class:** `public static class {ComponentName}Styles` containing the `DefaultDescriptor`.

**Example: `Styling/TableHeaderCellStyles.cs`**

```csharp
using TailwindVariants.NET;

namespace RizzyUI;

// 1. Styling Interface
public interface IHasTableHeaderCellStylingProperties
{
    public bool Sortable { get; }
    public SortDirection CurrentSortDirection { get; }
}

// 2. Slots Class
public sealed partial class TableHeaderCellSlots : ISlots
{
    [Slot("table-header-cell")]
    public string? Base { get; set; }
    [Slot("sort-indicator")]
    public string? SortIndicator { get; set; }
}

// 3. Static Styles Class
public static class TableHeaderCellStyles
{
    public static readonly TvDescriptor<RzComponent<TableHeaderCellSlots>, TableHeaderCellSlots> DefaultDescriptor = new(
        @base: "...",
        variants: new()
        {
            // Cast to the interface in variant expressions
            [c => ((IHasTableHeaderCellStylingProperties)c).Sortable] = new Variant<bool, TableHeaderCellSlots> { ... }
        }
    );
}
```

**3. Update the Component (`.razor.cs`):**
The component must then inherit from `RzComponent<{ComponentName}Slots>` and implement the styling interface.

```csharp
public partial class TableHeaderCell<TItem> : RzComponent<TableHeaderCellSlots>, IHasTableHeaderCellStylingProperties
{
    // ... implementation ...
    protected override TvDescriptor<RzComponent<TableHeaderCellSlots>, TableHeaderCellSlots> GetDescriptor() => Theme.TableHeaderCell;
}
```

---

## 8. Accessibility Best Practices

Ensuring components are accessible is paramount. LLMs must generate components that strive to adhere to WCAG standards where applicable.

* **Semantic HTML:**

  * Use the most appropriate HTML element for the component's role. The `Element` property in `RzComponent` (defaulting to "div") should be overridden in `OnInitialized()` if a more semantic tag like `<nav>`, `<button>`, `<aside>`, etc., is suitable.
  * Example: A navigation component should use `<nav>`, a button should use `<button>` or `<a>` with `role="button"`.

* **ARIA Attributes:**

  * **Roles:** Apply appropriate `role` attributes (e.g., `role="alert"`, `role="dialog"`, `role="menuitem"`, `role="tab"`, `role="switch"`). The root element pattern in §2 can have its `role` set via `AdditionalAttributes` or directly if static.
  * **Labels & Descriptions:**

    * Every interactive component MUST have an accessible name. This is typically provided via an `AriaLabel` parameter in the `.razor.cs` file. If the component has visible text that serves as its label, ensure it's associated (e.g., `<label for="...">` for form inputs, or `aria-labelledby` pointing to the ID of the visible text element).
    * Use `aria-label` for concise labels when visible text is insufficient or absent (e.g., icon-only buttons).
    * Use `aria-labelledby` to associate the component with existing visible text that acts as its label.
    * Use `aria-describedby` to associate the component with descriptive text that provides more context.
  * **States & Properties:** Use ARIA attributes to convey state:

    * `aria-expanded` (for accordions, dropdowns, collapsible sections)
    * `aria-selected` (for tabs, items in a listbox)
    * `aria-current` (for pagination, breadcrumbs, steps - e.g., `aria-current="page"` or `aria-current="step"`)
    * `aria-pressed` (for toggle buttons)
    * `aria-hidden` (use judiciously, e.g., for purely decorative icons or off-screen content)
    * `aria-modal="true"` (for modal dialogs)
    * `aria-live` (for dynamic content updates, e.g., alerts, status messages)
    * `aria-controls` (to link a control to the region it manages)
    * For inputs: `aria-required`, `aria-invalid`.

* **Keyboard Navigation & Focus Management:**

  * All interactive elements MUST be keyboard operable.
  * Use `tabindex="0"` for custom interactive elements that should be in the tab order.
  * Use `tabindex="-1"` for elements that should be programmatically focusable but not in the default tab order.
  * For composite widgets (like dropdowns, menus, tabs), implement appropriate keyboard navigation patterns (arrow keys, Home/End, Enter/Space). This is often handled by the Alpine.js logic.
  * Ensure a visible focus indicator. RizzyUI themes generally provide this, but be mindful if overriding default focus styles.
  * For modals and dropdowns that trap focus, use Alpine's `x-trap.inert="isOpen"` directive.

* **Screen Reader Text:**

  * Use the `sr-only` Tailwind class (or equivalent CSS) for text that should only be available to screen readers (e.g., providing context for an icon button).

    ```razor
    <button aria-label="@Localizer["RzComponentName.CloseButtonAriaLabel"]">
        <Blazicon Svg="@MdiIcon.Close" aria-hidden="true" />
        <span class="sr-only">@Localizer["RzComponentName.CloseButtonSrText"]</span> @* Alternative if aria-label isn't sufficient *@
    </button>
    ```

* **Images & Icons:**

  * Decorative icons should have `aria-hidden="true"`.
  * Informative icons (if not accompanied by text) need an accessible label (e.g., via `aria-label` on the button or a `sr-only` span).
  * Images (e.g., `RzAvatar`) must have meaningful `alt` text or `aria-label`.

* **Forms:**

  * Associate labels with form controls using `<label for="...">` and matching `id` on the input. `RzFieldLabel` handles this if `For` is provided.
  * Use `fieldset` and `legend` for groups of related controls (e.g., radio button groups).
  * Provide clear validation messages, associated with inputs using `aria-describedby`. `RzValidationMessage` typically handles this.

---

## 9. Localization

All user-facing strings within components (default labels, ARIA labels, titles, placeholders, etc.) MUST be localizable.

* **Accessing Localizer:**

  * The `RzComponent` base class injects `IStringLocalizer<RizzyLocalization> Localizer`. Use this to retrieve localized strings.

* **Resource Key Convention:**

  * Resource keys should follow the pattern: `ComponentName.ResourceKeyName`.
  * Example: For a default ARIA label in `RzFancyThing`, the key would be `RzFancyThing.DefaultAriaLabel`.
  * Example: For a "Close" button text, `RzModal.CloseButtonText`.

* **Parameter Defaults & Localization:**

  * Component parameters that accept user-facing strings (e.g., `AriaLabel`, `Title`, `Placeholder`) should allow users to provide their own values.
  * If the user does *not* provide a value for such a parameter (i.e., it remains `null` or its default), the component should attempt to load a localized default string.
  * This is typically done in `OnInitialized()` and/or `OnParametersSet()`:

    ```csharp
    // In RzFancyThing.razor.cs
    [Parameter] public string? AriaLabel { get; set; }
    [Parameter] public string? PlaceholderText { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        AriaLabel ??= Localizer["RzFancyThing.DefaultAriaLabel"];
        PlaceholderText ??= Localizer["RzFancyThing.DefaultPlaceholder"];
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        // If parameters can be changed after init and might become null
        AriaLabel ??= Localizer["RzFancyThing.DefaultAriaLabel"];
        PlaceholderText ??= Localizer["RzFancyThing.DefaultPlaceholder"];
    }
    ```

* **Providing New Resource Strings (LLM Output):**

  * When generating a new component that introduces new localizable strings, the LLM MUST provide the English (default culture) versions of these strings.
  * These should be presented in a clear key-value format, suitable for a developer to copy into the `src/RizzyUI/Resources/RizzyLocalization.resx` file.
  * This information should be provided *outside* the main `output ... ` block, typically alongside the "Manual Edits Required for Theme Integration" section.

  **Example Presentation for New Resource Strings:**

  ````markdown
  **New English Localization Strings for `RizzyLocalization.resx`:**

  Please add the following entries to `src/RizzyUI/Resources/RizzyLocalization.resx`:

  | Name                               | Value                         | Comment (Optional)                  |
  |------------------------------------|-------------------------------|-------------------------------------|
  | `RzFancyThing.DefaultAriaLabel`    | `Fancy interactive element`   | `Default ARIA label for RzFancyThing` |
  | `RzFancyThing.DefaultPlaceholder`  | `Enter fancy text here...`    | `Placeholder for RzFancyThing input`  |
  | `RzAnotherComponent.SomeText`      | `Another default string`      |                                     |

  Alternatively, in raw .resx XML format:
  ```xml
  <data name="RzFancyThing.DefaultAriaLabel" xml:space="preserve">
    <value>Fancy interactive element</value>
    <comment>Default ARIA label for RzFancyThing</comment>
  </data>
  <data name="RzFancyThing.DefaultPlaceholder" xml:space="preserve">
    <value>Enter fancy text here...</value>
    <comment>Placeholder for RzFancyThing input</comment>
  </data>
  <data name="RzAnotherComponent.SomeText" xml:space="preserve">
    <value>Another default string</value>
  </data>
  ```
  ````

---

## 10. Alpine.js Integration & Asset Bundling

RizzyUI components that require client-side interactivity leverage Alpine.js. The integration follows a specific pattern for defining components, managing their assets, and ensuring CSP compliance.

### 10.1 Alpine.js API Restrictions (CSP Build)

**It is CRITICAL to understand and adhere to these API restrictions when writing Alpine.js code for RizzyUI components.** Since Alpine.js, when built for Content Security Policy (CSP) compliance, can no longer interpret strings as plain JavaScript, it has to parse and construct JavaScript functions from them manually.

Due to this limitation, RizzyUI Alpine components **must** be referenced by name through `x-data`, must be loadable through the shell + Async Alpine runtime, and **must** expose properties and methods that are referenced by key only.

For example, an inline component like this **will not work** with the CSP build:

```html
<!-- Bad -->
<div x-data="{ count: 1 }">
    <button @click="count++">Increment</button>
    <span x-text="count"></span>
</div>
```

Instead, use a named Alpine component and reference methods and properties by key:

```html
<!-- Good -->
<div x-data="counter">
    <button @click="increment">Increment</button>
    <span x-text="count"></span>
</div>
```

```javascript
export default function counter() {
    return {
        count: 1,
        increment() {
            this.count++;
        },
    };
}
```

The CSP build supports accessing nested properties using dot notation:

```html
<!-- This works too -->
<div x-data="counter">
    <button @click="foo.increment">Increment</button>
    <span x-text="foo.count"></span>
</div>
```

```javascript
export default function counter() {
    return {
        foo: {
            count: 1,
            increment() {
                this.count++;
            },
        },
    };
}
```

**Key Takeaways for LLMs:**

* Never inline `x-data` objects in RizzyUI components.
* Always reference methods and properties by key only (e.g., `increment`, `count`, `foo.increment`).
* Avoid complex inline expressions like `count++`, `isActive = !isActive`, or `myFunction(param1, param2)`. Encapsulate such logic within methods on the Alpine component object.
* Dot notation for nested properties is allowed.

### 10.2 Alpine Child-Container Convention (in `.razor` file)

If a RizzyUI component uses Alpine.js, its root `<HtmlElement>` in the `.razor` file MUST contain a direct child `<div>` with the following attributes. This `div` serves as the root for the Alpine component.

```razor
<HtmlElement Element="@EffectiveElement" id="@Id" @attributes="@AdditionalAttributes" class="@SlotClasses.GetBase()">
    <div data-alpine-root="@Id" @* Crucial: Must match the Blazor component's @Id *@
         x-data="rzFancyThing"   @* Alpine component name, e.g., 'rzComponentName' *@
         x-load="@LoadStrategy"  @* Shared RzComponent parameter (default: "eager") *@
         data-assets="@_assets"  @* Serialized JSON string of asset URLs for this component *@
         data-nonce="@Nonce">    @* CSP nonce for inline scripts/styles loaded by this component *@
        @* Alpine-interactive content, x-ref, x-on:, :class, etc. goes here *@
    </div>
</HtmlElement>
```

* `data-alpine-root="@Id"`: This attribute is **essential**. It MUST exactly match the Blazor component's `@Id`. It's used by the `Rizzy.$data()` helper to locate the Alpine component's scope.
* `x-data="rzComponentName"`: Specifies the name of the Alpine.js component to initialize on this `div`. The name should follow the `rzComponentName` convention, where `rz` corresponds to `Rz` prefixed Blazor components.
* `x-load="@LoadStrategy"`: Required on the same element as `x-data` for Async Alpine loading. `LoadStrategy` is inherited from `RzComponent` and defaults to `"eager"`; emit `x-load` only when `LoadStrategy` is non-empty.
* `data-assets="@_assets"`: A JSON stringified array of asset URLs (JavaScript/CSS) that this specific Alpine component instance needs. This is populated from the C# code-behind.
* `data-nonce="@Nonce"`: The CSP nonce value, passed from the C# `RzComponent` base. This is used by the `require` utility to load assets in a CSP-compliant manner.
* Async Alpine is the source of truth for component loading; do not rely on bootstrap-time preload for component activation correctness.

### 10.3 Asset Declaration and Loading (Centralized)

Components **do not** hardcode asset URLs. Instead, they declare their dependencies using logical keys, which are resolved to URLs from a central configuration.

1. **Define Asset Keys (`.razor.cs`):**

   * The component defines a `ComponentAssetKeys` parameter, which is a `string[]`. This parameter should have a default value containing the logical keys for its required assets.
   * Example:

     ```csharp
     // In RzFancyThing.razor.cs
     [Parameter]
     public string[] ComponentAssetKeys { get; set; } = ["FancyThingCoreScript", "SomeOtherDependency"];
     ```

2. **Inject Configuration (`.razor.cs`):**

   * The component injects `IOptions<RizzyUIConfig>` to access the central asset URL mapping.

     ```csharp
     // In RzFancyThing.razor.cs
     [Inject]
     private IOptions<RizzyUIConfig> RizzyUIConfig { get; set; } = default!;
     ```

3. **Resolve and Serialize URLs (`.razor.cs`):**

   * A private method (e.g., `UpdateAssets`) is called in `OnInitialized` and `OnParametersSet`.
   * This method looks up the URLs for each key in `ComponentAssetKeys` from `RizzyUIConfig.Value.AssetUrls`.
   * The resolved URLs are serialized into a JSON string and stored in a private field (e.g., `_assets`), which is then bound to the `data-assets` attribute in the Razor markup.

     ```csharp
     // In RzFancyThing.razor.cs
     private string _assets = "[]";

     private void UpdateAssets()
     {
         var assetUrls = ComponentAssetKeys
             .Select(key => RizzyUIConfig.Value.AssetUrls.TryGetValue(key, out var url) ? url : null)
             .Where(url => !string.IsNullOrEmpty(url))
             .ToList();
         _assets = System.Text.Json.JsonSerializer.Serialize(assetUrls);
     }
     ```

### 10.4 Alpine component definition (individual JavaScript file)

Each interactive RizzyUI component written in Alpine.js **must** live in its own file under

```text
packages/rizzyui/src/js/lib/components/
```

The filename and exported function **must** match the value of the `x-data` attribute used in your `.razor` file. For example, `rzFancyThing.js` defines the `rzFancyThing` Alpine component.

Follow these rules when authoring a new JS file:

1. **Use ES modules and default exports.** Define your component as:

   ```javascript
   // packages/rizzyui/src/js/lib/components/rzFancyThing.js
   export default function rzFancyThing() {
       return {
           // component state and methods here
           init() {
               // initialization code
           },
           // other lifecycle methods and helpers
       };
   }
   ```

2. **Do not accept `Alpine` or `require` as parameters** on the exported function. Async Alpine loads the component factory directly.

3. **Keep logic CSP-safe.** Avoid inline expression-oriented patterns and expose methods and properties that can be referenced by key.

4. **If third-party assets are required, import the runtime asset loader directly.** For example:

   ```javascript
   import { require } from '../../runtime/rizzyRequire.js';

   export default function rzChart() {
       return {
           init() {
               const assetsToLoad = JSON.parse(this.$el.dataset.assets || '[]');
               const nonce = this.$el.dataset.nonce || '';

               if (assetsToLoad.length > 0) {
                   require(assetsToLoad, {
                       success: () => this.initializeChart(),
                       error: err => console.error('[rzChart] Failed to load assets.', err)
                   }, nonce);
                   return;
               }

               this.initializeChart();
           },
           initializeChart() {
               // chart initialization logic
           }
       };
   }
   ```

5. Continue to read `data-assets` and `data-nonce` from the component root when you need external assets. These values are supplied by the Blazor component.

### 10.5 Alpine component registration (bundle-based architecture)

The previous `packages/rizzyui/src/js/lib/components.js` file and its `registerComponents()` function are **not** part of the current architecture. Components are now lazily loaded through **bundles** and a **component-to-bundle manifest**.

To register a new component:

1. **Create your component file** as described in §10.4 and ensure the exported factory name matches your `x-data` attribute.

2. **Assign the component to an owning bundle** in `packages/rizzyui/src/js/runtime/componentBundleManifest.js`. This file maps each Alpine component name to a bundle identifier.

   Choose the bundle based on the component’s feature area and weight:

   * **core-common**: lightweight primitives used on most pages
   * **command-runtime**: command palette and related components
   * **advanced-input-runtime**: heavier input controls
   * **calendar-runtime**
   * **color-runtime**
   * **content-visual-runtime**
   * **dialogs-panels-runtime**
   * **menu-runtime**
   * **popover-tooltip-runtime**
   * **docs-runtime**
   * **effects-runtime**

   If a suitable bundle does not exist, create a new bundle file under `packages/rizzyui/src/js/bundles/` and add it to `bundleLoaderRegistry.js`.

3. **Export the component from its bundle file.** For example, if your component belongs to `core-common`:

   ```javascript
   export { default as rzFancyThing } from '../lib/components/rzFancyThing.js';
   // existing exports remain unchanged
   ```

4. **Do not modify `rizzyui.js` or `rizzyui-csp.js`** to register the component directly. These shell entrypoints delegate component loading to the async bundle runtime.

5. **Do not reintroduce eager global registration.** Bundle ownership plus Async Alpine loading is the required architecture.

6. **Exclude `RzEmpty` from the manifest** if it has no meaningful client-side behavior. `RzEmpty` remains in the component library, but it should not participate in the active JavaScript bundle graph unless there is a documented reason.

### 10.6 Async Alpine child-container loading rule

If a RizzyUI component uses Alpine.js, the Alpine root element must follow the child-container convention in §10.2. In addition, the following loading rule is mandatory:

* `x-load="@LoadStrategy"` must appear on the same element as `x-data`.
* `LoadStrategy` is inherited from `RzComponent`.
* The default value is `"eager"`.
* Emit `x-load` only when `LoadStrategy` is non-empty.
* Do not rely on bootstrap-time preloading for correctness. Async Alpine is the source of truth for component activation.

Example:

```razor
<HtmlElement Element="@EffectiveElement"
             id="@Id"
             @attributes="@AdditionalAttributes"
             class="@SlotClasses.GetBase()">
    <div data-alpine-root="@Id"
         x-data="rzFancyThing"
         x-load="@LoadStrategy"
         data-assets="@_assets"
         data-nonce="@Nonce">
        @ChildContent
    </div>
</HtmlElement>
```

For `RzAsChildComponent`-based components, ensure the equivalent attribute emission path also includes `x-load` when `LoadStrategy` is non-empty.

### 10.7 Summary of the new registration flow

When a Razor component renders with `x-data="rzFancyThing"` and `x-load`, the shell runtime in `rizzyui.js` or `rizzyui-csp.js` observes the `x-data` attribute. Async Alpine requests the factory for `rzFancyThing`. The async bundle registrar looks up `rzFancyThing` in `componentBundleManifest.js`, identifies its owning bundle, dynamically imports that bundle through `bundleLoaderRegistry.js`, and resolves the exported factory. The factory is then used by Async Alpine to initialize the requested component. Once loaded, that bundle is cached and reused.

This architecture has several important consequences:

* component loading is demand-driven
* one bundle can own multiple Alpine components
* bundles load once and are cached
* the standard and CSP entrypoints share the same async bundle graph
* large features stay out of the shell runtime by default

---

## 11. Unit tests (bUnit) (IMPORTANT: ONLY ON REQUEST)

When unit tests are specifically requested for a new or modified component, they should be generated using bUnit and adhere to the following guidelines. Tests ensure component correctness, accessibility, and integration with the RizzyUI theme system and Alpine.js patterns.

**SSR-only reminder (CRITICAL):**

* Do not write tests that depend on Blazor interactivity or client-side event dispatch into .NET (e.g., asserting an `EventCallback` was invoked by a simulated click). RizzyUI is SSR-only, so tests should primarily verify **rendered markup**, **attributes**, **classes**, **data-slot correctness**, and **Alpine integration elements**.

* **File Location and Naming:**

  * Test files should reside in the `src/RizzyUI.Tests/Components/` directory, mirroring the component's path under `src/RizzyUI/Components/`.
  * Example: For `src/RizzyUI/Components/Fancy/RzFancyThing/RzFancyThing.razor`, the test file would be `src/RizzyUI.Tests/Components/Fancy/RzFancyThingTests.cs`.
  * Use file-scoped namespaces matching the directory structure within the test project (e.g., `namespace RizzyUI.Tests.Components.Fancy;`).

* **Test Class Structure:**

  * Test classes MUST inherit from `BunitAlbaContext`.
  * Test classes MUST implement `IClassFixture<WebAppFixture>`.
  * The constructor MUST accept a `WebAppFixture` parameter and pass it to the `base(fixture)` constructor.
  * The `WebAppFixture` (via `BunitAlbaContext`) handles the setup of essential services like `IHttpContextAccessor`, `IRizzyNonceProvider`, and automatically calls `AddRizzyUI()`, which registers `TwMerge` and the default `RzTheme`.

  ```csharp
  // Example: src/RizzyUI.Tests/Components/Fancy/RzFancyThingTests.cs
  using Bunit;
  using Microsoft.AspNetCore.Components; // For RenderFragment, etc.
  using RizzyUI.Components.Fancy.RzFancyThing; // Import the component being tested
  // Add other necessary using statements (e.g., Blazicons, System.Linq.Expressions)

  namespace RizzyUI.Tests.Components.Fancy; // File-scoped namespace

  public class RzFancyThingTests : BunitAlbaContext, IClassFixture<WebAppFixture>
  {
      // private readonly IAlbaHost _host; // Only needed if testing HTTP interactions via Alba

      public RzFancyThingTests(WebAppFixture fixture) : base(fixture)
      {
          // _host = fixture.Host; // Store if needed for Alba HTTP tests
          // Services.AddRizzyUI(); // This is ALREADY CALLED by WebAppFixture, no need to call again.
      }

      // ... Test methods ...
  }
  ```

* **Test Method Guidelines (`[Fact]` or `[Theory]`):**

  1. **Default Render Test:**

     * Render the component with minimal or no parameters (or only required ones like `Id`).
     * Assert the root element exists and has the correct default tag (e.g., `div` unless overridden).
     * Verify the `Id` is correctly applied.
     * Assert that `cut.Find("selector").ClassList` contains the expected base classes from the `DefaultDescriptor`.
     * Assert default ARIA attributes (e.g., `role`, default `aria-label` if applicable from localization).
     * If the component uses Alpine.js:

       * Assert the presence of the Alpine child-container (`div[data-alpine-root='@Id']`).
       * Assert `x-data` attribute matches the component's Alpine module name.
       * Assert `x-load` is present when `LoadStrategy` is non-empty.
       * Assert `data-assets` attribute is present and contains the JSON serialized URLs resolved from the default `ComponentAssetKeys`.
       * Assert `data-nonce` attribute is present.

  2. **Parameter Variation Tests:**

     * For each significant parameter, create tests to verify its effect on rendering.
     * **CSS Classes:** Use `cut.Find("selector").ClassList.Contains("expected-class")`. Verify that dynamic style provider methods (e.g., `GetSizeCss`, `GetVariantCss`) are correctly applying classes.
     * **ARIA Attributes:** Assert that ARIA attributes change correctly based on parameters (e.g., `aria-expanded`, `aria-pressed`, `aria-current`).
     * **Conditional Rendering:** Assert elements are rendered or hidden based on boolean parameters (e.g., `ShowIcon`, `Dismissable`).
     * **Content Parameters:**

       * Test `Label` parameter vs. `ChildContent` precedence if applicable.
       * Verify `RenderFragment` parameters like `ChildContent`, `LeadingIcon`, `TrailingIcon`, `HeaderContent`, `FooterContent` are rendered correctly.
       * For icons (`SvgIcon?`), assert that `<Blazicon Svg="@IconParameter" ... />` is rendered with appropriate classes and `aria-hidden="true"` if decorative.

  3. **Accessibility Tests:**

     * Explicitly verify that `AriaLabel` parameter, when set, overrides any default.
     * Verify that if `AriaLabel` is *not* set, the component applies a default localized ARIA label (retrieved via `Localizer["ComponentName.DefaultAriaLabel"]`).
     * Check for other critical ARIA attributes relevant to the component's role.

  4. **Interaction Tests (if applicable):**

     * For interactive components (e.g., buttons, toggles), simulate user actions:

       * `cut.Find("button").Click()`
       * `cut.Find("input").Change("new value")`
     * Assert that `EventCallback` parameters are invoked.
     * Assert that component state (if exposed or reflected in markup/ARIA) changes as expected.

     **SSR-only clarification for §11.4 (CRITICAL):**

     * The above interaction patterns must **not** be used to validate Blazor client interactivity in RizzyUI.
     * If included at all, interaction simulations must be limited to verifying **static markup** outcomes or server-side-rendered differences driven by parameters, not client-side state changes in .NET.

  5. **Styling and `AdditionalAttributes`:**

     * Verify that classes passed via `AdditionalAttributes` (e.g., `<RzFancyThing class="my-custom-style">`) are correctly merged into the root element's class list.
     * Verify other `AdditionalAttributes` are passed through to the root element.

  6. **Localization of Defaults:**

     * Test that default text values (e.g., for `AriaLabel`, `Placeholder`, `Title` if not provided by user) are correctly sourced from `Localizer["ComponentName.ResourceKey"]`.

* **bUnit Assertions:**

  * Use `cut.Find("css-selector")` to locate elements.
  * Use `element.MarkupMatches("expected html")` for precise structural and attribute checks. Use `diff.MissingAttributes` or `diff.MissingChildren` from the diff result for debugging.
  * Use `element.ClassList` for asserting CSS classes.
  * Use `element.GetAttribute("attribute-name")` for asserting attribute values.
  * Use `cut.Instance` to access the component instance's properties and methods if needed (e.g., to check the `_assets` field).

* **Output Format:**

  * When tests are requested, the generated `.cs` test file(s) should be included within the `<files>` block of the `output ... ` section, just like component files.

---

## 12. Documentation Guidelines (RizzyUI.Docs)

**Any component that is created or modified MUST have a corresponding documentation page in `src/RizzyUI.Docs`.** This ensures the documentation site remains the source of truth for all API surfaces.

### 12.1 Page Contract and Layout (Mandatory Skeleton)

Every documentation page **MUST** use the following Razor skeleton exactly. You must preserve the `SideContent`/`MainContent` nesting, the `RzQuickReference` component, and the specific HTMX attributes on the breadcrumb links.

```razor
@page "/components/your-component-kebab-name"
@namespace RizzyUI.Pages
@using RizzyUI
@using Rizzy.Htmx
@inherits RzComponent

<PageTitle>Your Component Name</PageTitle>

<RzQuickReferenceContainer>
    <RzArticle ProseWidth="ProseWidth.UltraWide">
        <SideContent>
            <RzQuickReference />
        </SideContent>
        <MainContent>
            <RzBreadcrumb class="mb-4 not-prose">
                <BreadcrumbList>
                    <BreadcrumbItem>
                        <BreadcrumbLink Href="/" hx-boost="true" hx-select="#content" hx-target="#content" hx-swap="outerHTML">Docs</BreadcrumbLink>
                    </BreadcrumbItem>
                    <BreadcrumbSeparator />
                    <BreadcrumbItem>
                        <BreadcrumbLink Href="/components" hx-boost="true" hx-select="#content" hx-target="#content" hx-swap="outerHTML">Components</BreadcrumbLink>
                    </BreadcrumbItem>
                    <BreadcrumbSeparator />
                    <BreadcrumbItem>
                        <BreadcrumbPage>Your Component Name</BreadcrumbPage>
                    </BreadcrumbItem>
                </BreadcrumbList>
            </RzBreadcrumb>

            <RzHeading Level="HeadingLevel.H1" QuickReferenceTitle="Your Component Name" class="scroll-mt-20">Your Component Name</RzHeading>
            
            <RzParagraph>
                <!-- 1. What the component is for. 2. Subcomponents involved. 3. Interactivity details (Alpine/HTMX). -->
            </RzParagraph>

            <!-- Optional: "Under the Hood" Alert for implementation details -->
            <!-- <RzAlert ...><AlertTitle>Under the Hood</AlertTitle>...</RzAlert> -->

            <!-- REPEATABLE PATTERN: Usage Examples (Must include RzBrowser AND RzCodeViewer) -->
            <section class="my-8 py-2">
                <RzHeading Level="HeadingLevel.H2" QuickReferenceTitle="Basic Usage" class="scroll-mt-20">
                    Basic Usage
                </RzHeading>
                <RzParagraph>
                    <!-- Description of the specific example -->
                </RzParagraph>

                <RzBrowser Layout="typeof(PreviewLayout)">
                    <div class="mx-auto p-8 mb-5 flex justify-center items-center min-h-40">
                        <!-- LIVE DEMO MARKUP HERE -->
                    </div>
                </RzBrowser>

                <RzCodeViewer Language="@CodeLanguage.Razor" class="mb-10">
                    <!-- ESCAPED SOURCE CODE OF THE DEMO ABOVE HERE -->
                </RzCodeViewer>
            </section>

            <!-- REPEATABLE PATTERN: Component Parameters (Must use specific table styling) -->
            <section class="my-8 py-2">
                <RzHeading Level="HeadingLevel.H2" QuickReferenceTitle="Component Parameters" class="scroll-mt-20">
                    Component Parameters
                </RzHeading>
                <div class="not-prose mt-6 mb-10 overflow-hidden rounded-md border border-zinc-200 dark:border-zinc-800">
                    <table class="w-full text-sm">
                        <thead class="text-left bg-zinc-100/75 font-semibold text-zinc-700 dark:bg-zinc-900/50 dark:text-zinc-300">
                            <tr>
                                <th scope="col" class="w-1/5 px-4 py-3">Property</th>
                                <th scope="col" class="w-2/5 px-4 py-3">Description</th>
                                <th scope="col" class="w-1/5 px-4 py-3">Type</th>
                                <th scope="col" class="px-4 py-3">Default</th>
                            </tr>
                        </thead>
                        <tbody class="divide-y divide-zinc-200 dark:divide-zinc-800">
                            <tr class="align-top">
                                <td class="px-4 py-4"><code class="font-mono font-medium text-sky-600 dark:text-sky-400">PropertyName</code></td>
                                <td class="px-4 py-4 text-zinc-600 dark:text-zinc-400">Description of property.</td>
                                <td class="px-4 py-4"><code class="font-mono text-violet-600 dark:text-violet-400">string</code></td>
                                <td class="px-4 py-4 text-zinc-500"><code class="font-mono">null</code></td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </section>

        </MainContent>
    </RzArticle>
</RzQuickReferenceContainer>

```

### 12.2 Top-of-page Content

* **H1 then “One-Paragraph Contract”:**

  * Immediately after the H1, include a short paragraph covering:

    * What the component is for (use-cases).
    * What the “suite” is composed of (subcomponents).
    * What provides interactivity (Alpine, not Blazor runtime interactivity).
    * Any notable integration hooks (stable IDs, HTMX targeting, events).
* **“Under the Hood” Alert:**

  * Add an info alert near the top explaining implementation details:

    * Alpine `x-data="<name>"`.
    * Teleport strategy (e.g., `x-teleport="body"`).
    * Focus trapping / escape / backdrop click behaviors.
  * Keep it practical: mention the consequence (e.g., avoids z-index issues, enables predictable DOM placement).

### 12.3 Section Structure (Repeatable Pattern)

Every H2 section should follow this mini-template:

1. **H2 + Explanation:**

   * `<RzHeading Level="HeadingLevel.H2" QuickReferenceTitle="…" class="scroll-mt-20">`
   * **Note:** Always include `class="scroll-mt-20"` on headings for scroll positioning.
   * Short paragraph naming the scenario, stating what the example demonstrates, and mentioning relevant parameters.
2. **Live Demo Region:**

   * Provide a centered demo container (e.g., `mx-auto p-8 mb-5 flex justify-center items-center min-h-40`).
   * Demos should be minimal but real.
3. **Matching Code Block:**

   * Immediately follow each demo with an `RzCodeViewer` containing the exact markup used.
   * If multiple snippets exist, add `ViewerTitle` (e.g., “Blazor Component”, “Controller Action”).
   * **Copy/Paste Safe:** Show all required attributes (`AsChild`, `hx-*`, ids/targets).
4. **Progressive Complexity:**

   * Order sections from simplest to most integrated: Basic Usage → Appearance Customization → Integration (HTMX) → Advanced Flows.

### 12.4 Environment Limitations

* **Explicit Limitation Alerts:** If an example cannot work in the docs environment (e.g., requires a real backend endpoint not present in the static docs site), add a warning alert immediately before the demo.
* The alert must state what will *not* happen, what *would* happen in a real app, and what the developer should copy.

### 12.5 Parameter and API Reference

* **Parameters Section (Mandatory):**

  * Include a “Component Parameters” H2 near the bottom.
  * Break down by component type (e.g., `RzDialog`, `DialogContent`).
  * Tables must include: **Property, Description, Type, Default**.
  * Clearly mark required values (“Required” pill).
* **Alpine API Section:**

  * If the component exposes/relies on an Alpine API, include a table with: **Method, Parameters, Description**.
* **Event Names & Interoperability:**

  * Document event names, default values, and how HTMX/server code triggers them.

### 12.6 Example Quality Standards

* **Happy Path:** Basic examples must show the trigger, content surface, close mechanism, and exit strategies (Escape, backdrop).
* **Customization:** Show at least one example changing a meaningful parameter (size, visibility).
* **Integration:** When showing HTMX patterns, include both client markup (`hx-get`, `hx-target`) and server endpoint/controller samples.

### 12.7 Consistency

* **Terminology:** Pick one term (Dialog vs Modal) and explain the relationship.
* **Naming:** Component/parameter names in inline `<code>` must match API casing exactly.
* **Quick Reference:** Set `QuickReferenceTitle` on headers. Keep titles short and task-oriented.

### 12.8 Updating ComponentList.razor

* Any new component must be added to the side navigation in `src/RizzyUI.Docs/Components/Layout/ComponentList.razor`.
* This ensures the new documentation page is discoverable.

---

## 13. The output block example (canonical)

When asked to generate `RzFancyThing` (a non-generic component):

```output
<files>
  <file path="src/RizzyUI/Components/Fancy/RzFancyThing/RzFancyThing.razor">…</file>
  <file path="src/RizzyUI/Components/Fancy/RzFancyThing/RzFancyThing.razor.cs">…</file>
  <file path="src/RizzyUI.Docs/Components/Pages/Components/FancyThingInfo.razor">…</file>
</files>
```

When asked to generate `RzGenericThing<TItem>`:

```output
<files>
  <file path="src/RizzyUI/Components/Generic/RzGenericThing/RzGenericThing.razor">…</file>
  <file path="src/RizzyUI/Components/Generic/RzGenericThing/RzGenericThing.razor.cs">…</file>
  <file path="src/RizzyUI/Components/Generic/RzGenericThing/Styling/RzGenericThingStyles.cs">…</file>
  <file path="src/RizzyUI.Docs/Components/Pages/Components/GenericThingInfo.razor">…</file>
</files>
```

---

## 14. What **not** to place in the `output` block

Changes to **global theme scaffolding** (`RzTheme.cs`, `RzTheme.StyleProviders.cs`), **configuration** (`RizzyUIConfig.cs`, `ServiceCollectionExtensions.cs`), **localization resource files** (`RizzyLocalization.resx`), and **navigation menus** (`ComponentList.razor`) go in a **separate, preface section** that appears *before* the `output` block.
That section must identify each existing file and show the lines/entries to insert, either as a diff or as verbatim code snippets/tables.
Never embed these edits in `<file>` tags because CI merges them manually.

**Agent-mode note (direct repo edits):**

* If you are acting as an agent and can modify files directly, you may apply these edits in-place, but you must still present them as **clearly delineated cross-file edits** (diff-style or explicit snippets) in your report so maintainers can review them quickly.

---

## 15. Theme, Localization, Asset, and Documentation Integration (cross-file edits)

Whenever a new component is introduced, instruct the user accordingly:

**Manual Edits Required for Integration:**

**Theme Integration:**

1. **Add to `src/RizzyUI/RzTheme.StyleProviders.cs`**:

   ```csharp
   // For a non-generic component
   public virtual TvDescriptor<RzComponent<RzFancyThing.Slots>, RzFancyThing.Slots> RzFancyThing { get; set; }

   // For a generic component
   public virtual TvDescriptor<RzComponent<RzGenericThingSlots>, RzGenericThingSlots> RzGenericThing { get; set; }
   ```

2. **Add to `src/RizzyUI/RzTheme.cs` constructor**:

   ```csharp
   // For a non-generic component
   RzFancyThing = RizzyUI.RzFancyThing.DefaultDescriptor;

   // For a generic component
   RzGenericThing = RizzyUI.RzGenericThingStyles.DefaultDescriptor;
   ```

**Localization:**

Please add the following English (default culture) entries to `src/RizzyUI/Resources/RizzyLocalization.resx`:

| Name                            | Value                       | Comment (Optional)                    |
| ------------------------------- | --------------------------- | ------------------------------------- |
| `RzFancyThing.DefaultAriaLabel` | `Fancy interactive element` | `Default ARIA label for RzFancyThing` |

**Asset Management Integration:**

Please add the following default asset URLs to the `PostConfigure` action in `src/RizzyUI/Extensions/ServiceCollectionExtensions.cs`:

| Key (`string`)           | URL (`string`)                                                    |
| ------------------------ | ----------------------------------------------------------------- |
| `"FancyThingCoreScript"` | `"https://cdn.jsdelivr.net/npm/fancylib@1.2.3/dist/fancy.min.js"` |

**JavaScript Integration (if a new Alpine component `rzFancyThing` was created):**

1. **Create the component file** in:

   ```text
   packages/rizzyui/src/js/lib/components/rzFancyThing.js
   ```

2. **Add the component to the owning bundle manifest** in:

   ```text
   packages/rizzyui/src/js/runtime/componentBundleManifest.js
   ```

   Example:

   ```javascript
   rzFancyThing: 'core-common',
   ```

3. **Export the component from the owning bundle file**, for example:

   ```text
   packages/rizzyui/src/js/bundles/core-common.js
   ```

   ```javascript
   export { default as rzFancyThing } from '../lib/components/rzFancyThing.js';
   ```

4. **If a new bundle is introduced**, also add it to:

   ```text
   packages/rizzyui/src/js/runtime/bundleLoaderRegistry.js
   ```

   Example:

   ```javascript
   'my-new-runtime': () => import('../bundles/my-new-runtime.js'),
   ```

5. **Do not add new components to `rizzyui.js`, `rizzyui-csp.js`, or any legacy `components.js` registry.** The shell runtime plus Async Alpine manifest-driven loading is the required architecture.

**Documentation Navigation (if new component):**

1. **Add to `src/RizzyUI.Docs/Components/Layout/ComponentList.razor`**:

   ```razor
   <RzSideNavLink Href="components/fancy-thing">Fancy Thing</RzSideNavLink>
   ```

---

## 16. Final checklist for the LLM

* CRITICAL - Only generate or modify code directly related to the task requested. You are not permitted to modify code outside the scope of the request.
* **Component Naming:** Ensure only root-level components are prefixed with `Rz`.
* Prepend the cross-file edit instructions for theme, localization, asset management, and **documentation navigation** if needed (§15).
* Provide an `output` block for new or replaced component-specific files **and documentation pages** only (§1, §12, §13).
* Use the root element pattern (§2) and Alpine child-container convention if Alpine is used (§10.2).
* `.razor` files: Use `@inherits RzComponent<...>` and `SlotClasses.Get...()` for all classes (§3, §5).
* `.razor.cs` files:

  * Start with `/// <summary>...</summary>` for the class and public members.
  * Inherit from `RzComponent<TSlots>` or `RzAsChildComponent<TSlots>`.
  * **For non-generic components:** Define `Slots` and `DefaultDescriptor` inside the class.
  * **For generic components:** Implement the `IHas...StylingProperties` interface.
  * Implement `protected override TvDescriptor<...> GetDescriptor() => Theme.ComponentName;`.
  * **Ensure `RootClass()` method is NOT present.**
  * Handle default localized strings for parameters like `AriaLabel` (§9).
* Styling files (`Styling/ComponentNameStyles.cs` for generics):

  * Define the non-generic `Slots` class.
  * Define the `static class` containing the `DefaultDescriptor`.
  * Variant expressions in the descriptor **MUST** cast to the `IHas...StylingProperties` interface.
* Alpine.js: Strictly adhere to API restrictions by using named `x-data` components, key-based property and method references, and the bundle-based Async Alpine architecture.
* Documentation: Ensure the generated documentation page (`Info.razor`) strictly follows the layout, structure, and content rules in §12.
* Include unit tests *only* when specifically requested (§11).
* Adhere to all specified conventions and avoid manual concatenation of class strings.
* Do not include comments in Razor markup or using statements. Any comments in code blocks should be production-ready.
* If Alpine is used, ensure the Alpine root element includes `x-data` and `x-load="@LoadStrategy"` on the same element.
* Emit `x-load` only when `LoadStrategy` is non-empty.
* Ensure the component’s Alpine name is added to `componentBundleManifest.js`.
* Ensure the component is exported from exactly one bundle file in `packages/rizzyui/src/js/bundles/`.
* Do not reintroduce eager global component registration.
* Do not modify build artifacts in `packages/rizzyui/dist` or `src/RizzyUI/wwwroot`.
* If the component has no meaningful client-side behavior, prefer no Alpine runtime at all and do not add it to the bundle graph.

**SSR-only enforcement (CRITICAL):**

* Do not implement Blazor interactive patterns in components. All interactivity is Alpine/HTMX (see §10).

**Agent-only enforcement (CRITICAL):**

* AGENTS ONLY — run `npm install` in any directory containing `packages.json` (and do not skip equivalent Node manifest directories) except if it has a path prefixed with `src/RizzyUI/wwwroot/vendor/`.

---

### **Final Sign-Off Checklist (Version 3.5)**

#### **Part A: LLM Automated Verification Checklist**

* **[ ] 1. `Slots` Class Definition:**

  * For **non-generic** components: The `.razor.cs` file contains a `public sealed partial class Slots : ISlots`.
  * For **generic** components: The `Styling/{ComponentName}Styles.cs` file contains a `public sealed partial class {ComponentName}Slots : ISlots`.
* **[ ] 2. `Slots` Properties:** The `Slots` class has a `string?` property for *every* slot consumed by a `SlotClasses.Get...()` call in the `.razor` file.
* **[ ] 3. `[Slot]` Attribute:** Every property in every `Slots` class is decorated with `[Slot("kebab-case-name")]`.
* **[ ] 4. `DefaultDescriptor` Location:**

  * For **non-generic** components: The `.razor.cs` file contains a `public static readonly TvDescriptor`.
  * For **generic** components: The `Styling/{ComponentName}Styles.cs` file contains a `public static class {ComponentName}Styles` holding the `public static readonly TvDescriptor`.
* **[ ] 5. Descriptor Completeness:** The `DefaultDescriptor` provides a default class string for *every* slot defined in the `Slots` class.
* **[ ] 6. Interface Implementation (Generic Components Only):** For generic components, the component's `.razor.cs` file **implements** the `IHas...StylingProperties` interface.
* **[ ] 7. Styling File Structure (Generic Components Only):** The `Styling/{ComponentName}Styles.cs` file exists and contains the styling interface, the slots class, and the static styles class.
* **[ ] 8. Correct Variant Syntax:** All `variants` and `compoundVariants` that target a slot other than `Base` **MUST** use the `new() { [s => s.SlotName] = "..." }` syntax. For generic components, variant expressions **MUST** cast the component instance to the styling interface (e.g., `c => ((IHas...StylingProperties)c).PropertyName`).
* **[ ] 9. Nullable Enum Variant Type:** For nullable enum `[Parameter]`s used in variants, the `Variant<T, TSlots>` definition uses the **non-nullable** enum type for `T`.
* **[ ] 10. Inheritance:** The component's `.razor.cs` file inherits from `RzComponent<TSlots>` or `RzAsChildComponent<TSlots>`, where `TSlots` is the correct (and possibly non-generic) slots type.
* **[ ] 11. Correct `GetDescriptor` Implementation:** The component's `.razor.cs` file **MUST** contain the method `protected override TvDescriptor<...> GetDescriptor() => Theme.ComponentName;`.
* **[ ] 12. `RootClass()` Method Removed:** The `RootClass()` method has been completely removed from the component's `.razor.cs` file.
* **[ ] 13. Markup Inheritance:** The component's `.razor` file has the correct `@inherits` directive.
* **[ ] 14. Markup Class Attributes:** All `class` attributes in the `.razor` file have been updated to use the `SlotClasses.Get...()` accessors.
* **[ ] 15. `data-slot` on Root Element:** The root `HtmlElement` has a `data-slot="component-name"` attribute with a hardcoded, kebab-case name.
* **[ ] 16. `data-slot` on Internal Elements:** Every internal element with a `class="@SlotClasses.Get...()"` attribute also has a corresponding `data-slot="@...SlotNames.NameOf(...)"` attribute.
* **[ ] 17. Alpine Directives Preserved:** All non-class Alpine directives are present in the `.razor` file on their original elements.

#### **Part B: Documentation Verification Checklist**

* **[ ] 18. Documentation Page Exists:** A file in `src/RizzyUI.Docs/Components/Pages/Components/` exists and matches the component name.
* **[ ] 19. Structure Compliance:** The documentation page uses `RzQuickReferenceContainer`, `RzBreadcrumb`, and `RzCodeViewer` correctly.
* **[ ] 20. Content Compliance:** The documentation includes a Parameters table and (if applicable) Alpine API/Event details.
* **[ ] 21. Navigation Updated:** The new component is listed in `src/RizzyUI.Docs/Components/Layout/ComponentList.razor`.

#### **Part C: Human Developer Validation Checklist**

* **[ ] 22. Theme Integration:** Have the manual edits to `RzTheme.StyleProviders.cs` and `RzTheme.cs` been applied correctly?
* **[ ] 23. Obsolete Files Deleted:** Have the old `Default...Styles.cs` and `RzStylesBase...cs` files for the component been deleted?
* **[ ] 24. Build Success:** Does the entire `RizzyUI` solution build without errors?
* **[ ] 25. Unit Tests:** Do all existing unit tests for the component pass?
* **[ ] 26. Demo Application:** Visually confirm that the component renders and behaves exactly as it did before the refactor in the `RizzyUI.Docs` application.

```
