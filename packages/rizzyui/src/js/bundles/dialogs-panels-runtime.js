import registerRzModal from '../lib/components/rzModal.js';
import registerRzSheet from '../lib/components/rzSheet.js';
import registerRzSidebar from '../lib/components/rzSidebar.js';

let registered = false;

export function register(Alpine) {
    if (registered) return;
    registerRzModal(Alpine);
    registerRzSheet(Alpine);
    registerRzSidebar(Alpine);
    registered = true;
}
