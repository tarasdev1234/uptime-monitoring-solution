import Vue from 'vue'
import VueRouter from 'vue-router'
import { routes } from './routes'

Vue.use(VueRouter);

let router = new VueRouter({
    base: "/admin/",
    mode: 'history',
    linkActiveClass: 'm-menu__item--active',
    routes
})

export default router
