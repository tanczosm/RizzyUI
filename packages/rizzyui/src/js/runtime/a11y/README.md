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

These primitives are intentionally placeholders in this step and will be implemented in follow-up prompts.
