import { copyFile, mkdir } from 'node:fs/promises';
import { dirname, resolve } from 'node:path';

const projectRoot = resolve(import.meta.dirname, '..');

const assets = [
  ['node_modules/@docsearch/js/dist/umd/index.js', 'wwwroot/vendor/docsearch/docsearch.js'],
  ['node_modules/@docsearch/css/dist/style.css', 'wwwroot/vendor/docsearch/docsearch.css'],
  ['node_modules/htmx.org/dist/htmx.min.js', 'wwwroot/vendor/htmx/htmx.min.js'],
];

for (const [source, destination] of assets) {
  const sourcePath = resolve(projectRoot, source);
  const destinationPath = resolve(projectRoot, destination);
  await mkdir(dirname(destinationPath), { recursive: true });
  await copyFile(sourcePath, destinationPath);
}
