import registerRzCalendar from '../lib/components/rzCalendar.js';
import registerRzCalendarProvider from '../lib/components/rzCalendarProvider.js';
import registerRzDateEdit from '../lib/components/rzDateEdit.js';
import { require } from '../runtime/rizzyRequire.js';

let isRegistered = false;

export function register(Alpine) {
    if (isRegistered) {
        return;
    }

    registerRzCalendar(Alpine, require);
    registerRzCalendarProvider(Alpine);
    registerRzDateEdit(Alpine, require);

    isRegistered = true;
}
