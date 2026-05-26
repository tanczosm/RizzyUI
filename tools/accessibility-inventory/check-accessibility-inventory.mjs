#!/usr/bin/env node
import path from 'node:path';
import { promises as fs } from 'node:fs';

function parseArgs(argv) {
  const options = {
    componentsDir: 'Components',
    docsDir: '../RizzyUI.Docs/Components/Pages/Components',
    testsDir: '../RizzyUI.Tests/Components',
    interactivePrefixes: ['Rz'],
    ignoreDirs: ['_Internal', 'obj', 'bin'],
    exts: ['.razor']
  };

  for (let i = 0; i < argv.length; i += 1) {
    const arg = argv[i];
    if (!arg.startsWith('--')) continue;

    const [key, inlineValue] = arg.slice(2).split('=');
    const value = inlineValue ?? argv[i + 1];
    if (inlineValue === undefined) i += 1;

    switch (key) {
      case 'components-dir':
        options.componentsDir = value;
        break;
      case 'docs-dir':
        options.docsDir = value;
        break;
      case 'tests-dir':
        options.testsDir = value;
        break;
      case 'interactive-prefixes':
        options.interactivePrefixes = value.split(',').map(s => s.trim()).filter(Boolean);
        break;
      case 'ignore-dirs':
        options.ignoreDirs = value.split(',').map(s => s.trim()).filter(Boolean);
        break;
      default:
        break;
    }
  }

  return options;
}

async function walk(dir, predicate, ignoreDirs) {
  const results = [];
  async function recurse(current) {
    const entries = await fs.readdir(current, { withFileTypes: true });
    for (const entry of entries) {
      const fullPath = path.join(current, entry.name);
      if (entry.isDirectory()) {
        if (ignoreDirs.includes(entry.name)) continue;
        await recurse(fullPath);
        continue;
      }

      if (predicate(entry.name, fullPath)) {
        results.push(fullPath);
      }
    }
  }

  await recurse(dir);
  return results;
}

function isRootLevelComponent(filePath, interactivePrefixes) {
  const baseName = path.basename(filePath, '.razor');
  if (!interactivePrefixes.some(prefix => baseName.startsWith(prefix))) return false;
  if (baseName.endsWith('.razor')) return false;
  return true;
}

function toDocStem(componentName) {
  return componentName.replace(/^Rz/, '');
}

function hasMatchingDoc(docsFiles, componentName) {
  const stem = toDocStem(componentName);
  return docsFiles.some(file => {
    const fileName = path.basename(file, '.razor');
    return fileName === `${stem}Info` || fileName === `${componentName}Info`;
  });
}

function hasMatchingTest(testFiles, componentName) {
  const bare = componentName;
  const stem = toDocStem(componentName);
  return testFiles.some(file => {
    const fileName = path.basename(file);
    return (
      fileName.includes(`${bare}Tests`) ||
      fileName.includes(`${bare}A11yTests`) ||
      fileName.includes(`${stem}A11y`) ||
      fileName.toLowerCase().includes(`${stem.toLowerCase()}.a11y.`)
    );
  });
}

export async function runInventoryCheck(options) {
  const components = await walk(
    options.componentsDir,
    name => options.exts.includes(path.extname(name)),
    options.ignoreDirs
  );

  const docsFiles = await walk(options.docsDir, name => name.endsWith('.razor'), options.ignoreDirs);
  const testFiles = await walk(options.testsDir, name => /Tests\.cs$|\.a11y\.spec\.(ts|js)$/i.test(name), options.ignoreDirs);

  const rootInteractiveComponents = components
    .filter(file => isRootLevelComponent(file, options.interactivePrefixes))
    .map(file => path.basename(file, '.razor'))
    .filter((value, index, self) => self.indexOf(value) === index)
    .sort();

  const report = rootInteractiveComponents.map(component => {
    const hasDoc = hasMatchingDoc(docsFiles, component);
    const hasTest = hasMatchingTest(testFiles, component);
    return { component, hasDoc, hasTest };
  });

  return report;
}

function printSummary(report) {
  console.log('Accessibility Inventory (non-blocking warning phase)');
  console.log('-----------------------------------------------------');

  for (const row of report) {
    const docStatus = row.hasDoc ? 'docs: ✅' : 'docs: ⚠️ missing';
    const testStatus = row.hasTest ? 'tests: ✅' : 'tests: ⚠️ missing';
    console.log(`- ${row.component}: ${docStatus}, ${testStatus}`);
  }

  const missing = report.filter(x => !x.hasDoc || !x.hasTest);
  if (missing.length > 0) {
    console.log('\nWarnings:');
    for (const row of missing) {
      const missingParts = [];
      if (!row.hasDoc) missingParts.push('accessibility documentation');
      if (!row.hasTest) missingParts.push('accessibility test');
      console.log(`⚠️ ${row.component} is missing ${missingParts.join(' and ')}.`);
    }
  } else {
    console.log('\n✅ All detected root-level interactive components have docs and tests.');
  }

  console.log('\nThis check is non-blocking in Phase 0.5 and will become blocking in Phase 5.');
}

if (import.meta.url === `file://${process.argv[1]}`) {
  const options = parseArgs(process.argv.slice(2));
  runInventoryCheck(options)
    .then(report => {
      printSummary(report);
      process.exit(0);
    })
    .catch(error => {
      console.error('⚠️ Accessibility inventory check encountered an error:', error.message);
      process.exit(0);
    });
}
