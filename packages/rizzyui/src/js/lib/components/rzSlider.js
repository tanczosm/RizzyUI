export default function (Alpine) {
    Alpine.data('rzSlider', () => ({
        min: 0,
        max: 100,
        step: 1,
        orientation: 'horizontal',
        disabled: false,
        inverted: false,
        minStepsBetweenThumbs: 0,
        useIndexedNames: true,
        values: [0],
        activeThumbIndex: null,
        pointerId: null,
        boundPointerMove: null,
        boundPointerUp: null,

        init() {
            const data = this.$el.dataset;
            this.min = this.parseNumber(data.min, 0);
            this.max = this.parseNumber(data.max, 100);
            this.step = Math.max(this.parseNumber(data.step, 1), Number.EPSILON);
            this.orientation = data.orientation || 'horizontal';
            this.disabled = data.disabled === 'true';
            this.inverted = data.inverted === 'true';
            this.minStepsBetweenThumbs = Math.max(this.parseNumber(data.minStepsBetweenThumbs, 0), 0);
            this.useIndexedNames = data.useIndexedNames !== 'false';
            this.values = this.parseValues(data.values);
            this.boundPointerMove = this.handlePointerMove.bind(this);
            this.boundPointerUp = this.handlePointerUp.bind(this);
            this.values = this.applyConstraints(this.values);
            this.syncDom();
            this.syncHiddenInputs();
        },

        handlePointerDown(event) {
            if (this.disabled) return;
            const index = this.readThumbIndex(event.currentTarget);
            if (index < 0) return;
            this.activeThumbIndex = index;
            this.pointerId = event.pointerId;
            event.currentTarget.setPointerCapture(event.pointerId);
            window.addEventListener('pointermove', this.boundPointerMove);
            window.addEventListener('pointerup', this.boundPointerUp);
            event.preventDefault();
        },

        handlePointerMove(event) {
            if (this.disabled || this.activeThumbIndex === null) return;
            if (this.pointerId !== null && event.pointerId !== this.pointerId) return;
            const nextValue = this.positionToValue(event);
            this.values[this.activeThumbIndex] = nextValue;
            this.values = this.applyConstraints(this.values, this.activeThumbIndex);
            this.syncDom();
            this.syncHiddenInputs();
        },

        handlePointerUp(event) {
            if (this.pointerId !== null && event.pointerId !== this.pointerId) return;
            this.activeThumbIndex = null;
            this.pointerId = null;
            window.removeEventListener('pointermove', this.boundPointerMove);
            window.removeEventListener('pointerup', this.boundPointerUp);
        },

        handleTrackPointerDown(event) {
            if (this.disabled) return;
            const targetValue = this.positionToValue(event);
            const index = this.getNearestThumbIndex(targetValue);
            this.values[index] = targetValue;
            this.values = this.applyConstraints(this.values, index);
            this.syncDom();
            this.syncHiddenInputs();
        },

        handleThumbKeyDown(event) {
            if (this.disabled) return;
            const index = this.readThumbIndex(event.currentTarget);
            if (index < 0) return;
            let delta = 0;
            const pageDelta = this.step * 10;

            if (event.key === 'ArrowRight' || event.key === 'ArrowUp') delta = this.step;
            if (event.key === 'ArrowLeft' || event.key === 'ArrowDown') delta = -this.step;
            if (event.key === 'PageUp') delta = pageDelta;
            if (event.key === 'PageDown') delta = -pageDelta;
            if (event.key === 'Home') this.values[index] = this.min;
            if (event.key === 'End') this.values[index] = this.max;

            if (delta !== 0) {
                this.values[index] = this.values[index] + delta;
            }

            if (delta !== 0 || event.key === 'Home' || event.key === 'End') {
                this.values = this.applyConstraints(this.values, index);
                this.syncDom();
                this.syncHiddenInputs();
                event.preventDefault();
            }
        },

        syncDom() {
            const thumbs = this.getThumbs();
            const range = this.$refs.range;
            const percents = this.values.map(value => this.valueToPercent(value));

            thumbs.forEach((thumb, index) => {
                const percent = percents[index] ?? 0;
                if (this.orientation === 'vertical') {
                    thumb.style.bottom = `${percent}%`;
                } else {
                    thumb.style.left = `${percent}%`;
                }
                thumb.setAttribute('aria-valuenow', `${this.values[index]}`);
            });

            if (!range) return;
            const first = percents[0] ?? 0;
            const last = percents[percents.length - 1] ?? first;
            const start = percents.length > 1 ? Math.min(first, last) : (this.inverted ? last : 0);
            const size = Math.abs(last - start);

            if (this.orientation === 'vertical') {
                range.style.bottom = `${start}%`;
                range.style.height = `${size}%`;
            } else {
                range.style.left = `${start}%`;
                range.style.width = `${size}%`;
            }
        },

        syncHiddenInputs() {
            const inputs = this.$el.querySelectorAll('[data-slider-input]');
            inputs.forEach((input, index) => {
                if (this.values[index] !== undefined) {
                    input.value = `${this.values[index]}`;
                }
            });
        },

        applyConstraints(values, changedIndex = null) {
            const constrained = values.map(value => this.snapValue(this.clamp(value, this.min, this.max)));
            if (constrained.length <= 1) return constrained;

            for (let index = 1; index < constrained.length; index += 1) {
                const minAllowed = constrained[index - 1] + this.minStepsBetweenThumbs;
                if (constrained[index] < minAllowed) constrained[index] = minAllowed;
            }
            for (let index = constrained.length - 2; index >= 0; index -= 1) {
                const maxAllowed = constrained[index + 1] - this.minStepsBetweenThumbs;
                if (constrained[index] > maxAllowed) constrained[index] = maxAllowed;
            }

            if (changedIndex === null) {
                constrained.sort((a, b) => a - b);
            }

            return constrained.map(value => this.snapValue(this.clamp(value, this.min, this.max)));
        },

        snapValue(value) {
            const stepped = Math.round((value - this.min) / this.step) * this.step + this.min;
            return this.clamp(Number(stepped.toFixed(6)), this.min, this.max);
        },

        getNearestThumbIndex(targetValue) {
            let nearestIndex = 0;
            let nearestDistance = Number.MAX_VALUE;
            this.values.forEach((value, index) => {
                const distance = Math.abs(targetValue - value);
                if (distance < nearestDistance) {
                    nearestDistance = distance;
                    nearestIndex = index;
                }
            });
            return nearestIndex;
        },

        positionToValue(event) {
            const track = this.$refs.track;
            if (!track) return this.min;
            const rect = track.getBoundingClientRect();
            let ratio = 0;

            if (this.orientation === 'vertical') {
                ratio = (rect.bottom - event.clientY) / rect.height;
            } else {
                ratio = (event.clientX - rect.left) / rect.width;
            }

            ratio = this.clamp(ratio, 0, 1);
            if (this.inverted) ratio = 1 - ratio;
            const rawValue = this.min + ratio * (this.max - this.min);
            return this.snapValue(rawValue);
        },

        valueToPercent(value) {
            if (this.max === this.min) return 0;
            const ratio = (value - this.min) / (this.max - this.min);
            const percent = this.clamp(ratio * 100, 0, 100);
            return this.inverted ? 100 - percent : percent;
        },

        parseValues(rawValues) {
            try {
                const parsed = JSON.parse(rawValues || '[]');
                if (Array.isArray(parsed) && parsed.length > 0) {
                    return parsed.map(value => this.parseNumber(value, this.min));
                }
            } catch (_) {
                return [this.min];
            }
            return [this.min];
        },

        parseNumber(value, fallback) {
            const parsed = Number(value);
            return Number.isFinite(parsed) ? parsed : fallback;
        },

        readThumbIndex(target) {
            if (!target) return -1;
            const parsed = Number(target.dataset.thumbIndex);
            return Number.isInteger(parsed) ? parsed : -1;
        },

        getThumbs() {
            return Array.from(this.$el.querySelectorAll('[data-thumb-index]'));
        },

        clamp(value, min, max) {
            return Math.min(Math.max(value, min), max);
        },
    }));
}
