import { expect, test } from '@playwright/test';
import { expectActiveElement, expectRoleAndAccessibleName, pressKey, pressShiftTab, pressTab, runAxeScan } from './helpers/accessibility-helpers';

test.describe('Sheet accessibility contract', () => {
  test('modal sheet traps focus and restores focus on escape close', async ({ page }) => {
    await page.goto('/accessibility-sheet.html');
    const trigger = page.locator('#modal-trigger');
    await trigger.click();

    const modal = await expectRoleAndAccessibleName(page, 'dialog', 'Profile settings');
    await expect(modal).toHaveAttribute('aria-modal', 'true');
    await expect(modal).toHaveAttribute('aria-labelledby', 'modal-title');

    await expectActiveElement(page, page.locator('#modal-first'));
    await pressTab(page);
    await expectActiveElement(page, page.locator('#modal-last'));
    await pressTab(page);
    await expectActiveElement(page, page.locator('#modal-close'));
    await pressTab(page);
    await expectActiveElement(page, page.locator('#modal-first'));
    await pressShiftTab(page);
    await expectActiveElement(page, page.locator('#modal-close'));

    await pressKey(page, 'Escape');
    await expect(modal).toBeHidden();
    await expectActiveElement(page, trigger);
  });

  test('non-modal sheet does not trap focus and supports close controls', async ({ page }) => {
    await page.goto('/accessibility-sheet.html');
    await page.locator('#nonmodal-trigger').click();
    const nonModal = await expectRoleAndAccessibleName(page, 'complementary', 'Navigation drawer');
    await expect(nonModal).not.toHaveAttribute('aria-modal', 'true');

    await pressTab(page);
    await expectActiveElement(page, page.locator('#outside-target'));
    await page.locator('#nonmodal-close').click();
    await expect(nonModal).toBeHidden();
  });

  test('sheet fixture has no axe violations', async ({ page }) => {
    await page.goto('/accessibility-sheet.html');
    await runAxeScan(page);
  });
});
