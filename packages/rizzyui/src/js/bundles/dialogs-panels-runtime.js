import registerRzModal from '../lib/components/rzModal.js';
import registerRzSheet from '../lib/components/rzSheet.js';
import registerRzSidebar from '../lib/components/rzSidebar.js';

let isRegistered = false;

export function register(Alpine) {
    if (isRegistered) {
        return;
    }

    registerRzModal(Alpine);
    registerRzSheet(Alpine);
    registerRzSidebar(Alpine);

    isRegistered = true;
}
