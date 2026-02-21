export default function(Alpine) {
    Alpine.data('rzModal', () => ({
        modalOpen: false,
        eventTriggerName: '',
        closeEventName: 'rz:modal-close',
        closeOnEscape: true,
        closeOnClickOutside: true,
        modalId: '',
        bodyId: '',
        footerId: '',
        nonce: '',
        _escapeListener: null,
        _openListener: null,
        _closeEventListener: null,

        init() {
            this.modalId = this.$el.dataset.modalId || '';
            this.bodyId = this.$el.dataset.bodyId || '';
            this.footerId = this.$el.dataset.footerId || '';
            this.nonce = this.$el.dataset.nonce || '';
            this.eventTriggerName = this.$el.dataset.eventTriggerName || '';
            this.closeEventName = this.$el.dataset.closeEventName || this.closeEventName;
            this.closeOnEscape = this.$el.dataset.closeOnEscape !== 'false';
            this.closeOnClickOutside = this.$el.dataset.closeOnClickOutside !== 'false';

            this.$el.dispatchEvent(new CustomEvent('rz:modal-initialized', {
                detail: { modalId: this.modalId, bodyId: this.bodyId, footerId: this.footerId },
                bubbles: true
            }));

            if (this.eventTriggerName) {
                this._openListener = (event) => {
                    this.openModal(event);
                };
                window.addEventListener(this.eventTriggerName, this._openListener);
            }

            this._closeEventListener = () => {
                if (this.modalOpen) {
                    this.closeModalInternally('event');
                }
            };
            window.addEventListener(this.closeEventName, this._closeEventListener);

            this._escapeListener = (event) => {
                if (this.modalOpen && this.closeOnEscape && event.key === 'Escape') {
                    this.closeModalInternally('escape');
                }
            };
            window.addEventListener('keydown', this._escapeListener);

            this.$watch('modalOpen', value => {
                const currentWidth = document.body.offsetWidth;
                document.body.classList.toggle('overflow-hidden', value);
                const scrollBarWidth = document.body.offsetWidth - currentWidth;
                document.body.style.setProperty('--page-scrollbar-width', `${scrollBarWidth}px`);
                if (value) {
                    this.$nextTick(() => {
                        const dialogElement = this.$el.querySelector('[role="dialog"], [role="alertdialog"], [role="document"]');
                        const focusable = dialogElement?.querySelector('button, [href], input:not([type=\'hidden\']), select, textarea, [tabindex]:not([tabindex="-1"])');
                        focusable?.focus();
                        this.$el.dispatchEvent(new CustomEvent('rz:modal-after-open', {
                            detail: { modalId: this.modalId },
                            bubbles: true
                        }));
                    });
                } else {
                    this.$nextTick(() => {
                        this.$el.dispatchEvent(new CustomEvent('rz:modal-after-close', {
                            detail: { modalId: this.modalId },
                            bubbles: true
                        }));
                    });
                }
            });
        },

        notModalOpen() {
            return !this.modalOpen;
        },

        destroy() {
            if (this._openListener && this.eventTriggerName) {
                window.removeEventListener(this.eventTriggerName, this._openListener);
            }
            if (this._closeEventListener) {
                window.removeEventListener(this.closeEventName, this._closeEventListener);
            }
            if (this._escapeListener) {
                window.removeEventListener('keydown', this._escapeListener);
            }
            document.body.classList.remove('overflow-hidden');
            document.body.style.setProperty('--page-scrollbar-width', '0px');
        },

        openModal(event = null) {
            const beforeOpenEvent = new CustomEvent('rz:modal-before-open', {
                detail: { modalId: this.modalId, originalEvent: event },
                bubbles: true,
                cancelable: true
            });
            this.$el.dispatchEvent(beforeOpenEvent);

            if (!beforeOpenEvent.defaultPrevented) {
                this.modalOpen = true;
            }
        },

        closeModalInternally(reason = 'unknown') {
            const beforeCloseEvent = new CustomEvent('rz:modal-before-close', {
                detail: { modalId: this.modalId, reason: reason },
                bubbles: true,
                cancelable: true
            });
            this.$el.dispatchEvent(beforeCloseEvent);

            if (!beforeCloseEvent.defaultPrevented) {
                document.activeElement?.blur && document.activeElement.blur();
                this.modalOpen = false;
                document.body.classList.remove('overflow-hidden');
                document.body.style.setProperty('--page-scrollbar-width', '0px');
            }
        },

        closeModal() {
            this.closeModalInternally('button');
        },

        handleClickOutside() {
            if (this.closeOnClickOutside) {
                this.closeModalInternally('backdrop');
            }
        }
    }));
}
