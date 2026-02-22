function getHasAlpha(value) {
    const normalized = value.trim().toLowerCase();

    if (normalized === 'transparent') return true;
    if (/^#(?:[0-9a-f]{4}|[0-9a-f]{8})$/i.test(normalized)) return true;
    if (/\b(?:rgba|hsla)\s*\(/i.test(normalized)) return true;
    if (/\b(?:rgb|hsl|lab|lch|oklab|oklch|color)\s*\([^)]*\/\s*[\d.]+%?\s*\)/i.test(normalized)) return true;

    return false;
}

export default function(Alpine) {
    Alpine.data('rzColorSwatch', () => ({
        value: '',
        ariaLabel: 'No color selected',
        swatchStyle: 'background: linear-gradient(to bottom right, transparent calc(50% - 1px), hsl(var(--destructive)) calc(50% - 1px) calc(50% + 1px), transparent calc(50% + 1px)) no-repeat;',

        init() {
            this.value = this.$el.dataset.initialValue || '';
            this.refresh();
        },

        getValue() {
            return this.value;
        },

        setValue(nextValue) {
            this.value = typeof nextValue === 'string' ? nextValue : '';
            this.refresh();
        },

        getWithoutTransparency() {
            return this.$el.dataset.withoutTransparency === 'true';
        },

        refresh() {
            const colorValue = this.value && this.value.trim().length > 0 ? this.value.trim() : '';

            if (!colorValue) {
                this.ariaLabel = 'No color selected';
                this.swatchStyle = 'background: linear-gradient(to bottom right, transparent calc(50% - 1px), hsl(var(--destructive)) calc(50% - 1px) calc(50% + 1px), transparent calc(50% + 1px)) no-repeat;';
                return;
            }

            this.ariaLabel = 'Color swatch: ' + colorValue;

            if (!this.getWithoutTransparency() && getHasAlpha(colorValue)) {
                this.swatchStyle = `background: linear-gradient(${colorValue}, ${colorValue}), repeating-conic-gradient(#ccc 0% 25%, #fff 0% 50%) 0% 50% / 10px 10px;`;
                return;
            }

            this.swatchStyle = 'background-color: ' + colorValue + ';';
        }
    }));
}
