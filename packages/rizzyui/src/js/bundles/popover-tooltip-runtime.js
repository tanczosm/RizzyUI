import registerRzPopover from '../lib/components/rzPopover.js';
import registerRzTooltip from '../lib/components/rzTooltip.js';

let registered = false;

export function register(Alpine) {
    if (registered) return;
    registerRzPopover(Alpine);
    registerRzTooltip(Alpine);
    registered = true;
}
