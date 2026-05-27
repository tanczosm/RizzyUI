# Command Accessibility

## Pattern
RzCommand implements a WAI-ARIA combobox + listbox command palette pattern.

## Semantics
- Input uses `role="combobox"`, `aria-expanded`, `aria-controls`, `aria-activedescendant`, and `aria-autocomplete="list"`.
- Results container uses `role="listbox"` and stable id linked by `aria-controls`.
- Result rows use `role="option"` with stable ids and `aria-selected`.

## Keyboard
- Typing filters commands.
- `ArrowUp` / `ArrowDown` moves active option.
- `Home` / `End` moves to first/last option.
- `Enter` executes active command.
- `Escape` closes command palette.
- `Tab` / `Shift+Tab` leaves palette.

## Focus management
DOM focus remains on the input. Visual and SR active option state uses `aria-activedescendant`; active option is scrolled into view.

## Screen reader behavior
- Active option changes are exposed through `aria-activedescendant`.
- Result count is politely announced via shared `liveAnnouncer` only when transitioning between zero and non-zero results.
- Input has SR instructions via `aria-describedby`.

## SSR considerations
The component remains SSR-first and reattaches Alpine/runtime primitives (`activeDescendant`, `typeahead`, `dismissableLayer`) after enhanced navigation updates.

## Limitations
- No async incremental loading semantics beyond current synchronous list behavior.
- No grouped announcement summaries.

## Testing
- bUnit checks SSR markup and data attributes.
- Playwright coverage validates keyboard interactions, ARIA state, and no Axe critical violations for the command page.
