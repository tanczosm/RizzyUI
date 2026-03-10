import registerRzCommand from '../lib/components/rzCommand.js';
import registerRzCommandGroup from '../lib/components/rzCommandGroup.js';
import registerRzCommandItem from '../lib/components/rzCommandItem.js';
import registerRzCommandList from '../lib/components/rzCommandList.js';

let isRegistered = false;

export function register(Alpine) {
    if (isRegistered) {
        return;
    }

    registerRzCommand(Alpine);
    registerRzCommandGroup(Alpine);
    registerRzCommandItem(Alpine);
    registerRzCommandList(Alpine);

    isRegistered = true;
}
