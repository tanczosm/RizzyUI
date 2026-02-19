export default function (Alpine, require) {
    Alpine.data('rzColorPicker', () => ({
        colorValue: '',
        inputId: '',
        config: {},
        thumbnailStyle: 'background-color: transparent;',

        init() {
            this.inputId = this.$el.dataset.inputId;
            this.colorValue = this.$el.dataset.defaultColor || '';
            this.syncThumbnail();

            const configString = this.$el.dataset.config;
            this.config = configString ? JSON.parse(configString) : {};

            const assets = JSON.parse(this.$el.dataset.assets || '[]');
            const nonce = this.$el.dataset.nonce;

            require(assets, {
                success: () => {
                    if (!window.Coloris || !this.inputId) {
                        return;
                    }

                    window.Coloris.init();
                    window.Coloris({
                        el: `#${this.inputId}`,
                        ...this.config,
                    });

                    const input = document.getElementById(this.inputId);
                    if (!input) {
                        return;
                    }

                    this.colorValue = input.value || this.colorValue;
                    this.syncThumbnail();

                    input.addEventListener('input', () => {
                        this.colorValue = input.value || '';
                        this.syncThumbnail();
                    });
                },
                error: () => {
                    console.error('Failed to load Coloris assets.');
                }
            }, nonce);
        },

        syncThumbnail() {
            this.thumbnailStyle = this.colorValue ? `background-color: ${this.colorValue};` : 'background-color: transparent;';
        }
    }));
}
