import "babel-polyfill";
import Vue from 'vue';
import Vuetify from 'vuetify';
import VeeValidate from 'vee-validate';
import colors from 'vuetify/es5/util/colors'

Vue.use(VeeValidate);

Vue.use(Vuetify,
    {
        theme: {
            primary: colors.blue.darken1, // #E53935
            secondary: colors.blue.lighten4, // #FFCDD2
            accent: colors.blue.base // #3F51B5
        }
    });
/* eslint-disable no-new */
new Vue({
    el: '#app',
    render: function (h) {
        return h(require('./components/Quota/Quota.vue.html'));
    }
});


