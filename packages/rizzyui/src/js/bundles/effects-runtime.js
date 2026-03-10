import registerRzConfetti from '../lib/components/rzConfetti.js';
import { require } from '../runtime/rizzyRequire.js';

let isRegistered = false;

export function register(Alpine) {
    if (isRegistered) {
        return;
    }

    registerRzConfetti(Alpine, require);

    isRegistered = true;
}
