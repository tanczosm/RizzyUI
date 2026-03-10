import { componentBundleManifest } from './componentBundleManifest.js';
import { bundleLoaderRegistry } from './bundleLoaderRegistry.js';

const bundleLoadCache = new Map();
const bundleRegistrationState = new Map();

function getComponentFactory(Alpine, componentName) {
    try {
        return Alpine.data(componentName);
    } catch {
        return null;
    }
}

async function ensureBundleRegistered(Alpine, bundleName) {
    if (bundleRegistrationState.get(bundleName) === true) {
        return;
    }

    if (!bundleLoadCache.has(bundleName)) {
        const loader = bundleLoaderRegistry[bundleName];
        if (!loader) {
            throw new Error(`[RizzyUI] No bundle loader registered for '${bundleName}'.`);
        }

        bundleLoadCache.set(bundleName, loader().then((module) => {
            if (bundleRegistrationState.get(bundleName) !== true) {
                if (typeof module.register !== 'function') {
                    throw new Error(`[RizzyUI] Bundle '${bundleName}' must export a register(Alpine) function.`);
                }
                module.register(Alpine);
                bundleRegistrationState.set(bundleName, true);
            }
            return module;
        }).catch((error) => {
            bundleLoadCache.delete(bundleName);
            throw error;
        }));
    }

    await bundleLoadCache.get(bundleName);
}

export async function resolveAsyncComponentFactory(Alpine, componentName) {
    const bundleName = componentBundleManifest[componentName];

    if (!bundleName) {
        throw new Error(`[RizzyUI] No owning bundle found for Alpine component '${componentName}'.`);
    }

    await ensureBundleRegistered(Alpine, bundleName);

    const factory = getComponentFactory(Alpine, componentName);
    if (typeof factory !== 'function') {
        throw new Error(`[RizzyUI] Bundle '${bundleName}' loaded but did not register Alpine.data('${componentName}').`);
    }

    return factory;
}

export function registerAsyncBundleLoaders(Alpine) {
    if (typeof Alpine.asyncData !== 'function') {
        throw new Error('[RizzyUI] AsyncAlpine is required: Alpine.asyncData is unavailable.');
    }

    Object.keys(componentBundleManifest).forEach((componentName) => {
        Alpine.asyncData(componentName, async () => {
            return resolveAsyncComponentFactory(Alpine, componentName);
        });
    });
}
