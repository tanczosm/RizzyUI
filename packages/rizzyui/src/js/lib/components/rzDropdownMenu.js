import Alpine from 'alpinejs';
import { computePosition, offset, flip, shift } from '@floating-ui/dom';

export default () => ({
    open: false,
    isModal: true,
    ariaExpanded: 'false',
    trapActive: false,
    focusedIndex: null,
    menuItems: [],
    parentEl: null,
    triggerEl: null,
    contentEl: null,
    anchor: 'bottom',
    pixelOffset: 3,
    isSubmenuActive: false,
    navThrottle: 100,
    _lastNavAt: 0,
    selfId: null,

    init() {
        if (!this.$el.id) this.$el.id = crypto.randomUUID();
        this.selfId = this.$el.id;
        this.parentEl = this.$el;
        this.triggerEl = this.$refs.trigger;
        this.anchor = this.$el.dataset.anchor || 'bottom';
        this.pixelOffset = parseInt(this.$el.dataset.offset) || 6;
        this.isModal = (this.$el.dataset.modal !== 'false');

        this.$watch('open', (value) => {
            if (value) {
                this._lastNavAt = 0;
                this.$nextTick(() => {
                    this.contentEl = document.getElementById(`${this.selfId}-content`);
                    if (!this.contentEl) return;

                    this.updatePosition();
                    this.menuItems = Array.from(this.contentEl.querySelectorAll('[role^="menuitem"]:not([disabled],[aria-disabled="true"])'));
                });
                this.ariaExpanded = 'true';
                this.triggerEl.dataset.state = 'open';
                this.trapActive = this.isModal;
            } else {
                this.focusedIndex = null;
                this.closeAllSubmenus();
                this.ariaExpanded = 'false';
                delete this.triggerEl.dataset.state;
                this.trapActive = false;
                this.contentEl = null;
            }
        });
    },

    updatePosition() {
        if (!this.triggerEl || !this.contentEl) return;
        this.contentEl.style.setProperty('--rizzy-dropdown-trigger-width', `${this.triggerEl.offsetWidth}px`);
        computePosition(this.triggerEl, this.contentEl, {
            placement: this.anchor,
            middleware: [offset(this.pixelOffset), flip(), shift({ padding: 8 })],
        }).then(({ x, y }) => {
            Object.assign(this.contentEl.style, { left: `${x}px`, top: `${y}px` });
        });
    },

    toggle() {
        if (this.open) {
            this.open = false;
            this.$nextTick(() => this.triggerEl?.focus());
        } else {
            this.open = true;
            this.focusedIndex = -1;
        }
    },

    handleOutsideClick() {
        if (!this.open) return;
        this.open = false;
        this.$nextTick(() => this.triggerEl?.focus());
    },

    handleTriggerKeydown(event) {
        if (['Enter', ' ', 'ArrowDown', 'ArrowUp'].includes(event.key)) {
            event.preventDefault();
            this.open = true;
            this.$nextTick(() => {
                if (event.key === 'ArrowUp') this.focusLastItem();
                else this.focusFirstItem();
            });
        }
    },

    focusNextItem() {
        const now = Date.now();
        if (now - this._lastNavAt < this.navThrottle) return;
        this._lastNavAt = now;
        if (!this.menuItems.length) return;
        this.focusedIndex = (this.focusedIndex === null || this.focusedIndex >= this.menuItems.length - 1) ? 0 : this.focusedIndex + 1;
        this.focusCurrentItem();
    },

    focusPreviousItem() {
        const now = Date.now();
        if (now - this._lastNavAt < this.navThrottle) return;
        this._lastNavAt = now;
        if (!this.menuItems.length) return;
        this.focusedIndex = (this.focusedIndex === null || this.focusedIndex <= 0) ? this.menuItems.length - 1 : this.focusedIndex - 1;
        this.focusCurrentItem();
    },

    focusFirstItem() { if (!this.menuItems.length) return; this.focusedIndex = 0; this.focusCurrentItem(); },
    focusLastItem() { if (!this.menuItems.length) return; this.focusedIndex = this.menuItems.length - 1; this.focusCurrentItem(); },
    focusCurrentItem() { if (this.focusedIndex !== null && this.menuItems[this.focusedIndex]) this.$nextTick(() => this.menuItems[this.focusedIndex].focus()); },

    focusSelectedItem(item) {
        if (!item || item.getAttribute('aria-disabled') === 'true' || item.hasAttribute('disabled')) return;
        const index = this.menuItems.indexOf(item);
        if (index !== -1) {
            this.focusedIndex = index;
            item.focus();
        }
    },

    handleItemClick(event) {
        const item = event.currentTarget;
        if (item.getAttribute('aria-disabled') === 'true' || item.hasAttribute('disabled')) return;
        if (item.getAttribute('aria-haspopup') === 'menu') {
            Alpine.$data(item.closest('[x-data^="rzDropdownSubmenu"]'))?.toggleSubmenu();
            return;
        }
        this.open = false;
        this.$nextTick(() => this.triggerEl?.focus());
    },

    handleItemMouseEnter(event) {
        const item = event.currentTarget;
        this.focusSelectedItem(item);
        if (item.getAttribute('aria-haspopup') !== 'menu') {
            this.closeAllSubmenus();
        }
    },

    handleWindowEscape() { if (this.open) { this.open = false; this.$nextTick(() => this.triggerEl?.focus()); } },
    handleContentTabKey() { if (this.open) { this.open = false; this.$nextTick(() => this.triggerEl?.focus()); } },
    handleTriggerMouseover() { this.$nextTick(() => this.$el.firstElementChild?.focus()); },

    closeAllSubmenus() {
        const submenus = this.parentEl.querySelectorAll('[x-data^="rzDropdownSubmenu"]');
        submenus.forEach(el => Alpine.$data(el)?.closeSubmenu());
        this.isSubmenuActive = false;
    }
});
