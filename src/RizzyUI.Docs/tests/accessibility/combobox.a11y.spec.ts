import { expect, test, type Locator } from '@playwright/test';

async function typeComboboxQuery(input: Locator, query: string) {
  await input.focus();
  await input.pressSequentially(query);
  await expect(input).toHaveValue(query);
}

test.describe('Tom Select combobox accessibility baseline', () => {
  test('supports accessible name, search, keyboard navigation, selection, escape, and disabled options', async ({ page }) => {
    await page.goto('/accessibility-combobox.html');
    const available = await page.evaluate(() => typeof window['TomSelect'] === 'function');
    test.skip(!available, 'Tom Select CDN asset was unavailable in this environment.');
    await page.waitForFunction(() => !!window['__ts']);

    const input = page.locator('.ts-wrapper input').first();
    await expect(input).toHaveAttribute('aria-labelledby', 'fruit-label');
    await expect(input).toHaveAttribute('aria-describedby', 'fruit-help');

    await typeComboboxQuery(input, 'ora');
    await expect(page.locator('.ts-dropdown .option[data-value="orange"]')).toBeVisible();
    await page.keyboard.press('Enter');

    await expect(page.locator('#fruit-select')).toHaveValue('orange');

    await typeComboboxQuery(input, 'cher');
    await expect(page.locator('.ts-dropdown .option[data-value="cherry"]')).toBeVisible();
    await page.keyboard.press('ArrowDown');
    await page.keyboard.press('Enter');

    await expect(page.locator('#fruit-select')).not.toHaveValue('cherry');

    await input.focus();
    await page.keyboard.press('Escape');
    await expect(page.locator('.ts-dropdown')).toBeHidden();
  });

  test('supports clean teardown and re-initialization', async ({ page }) => {
    await page.goto('/accessibility-combobox.html');
    const available = await page.evaluate(() => typeof window['TomSelect'] === 'function');
    test.skip(!available, 'Tom Select CDN asset was unavailable in this environment.');
    await page.waitForFunction(() => typeof window['replaceComboboxControl'] === 'function');

    await page.evaluate(() => {
      window['replaceComboboxControl']();
    });

    const input = page.locator('.ts-wrapper input').first();
    await expect(input).toBeVisible();
    await typeComboboxQuery(input, 'ban');
    await expect(page.locator('.ts-dropdown .option[data-value="banana"]')).toBeVisible();
    await page.keyboard.press('Enter');

    await expect(page.locator('#fruit-select')).toHaveValue('banana');
  });
});
