/**
 * RizzyUI CSP entrypoint.
 * Intentionally shell-only and required alongside rizzyui.js.
 */
import Alpine from '@alpinejs/csp';
import { bootstrapRizzyUI } from './lib/bootstrap.js';

const RizzyUI = bootstrapRizzyUI(Alpine);

Alpine.start();

export default RizzyUI;
