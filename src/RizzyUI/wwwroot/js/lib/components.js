import loadjs from "./loadjs/loadjs.js";

async function generateBundleId(paths) {
    paths = [...paths].sort();
    const joinedPaths = paths.join('|');
    const encoder = new TextEncoder();
    const data = encoder.encode(joinedPaths);
    const hashBuffer = await crypto.subtle.digest('SHA-256', data);
    const hashArray = Array.from(new Uint8Array(hashBuffer));
    return hashArray.map(b => b.toString(16).padStart(2, '0')).join('');
}

function rizzyRequire(paths, callbackOrNonce, nonce) {
    let cbObj = undefined;
    let csp = undefined;

    if (typeof callbackOrNonce === 'function') {
        cbObj = { success: callbackOrNonce };
    } else if (callbackOrNonce && typeof callbackOrNonce === 'object') {
        cbObj = callbackOrNonce;
    } else if (typeof callbackOrNonce === 'string') {
        csp = callbackOrNonce;
    }

    if (!csp && typeof nonce === 'string') csp = nonce;

    const files = Array.isArray(paths) ? paths : [paths];

    return generateBundleId(files).then((bundleId) => {
        if (!loadjs.isDefined(bundleId)) {
            loadjs(files, bundleId, {
                async: false,
                inlineScriptNonce: csp,
                inlineStyleNonce: csp,
            });
        }

        return new Promise((resolve, reject) => {
            loadjs.ready(bundleId, {
                success: () => {
                    try {
                        if (cbObj && typeof cbObj.success === 'function') cbObj.success();
                    } catch (e) {
                        console.error('[rizzyRequire] success callback threw:', e);
                    }
                    resolve({ bundleId });
                },
                error: (depsNotFound) => {
                    try {
                        if (cbObj && typeof cbObj.error === 'function') {
                            cbObj.error(depsNotFound);
                        }
                    } catch (e) {
                        console.error('[rizzyRequire] error callback threw:', e);
                    }
                    reject(
                        new Error(
                            `[rizzyRequire] Failed to load bundle ${bundleId} (missing: ${Array.isArray(depsNotFound) ? depsNotFound.join(', ') : String(depsNotFound)})`
                        )
                    );
                },
            });
        });
    });
}

export { rizzyRequire as require };
