import Vue from 'vue'
//import axios from 'axios'
import axios from './http/axios-config'
import router from './router/index'
import store from './store'
import { sync } from 'vuex-router-sync'
import App from 'components/app-root'
import SelectPicker from 'components/core/select-picker'
import httpVueLoader from 'http-vue-loader'

Vue.component('select-picker', SelectPicker);
Vue.use(httpVueLoader);

const dynComp = httpVueLoader("/plugins/test.vue");

router.addRoutes([
    {
        name: "dyn",
        path: "/dyn",
        component: dynComp,
        display: "Dyn",
    }
]);

Vue.prototype.$http = axios;

sync(store, router)

const app = new Vue({
    store,
    router,
    ...App
})

export {
    app,
    router,
    store
}
