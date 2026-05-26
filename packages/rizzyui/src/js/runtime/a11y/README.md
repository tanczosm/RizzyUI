# RizzyUI runtime accessibility primitives

This folder centralizes shared, SSR-safe accessibility runtime primitives used by multiple RizzyUI components.

## Purpose

- Provide reusable building blocks for focus management, dismissable layers, roving focus, typeahead navigation, and announcements.
- Keep accessibility behavior consistent across components without relying on Blazor interactive events.
- Preserve compatibility with both `rizzyui.js` and `rizzyui-csp.js` bundles by using static ES module imports/exports only.

## Conventions

- Use one primitive per file in this folder.
- Use descriptive file names that match the primitive name.
- Use named exports only; do not use default exports.
- Keep primitives framework-agnostic and dependency-free.
- Keep APIs serializable and stable where events are involved.

## Planned primitives

- `focusScope.js`: Focus containment, initial focus, and focus restore helpers.
- `dismissableLayer.js`: Outside interaction + escape-key dismissal orchestration.
- `rovingFocusGroup.js`: Keyboard roving tabindex/focus movement helpers.
- `typeaheadNavigator.js`: Typeahead matching and active-item navigation.
- `ariaAnnouncer.js`: Shared live-region announcement scheduling and verbosity control.

- `focusable.js`: Shared helpers to evaluate focusable/tabbable elements and move focus to first/last valid target.

## focusable.js API

- `getFocusableElements(root)`: Returns focusable descendants within `root` only; skips hidden, disabled, inert, and `aria-hidden` nodes.
- `isFocusable(element)`: True when an element can receive focus programmatically (including `tabindex="-1"`).
- `isTabbable(element)`: True when an element is included in sequential Tab order (`tabindex >= 0` when set).
- `focusFirst(root)`: Focuses and returns the first focusable descendant.
- `focusLast(root)`: Focuses and returns the last focusable descendant.

Caveat: Browser-specific edge cases around details/summary and SVG focus behavior can vary slightly by engine, so these helpers intentionally enforce a conservative, cross-browser baseline.


## focusScope.js API

- `createFocusScope(container, options?)`: Creates a scope with `activate()`, `deactivate()`, and `isActive()` methods.
- `options.initialFocus`: Selector or element that receives initial focus when the scope is activated.
- `options.fallbackFocus`: Selector or element used when no focusable descendants exist or when restore target is unavailable.
- `options.throwOnNoFocusable`: When `true`, activation throws if no focusable descendants are available.

### Usage

```js
import { createFocusScope } from './focusScope.js';

const scope = createFocusScope(dialogElement, {
  initialFocus: '[data-autofocus]',
  fallbackFocus: '[data-close-button]',
});

scope.activate();

// ...later, when closing
scope.deactivate();
```

### Behavior notes

- On activation, the scope captures `document.activeElement` so focus can be restored on close.
- Tab/Shift+Tab navigation is trapped inside the container.
- Nested scopes are supported; only the most recently activated scope traps keyboard tabbing.
- On deactivation, focus restores to the opener when possible, then falls back to `fallbackFocus`, then `document.body`.


## dismissableLayer.js API

- `registerDismissableLayer(options)`: Registers a layer and returns `unregister()`.
- `options.root`: Required root element for the overlay.
- `options.onDismiss(context)`: Required callback fired when dismissal is confirmed.
- `options.onEscape(event)`: Optional hook invoked before escape-driven dismissal.
- `options.onOutsidePointer(event)`: Optional hook invoked before outside pointer dismissal.
- `options.onOutsideFocus(event)`: Optional hook invoked before outside focus dismissal.
- `options.dismissOnOutsideFocus`: Set `true` to dismiss on `focusin` transitions outside the root.
- `createDismissableLayer()`: Returns `{ registerLayer }` for consumers that want a tiny manager object.

### Public event contract

When dismissal is attempted on the active top layer, the runtime dispatches:

- `rz:dismiss` (cancelable, bubbles) on the layer root.
- `detail.reason`: one of `escape`, `outside-pointer`, or `outside-focus`.
- `detail.layerId`: the registered layer id.
- `detail.originalEvent`: the source DOM event.

Any listener can call `event.preventDefault()` on `rz:dismiss` to cancel dismissal (for example unsaved changes checks).

### Example: basic layer registration

```js
import { registerDismissableLayer } from './dismissableLayer.js';

const unregister = registerDismissableLayer({
  root: dialogElement,
  onDismiss: ({ reason }) => {
    if (reason === 'escape' || reason === 'outside-pointer') {
      closeDialog();
    }
  }
});

// when closed/disposed
unregister();
```

### Example: nested overlays

```js
const unregisterDialog = registerDismissableLayer({
  root: dialogEl,
  onDismiss: () => closeDialog()
});

const unregisterMenu = registerDismissableLayer({
  root: menuEl,
  onDismiss: () => closeMenu()
});

// Escape now dismisses menu first because it is topmost.
// After menu unregisters, Escape dismisses dialog.
```

### Example: prevent dismissal

```js
dialogElement.addEventListener('rz:dismiss', (event) => {
  if (hasUnsavedChanges()) {
    event.preventDefault();
  }
});
```
