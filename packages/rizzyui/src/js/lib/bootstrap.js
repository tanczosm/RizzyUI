import collapse from '@alpinejs/collapse';
import intersect from '@alpinejs/intersect';
import focus from '@alpinejs/focus';
import AsyncAlpine from 'async-alpine';

import toast from "./notify/toast";
import { require } from './components.js';
import $data from './alpineData.js';
import props from './alpineProps.js';
import registerAsyncComponent from './alpineModuleRegistrar.js';
import registerMobileDirective from './directives/mobile.js';
import registerSyncDirective from './directives/sync-prop.js'
import { themeController } from './theme.js';
import { registerStores } from './stores.js';

let cachedRizzyUI = null;

export function bootstrapRizzyUI(Alpine) {
    if (cachedRizzyUI) return cachedRizzyUI;

    Alpine.plugin(collapse);
    Alpine.plugin(intersect);
    Alpine.plugin(focus);
    Alpine.plugin(AsyncAlpine);

    const componentModules = import.meta.glob('./components/*.js');
    for (const path in componentModules) {
        const componentName = path.split('/').pop().replace('.js', '');
        const loadModule = componentModules[path];

        AsyncAlpine.data(componentName, async () => {
            const module = await loadModule();
            return module.default;
        });
    }
    if (typeof document !== 'undefined') {
        document.addEventListener('alpine:init', () => {
            registerStores(Alpine);
        });
    }

    registerMobileDirective(Alpine);
    registerSyncDirective(Alpine);

    cachedRizzyUI = {
        Alpine,
        require,
        toast,
        $data,
        props,
        registerAsyncComponent,
        theme: themeController
    };

    if (typeof window !== 'undefined') {
        themeController.init();

        window.Alpine = Alpine;
        window.Rizzy = { ...(window.Rizzy || {}), ...cachedRizzyUI };

        document.dispatchEvent(new CustomEvent("rz:init", {
            detail: { Rizzy: window.Rizzy }
        }));
    }

    return cachedRizzyUI;
}
