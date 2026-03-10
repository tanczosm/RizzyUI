import registerRzCombobox from '../lib/components/rzCombobox.js';
import registerRzFileInput from '../lib/components/rzFileInput.js';
import registerRzInputOTP from '../lib/components/rzInputOTP.js';
import registerRzScrollArea from '../lib/components/rzScrollArea.js';
import registerRzSlider from '../lib/components/rzSlider.js';
import { require } from '../runtime/assetRequire.js';

let registered = false;

export function register(Alpine) {
    if (registered) return;
    registerRzCombobox(Alpine, require);
    registerRzFileInput(Alpine);
    registerRzInputOTP(Alpine);
    registerRzScrollArea(Alpine);
    registerRzSlider(Alpine, require);
    registered = true;
}
