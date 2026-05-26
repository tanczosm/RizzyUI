# Accessibility Inventory Check

RizzyUI includes a non-blocking accessibility inventory check that scans root-level interactive components and warns when either of these artifacts are missing:

- A component documentation page in `src/RizzyUI.Docs/Components/Pages/Components`
- A component accessibility test file in `src/RizzyUI.Tests/Components`

## Script location

- `tools/accessibility-inventory/check-accessibility-inventory.mjs`

## How detection works

The check recursively scans:

- Components: `src/RizzyUI/Components` for `.razor` files with root component names prefixed by `Rz`
- Docs: `src/RizzyUI.Docs/Components/Pages/Components` for `{ComponentWithoutRz}Info.razor` (or `{ComponentName}Info.razor`)
- Tests: `src/RizzyUI.Tests/Components` for files matching component test naming patterns, including:
  - `*Tests.cs`
  - `*A11yTests.cs`
  - `*.a11y.spec.ts` / `*.a11y.spec.js`

Ignored folders include `_Internal`, `obj`, and `bin`.

## Manual usage

From `src/RizzyUI`:

```bash
npm run check-accessibility-inventory
```

Optional overrides:

```bash
node ../../tools/accessibility-inventory/check-accessibility-inventory.mjs \
  --components-dir src/RizzyUI/Components \
  --docs-dir src/RizzyUI.Docs/Components/Pages/Components \
  --tests-dir src/RizzyUI.Tests/Components
```

## Phase behavior

This check is intentionally **non-blocking** in Phase 0.5. It always exits successfully and prints warnings for missing docs/tests.

In **Phase 5**, this check will be changed to fail the build when gaps are detected.


## Runtime primitive docs

For runtime accessibility primitive API guidance, examples, and caveats, see `docs/internal/runtime-primitives/README.md` and the per-primitive markdown files in that same directory.
