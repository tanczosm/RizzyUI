import Alpine from '@alpinejs/csp';
import { bootstrapRizzyUI } from './lib/bootstrap.js';

const RizzyUI = bootstrapRizzyUI(Alpine);

Alpine.start();

export default RizzyUI;
