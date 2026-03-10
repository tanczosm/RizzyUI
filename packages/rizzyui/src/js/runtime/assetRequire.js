import loadjs from '../lib/loadjs/loadjs.js';

async function generateBundleId(paths) {
    const sortedPaths = [...paths].sort();
    const joinedPaths = sortedPaths.join('|');
    const encoder = new TextEncoder();
    const data = encoder.encode(joinedPaths);
    const hashBuffer = await crypto.subtle.digest('SHA-256', data);
    const hashArray = Array.from(new Uint8Array(hashBuffer));
    return hashArray.map((byte) => byte.toString(16).padStart(2, '0')).join('');
}

async function require(paths, callbackOrNonce, nonce) {
    if (!paths) {
        throw new Error('[Rizzy.require] No path(s) provided.');
    }

    if (typeof paths === 'string') {
        paths = [paths];
    }

    const cbObj = typeof callbackOrNonce === 'function'
        ? { success: callbackOrNonce }
        : (typeof callbackOrNonce === 'object' && callbackOrNonce !== null ? callbackOrNonce : null);

    const actualNonce = typeof callbackOrNonce === 'string'
        ? callbackOrNonce
        : nonce;

    return generateBundleId(paths).then((bundleId) => {
        const options = {
            returnPromise: false,
            numRetries: 1,
        };

        if (actualNonce) {
            options.nonce = actualNonce;
        }

        loadjs(paths, bundleId, options);

        return new Promise((resolve, reject) => {
            loadjs.ready(bundleId, {
                success: () => {
                    if (cbObj?.success) {
                        cbObj.success();
                    }
                    resolve({ bundleId });
                },
                error: (depsNotFound) => {
                    if (cbObj?.error) {
                        cbObj.error(depsNotFound);
                    }
                    reject(
                        new Error(
                            `[Rizzy.require] Failed to load bundle ${bundleId} (missing: ${Array.isArray(depsNotFound) ? depsNotFound.join(', ') : String(depsNotFound)})`
                        )
                    );
                },
            });
        });
    });
}

export { require };
