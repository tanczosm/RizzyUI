export default function (Alpine) {
    Alpine.data('rzBlurFade', () => ({
        state: 'visible',
        ready: 'false',
        presentationStyle: '',
        config: null,
        observer: null,
        reducedMotion: false,

        init() {
            this.loadConfig();

            if (!this.config) {
                this.applyVisibleState();
                this.ready = 'true';
                return;
            }

            this.reducedMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;
            if (this.reducedMotion) {
                this.applyVisibleState();
                this.ready = 'true';
                return;
            }

            this.ready = 'true';

            if (this.config.inView) {
                this.startInViewReveal();
                return;
            }

            this.startOnLoadReveal();
        },

        loadConfig() {
            const dataset = this.$el.dataset || {};
            const direction = this.normalizeDirection(dataset.direction);
            const offset = this.parsePositive(dataset.offset, 6);
            const duration = this.parsePositive(dataset.duration, 0.4);
            const delay = this.parsePositive(dataset.delay, 0);
            const blur = this.normalizeBlur(dataset.blur);
            const inView = String(dataset.inView).toLowerCase() === 'true';
            const inViewMargin = this.normalizeMargin(dataset.inViewMargin);

            this.config = {
                direction,
                offset,
                duration,
                delay,
                blur,
                inView,
                inViewMargin,
            };
        },

        normalizeDirection(direction) {
            if (direction === 'up' || direction === 'down' || direction === 'left' || direction === 'right') {
                return direction;
            }

            return 'down';
        },

        parsePositive(value, fallback) {
            const parsed = Number(value);
            if (!Number.isFinite(parsed)) {
                return fallback;
            }

            return parsed < 0 ? 0 : parsed;
        },

        normalizeBlur(value) {
            if (typeof value !== 'string' || value.trim() === '') {
                return '6px';
            }

            return value.trim();
        },

        normalizeMargin(value) {
            if (typeof value !== 'string' || value.trim() === '') {
                return '-50px';
            }

            return value.trim();
        },

        applyVisibleState() {
            this.state = 'visible';
            this.presentationStyle = this.buildVisibleStyle();
        },

        applyHiddenState() {
            this.state = 'hidden';
            this.presentationStyle = this.buildHiddenStyle();
        },

        buildVisibleStyle() {
            return this.buildTransitionStyle(1, 'translate3d(0, 0, 0)', 'blur(0px)');
        },

        buildHiddenStyle() {
            const transform = this.buildHiddenTransform();
            const filter = `blur(${this.config.blur})`;
            return this.buildTransitionStyle(0, transform, filter);
        },

        buildTransitionStyle(opacity, transform, filter) {
            const duration = this.config.duration;
            const delay = this.config.delay;

            return `opacity: ${opacity}; transform: ${transform}; filter: ${filter}; transition-duration: ${duration}s; transition-delay: ${delay}s;`;
        },

        buildHiddenTransform() {
            const offset = this.config.offset;

            if (this.config.direction === 'up') {
                return `translate3d(0, ${offset}px, 0)`;
            }

            if (this.config.direction === 'down') {
                return `translate3d(0, -${offset}px, 0)`;
            }

            if (this.config.direction === 'left') {
                return `translate3d(${offset}px, 0, 0)`;
            }

            return `translate3d(-${offset}px, 0, 0)`;
        },

        startOnLoadReveal() {
            this.applyHiddenState();
            window.requestAnimationFrame(() => {
                this.applyVisibleState();
            });
        },

        startInViewReveal() {
            this.applyHiddenState();

            if (!('IntersectionObserver' in window)) {
                this.applyVisibleState();
                return;
            }

            try {
                this.observer = new IntersectionObserver(this.handleIntersection.bind(this), {
                    root: null,
                    rootMargin: this.config.inViewMargin,
                    threshold: 0,
                });
                this.observer.observe(this.$el);
            } catch {
                this.applyVisibleState();
                this.disconnectObserver();
            }
        },

        handleIntersection(entries) {
            const hasIntersection = entries.some((entry) => entry && entry.isIntersecting);
            if (!hasIntersection) {
                return;
            }

            this.applyVisibleState();
            this.disconnectObserver();
        },

        disconnectObserver() {
            if (!this.observer) {
                return;
            }

            this.observer.disconnect();
            this.observer = null;
        },

        destroy() {
            this.disconnectObserver();
        },
    }));
}
