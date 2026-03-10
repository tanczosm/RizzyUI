import { ValidationService } from 'aspnet-client-validation';

let validationInstance = null;

export async function ensureValidationRuntime() {
    if (validationInstance) return validationInstance;
    validationInstance = new ValidationService();
    validationInstance.bootstrap({ watch: true });
    return validationInstance;
}
