import registerRzCommand from '../lib/components/rzCommand.js';
import registerRzCommandGroup from '../lib/components/rzCommandGroup.js';
import registerRzCommandItem from '../lib/components/rzCommandItem.js';
import registerRzCommandList from '../lib/components/rzCommandList.js';

let registered = false;

export function register(Alpine) {
    if (registered) return;
    registerRzCommand(Alpine);
    registerRzCommandGroup(Alpine);
    registerRzCommandItem(Alpine);
    registerRzCommandList(Alpine);
    registered = true;
}
