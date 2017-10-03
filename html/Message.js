Vue.component('message', {
  template: '#message_template',
  data() {
    return {};
  },
  computed: {
    textEscaped() {
      let s = this.template ? this.template : this.templates[this.templateId];

      if (this.template) {
        //We disable templateId since we are using a direct raw template
        this.templateId = -1;
      }

      //This hack is required to preserve backwards compatability
      if (this.templateId == CONFIG.defaultTemplateId
          && this.args.length == 1) {
        s = this.templates[CONFIG.defaultAltTemplateId] //Swap out default template :/
      }

      s = s.replace(/{(\d+)}/g, (match, number) => {
        const argEscaped = this.args[number] != undefined ? this.escape(this.args[number]) : match
        return argEscaped;
      });
      return s;
    },
  },
  methods: {
    escape(unsafe) {
      return String(unsafe)
       .replace(/&/g, '&amp;')
       .replace(/</g, '&lt;')
       .replace(/>/g, '&gt;')
       .replace(/"/g, '&quot;')
       .replace(/'/g, '&#039;');
    },
  },
  props: {
    templates: {
      type: Object,
    },
    args: {
      type: Array,
    },
    template: {
      type: String,
      default: null,
    },
    templateId: {
      type: String,
      default: CONFIG.defaultTemplateId,
    },
    multiline: {
      type: Boolean,
      default: false,
    },
    color: { //deprecated
      type: Array,
      default: false,
    },
  },
});
