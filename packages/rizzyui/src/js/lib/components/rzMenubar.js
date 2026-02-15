import { computePosition, offset, flip, shift } from '@floating-ui/dom';

export default function (Alpine) {
    Alpine.data('rzMenubar', () => ({
        handleWindowEscape() {
            const menus = this.$el.querySelectorAll('[x-data="rzMenubarMenu"]');
            menus.forEach((menu) => {
                const data = Alpine.$data(menu);
                if (data?.open) data.closeMenu();
            });
        }
    }));

    Alpine.data('rzMenubarMenu', () => ({
        open: false,
        ariaExpanded: 'false',
        triggerEl: null,
        contentEl: null,
        menuItems: [],
        focusedIndex: null,

        init() {
            this.triggerEl = this.$refs.trigger;
            this.$watch('open', (value) => {
                if (value) {
                    this.ariaExpanded = 'true';
                    this.triggerEl?.setAttribute('data-state', 'open');
                    this.$nextTick(() => {
                        this.contentEl = document.getElementById(`${this.$el.dataset.menuId}-content`);
                        if (!this.contentEl) return;
                        this.updatePosition();
                        this.menuItems = Array.from(this.contentEl.querySelectorAll('[role^="menuitem"]:not([disabled],[aria-disabled="true"])'));
                    });
                } else {
                    this.ariaExpanded = 'false';
                    this.triggerEl?.removeAttribute('data-state');
                    this.focusedIndex = null;
                    this.closeAllSubmenus();
                }
            });
        },

        updatePosition() {
            if (!this.triggerEl || !this.contentEl) return;
            computePosition(this.triggerEl, this.contentEl, {
                placement: 'bottom-start',
                middleware: [offset(4), flip(), shift({ padding: 8 })]
            }).then(({ x, y }) => {
                Object.assign(this.contentEl.style, { left: `${x}px`, top: `${y}px` });
            });
        },

        closeSiblingMenus() {
            const menus = this.$el.parentElement?.querySelectorAll('[x-data="rzMenubarMenu"]') ?? [];
            menus.forEach((menu) => {
                if (menu !== this.$el) {
                    const data = Alpine.$data(menu);
                    if (data?.open) data.closeMenu();
                }
            });
        },

        toggle() {
            if (this.open) {
                this.closeMenu();
                this.triggerEl?.focus();
                return;
            }
            this.openMenu();
        },

        openMenu() {
            this.closeSiblingMenus();
            this.open = true;
        },

        closeMenu() {
            this.open = false;
        },

        handleOutsideClick() {
            if (!this.open) return;
            this.closeMenu();
            this.triggerEl?.focus();
        },

        handleTriggerMouseEnter() {
            const siblings = this.$el.parentElement?.querySelectorAll('[x-data="rzMenubarMenu"]') ?? [];
            const anotherOpen = Array.from(siblings).some((menu) => menu !== this.$el && Alpine.$data(menu)?.open);
            if (anotherOpen) this.openMenu();
        },

        handleTriggerKeydown(event) {
            if (!['Enter', ' ', 'ArrowDown'].includes(event.key)) return;
            this.openMenu();
            this.$nextTick(() => this.focusFirstItem());
        },

        focusNextMenubarTrigger() {
            this.focusAdjacentTrigger(1);
        },

        focusPreviousMenubarTrigger() {
            this.focusAdjacentTrigger(-1);
        },

        focusAdjacentTrigger(delta) {
            const triggers = Array.from(this.$el.parentElement?.querySelectorAll('[x-data="rzMenubarMenu"] [x-ref="trigger"]') ?? []);
            if (!triggers.length || !this.triggerEl) return;
            const index = triggers.indexOf(this.triggerEl);
            if (index < 0) return;
            const nextIndex = (index + delta + triggers.length) % triggers.length;
            const nextTrigger = triggers[nextIndex];
            nextTrigger?.focus();
            const nextMenuData = Alpine.$data(nextTrigger.closest('[x-data="rzMenubarMenu"]'));
            if (this.open && nextMenuData) nextMenuData.openMenu();
        },

        focusNextItem() {
            if (!this.menuItems.length) return;
            this.focusedIndex = this.focusedIndex === null || this.focusedIndex >= this.menuItems.length - 1 ? 0 : this.focusedIndex + 1;
            this.focusCurrentItem();
        },

        focusPreviousItem() {
            if (!this.menuItems.length) return;
            this.focusedIndex = this.focusedIndex === null || this.focusedIndex <= 0 ? this.menuItems.length - 1 : this.focusedIndex - 1;
            this.focusCurrentItem();
        },

        focusFirstItem() {
            if (!this.menuItems.length) return;
            this.focusedIndex = 0;
            this.focusCurrentItem();
        },

        focusLastItem() {
            if (!this.menuItems.length) return;
            this.focusedIndex = this.menuItems.length - 1;
            this.focusCurrentItem();
        },

        focusCurrentItem() {
            if (this.focusedIndex === null) return;
            this.menuItems[this.focusedIndex]?.focus();
        },

        handleItemMouseEnter(event) {
            const item = event.currentTarget;
            if (!item || item.getAttribute('aria-disabled') === 'true') return;
            item.focus();
            if (item.getAttribute('aria-haspopup') !== 'menu') this.closeAllSubmenus();
        },

        handleItemClick(event) {
            const item = event.currentTarget;
            if (!item || item.getAttribute('aria-disabled') === 'true') return;
            if (item.getAttribute('aria-haspopup') === 'menu') return;
            this.closeMenu();
            this.triggerEl?.focus();
        },

        toggleCheckboxItem(event) {
            const item = event.currentTarget;
            if (!item) return;
            const checked = item.getAttribute('aria-checked') === 'true';
            item.setAttribute('aria-checked', checked ? 'false' : 'true');
        },

        selectRadioItem(event) {
            const item = event.currentTarget;
            if (!item) return;
            const group = item.closest('[data-radio-group-value]');
            if (!group) return;
            const items = group.querySelectorAll('[role="menuitemradio"]');
            items.forEach((radio) => radio.setAttribute('aria-checked', 'false'));
            item.setAttribute('aria-checked', 'true');
            group.dataset.radioGroupValue = item.dataset.radioItemValue || '';
        },

        closeAllSubmenus() {
            const submenus = this.contentEl?.querySelectorAll('[x-data="rzMenubarSubmenu"]') ?? [];
            submenus.forEach((submenu) => {
                const data = Alpine.$data(submenu);
                if (data?.open) data.closeSubmenu();
            });
        }
    }));

    Alpine.data('rzMenubarSubmenu', () => ({
        open: false,
        ariaExpanded: 'false',
        triggerEl: null,
        contentEl: null,

        init() {
            this.triggerEl = this.$refs.subTrigger;
            this.$watch('open', (value) => {
                this.ariaExpanded = value ? 'true' : 'false';
                if (value) {
                    this.$nextTick(() => {
                        this.contentEl = document.getElementById(`${this.$el.id}-subcontent`);
                        if (!this.contentEl || !this.triggerEl) return;
                        computePosition(this.triggerEl, this.contentEl, {
                            placement: this.$el.dataset.subAnchor || 'right-start',
                            middleware: [offset(parseInt(this.$el.dataset.subOffset || '-4', 10)), flip(), shift({ padding: 8 })]
                        }).then(({ x, y }) => {
                            Object.assign(this.contentEl.style, { left: `${x}px`, top: `${y}px` });
                        });
                    });
                }
            });
        },

        toggleSubmenu() {
            this.open = !this.open;
        },

        closeSubmenu() {
            this.open = false;
        },

        handleTriggerMouseEnter() {
            this.open = true;
        },

        handleTriggerMouseLeave() {},
        handleContentMouseEnter() {},
        handleContentMouseLeave() {
            this.open = false;
        },

        handleTriggerKeydown(event) {
            if (['Enter', ' ', 'ArrowRight'].includes(event.key)) {
                this.open = true;
                this.$nextTick(() => this.contentEl?.querySelector('[role^="menuitem"]')?.focus());
            }
        },

        handleSubmenuEscape() {
            this.open = false;
            this.triggerEl?.focus();
        },

        handleSubmenuArrowLeft() {
            this.open = false;
            this.triggerEl?.focus();
        },

        focusNextItem() {},
        focusPreviousItem() {},
        focusFirstItem() {},
        focusLastItem() {}
    }));
}
