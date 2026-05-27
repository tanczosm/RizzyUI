import { expect, test } from '@playwright/test';

test.describe('Tom Select combobox accessibility baseline', () => {
  test('supports accessible name, search, keyboard navigation, selection, escape, and disabled options', async ({ page }) => {
    await page.goto('/accessibility-combobox.html');

    const input = page.locator('.ts-wrapper input');
    await expect(input).toHaveAttribute('aria-labelledby', 'fruit-label');
    await expect(input).toHaveAttribute('aria-describedby', 'fruit-help');

    await input.focus();
    await input.fill('ora');
    await page.keyboard.press('ArrowDown');
    await page.keyboard.press('Enter');

    await expect(page.locator('#fruit-select')).toHaveValue('orange');

    await input.focus();
    await page.keyboard.press('ArrowDown');
    await page.keyboard.press('ArrowDown');
    await page.keyboard.press('ArrowDown');
    await page.keyboard.press('Enter');

    await expect(page.locator('#fruit-select')).not.toHaveValue('cherry');

    await input.focus();
    await page.keyboard.press('Escape');
    await expect(page.locator('.ts-dropdown')).toBeHidden();
  });

  test('supports clean teardown and re-initialization', async ({ page }) => {
    await page.goto('/accessibility-combobox.html');

    await page.evaluate(() => {
      (window as any).replaceComboboxControl();
    });

    const input = page.locator('.ts-wrapper input');
    await input.focus();
    await input.fill('ban');
    await page.keyboard.press('ArrowDown');
    await page.keyboard.press('Enter');

    await expect(page.locator('#fruit-select')).toHaveValue('banana');
  });
});
