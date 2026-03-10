import registerRzColorPicker from '../lib/components/rzColorPicker.js';
import registerRzColorPickerProvider from '../lib/components/rzColorPickerProvider.js';
import registerRzColorSwatch from '../lib/components/rzColorSwatch.js';
import { require } from '../runtime/rizzyRequire.js';

let isRegistered = false;

export function register(Alpine) {
    if (isRegistered) {
        return;
    }

    registerRzColorPicker(Alpine, require);
    registerRzColorPickerProvider(Alpine, require);
    registerRzColorSwatch(Alpine);

    isRegistered = true;
}
