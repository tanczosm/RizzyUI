/**
 * RizzyUI default entrypoint.
 * Intentionally shell-only: no eager component registration.
 */
import Alpine from 'alpinejs';
import { bootstrapRizzyUI } from './lib/bootstrap.js';

const RizzyUI = bootstrapRizzyUI(Alpine);

Alpine.start();

export default RizzyUI;
