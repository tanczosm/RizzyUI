import registerRzConfetti from '../lib/components/rzConfetti.js';
import { require } from '../runtime/assetRequire.js';

let registered = false;

export function register(Alpine) {
    if (registered) return;
    registerRzConfetti(Alpine, require);
    registered = true;
}
