import registerRzPopover from '../lib/components/rzPopover.js';
import registerRzTooltip from '../lib/components/rzTooltip.js';

let isRegistered = false;

export function register(Alpine) {
    if (isRegistered) {
        return;
    }

    registerRzPopover(Alpine);
    registerRzTooltip(Alpine);

    isRegistered = true;
}
