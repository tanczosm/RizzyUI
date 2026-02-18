export default function (Alpine) {
    Alpine.data('rzEventViewer', () => ({
        eventNames: [],
        entries: [],
        error: null,
        paused: false,
        target: 'window',
        targetEl: null,
        maxEntries: 200,
        autoScroll: true,
        pretty: true,
        showTimestamp: true,
        showEventMeta: false,
        level: 'info',
        filterPath: '',
        pauseText: 'Pause',
        resumeText: 'Resume',
        _handlers: new Map(),
        _boundEvents: new Set(),
        _entryId: 0,

        hasError() {
            return this.error !== null;
        },

        getToggleText() {
            return this.paused ? this.resumeText : this.pauseText;
        },

        init() {
            this.target = this.$el.dataset.target || 'window';
            this.maxEntries = Number.parseInt(this.$el.dataset.maxEntries || '200', 10);
            this.autoScroll = this.parseBoolean(this.$el.dataset.autoScroll, true);
            this.pretty = this.parseBoolean(this.$el.dataset.pretty, true);
            this.showTimestamp = this.parseBoolean(this.$el.dataset.showTimestamp, true);
            this.showEventMeta = this.parseBoolean(this.$el.dataset.showEventMeta, false);
            this.level = this.$el.dataset.level || 'info';
            this.filterPath = this.$el.dataset.filter || '';
            this.pauseText = this.$el.dataset.pauseText || this.pauseText;
            this.resumeText = this.$el.dataset.resumeText || this.resumeText;

            this.eventNames = this.resolveEventNames();
            if (this.eventNames.length === 0) {
                this.error = 'At least one event name is required.';
                return;
            }

            this.targetEl = this.resolveTargetElement(this.target);
            if (!this.targetEl) {
                this.error = `Unable to resolve target: ${this.target}`;
                return;
            }

            for (const eventName of this.eventNames) {
                const handler = this.createHandler(eventName);
                this._handlers.set(eventName, handler);
                this.targetEl.addEventListener(eventName, handler);
                this._boundEvents.add(eventName);
            }

            this.appendSystemEntry(`Listening for: ${this.eventNames.join(', ')}`);
        },

        destroy() {
            if (!this.targetEl) {
                this._handlers.clear();
                this._boundEvents.clear();
                return;
            }

            for (const [eventName, handler] of this._handlers.entries()) {
                this.targetEl.removeEventListener(eventName, handler);
            }

            this._handlers.clear();
            this._boundEvents.clear();
        },

        parseBoolean(value, defaultValue) {
            if (typeof value !== 'string' || value.length === 0) {
                return defaultValue;
            }

            return value === 'true';
        },

        resolveEventNames() {
            const names = [];
            const single = this.$el.dataset.eventName || '';
            const multiple = this.$el.dataset.eventNames || '';

            if (single.trim().length > 0) {
                names.push(single.trim());
            }

            if (multiple.trim().length > 0) {
                const raw = multiple.trim();
                if (raw.startsWith('[')) {
                    try {
                        const parsed = JSON.parse(raw);
                        if (Array.isArray(parsed)) {
                            for (const entry of parsed) {
                                if (typeof entry === 'string') {
                                    names.push(entry.trim());
                                } else {
                                    this.appendSystemEntry('Ignored non-string event name in JSON array.');
                                }
                            }
                        }
                    } catch {
                        this.appendSystemEntry('Failed to parse JSON event names; treating as CSV.');
                        names.push(...raw.split(',').map(part => part.trim()));
                    }
                } else {
                    names.push(...raw.split(',').map(part => part.trim()));
                }
            }

            const deduped = [];
            const seen = new Set();

            for (const name of names) {
                if (!name || seen.has(name)) {
                    continue;
                }

                seen.add(name);
                deduped.push(name);
            }

            return deduped;
        },

        resolveTargetElement(target) {
            if (target === 'window') {
                return window;
            }

            if (target === 'document') {
                return document;
            }

            return document.querySelector(target);
        },

        createHandler(eventName) {
            return (evt) => {
                if (this.paused) {
                    return;
                }

                const eventType = evt?.type || eventName;
                const rawDetail = evt instanceof CustomEvent ? evt.detail : evt?.detail;
                const filteredDetail = this.applyFilter(rawDetail, this.filterPath);
                const text = this.formatEntry(eventType, filteredDetail);

                this.entries.push({
                    id: `${eventType}-${this._entryId++}`,
                    type: eventType,
                    level: this.level,
                    text
                });

                if (this.entries.length > this.maxEntries) {
                    this.entries.splice(0, this.entries.length - this.maxEntries);
                }

                if (this.autoScroll) {
                    this.$nextTick(() => {
                        if (this.$refs.console) {
                            this.$refs.console.scrollTop = this.$refs.console.scrollHeight;
                        }
                    });
                }
            };
        },

        formatEntry(eventType, detail) {
            const body = this.stringifyDetail(detail);
            const parts = [];

            if (this.showTimestamp) {
                parts.push(`[${new Date().toLocaleTimeString()}]`);
            }

            const shouldPrefixType = this.showEventMeta || this.eventNames.length > 1;
            if (shouldPrefixType) {
                parts.push(`[${eventType}]`);
            }

            if (this.showEventMeta) {
                parts.push(`[level:${this.level}]`);
            }

            if (body.length > 0) {
                parts.push(body);
            }

            return parts.join(' ');
        },

        stringifyDetail(value) {
            if (value === undefined) {
                return 'undefined';
            }

            if (value === null) {
                return 'null';
            }

            if (typeof value === 'string') {
                return value;
            }

            if (typeof value === 'number' || typeof value === 'boolean') {
                return String(value);
            }

            try {
                if (this.pretty) {
                    return JSON.stringify(value, null, 2);
                }

                return JSON.stringify(value);
            } catch {
                return '[unserializable detail]';
            }
        },

        applyFilter(detail, path) {
            if (!path || path.trim().length === 0) {
                return detail;
            }

            const parts = path.split('.').map(segment => segment.trim()).filter(Boolean);
            let current = detail;

            for (const part of parts) {
                if (current === null || current === undefined) {
                    return undefined;
                }

                if (typeof current !== 'object' || !(part in current)) {
                    return undefined;
                }

                current = current[part];
            }

            return current;
        },

        appendSystemEntry(message) {
            this.entries.push({
                id: `system-${this._entryId++}`,
                type: 'system',
                level: 'info',
                text: message
            });
        },

        clearEntries() {
            this.entries = [];
        },

        togglePaused() {
            this.paused = !this.paused;
        },

        copyEntries() {
            const payload = this.entries.map(entry => entry.text).join('\n');
            if (navigator.clipboard && typeof navigator.clipboard.writeText === 'function') {
                navigator.clipboard.writeText(payload);
            }
        }
    }));
}
