import { computePosition, offset, flip, shift } from '@floating-ui/dom';

export default function(Alpine) {
    Alpine.data('rzMenubar', () => ({
        openMenuValue: '',
        loop: true,
        triggerElements: [],

        get ariaExpanded() {
            const value = this.currentMenuValue;
            return value && value === this.openMenuValue ? 'true' : 'false';
        },

        get isMenuOpen() {
            return this.openMenuValue === this.$el.dataset.menuValue;
        },

        init() {
            this.loop = this.$el.dataset.loop !== 'false';
            this.triggerElements = Array.from(this.$el.querySelectorAll('[data-slot="menubar-trigger"]'));
        },

        handleTriggerPointerDown(event) {
            const value = event.currentTarget.dataset.menuValue;
            if (!value) return;
            if (this.openMenuValue === value) {
                this.closeMenu();
                return;
            }
            this.openMenu(value);
            event.preventDefault();
        },

        handleTriggerPointerEnter(event) {
            const value = event.currentTarget.dataset.menuValue;
            if (!value || !this.openMenuValue || this.openMenuValue === value) return;
            this.openMenu(value);
            event.currentTarget.focus();
        },

        handleTriggerKeydown(event) {
            const key = event.key;
            const value = event.currentTarget.dataset.menuValue;
            if (!value) return;

            if (key === 'Enter' || key === ' ' || key === 'ArrowDown') {
                this.openMenu(value);
                event.preventDefault();
                return;
            }

            if (key === 'ArrowRight' || key === 'ArrowLeft') {
                this.focusNextTrigger(event.currentTarget, key === 'ArrowRight');
                event.preventDefault();
            }
        },

        handleContentKeydown(event) {
            if (event.key === 'ArrowRight' || event.key === 'ArrowLeft') {
                this.focusNextTriggerByMenu(event.currentTarget, event.key === 'ArrowRight');
                event.preventDefault();
            }
        },

        focusCurrentTarget(event) {
            event.currentTarget.focus();
        },

        handleItemClick(event) {
            if (event.currentTarget.getAttribute('aria-disabled') === 'true') return;
            if (event.currentTarget.getAttribute('aria-haspopup') === 'menu') return;
            this.closeMenu();
        },

        handleCheckboxItemClick(event) {
            const item = event.currentTarget;
            if (item.dataset.disabled === 'true') return;
            const checked = item.getAttribute('aria-checked') === 'true';
            item.setAttribute('aria-checked', checked ? 'false' : 'true');
            item.dataset.state = checked ? 'unchecked' : 'checked';
        },

        handleRadioItemClick(event) {
            const item = event.currentTarget;
            if (item.dataset.disabled === 'true') return;
            const group = item.closest('[data-menubar-radio-group]');
            if (!group) return;
            const value = item.dataset.value;
            group.dataset.selectedValue = value;
            const peers = group.querySelectorAll('[role="menuitemradio"]');
            peers.forEach((peer) => {
                const isMatch = peer.dataset.value === value;
                peer.setAttribute('aria-checked', isMatch ? 'true' : 'false');
                peer.dataset.state = isMatch ? 'checked' : 'unchecked';
            });
        },

        handleOutsideClick() {
            this.closeMenu();
        },

        handleWindowEscape() {
            this.closeMenu();
        },

        openMenu(value) {
            this.openMenuValue = value;
            this.$nextTick(() => {
                const trigger = this.$el.querySelector(`[data-slot=\"menubar-trigger\"][data-menu-value=\"${value}\"]`);
                const content = document.getElementById(`${value}-content`) || this.$el.querySelector(`[data-slot=\"menubar-content\"][data-menu-value=\"${value}\"]`);
                if (!trigger || !content) return;
                trigger.dataset.state = 'open';
                computePosition(trigger, content, {
                    placement: 'bottom-start',
                    middleware: [offset(6), flip(), shift({ padding: 8 })],
                }).then(({ x, y }) => {
                    Object.assign(content.style, { left: `${x}px`, top: `${y}px` });
                });
            });
        },

        closeMenu() {
            this.$el.querySelectorAll('[data-slot="menubar-trigger"]').forEach((trigger) => {
                delete trigger.dataset.state;
            });
            this.openMenuValue = '';
        },

        focusNextTrigger(current, isNext) {
            const triggers = this.triggerElements;
            const currentIndex = triggers.indexOf(current);
            if (currentIndex < 0) return;
            let nextIndex = currentIndex + (isNext ? 1 : -1);
            if (this.loop) {
                if (nextIndex < 0) nextIndex = triggers.length - 1;
                if (nextIndex >= triggers.length) nextIndex = 0;
            } else if (nextIndex < 0 || nextIndex >= triggers.length) {
                return;
            }
            const nextTrigger = triggers[nextIndex];
            nextTrigger.focus();
            if (this.openMenuValue) this.openMenu(nextTrigger.dataset.menuValue);
        },

        focusNextTriggerByMenu(content, isNext) {
            const value = content.dataset.menuValue;
            const trigger = this.$el.querySelector(`[data-slot=\"menubar-trigger\"][data-menu-value=\"${value}\"]`);
            if (!trigger) return;
            this.focusNextTrigger(trigger, isNext);
        },

        get currentMenuValue() {
            return this.$el.dataset.menuValue || '';
        }
    }));

    Alpine.data('rzMenubarSub', () => ({
        open: false,
        triggerEl: null,
        contentEl: null,
        anchor: 'right-start',
        pixelOffset: 4,

        get ariaExpanded() {
            return this.open ? 'true' : 'false';
        },

        init() {
            this.anchor = this.$el.dataset.subAnchor || 'right-start';
            this.pixelOffset = parseInt(this.$el.dataset.subOffset || '4', 10);
            this.triggerEl = this.$refs.subTrigger;
        },

        toggleSubmenu() {
            this.open = !this.open;
            if (this.open) this.positionSubmenu();
        },

        handleSubTriggerMouseEnter() {
            this.open = true;
            this.positionSubmenu();
        },

        handleSubTriggerKeydown(event) {
            if (event.key === 'Enter' || event.key === ' ' || event.key === 'ArrowRight') {
                this.open = true;
                this.positionSubmenu();
                event.preventDefault();
            }
            if (event.key === 'ArrowLeft' || event.key === 'Escape') {
                this.closeSubmenu();
                this.triggerEl?.focus();
                event.preventDefault();
            }
        },

        closeSubmenu() {
            this.open = false;
        },

        positionSubmenu() {
            this.$nextTick(() => {
                this.contentEl = this.$refs.subContent;
                if (!this.triggerEl || !this.contentEl) return;
                computePosition(this.triggerEl, this.contentEl, {
                    placement: this.anchor,
                    middleware: [offset(this.pixelOffset), flip(), shift({ padding: 8 })],
                }).then(({ x, y }) => {
                    Object.assign(this.contentEl.style, { left: `${x}px`, top: `${y}px` });
                });
            });
        }
    }));
}
