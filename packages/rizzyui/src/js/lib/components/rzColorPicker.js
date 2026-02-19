export default function(Alpine, require) {
    Alpine.data('rzColorPicker', () => ({
        swatchStyle: 'background-color: transparent;',

        init() {
            const inputId = this.$el.dataset.inputId;
            const input = document.getElementById(inputId);
            if (!input) {
                return;
            }

            const config = this.parseConfig();
            const assets = this.parseAssets();
            const nonce = this.$el.dataset.nonce;

            this.updateSwatch(input.value);

            input.addEventListener('input', (event) => {
                this.handleInputEvent(event);
            });

            require(assets, {
                success: () => {
                    if (!window.Coloris) {
                        return;
                    }

                    window.Coloris({
                        el: '#' + inputId,
                        themeMode: 'auto',
                        ...config,
                    });
                },
                error: () => {
                    console.error('Failed to load Coloris assets.');
                }
            }, nonce);
        },

        parseConfig() {
            const configText = this.$el.dataset.config;
            return configText ? JSON.parse(configText) : {};
        },

        parseAssets() {
            const assetsText = this.$el.dataset.assets;
            return assetsText ? JSON.parse(assetsText) : [];
        },

        handleInputEvent(event) {
            this.updateSwatch(event.target.value);
        },

        updateSwatch(value) {
            const safeValue = value && value.trim() ? value : 'transparent';
            this.swatchStyle = 'background-color: ' + safeValue + ';';
        },
    }));
}
