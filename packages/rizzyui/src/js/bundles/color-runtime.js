import registerRzColorPicker from '../lib/components/rzColorPicker.js';
import registerRzColorPickerProvider from '../lib/components/rzColorPickerProvider.js';
import registerRzColorSwatch from '../lib/components/rzColorSwatch.js';
import { require } from '../runtime/assetRequire.js';

let registered = false;

export function register(Alpine) {
    if (registered) return;
    registerRzColorPicker(Alpine, require);
    registerRzColorPickerProvider(Alpine, require);
    registerRzColorSwatch(Alpine);
    registered = true;
}
