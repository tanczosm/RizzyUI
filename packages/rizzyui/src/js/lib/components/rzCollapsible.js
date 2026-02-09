
export default () => ({
        isOpen: false,

        init() {
            this.isOpen = this.$el.dataset.defaultOpen === 'true';
        },

        toggle() {
            this.isOpen = !this.isOpen;
        },

        state() {
            return this.isOpen ? 'open' : 'closed';
        }
    });
