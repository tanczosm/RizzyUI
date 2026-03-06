import confetti from 'canvas-confetti';

const hostRegistry = new Map();
const hostElementRegistry = new WeakMap();
let listenersAttached = false;

function parseJson(value, fallback) {
    if (!value) return fallback;
    try {
        return JSON.parse(value);
    } catch {
        return fallback;
    }
}

function hasReducedMotion() {
    return window.matchMedia('(prefers-reduced-motion: reduce)').matches;
}

function asElement(node) {
    if (!node) return null;
    return node.nodeType === 1 ? node : node.parentElement;
}

function findTrigger(target) {
    const el = asElement(target);
    return el ? el.closest('[data-confetti-trigger="true"]') : null;
}

function resolveHost(trigger) {
    const explicitTarget = trigger.dataset.confettiTarget;
    if (explicitTarget && hostRegistry.has(explicitTarget)) {
        return hostRegistry.get(explicitTarget);
    }

    const nearest = trigger.closest('[data-confetti-host]');
    if (!nearest) return null;

    if (hostElementRegistry.has(nearest)) {
        return hostElementRegistry.get(nearest);
    }

    return hostRegistry.get(nearest.dataset.confettiHost) || null;
}

function buildCenteredOrigin(el) {
    const rect = el.getBoundingClientRect();
    const x = (rect.left + rect.width / 2) / window.innerWidth;
    const y = (rect.top + rect.height / 2) / window.innerHeight;
    return { x, y };
}

function attachDelegatedListeners() {
    if (listenersAttached) return;
    listenersAttached = true;

    document.addEventListener('click', (event) => {
        const trigger = findTrigger(event.target);
        if (!trigger || trigger.dataset.confettiTriggerEvent !== 'click' || trigger.hasAttribute('disabled') || trigger.getAttribute('aria-disabled') === 'true') {
            return;
        }

        const host = resolveHost(trigger);
        if (!host) return;
        host.trigger(trigger);
    });

    document.addEventListener('focusin', (event) => {
        const trigger = findTrigger(event.target);
        if (!trigger || trigger.dataset.confettiTriggerEvent !== 'focus' || trigger.hasAttribute('disabled') || trigger.getAttribute('aria-disabled') === 'true') {
            return;
        }

        const host = resolveHost(trigger);
        if (!host) return;
        host.trigger(trigger);
    });

    document.addEventListener('mouseover', (event) => {
        const trigger = findTrigger(event.target);
        if (!trigger || trigger.dataset.confettiTriggerEvent !== 'mouse-enter' || trigger.hasAttribute('disabled') || trigger.getAttribute('aria-disabled') === 'true') {
            return;
        }

        if (trigger.contains(event.relatedTarget)) {
            return;
        }

        const host = resolveHost(trigger);
        if (!host) return;
        host.trigger(trigger);
    });
}

function mergeRequest(baseRequest, overlayRequest) {
    if (!baseRequest && !overlayRequest) return {};

    const merged = {
        ...(baseRequest || {}),
        ...(overlayRequest || {}),
    };

    if (baseRequest?.origin || overlayRequest?.origin) {
        merged.origin = {
            ...(baseRequest?.origin || {}),
            ...(overlayRequest?.origin || {}),
        };
    }

    return merged;
}

function runFireworks(instance, request, timers) {
    const duration = 5000;
    const end = Date.now() + duration;
    const defaults = mergeRequest({ startVelocity: 30, spread: 360, ticks: 60, zIndex: 0 }, request);
    const timer = window.setInterval(() => {
        const timeLeft = end - Date.now();
        if (timeLeft <= 0) {
            window.clearInterval(timer);
            return;
        }

        const particleCount = 50 * (timeLeft / duration);
        instance({ ...defaults, particleCount, origin: { x: Math.random() * 0.2 + 0.1, y: Math.random() - 0.2 } });
        instance({ ...defaults, particleCount, origin: { x: Math.random() * 0.2 + 0.7, y: Math.random() - 0.2 } });
    }, 250);
    timers.push(timer);
}

function runSideCannons(instance, request, frames) {
    const end = Date.now() + 3000;
    const defaults = mergeRequest({ particleCount: 2, spread: 55, startVelocity: 60, colors: ['#a786ff', '#fd8bbc', '#eca184', '#f8deb1'] }, request);

    const frame = () => {
        if (Date.now() > end) return;

        instance({ ...defaults, angle: 60, origin: { x: 0, y: 0.5 } });
        instance({ ...defaults, angle: 120, origin: { x: 1, y: 0.5 } });

        const id = window.requestAnimationFrame(frame);
        frames.push(id);
    };

    frame();
}

function runStars(instance, request, timers) {
    const defaults = mergeRequest({ spread: 360, ticks: 50, gravity: 0, decay: 0.94, startVelocity: 30, colors: ['#FFE400', '#FFBD00', '#E89400', '#FFCA6C', '#FDFFB8'] }, request);

    const shoot = () => {
        instance({ ...defaults, particleCount: 40, scalar: 1.2, shapes: ['star'] });
        instance({ ...defaults, particleCount: 10, scalar: 0.75, shapes: ['circle'] });
    };

    timers.push(window.setTimeout(shoot, 0));
    timers.push(window.setTimeout(shoot, 100));
    timers.push(window.setTimeout(shoot, 200));
}

