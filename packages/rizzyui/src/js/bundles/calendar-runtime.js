import registerRzCalendar from '../lib/components/rzCalendar.js';
import registerRzCalendarProvider from '../lib/components/rzCalendarProvider.js';
import registerRzDateEdit from '../lib/components/rzDateEdit.js';
import { require } from '../runtime/assetRequire.js';

let registered = false;

export function register(Alpine) {
    if (registered) return;
    registerRzCalendar(Alpine, require);
    registerRzCalendarProvider(Alpine);
    registerRzDateEdit(Alpine, require);
    registered = true;
}
