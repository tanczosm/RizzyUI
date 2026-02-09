import Alpine from 'alpinejs';
import { computePosition, offset, flip, shift } from '@floating-ui/dom';

export default () => ({
    open: false,
    ariaExpanded: 'false',
    parentDropdown: null,
    triggerEl: null,
    contentEl: null,
    menuItems: [],
    focusedIndex: null,
    anchor: 'right-start',
    pixelOffset: 0,
    navThrottle: 100,
    _lastNavAt: 0,
    selfId: null,
    siblingContainer: null,
    closeTimeout: null,
    closeDelay: 150,

    init() {
        if (!this.$el.id) this.$el.id = crypto.randomUUID();
        this.selfId = this.$el.id;

        const parentId = this.$el.dataset.parentId;
        if (parentId) {
            const parentEl = document.getElementById(parentId);
            if (parentEl) this.parentDropdown = Alpine.$data(parentEl);
        }
        if (!this.parentDropdown) {
            console.error('RzDropdownSubmenu could not find its parent RzDropdownMenu controller.');
            return;
        }

        this.triggerEl = this.$refs.subTrigger;
        this.siblingContainer = this.$el.parentElement;
        this.anchor = this.$el.dataset.subAnchor || this.anchor;
        this.pixelOffset = parseInt(this.$el.dataset.subOffset) || this.pixelOffset;

        this.$watch('open', (value) => {
            if (value) {
                this._lastNavAt = 0;
                this.parentDropdown.isSubmenuActive = true;
                this.$nextTick(() => {
                    this.contentEl = document.getElementById(`${this.selfId}-subcontent`);
                    if (!this.contentEl) return;

                    this.updatePosition(this.contentEl);
                    this.menuItems = Array.from(this.contentEl.querySelectorAll('[role^="menuitem"]:not([disabled], [aria-disabled="true"])'));
                });
                this.ariaExpanded = 'true';
                this.triggerEl.dataset.state = 'open';
            } else {
                this.focusedIndex = null;
                this.ariaExpanded = 'false';
                delete this.triggerEl.dataset.state;
                this.$nextTick(() => {
                    const anySubmenuIsOpen = this.parentDropdown.parentEl.querySelector('[x-data^="rzDropdownSubmenu"] [data-state="open"]');
                    if (!anySubmenuIsOpen) this.parentDropdown.isSubmenuActive = false;
                });
                this.contentEl = null;
            }
        });
    },

    updatePosition(contentEl) {
        if (!this.triggerEl || !contentEl) return;
        computePosition(this.triggerEl, contentEl, {
            placement: this.anchor,
            middleware: [offset(this.pixelOffset), flip(), shift({ padding: 8 })],
        }).then(({ x, y }) => {
            Object.assign(contentEl.style, { left: `${x}px`, top: `${y}px` });
        });
    },

    handleTriggerMouseEnter() { clearTimeout(this.closeTimeout); this.triggerEl.focus(); this.openSubmenu(); },
    handleTriggerMouseLeave() { this.closeTimeout = setTimeout(() => this.closeSubmenu(), this.closeDelay); },
    handleContentMouseEnter() { clearTimeout(this.closeTimeout); },

    handleContentMouseLeave() {
        const childSubmenus = this.contentEl?.querySelectorAll('[x-data^="rzDropdownSubmenu"]');
        if (childSubmenus) {
            const isAnyChildOpen = Array.from(childSubmenus).some(el => Alpine.$data(el)?.open);
            if (isAnyChildOpen) return;
        }
        this.closeTimeout = setTimeout(() => this.closeSubmenu(), this.closeDelay);
    },

    openSubmenu(focusFirst = false) {
        if (this.open) return;
        this.closeSiblingSubmenus();
        this.open = true;
        if (focusFirst) this.$nextTick(() => requestAnimationFrame(() => this.focusFirstItem()));
    },

    closeSubmenu() {
        const childSubmenus = this.contentEl?.querySelectorAll('[x-data^="rzDropdownSubmenu"]');
        childSubmenus?.forEach(el => Alpine.$data(el)?.closeSubmenu());
        this.open = false;
    },

    closeSiblingSubmenus() {
        if (!this.siblingContainer) return;
        const siblings = Array.from(this.siblingContainer.children).filter(
            el => el.hasAttribute('x-data') && el.getAttribute('x-data').startsWith('rzDropdownSubmenu') && el.id !== this.selfId
        );
        siblings.forEach(el => Alpine.$data(el)?.closeSubmenu());
    },

    toggleSubmenu() { this.open ? this.closeSubmenu() : this.openSubmenu(); },
    openSubmenuAndFocusFirst() { this.openSubmenu(true); },

    handleTriggerKeydown(e) {
        if (['ArrowRight', 'Enter', ' '].includes(e.key)) {
            e.preventDefault();
            this.openSubmenuAndFocusFirst();
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
    focusCurrentItem() { if (this.focusedIndex !== null && this.menuItems[this.focusedIndex]) this.menuItems[this.focusedIndex].focus(); },

    handleItemClick(event) {
        const item = event.currentTarget;
        if (item.getAttribute('aria-disabled') === 'true' || item.hasAttribute('disabled')) return;
        if (item.getAttribute('aria-haspopup') === 'menu') {
            Alpine.$data(item.closest('[x-data^="rzDropdownSubmenu"]'))?.toggleSubmenu();
            return;
        }
        this.parentDropdown.open = false;
        this.$nextTick(() => this.parentDropdown.triggerEl?.focus());
    },

    handleItemMouseEnter(event) {
        const item = event.currentTarget;
        if (item.getAttribute('aria-disabled') === 'true' || item.hasAttribute('disabled')) return;

        const index = this.menuItems.indexOf(item);
        if (index !== -1) {
            this.focusedIndex = index;
            item.focus();
        }

        if (item.getAttribute('aria-haspopup') === 'menu') {
            Alpine.$data(item.closest('[x-data^="rzDropdownSubmenu"]'))?.openSubmenu();
        } else {
            this.closeSiblingSubmenus();
        }
    },

    handleSubmenuEscape() { if (this.open) { this.open = false; this.$nextTick(() => this.triggerEl?.focus()); } },
    handleSubmenuArrowLeft() { if (this.open) { this.open = false; this.$nextTick(() => this.triggerEl?.focus()); } }
});