function runCustomShapes(instance, request, timers) {
    const scalar = request.scalar ?? 2;
    const triangle = confetti.shapeFromPath?.({ path: 'M0 10 L5 0 L10 10z' });
    const square = confetti.shapeFromPath?.({ path: 'M0 0 L10 0 L10 10 L0 10 Z' });
    const coin = confetti.shapeFromPath?.({ path: 'M5 0 A5 5 0 1 0 5 10 A5 5 0 1 0 5 0 Z' });
    const tree = confetti.shapeFromPath?.({ path: 'M5 0 L10 10 L0 10 Z' });
    const pathShapes = [triangle, square, coin, tree].filter(Boolean);

    if (!pathShapes.length) {
        runStars(instance, request, timers);
        return;
    }

    const defaults = mergeRequest({ spread: 360, ticks: 60, gravity: 0, decay: 0.96, startVelocity: 20, scalar, shapes: pathShapes }, request);

    const shoot = () => {
        instance({ ...defaults, particleCount: 30 });
        instance({ ...defaults, particleCount: 5 });
        instance({ ...defaults, particleCount: 15, scalar: scalar / 2, shapes: ['circle'] });
    };

    timers.push(window.setTimeout(shoot, 0));
    timers.push(window.setTimeout(shoot, 100));
    timers.push(window.setTimeout(shoot, 200));
}

function runEmoji(instance, request, timers) {
    const scalar = request.scalar ?? 2;
    const unicorn = confetti.shapeFromText?.({ text: '🦄', scalar });

    if (!unicorn) {
        runStars(instance, request, timers);
        return;
    }

    const defaults = mergeRequest({ spread: 360, ticks: 60, gravity: 0, decay: 0.96, startVelocity: 20, shapes: [unicorn], scalar }, request);

    const shoot = () => {
        instance({ ...defaults, particleCount: 30 });
        instance({ ...defaults, particleCount: 5 });
        instance({ ...defaults, particleCount: 15, scalar: scalar / 2, shapes: ['circle'] });
    };

    timers.push(window.setTimeout(shoot, 0));
    timers.push(window.setTimeout(shoot, 100));
    timers.push(window.setTimeout(shoot, 200));
}

export default function registerRzConfetti(Alpine) {
    Alpine.data('rzConfetti', () => ({
        instance: null,
        hostId: '',
        options: {},
        globalOptions: { resize: true, useWorker: true },
        pattern: 'burst',
        respectReducedMotion: true,
        timers: [],
        frames: [],

        init() {
            this.hostId = this.$el.dataset.confettiHost || this.$el.id;
            this.options = parseJson(this.$el.dataset.confettiOptions, {});
            this.globalOptions = parseJson(this.$el.dataset.confettiGlobalOptions, { resize: true, useWorker: true });
            this.pattern = this.$el.dataset.confettiPattern || 'burst';
            this.respectReducedMotion = this.$el.dataset.confettiRespectReducedMotion !== 'false';

            const canvas = this.$refs.canvas;
            if (!canvas) {
                console.warn('[rzConfetti] Canvas reference was not found.');
                return;
            }

            try {
                this.instance = confetti.create(canvas, this.globalOptions);
            } catch {
                try {
                    this.instance = confetti.create(canvas, { ...this.globalOptions, useWorker: false });
                } catch (error) {
                    console.warn('[rzConfetti] Failed to initialize confetti instance.', error);
                    this.instance = null;
                    return;
                }
            }

            hostRegistry.set(this.hostId, this);
            hostElementRegistry.set(this.$el, this);

            const hostRoot = this.$el.closest('[data-slot="confetti"]');
            if (hostRoot) {
                hostElementRegistry.set(hostRoot, this);
            }

            attachDelegatedListeners();

            const manualStart = this.$el.dataset.confettiManualStart === 'true';
            if (!manualStart) {
                this.runPattern(this.pattern, this.options);
            }
        },

        resolveRequest(request) {
            return mergeRequest(this.options, request || {});
        },

        fire(request = {}) {
            if (!this.instance) return;

            const payload = this.resolveRequest(request);
            const disableForReducedMotion = payload.disableForReducedMotion === true;
            if (this.respectReducedMotion && hasReducedMotion() && !disableForReducedMotion) {
                return;
            }

            this.instance(payload);
        },

        runPattern(patternName, request = {}) {
            if (!this.instance) return;

            const pattern = patternName || this.pattern;
            const payload = this.resolveRequest(request);

            if (pattern === 'fireworks') {
                runFireworks(this.instance, payload, this.timers);
                return;
            }

            if (pattern === 'side-cannons') {
                runSideCannons(this.instance, payload, this.frames);
                return;
            }

            if (pattern === 'stars') {
                runStars(this.instance, payload, this.timers);
                return;
            }

            if (pattern === 'custom-shapes') {
                runCustomShapes(this.instance, payload, this.timers);
                return;
            }

            if (pattern === 'emoji') {
                runEmoji(this.instance, payload, this.timers);
                return;
            }

            if (pattern === 'random-direction') {
                this.fire({ ...payload, angle: Math.random() * 360 });
                return;
            }

            this.fire(payload);
        },

        trigger(triggerEl) {
            const pattern = triggerEl.dataset.confettiPattern || this.pattern;
            const request = parseJson(triggerEl.dataset.confettiRequest, {});

            if (!request.origin && (pattern === 'burst' || pattern === 'random-direction')) {
                request.origin = buildCenteredOrigin(triggerEl);
            }

            this.runPattern(pattern, request);
        },

        reset() {
            for (const timer of this.timers) {
                window.clearTimeout(timer);
                window.clearInterval(timer);
            }
            this.timers = [];

            for (const frame of this.frames) {
                window.cancelAnimationFrame(frame);
            }
            this.frames = [];

            if (this.instance && typeof this.instance.reset === 'function') {
                this.instance.reset();
            }
        },

        destroy() {
            this.reset();
            if (this.hostId) {
                const currentHost = hostRegistry.get(this.hostId);
                if (currentHost === this) {
                    hostRegistry.delete(this.hostId);
                }
            }
            this.instance = null;
        },
    }));
}
