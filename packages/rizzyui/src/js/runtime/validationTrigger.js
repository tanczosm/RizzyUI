import { ensureValidationRuntime } from './validationRuntime.js';

let autoTriggerInstalled = false;

export function registerValidationTrigger(Alpine) {
    Alpine.directive('rz-validate', (el) => {
        ensureValidationRuntime().then((validation) => {
            validation.scan(el);
        });
    });

    if (autoTriggerInstalled || typeof document === 'undefined') return;
    autoTriggerInstalled = true;

    document.addEventListener('focusin', (event) => {
        const form = event.target instanceof Element
            ? event.target.closest('form[data-val="true"]')
            : null;

        if (form) {
            ensureValidationRuntime();
        }
    }, { once: true, capture: true });
}
