export default function (Alpine) {
    Alpine.data('rzInputOTP', () => ({
        value: '',
        length: 0,
        activeIndex: 0,
        isFocused: false,
        isInvalid: false,
        otpType: 'numeric',
        slots: [],

        /**
         * Initializes OTP behavior and hydrates visual slots.
         */
        init() {
            if (this.$el.dataset.rzOtpInitialized === 'true') {
                this.syncFromInput();
                return;
            }

            this.$el.dataset.rzOtpInitialized = 'true';
            this.length = Number(this.$el.dataset.length || '0');
            this.otpType = this.$el.dataset.otpType || 'numeric';
            this.isInvalid = this.$el.dataset.invalid === 'true';
            this.syncFromInput();
        },

        /**
         * Handles input typing updates.
         */
        onInput(event) {
            this.syncFromInput(event?.target);
        },

        /**
         * Handles paste behavior for OTP values.
         * @param {ClipboardEvent} event
         */
        onPaste(event) {
            event.preventDefault();
            const text = event.clipboardData ? event.clipboardData.getData('text') : '';
            const filtered = this.sanitizeValue(text);
            this.value = filtered;
            this.activeIndex = this.normalizeIndex(filtered.length);
            this.applyValueToInput();
            this.refreshSlots();
        },

        /**
         * Handles keyboard navigation and deletion.
         * @param {KeyboardEvent} event
         */
        onKeyDown(event) {
            if (event.key === 'ArrowLeft') {
                event.preventDefault();
                this.moveLeft();
                return;
            }

            if (event.key === 'ArrowRight') {
                event.preventDefault();
                this.moveRight();
                return;
            }

            if (event.key === 'Home') {
                event.preventDefault();
                this.moveHome();
                return;
            }

            if (event.key === 'End') {
                event.preventDefault();
                this.moveEnd();
                return;
            }

            if (event.key === 'Backspace') {
                this.handleBackspace();
            }
        },

        /**
         * Sets focus state to true.
         */
        onFocus() {
            this.isFocused = true;
            this.setActiveFromCaret();
            this.refreshSlots();
        },

        /**
         * Sets focus state to false.
         */
        onBlur() {
            this.isFocused = false;
            this.refreshSlots();
        },

        /**
         * Prevents slot mousedown default so input keeps control.
         * @param {MouseEvent} event
         */
        preventMouseDown(event) {
            event.preventDefault();
        },

        /**
         * Focuses the native input and updates active slot from clicked index.
         * @param {MouseEvent} event
         */
        focusSlot(event) {
            const target = event.currentTarget;
            const index = Number(target.dataset.index || '0');
            this.activeIndex = this.normalizeIndex(index);
            this.focusInput();
            this.setCaretToActiveIndex();
            this.refreshSlots();
        },

        moveLeft() {
            this.activeIndex = this.normalizeIndex(this.activeIndex - 1);
            this.setCaretToActiveIndex();
            this.refreshSlots();
        },

        moveRight() {
            this.activeIndex = this.normalizeIndex(this.activeIndex + 1);
            this.setCaretToActiveIndex();
            this.refreshSlots();
        },

        moveHome() {
            this.activeIndex = 0;
            this.setCaretToActiveIndex();
            this.refreshSlots();
        },

        moveEnd() {
            this.activeIndex = this.normalizeIndex(this.value.length);
            this.setCaretToActiveIndex();
            this.refreshSlots();
        },

        handleBackspace() {
            const input = this.$refs.input;
            if (!input) return;

            const start = input.selectionStart || 0;
            const end = input.selectionEnd || 0;
            if (start !== end) return;
            if (start <= 0) return;

            this.activeIndex = this.normalizeIndex(start - 1);
            this.refreshSlots();
        },

        syncFromInput(sourceInput) {
            const input = sourceInput || this.$refs.input;
            if (!input) return;

            this.value = this.sanitizeValue(input.value || '');
            if (this.value !== input.value) {
                input.value = this.value;
            }

            this.setActiveFromCaret(this.value.length);
            this.refreshSlots();
        },

        sanitizeValue(raw) {
            if (!raw) return '';
            const pattern = this.otpType === 'alphanumeric' ? /[^a-zA-Z0-9]/g : /[^0-9]/g;
            return raw.replace(pattern, '').slice(0, this.length);
        },

        applyValueToInput() {
            const input = this.$refs.input;
            if (!input) return;
            input.value = this.value;
            this.setCaretToActiveIndex();
        },

        setActiveFromCaret(fallbackIndex) {
            const input = this.$refs.input;
            if (!input) {
                this.activeIndex = this.normalizeIndex(fallbackIndex ?? 0);
                return;
            }

            const selectionStart = input.selectionStart;
            const caret = selectionStart == null
                ? Number.isFinite(fallbackIndex) ? Number(fallbackIndex) : 0
                : Number(selectionStart);

            this.activeIndex = this.normalizeIndex(caret);
        },

        setCaretToActiveIndex() {
            const input = this.$refs.input;
            if (!input) return;
            const index = this.normalizeIndex(this.activeIndex);
            input.setSelectionRange(index, index);
        },

        focusInput() {
            const input = this.$refs.input;
            if (!input) return;
            input.focus();
        },

        refreshSlots() {
            this.buildSlotsState();
            this.updateSlotDom();
        },

        buildSlotsState() {
            const next = [];
            let i = 0;
            while (i < this.length) {
                const char = this.value.charAt(i) || '';
                const isActive = i === this.activeIndex;
                const hasFakeCaret = this.isFocused && isActive && char === '';
                next.push({ char, isActive, hasFakeCaret });
                i += 1;
            }
            this.slots = next;
        },

        updateSlotDom() {
            const slotElements = this.$el.querySelectorAll('[data-input-otp-slot="true"]');
            slotElements.forEach((slotElement) => {
                const index = Number(slotElement.dataset.index || '0');
                const state = this.slots[index] || { char: '', isActive: false, hasFakeCaret: false };

                slotElement.dataset.active = state.isActive ? 'true' : 'false';
                if (this.isInvalid) {
                    slotElement.setAttribute('aria-invalid', 'true');
                } else {
                    slotElement.removeAttribute('aria-invalid');
                }

                const charElement = slotElement.querySelector('[data-input-otp-char="true"]');
                if (charElement) {
                    charElement.textContent = state.char;
                }

                const caretWrapper = slotElement.querySelector('[data-input-otp-caret="true"]');
                if (caretWrapper) {
                    if (state.hasFakeCaret) {
                        caretWrapper.classList.remove('hidden');
                    } else {
                        caretWrapper.classList.add('hidden');
                    }
                }
            });
        },

        normalizeIndex(index) {
            if (this.length <= 0) return 0;
            if (index < 0) return 0;
            if (index > this.length - 1) return this.length - 1;
            return index;
        }
    }));
}
