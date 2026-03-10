import registerRzCarousel from '../lib/components/rzCarousel.js';
import registerRzChart from '../lib/components/rzChart.js';
import registerRzHighlighter from '../lib/components/rzHighlighter.js';
import registerRzNumberTicker from '../lib/components/rzNumberTicker.js';
import registerRzShineBorder from '../lib/components/rzShineBorder.js';
import registerRzTypingAnimation from '../lib/components/rzTypingAnimation.js';
import { require } from '../runtime/assetRequire.js';

let registered = false;

export function register(Alpine) {
    if (registered) return;
    registerRzCarousel(Alpine, require);
    registerRzChart(Alpine, require);
    registerRzHighlighter(Alpine, require);
    registerRzNumberTicker(Alpine);
    registerRzShineBorder(Alpine);
    registerRzTypingAnimation(Alpine);
    registered = true;
}
