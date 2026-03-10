import { bundleLoaderRegistry } from './bundleLoaderRegistry.js';
import { componentBundleManifest } from './componentBundleManifest.js';

const registrationPromises = new Map();

function parseComponentNameFromXData(expression) {
    if (!expression) {
        return null;
    }

    const parsedName = expression.trim().split(/[({]/)[0]?.trim();
    return parsedName || null;
}

function getComponentFactory(Alpine, componentName) {
    const factory = Alpine.data(componentName);
    if (!factory) {
        throw new Error(`[RizzyUI] Alpine component '${componentName}' was requested but is not registered after bundle load.`);
    }
    return factory;
}

async function ensureBundleRegistered(Alpine, componentName) {
    const bundleName = componentBundleManifest[componentName];
    if (!bundleName) {
        throw new Error(`[RizzyUI] No owning bundle was found for component '${componentName}'.`);
    }

    if (!registrationPromises.has(bundleName)) {
        const loader = bundleLoaderRegistry[bundleName];
        if (!loader) {
            throw new Error(`[RizzyUI] Bundle loader '${bundleName}' is missing.`);
        }

        registrationPromises.set(bundleName, (async () => {
            const module = await loader();
            if (typeof module.register !== 'function') {
                throw new Error(`[RizzyUI] Bundle '${bundleName}' does not export register(Alpine).`);
            }
            await module.register(Alpine);
            return true;
        })());
    }

    return registrationPromises.get(bundleName);
}

export function registerAsyncBundleComponents(Alpine) {
    for (const componentName of Object.keys(componentBundleManifest)) {
        Alpine.asyncData(componentName, async () => {
            await ensureBundleRegistered(Alpine, componentName);
            return getComponentFactory(Alpine, componentName);
        });
    }
}

export async function preloadBundlesForDocument(Alpine, root = document) {
    if (!root || typeof root.querySelectorAll !== 'function') {
        return;
    }

    const componentNames = new Set();
    const xDataNodes = root.querySelectorAll('[x-data]');

    for (const node of xDataNodes) {
        const componentName = parseComponentNameFromXData(node.getAttribute('x-data'));
        if (componentName && componentBundleManifest[componentName]) {
            componentNames.add(componentName);
        }
    }

    await Promise.all([...componentNames].map(componentName => ensureBundleRegistered(Alpine, componentName)));
}

export async function loadComponentDefinition(Alpine, componentName) {
    await ensureBundleRegistered(Alpine, componentName);
    return getComponentFactory(Alpine, componentName);
}
