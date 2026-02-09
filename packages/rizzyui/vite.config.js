import { defineConfig } from 'vite';
import path from 'node:path';
import copy from 'rollup-plugin-copy';

const ROOT           = __dirname;
const SRC_DIR        = path.resolve(ROOT, 'src/js');
const DIST_DIR       = path.resolve(ROOT, 'dist/js');
const TARGET_WWWROOT = path.resolve(ROOT, '../../src/RizzyUI/wwwroot/js');

const entryName   = process.env.ENTRY || 'rizzyui';
const isMinified  = process.env.MINIFY === 'true';

export default defineConfig({
    build: {
        emptyOutDir: !isMinified && entryName === 'rizzyui',
        outDir: DIST_DIR,
        sourcemap: !isMinified,
        minify: isMinified ? 'esbuild' : false,
        target: 'esnext',

        lib: {
            entry: path.resolve(SRC_DIR, `${entryName}.js`),
            fileName: () => `${entryName}${isMinified ? '.min' : ''}`,
            formats: ['es']
        },

        rollupOptions: {
            external: ['htmx.org'],
            output: {
                globals: { 'htmx.org': 'htmx' },
                entryFileNames: '[name].js',
                chunkFileNames: 'chunks/[name]-[hash].js',
            },
            plugins: [
                copy({
                    targets: [{ src: `${DIST_DIR}/*`, dest: TARGET_WWWROOT }],
                    hook: 'writeBundle',
                    overwrite: true
                })
            ]
        }
    }
});
