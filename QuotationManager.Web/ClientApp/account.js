import "babel-polyfill";
import Vue from 'vue';
import Vuetify from 'vuetify';
import VeeValidate from 'vee-validate';

Vue.use(VeeValidate);
Vue.use(Vuetify);

new Vue({
    el: '#app-account',
    render: function (h) {
        return h(require('./components/Account/Account.vue.html'));
    }
});
