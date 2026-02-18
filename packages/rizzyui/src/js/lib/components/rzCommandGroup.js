// packages/rizzyui/src/js/lib/components/rzCommandGroup.js
export default function(Alpine) {
    Alpine.data('rzCommandGroup', () => ({
        parent: null,
        heading: '',

        init() {
            const parentEl = this.$el.closest('[x-data="rzCommand"]');
            if (!parentEl) {
                console.error('CommandGroup must be a child of RzCommand.');
                return;
            }
            this.parent = Alpine.$data(parentEl);

            this.heading = this.$el.dataset.heading;
            const headingTemplate = this.$el.querySelector('template');

            if (this.heading && headingTemplate?.content) {
                this.parent.registerGroupTemplate(this.heading, headingTemplate.content.cloneNode(true));
            }
        }
    }));
}
