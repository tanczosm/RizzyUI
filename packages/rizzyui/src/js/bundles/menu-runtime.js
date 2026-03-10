import registerRzDropdownMenu from '../lib/components/rzDropdownMenu.js';
import registerRzMenubar from '../lib/components/rzMenubar.js';
import registerRzNavigationMenu from '../lib/components/rzNavigationMenu.js';
import $data from '../lib/alpineData.js';

let isRegistered = false;

export function register(Alpine) {
    if (isRegistered) {
        return;
    }

    registerRzDropdownMenu(Alpine);
    registerRzMenubar(Alpine);
    registerRzNavigationMenu(Alpine, $data);

    isRegistered = true;
}
