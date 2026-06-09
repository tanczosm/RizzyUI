
function getCspNonce() {
    const nonceScript = document.querySelector('script[nonce]');
    return nonceScript ? nonceScript.getAttribute('nonce') : null;
}

function loadStylesheetOnce(href, id) {
    if (!href) {
        return Promise.reject(new Error('Stylesheet URL is missing.'));
    }

    const existing = document.getElementById(id);
    if (existing) {
        return Promise.resolve();
    }

    return new Promise((resolve, reject) => {
        const link = document.createElement('link');
        link.id = id;
        link.rel = 'stylesheet';
        link.href = href;
        const nonce = getCspNonce();
        if (nonce) {
            link.setAttribute('nonce', nonce);
        }
        link.addEventListener('load', () => resolve(), { once: true });
        link.addEventListener('error', () => reject(new Error(`Unable to load stylesheet: ${href}`)), { once: true });
        document.head.appendChild(link);
    });
}

function loadScriptOnce(src, id) {
    if (!src) {
        return Promise.reject(new Error('Script URL is missing.'));
    }

    const existing = document.getElementById(id);
    if (existing) {
        return Promise.resolve();
    }

    return new Promise((resolve, reject) => {
        const script = document.createElement('script');
        script.id = id;
        script.src = src;
        script.async = true;
        const nonce = getCspNonce();
        if (nonce) {
            script.setAttribute('nonce', nonce);
        }
        script.addEventListener('load', () => resolve(), { once: true });
        script.addEventListener('error', () => reject(new Error(`Unable to load script: ${src}`)), { once: true });
        document.body.appendChild(script);
    });
}

document.addEventListener('alpine:init', () => {

    Alpine.data('docSearch', () => ({
        isLoading: false,
        isReady: false,
        errorMessage: '',
        assetsPromise: null,

        async open() {
            if (this.isLoading) {
                return;
            }

            this.errorMessage = '';
            this.isLoading = true;

            try {
                await this.ensureReady();
                this.activateSearchDialog();
            } catch (error) {
                this.assetsPromise = null;
                this.isReady = false;
                this.errorMessage = 'Search failed to load. Activate search again to retry.';
                console.warn('[RizzyUI Docs] DocSearch failed to load.', error);
            } finally {
                this.isLoading = false;
            }
        },

        async ensureReady() {
            if (this.isReady) {
                return;
            }

            if (!this.assetsPromise) {
                this.assetsPromise = this.loadAssets();
            }

            await this.assetsPromise;

            if (!globalThis.docsearch) {
                throw new Error('DocSearch global is unavailable.');
            }

            if (!globalThis.__rizzyDocsDocSearchInitialized) {
                globalThis.docsearch({
                    container: '#docsearch-container',
                    appId: '4NNHPLPB1Z',
                    indexName: 'RizzyUI Documentation',
                    apiKey: '52ed9e7932048b8906b7babc7739551b',
                    askAi: '',
                });
                globalThis.__rizzyDocsDocSearchInitialized = true;
            }

            this.isReady = true;
        },

        loadAssets() {
            const cssHref = this.$el.dataset.docsearchCss;
            const scriptSrc = this.$el.dataset.docsearchJs;

            return Promise.all([
                loadStylesheetOnce(cssHref, 'rizzy-docsearch-css'),
                loadScriptOnce(scriptSrc, 'rizzy-docsearch-js'),
            ]);
        },

        activateSearchDialog() {
            const generatedButton = document.querySelector('#docsearch-container .DocSearch-Button');
            if (generatedButton instanceof HTMLElement) {
                generatedButton.click();
            }
        }
    }));

    Alpine.data('demoTest', () => ({
        tabRef: null,
        init() {
            this.tabRef = document.getElementById("tabDemo");
        },
        handleButtonClick() {
            var data = Alpine.$data(this.tabRef); // This is how to get data for an element

            console.log(data.buttonRef);
        },
        destroy() {

        }
    }));



    Alpine.data('comboboxBasicDemo', () => ({
        selectedFruit: 'None',

        init() {
            this.selectedFruit = this.$el.dataset.initialSelected || 'None';
        },

        handleComboboxChange(event) {
            this.selectedFruit = event.detail?.text || 'None';
        }
    }));

    Alpine.data('modalPageController', () => ({
        // Dispatch function for basic modal
        showBasicModal() {
            console.log("Dispatching show-basic-modal"); // Debug log
            window.dispatchEvent(new CustomEvent('show-basic-modal'));
        },

        // Dispatch function for small modal
        showSmallModal() {
            console.log("Dispatching show-small-modal"); // Debug log
            window.dispatchEvent(new CustomEvent('show-small-modal'));
        },

        // Dispatch function for custom title modal
        showCustomTitleModal() {
            console.log("Dispatching show-custom-title-modal"); // Debug log
            window.dispatchEvent(new CustomEvent('show-custom-title-modal'));
        },

        // Dispatch function for HTMX load example modal
        showHtmxLoadModal() {
            console.log("Dispatching show-htmx-load-modal"); // Debug log
            window.dispatchEvent(new CustomEvent('show-htmx-load-modal'));
            // Note: The HTMX request itself is still handled by hx-get on the button
        },

        // Dispatch function for HTMX close example modal
        showHtmxCloseModal() {
            console.log("Dispatching show-htmx-close-modal"); // Debug log
            window.dispatchEvent(new CustomEvent('show-htmx-close-modal'));
        }
    }));    
});

