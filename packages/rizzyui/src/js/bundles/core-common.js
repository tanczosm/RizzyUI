import registerRzAccordion from '../lib/components/rzAccordion.js';
import registerAccordionItem from '../lib/components/accordionItem.js';
import registerRzAlert from '../lib/components/rzAlert.js';
import registerRzAspectRatio from '../lib/components/rzAspectRatio.js';
import registerRzBackToTop from '../lib/components/rzBackToTop.js';
import registerRzClipboard from '../lib/components/rzClipboard.js';
import registerRzCollapsible from '../lib/components/rzCollapsible.js';
import registerRzDarkModeToggle from '../lib/components/rzDarkModeToggle.js';
import registerRzHeading from '../lib/components/rzHeading.js';
import registerRzIndicator from '../lib/components/rzIndicator.js';
import registerRzInputGroupAddon from '../lib/components/rzInputGroupAddon.js';
import registerRzPrependInput from '../lib/components/rzPrependInput.js';
import registerRzProgress from '../lib/components/rzProgress.js';
import registerRzTabs from '../lib/components/rzTabs.js';
import registerRzToggle from '../lib/components/rzToggle.js';

let registered = false;

export function register(Alpine) {
    if (registered) return;
    registerRzAccordion(Alpine);
    registerAccordionItem(Alpine);
    registerRzAlert(Alpine);
    registerRzAspectRatio(Alpine);
    registerRzBackToTop(Alpine);
    registerRzClipboard(Alpine);
    registerRzCollapsible(Alpine);
    registerRzDarkModeToggle(Alpine);
    registerRzHeading(Alpine);
    registerRzIndicator(Alpine);
    registerRzInputGroupAddon(Alpine);
    registerRzPrependInput(Alpine);
    registerRzProgress(Alpine);
    registerRzTabs(Alpine);
    registerRzToggle(Alpine);
    registered = true;
}
