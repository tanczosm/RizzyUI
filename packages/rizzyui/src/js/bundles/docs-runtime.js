import registerRzBrowser from '../lib/components/rzBrowser.js';
import registerRzCodeViewer from '../lib/components/rzCodeViewer.js';
import registerRzEmbeddedPreview from '../lib/components/rzEmbeddedPreview.js';
import registerRzEventViewer from '../lib/components/rzEventViewer.js';
import registerRzMarkdown from '../lib/components/rzMarkdown.js';
import registerRzQuickReferenceContainer from '../lib/components/rzQuickReferenceContainer.js';
import { require } from '../runtime/rizzyRequire.js';

let isRegistered = false;

export function register(Alpine) {
    if (isRegistered) {
        return;
    }

    registerRzBrowser(Alpine);
    registerRzCodeViewer(Alpine, require);
    registerRzEmbeddedPreview(Alpine);
    registerRzEventViewer(Alpine);
    registerRzMarkdown(Alpine, require);
    registerRzQuickReferenceContainer(Alpine);

    isRegistered = true;
}
