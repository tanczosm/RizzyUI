import collapse from '@alpinejs/collapse';
import intersect from '@alpinejs/intersect';
import focus from '@alpinejs/focus';
import AsyncAlpine from 'async-alpine';
import toast from './notify/toast.js';
import $data from './alpineData.js';
import props from './alpineProps.js';
import registerAsyncComponent from './alpineModuleRegistrar.js';
import registerMobileDirective from './directives/mobile.js';
import registerSyncDirective from './directives/sync-prop.js';
import { themeController } from './theme.js';
import { registerStores } from './stores.js';
import { require } from '../runtime/assetRequire.js';
import { registerAsyncBundleLoaders } from '../runtime/asyncBundleRegistrar.js';
import { registerValidationTrigger } from '../runtime/validationTrigger.js';

let cachedRizzyUI = null;

export function bootstrapRizzyUI(Alpine) {
    if (cachedRizzyUI) return cachedRizzyUI;

    Alpine.plugin(collapse);
    Alpine.plugin(intersect);
    Alpine.plugin(focus);
    Alpine.plugin(AsyncAlpine);

    registerAsyncBundleLoaders(Alpine);
    registerMobileDirective(Alpine);
    registerSyncDirective(Alpine);
    registerValidationTrigger(Alpine);

    if (typeof document !== 'undefined') {
        document.addEventListener('alpine:init', () => {
            registerStores(Alpine);
        });
    }

    cachedRizzyUI = {
        Alpine,
        require,
        toast,
        $data,
        props,
        registerAsyncComponent,
        theme: themeController,
    };

    if (typeof window !== 'undefined') {
        themeController.init();

        window.Alpine = Alpine;
        window.Rizzy = { ...(window.Rizzy || {}), ...cachedRizzyUI };

        document.dispatchEvent(new CustomEvent('rz:init', {
            detail: { Rizzy: window.Rizzy },
        }));
    }

    return cachedRizzyUI;
}
