import test from 'node:test';
import assert from 'node:assert/strict';
import os from 'node:os';
import path from 'node:path';
import { promises as fs } from 'node:fs';
import { runInventoryCheck } from '../check-accessibility-inventory.mjs';

async function mk(filePath) {
  await fs.mkdir(path.dirname(filePath), { recursive: true });
  await fs.writeFile(filePath, '');
}

test('detects missing accessibility docs and tests for root-level interactive components', async () => {
  const root = await fs.mkdtemp(path.join(os.tmpdir(), 'rz-a11y-'));

  await mk(path.join(root, 'src/RizzyUI/Components/Form/RzGood/RzGood.razor'));
  await mk(path.join(root, 'src/RizzyUI/Components/Form/RzMissingDoc/RzMissingDoc.razor'));
  await mk(path.join(root, 'src/RizzyUI/Components/Form/RzMissingTest/RzMissingTest.razor'));
  await mk(path.join(root, 'src/RizzyUI.Docs/Components/Pages/Components/GoodInfo.razor'));
  await mk(path.join(root, 'src/RizzyUI.Docs/Components/Pages/Components/MissingTestInfo.razor'));
  await mk(path.join(root, 'src/RizzyUI.Tests/Components/Form/RzGoodTests.cs'));
  await mk(path.join(root, 'src/RizzyUI.Tests/Components/Form/RzMissingDocA11yTests.cs'));

  const report = await runInventoryCheck({
    componentsDir: path.join(root, 'src/RizzyUI/Components'),
    docsDir: path.join(root, 'src/RizzyUI.Docs/Components/Pages/Components'),
    testsDir: path.join(root, 'src/RizzyUI.Tests/Components'),
    interactivePrefixes: ['Rz'],
    ignoreDirs: ['_Internal', 'obj', 'bin'],
    exts: ['.razor']
  });

  const byComponent = Object.fromEntries(report.map(x => [x.component, x]));

  assert.equal(byComponent.RzGood.hasDoc, true);
  assert.equal(byComponent.RzGood.hasTest, true);
  assert.equal(byComponent.RzMissingDoc.hasDoc, false);
  assert.equal(byComponent.RzMissingDoc.hasTest, true);
  assert.equal(byComponent.RzMissingTest.hasDoc, true);
  assert.equal(byComponent.RzMissingTest.hasTest, false);
});
