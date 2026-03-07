export default function (Alpine, require) {
    Alpine.data('rzHighlighter', () => ({
        annotation: null,
        resizeObserver: null,
        intersectionObserver: null,
        rafId: 0,
        hasShown: false,
        config: null,

        init() {
            this.config = this.readConfig();

            const assets = this.parseAssets(this.$el.dataset.assets);
            const nonce = this.$el.dataset.nonce;

            require(assets, {
                success: () => {
                    this.start();
                },
                error: () => {
                    console.error('Failed to load assets for rzHighlighter.');
                }
            }, nonce);
        },

        destroy() {
            this.cleanup();
        },

        readConfig() {
            const root = this.$el.parentElement;
            const dataset = root?.dataset ?? {};

            return {
                action: dataset.action || 'highlight',
                color: dataset.color || '#ffd1dc',
                strokeWidth: this.toNumber(dataset.strokeWidth, 1.5),
                animationDuration: this.toInteger(dataset.animationDuration, 600),
                iterations: this.toInteger(dataset.iterations, 2),
                padding: this.toInteger(dataset.padding, 2),
                multiline: this.toBool(dataset.multiline, true),
                startOnView: this.toBool(dataset.startOnView, false),
                viewMargin: dataset.viewMargin || '-10%'
            };
        },

        parseAssets(value) {
            if (!value) {
                return [];
            }

            try {
                const parsed = JSON.parse(value);
                return Array.isArray(parsed) ? parsed : [];
            } catch {
                return [];
            }
        },

        start() {
            if (this.config.startOnView) {
                this.observeViewport();
                return;
            }

            this.showAnnotation();
        },

        observeViewport() {
            const target = this.$refs.target;
            if (!target || typeof IntersectionObserver === 'undefined') {
                this.showAnnotation();
                return;
            }

            this.intersectionObserver = new IntersectionObserver((entries) => {
                const isVisible = entries.some(entry => entry.isIntersecting);
                if (!isVisible || this.hasShown) {
                    return;
                }

                this.showAnnotation();
                this.intersectionObserver?.disconnect();
                this.intersectionObserver = null;
            }, { rootMargin: this.config.viewMargin });

            this.intersectionObserver.observe(target);
        },

        showAnnotation() {
            const target = this.$refs.target;
            const annotate = window.roughNotation?.annotate;

            if (!target || typeof annotate !== 'function') {
                return;
            }

            this.removeAnnotation();
            this.annotation = annotate(target, {
                type: this.config.action,
                color: this.config.color,
                strokeWidth: this.config.strokeWidth,
                animationDuration: this.config.animationDuration,
                iterations: this.config.iterations,
                padding: this.config.padding,
                multiline: this.config.multiline
            });

            this.annotation.show();
            this.hasShown = true;
            this.observeResize();
        },

        observeResize() {
            const target = this.$refs.target;
            if (!target || typeof ResizeObserver === 'undefined') {
                return;
            }

            this.resizeObserver = new ResizeObserver(() => {
                this.scheduleRefresh();
            });

            this.resizeObserver.observe(target);
            this.resizeObserver.observe(document.body);
        },

        scheduleRefresh() {
            if (this.rafId) {
                cancelAnimationFrame(this.rafId);
            }

            this.rafId = requestAnimationFrame(() => {
                this.refreshAnnotation();
            });
        },

        refreshAnnotation() {
            if (!this.hasShown) {
                return;
            }

            this.showAnnotation();
        },

        removeAnnotation() {
            if (!this.annotation) {
                return;
            }

            this.annotation.remove();
            this.annotation = null;
        },

        cleanup() {
            if (this.rafId) {
                cancelAnimationFrame(this.rafId);
                this.rafId = 0;
            }

            this.removeAnnotation();
            this.resizeObserver?.disconnect();
            this.intersectionObserver?.disconnect();
            this.resizeObserver = null;
            this.intersectionObserver = null;
        },

        toBool(value, fallback) {
            if (value === 'true') {
                return true;
            }
            if (value === 'false') {
                return false;
            }

            return fallback;
        },

        toInteger(value, fallback) {
            const parsed = Number.parseInt(value, 10);
            return Number.isNaN(parsed) ? fallback : parsed;
        },

        toNumber(value, fallback) {
            const parsed = Number.parseFloat(value);
            return Number.isNaN(parsed) ? fallback : parsed;
        }
    }));
}
