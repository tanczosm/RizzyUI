import registerRzBrowser from '../lib/components/rzBrowser.js';
import registerRzCodeViewer from '../lib/components/rzCodeViewer.js';
import registerRzEmbeddedPreview from '../lib/components/rzEmbeddedPreview.js';
import registerRzEventViewer from '../lib/components/rzEventViewer.js';
import registerRzQuickReferenceContainer from '../lib/components/rzQuickReferenceContainer.js';
import registerRzMarkdown from '../lib/components/rzMarkdown.js';
import { require } from '../runtime/assetRequire.js';

let registered = false;

export function register(Alpine) {
    if (registered) return;
    registerRzBrowser(Alpine);
    registerRzCodeViewer(Alpine);
    registerRzEmbeddedPreview(Alpine);
    registerRzEventViewer(Alpine);
    registerRzQuickReferenceContainer(Alpine);
    registerRzMarkdown(Alpine, require);
    registered = true;
}
