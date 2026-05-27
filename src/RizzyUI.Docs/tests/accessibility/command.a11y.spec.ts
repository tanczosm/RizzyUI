import { test, expect } from '@playwright/test';
import AxeBuilder from '@axe-core/playwright';

test('command combobox/listbox behavior and accessibility', async ({ page }) => {
  await page.goto('/components/command');

  const input = page.locator('[data-slot="command-input"]').first();
  await input.focus();

  await expect(input).toHaveAttribute('role', 'combobox');
  await expect(input).toHaveAttribute('aria-controls', /rz-command-list-/);

  await input.press('ArrowDown');
  const activeId = await input.getAttribute('aria-activedescendant');
  expect(activeId).toBeTruthy();
  await expect(input).toBeFocused();

  await input.fill('zzzz-no-match');
  await expect(input).toHaveAttribute('aria-expanded', 'false');

  await input.fill('');
  await input.press('a');
  const typeaheadActive = await input.getAttribute('aria-activedescendant');
  expect(typeaheadActive).toBeTruthy();

  await input.press('Escape');
  await expect(input).toHaveAttribute('aria-expanded', 'false');

  const axe = await new AxeBuilder({ page }).analyze();
  expect(axe.violations).toEqual([]);
});
