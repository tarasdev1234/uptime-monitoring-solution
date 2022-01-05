<template>
    <div class="m-grid m-grid--hor m-grid--root m-page">
        <header-component />
        <div class="m-grid__item m-grid__item--fluid m-grid m-grid--ver-desktop m-grid--desktop m-body">
            <nav-menu params="route: route"></nav-menu>
            <router-view></router-view>
        </div>
        <footer-component />
    </div>
</template>

<script>
    import NavMenu from './nav-menu'
    import Header from './header'
    import Footer from './footer'

    export default {
        components: {
            'nav-menu': NavMenu,
            'header-component': Header,
            'footer-component': Footer
        },

        data() {
            return {}
        }
    }
</script>

<style>
</style>