(function registerToastProviderDemos() {
    if (globalThis.__rizzyDocsToastProviderDemosRegistered) {
        return;
    }

    globalThis.__rizzyDocsToastProviderDemosRegistered = true;

    function getToastRuntime() {
        const toast = globalThis.Rizzy && globalThis.Rizzy.toast;
        if (!toast) {
            console.warn('[RizzyUI Docs] Rizzy.toast is unavailable. Ensure the RizzyUI runtime and RzToastProvider are loaded.');
            return null;
        }

        return toast;
    }

    function showDefault(toast) {
        toast.show({
            title: 'Notification',
            description: 'The background check completed.',
        });
    }

    function showTitleDescription(toast) {
        toast.info('The report is being generated. You can continue working.', 'Report started');
    }

    function showSuccess(toast) {
        toast.success('Project settings saved.', 'Saved');
    }

    function showInfo(toast) {
        toast.info('Build started. You can continue working while it runs.', 'Build started');
    }

    function showWarning(toast) {
        toast.warning('Your session expires in 2 minutes.', 'Session expiring');
    }

    function showError(toast) {
        toast.error('Unable to upload image. Check the file type and try again.', 'Upload failed');
    }

    function showAction(toast) {
        toast.success('Message archived.', 'Archived', {
            action: {
                label: 'Undo',
                dismissOnClick: true,
                onClick: toastHandle => {
                    const runtime = getToastRuntime();
                    if (runtime) {
                        runtime.info('The message was restored.', 'Undo complete');
                    }

                    toastHandle.dismiss();
                },
            },
        });
    }

    function showPersistent(toast) {
        toast.info('Waiting for the background worker to respond.', 'Still working', {
            autoclose: false,
            progress: false,
        });
    }

    function showDuration(toast) {
        toast.success('Export completed and is ready to download.', 'Export ready', {
            duration: 7000,
        });
    }

    function showPosition(toast, demo) {
        if (demo === 'position-top-right') {
            toast.info('This toast appears in the top-right stack.', 'Top right', {
                position: 'top-right',
            });
            return;
        }

        if (demo === 'position-bottom-left') {
            toast.info('This toast uses the Simple Notify alias bottom left.', 'Bottom left', {
                position: 'bottom left',
            });
            return;
        }

        if (demo === 'position-center') {
            toast.info('This toast appears in the center stack.', 'Center', {
                position: 'center',
            });
            return;
        }

        toast.info('This toast uses the Simple Notify alias right y-center.', 'Right center', {
            position: 'right y-center',
        });
    }

    function runLoadingDemo(toast) {
        toast.loading('Uploading changes...', 'Syncing', {
            id: 'docs-loading-demo',
        });

        window.setTimeout(() => {
            const runtime = getToastRuntime();
            if (!runtime) {
                return;
            }

            runtime.update('docs-loading-demo', {
                status: 'success',
                title: 'Synced',
                text: 'All changes are online.',
                autoclose: true,
                duration: 3000,
                progress: true,
            });
        }, 1500);
    }

    function showDismissible(toast) {
        toast.info('This toast can be dismissed by id.', 'Tracked toast', {
            id: 'docs-dismiss-demo',
            autoclose: false,
            progress: false,
        });
    }

    function dispatchToastEvent() {
        window.dispatchEvent(new CustomEvent('rz:toast', {
            detail: {
                id: 'docs-event-demo',
                status: 'success',
                title: 'Saved',
                description: 'The event API created this toast.',
            },
        }));
    }

    function showDedupe(toast) {
        toast.show({
            dedupeKey: 'docs-storage-warning',
            status: 'warning',
            title: 'Storage warning',
            text: 'Storage is still almost full.',
            incrementCount: true,
        });
    }

    function createStack(toast) {
        for (let index = 1; index <= 7; index += 1) {
            toast.info(`Stacked notification ${index}`, 'Stack demo', {
                position: 'bottom-right',
                duration: 8000,
            });
        }
    }

    document.addEventListener('click', event => {
        const trigger = event.target.closest('[data-rz-toast-demo]');
        if (!trigger) {
            return;
        }

        const demo = trigger.dataset.rzToastDemo;
        if (!demo) {
            return;
        }

        if (demo === 'event') {
            dispatchToastEvent();
            return;
        }

        const toast = getToastRuntime();
        if (!toast) {
            return;
        }

        switch (demo) {
            case 'default':
                showDefault(toast);
                break;
            case 'title-description':
                showTitleDescription(toast);
                break;
            case 'success':
                showSuccess(toast);
                break;
            case 'info':
                showInfo(toast);
                break;
            case 'warning':
                showWarning(toast);
                break;
            case 'error':
                showError(toast);
                break;
            case 'action':
                showAction(toast);
                break;
            case 'persistent':
                showPersistent(toast);
                break;
            case 'duration':
                showDuration(toast);
                break;
            case 'position-top-right':
            case 'position-bottom-left':
            case 'position-center':
            case 'position-right-center':
                showPosition(toast, demo);
                break;
            case 'loading':
                runLoadingDemo(toast);
                break;
            case 'dismiss-show':
                showDismissible(toast);
                break;
            case 'dismiss-id':
                toast.dismiss('docs-dismiss-demo');
                break;
            case 'clear':
                toast.clear();
                break;
            case 'dedupe':
                showDedupe(toast);
                break;
            case 'stack':
                createStack(toast);
                break;
            default:
                console.warn(`[RizzyUI Docs] Unknown toast demo '${demo}'.`);
                break;
        }
    });
}());
