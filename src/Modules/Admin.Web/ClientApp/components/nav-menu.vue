<template>
    <div id="m_aside_left" class="m-grid__item	m-aside-left  m-aside-left--skin-dark ">
        <!-- BEGIN: Aside Menu -->
        <div id="m_ver_menu" class="m-aside-menu  m-aside-menu--skin-dark m-aside-menu--submenu-skin-dark " data-menu-vertical="true" data-menu-scrollable="false" data-menu-dropdown-timeout="500">
            <ul class="m-menu__nav  m-menu__nav--dropdown-submenu-arrow ">

                <router-link class="m-menu__item" tag="li" to="/" :exact="true">
                    <a class="m-menu__link">
                        <i class="m-menu__link-icon flaticon-line-graph"></i>
                        <span class="m-menu__link-title">
                            <span class="m-menu__link-wrap">
                                <span class="m-menu__link-text">
                                    Dashboard
                                </span>
                            </span>
                        </span>
                    </a>
                </router-link>
                <li class="m-menu__section">
                    <h4 class="m-menu__section-text">
                        Menu
                    </h4>
                    <i class="m-menu__section-icon flaticon-more-v3"></i>
                </li>
                <router-link tag="li" to="/brands" class="m-menu__item">
                    <a class="m-menu__link m-menu__toggle">
                        <i class="m-menu__link-icon flaticon-suitcase"></i>
                        <span class="m-menu__link-text">
                            Brands
                        </span>
                    </a>
                </router-link>
                <li class="m-menu__item  m-menu__item--submenu" aria-haspopup="true" data-menu-submenu-toggle="hover">
                    <a href="#" class="m-menu__link m-menu__toggle">
                        <i class="m-menu__link-icon flaticon-users"></i>
                        <span class="m-menu__link-text">
                            Users
                        </span>
                        <i class="m-menu__ver-arrow la la-angle-right"></i>
                    </a>
                    <div class="m-menu__submenu ">
                        <span class="m-menu__arrow"></span>
                        <ul class="m-menu__subnav">
                            <li class="m-menu__item  m-menu__item--parent" aria-haspopup="true">
                                <span class="m-menu__link">
                                    <span class="m-menu__link-text">
                                        Users
                                    </span>
                                </span>
                            </li>
                            <router-link tag="li" to="/user" class="m-menu__item " aria-haspopup="true">
                                <a class="m-menu__link ">
                                    <i class="m-menu__link-bullet m-menu__link-bullet--dot">
                                        <span></span>
                                    </i>
                                    <span class="m-menu__link-text">
                                        Users
                                    </span>
                                </a>
                            </router-link>
                            <router-link tag="li" to="/roles" class="m-menu__item " aria-haspopup="true">
                                <a class="m-menu__link ">
                                    <i class="m-menu__link-bullet m-menu__link-bullet--dot">
                                        <span></span>
                                    </i>
                                    <span class="m-menu__link-text">
                                        Roles
                                    </span>
                                </a>
                            </router-link>
                            <router-link tag="li" to="/companies" class="m-menu__item " aria-haspopup="true">
                                <a class="m-menu__link ">
                                    <i class="m-menu__link-bullet m-menu__link-bullet--dot">
                                        <span></span>
                                    </i>
                                    <span class="m-menu__link-text">
                                        Companies
                                    </span>
                                </a>
                            </router-link>
                        </ul>
                    </div>
                </li>
                <li class="m-menu__item  m-menu__item--submenu" aria-haspopup="true" data-menu-submenu-toggle="hover">
                    <a href="#" class="m-menu__link m-menu__toggle">
                        <i class="m-menu__link-icon flaticon-piggy-bank"></i>
                        <span class="m-menu__link-text">
                            Billing
                        </span>
                        <i class="m-menu__ver-arrow la la-angle-right"></i>
                    </a>
                    <div class="m-menu__submenu ">
                        <span class="m-menu__arrow"></span>
                        <ul class="m-menu__subnav">
                            <li class="m-menu__item  m-menu__item--parent" aria-haspopup="true">
                                <span class="m-menu__link">
                                    <span class="m-menu__link-text">
                                        Billing
                                    </span>
                                </span>
                            </li>
                            <router-link tag="li" to="/billing/currencies" class="m-menu__item " aria-haspopup="true">
                                <a class="m-menu__link ">
                                    <i class="m-menu__link-bullet m-menu__link-bullet--dot">
                                        <span></span>
                                    </i>
                                    <span class="m-menu__link-text">
                                        Currencies
                                    </span>
                                </a>
                            </router-link>
                            <router-link tag="li" to="/billing/product-groups" class="m-menu__item " aria-haspopup="true">
                                <a class="m-menu__link ">
                                    <i class="m-menu__link-bullet m-menu__link-bullet--dot">
                                        <span></span>
                                    </i>
                                    <span class="m-menu__link-text">
                                        Product Groups
                                    </span>
                                </a>
                            </router-link>
                            <router-link tag="li" to="/billing/products" class="m-menu__item " aria-haspopup="true">
                                <a class="m-menu__link ">
                                    <i class="m-menu__link-bullet m-menu__link-bullet--dot">
                                        <span></span>
                                    </i>
                                    <span class="m-menu__link-text">
                                        Products/Services
                                    </span>
                                </a>
                            </router-link>
                        </ul>
                    </div>
                </li>
                <li class="m-menu__section">
                    <h4 class="m-menu__section-text">
                        Support
                    </h4>
                </li>
                <li class="m-menu__item  m-menu__item--submenu" aria-haspopup="true" data-menu-submenu-toggle="hover">
                    <a href="#" class="m-menu__link m-menu__toggle">
                        <i class="m-menu__link-icon fa fa-ticket"></i>
                        <span class="m-menu__link-text">
                            Tickets
                        </span>
                        <i class="m-menu__ver-arrow la la-angle-right"></i>
                    </a>
                    <div class="m-menu__submenu ">
                        <span class="m-menu__arrow"></span>
                        <ul class="m-menu__subnav">
                            <li class="m-menu__item  m-menu__item--parent" aria-haspopup="true">
                                <span class="m-menu__link">
                                    <span class="m-menu__link-text">
                                        Tickets
                                    </span>
                                </span>
                            </li>
                            <li class="m-menu__section">
                                <h4 class="m-menu__section-text">
                                    My tickets
                                </h4>
                                <i class="m-menu__section-icon flaticon-more-v3"></i>
                            </li>
                            <router-link tag="li" :to="{ name: 'tickets', params: { status: 'active', global: false }}" class="m-menu__item " aria-haspopup="true">
                                <a class="m-menu__link ">
                                    <i class="m-menu__link-bullet m-menu__link-bullet--dot">
                                        <span></span>
                                    </i>
                                    <span class="m-menu__link-title">
                                        <span class="m-menu__link-wrap">
                                            <span class="m-menu__link-text">
                                                Active
                                            </span>
                                            <span v-show="summary.active > 0" class="m-menu__link-badge">
                                                <span class="m-badge m-badge--danger">
                                                    {{ summary.active }}
                                                </span>
                                            </span>
                                        </span>
                                    </span>
                                </a>
                            </router-link>
                            <router-link tag="li" :to="{ name: 'tickets', params: { status: 'waiting', global: false }}" class="m-menu__item " aria-haspopup="true">
                                <a class="m-menu__link ">
                                    <i class="m-menu__link-bullet m-menu__link-bullet--dot">
                                        <span></span>
                                    </i>
                                    <span class="m-menu__link-title">
                                        <span class="m-menu__link-wrap">
                                            <span class="m-menu__link-text">
                                                Waiting
                                            </span>
                                            <span v-show="summary.waiting > 0" class="m-menu__link-badge">
                                                <span class="m-badge m-badge--danger">
                                                    {{ summary.waiting }}
                                                </span>
                                            </span>
                                        </span>
                                    </span>
                                </a>
                            </router-link>
                            <router-link tag="li" :to="{ name: 'tickets', params: { status: 'closed', global: false }}" class="m-menu__item " aria-haspopup="true">
                                <a class="m-menu__link ">
                                    <i class="m-menu__link-bullet m-menu__link-bullet--dot">
                                        <span></span>
                                    </i>
                                    <span class="m-menu__link-text">
                                        Closed
                                    </span>
                                </a>
                            </router-link>
                            <li class="m-menu__section">
                                <h4 class="m-menu__section-text">
                                    Global tickets
                                </h4>
                                <i class="m-menu__section-icon flaticon-more-v3"></i>
                            </li>
                            <router-link tag="li" :to="{ name: 'tickets', params: { status: 'queue', global: true }}" class="m-menu__item " aria-haspopup="true">
                                <a class="m-menu__link ">
                                    <i class="m-menu__link-bullet m-menu__link-bullet--dot">
                                        <span></span>
                                    </i>
                                    <span class="m-menu__link-title">
                                        <span class="m-menu__link-wrap">
                                            <span class="m-menu__link-text">
                                                Queue
                                            </span>
                                            <span v-show="summary.queue > 0" class="m-menu__link-badge">
                                                <span class="m-badge m-badge--danger">
                                                    {{ summary.queue }}
                                                </span>
                                            </span>
                                        </span>
                                    </span>
                                </a>
                            </router-link>
                            <router-link tag="li" :to="{ name: 'tickets', params: { status: 'gactive', global: true }}" class="m-menu__item " aria-haspopup="true">
                                <a class="m-menu__link ">
                                    <i class="m-menu__link-bullet m-menu__link-bullet--dot">
                                        <span></span>
                                    </i>
                                    <span class="m-menu__link-title">
                                        <span class="m-menu__link-wrap">
                                            <span class="m-menu__link-text">
                                                Active
                                            </span>
                                            <span v-show="summary.activeGlobal > 0" class="m-menu__link-badge">
                                                <span class="m-badge m-badge--danger">
                                                    {{ summary.activeGlobal }}
                                                </span>
                                            </span>
                                        </span>
                                    </span>
                                </a>
                            </router-link>
                            <router-link tag="li" :to="{ name: 'tickets', params: { status: 'gwaiting', global: true }}" class="m-menu__item " aria-haspopup="true">
                                <a class="m-menu__link ">
                                    <i class="m-menu__link-bullet m-menu__link-bullet--dot">
                                        <span></span>
                                    </i>
                                    <span class="m-menu__link-title">
                                        <span class="m-menu__link-wrap">
                                            <span class="m-menu__link-text">
                                                Waiting
                                            </span>
                                            <span v-show="summary.waitingGlobal > 0" class="m-menu__link-badge">
                                                <span class="m-badge m-badge--danger">
                                                    {{ summary.waitingGlobal }}
                                                </span>
                                            </span>
                                        </span>
                                    </span>
                                </a>
                            </router-link>
                            <router-link tag="li" :to="{ name: 'tickets', params: { status: 'gclosed', global: true }}" class="m-menu__item " aria-haspopup="true">
                                <a class="m-menu__link ">
                                    <i class="m-menu__link-bullet m-menu__link-bullet--dot">
                                        <span></span>
                                    </i>
                                    <span class="m-menu__link-text">
                                        Closed
                                    </span>
                                </a>
                            </router-link>
                        </ul>
                    </div>
                </li>
                <router-link tag="li" to="/departments" class="m-menu__item " aria-haspopup="true">
                    <a class="m-menu__link">
                        <i class="m-menu__link-icon flaticon-puzzle"></i>
                        <span class="m-menu__link-text">
                            Departments
                        </span>
                    </a>
                </router-link>
                <li class="m-menu__item  m-menu__item--submenu" aria-haspopup="true" data-menu-submenu-toggle="hover">
                    <a href="#" class="m-menu__link m-menu__toggle">
                        <i class="m-menu__link-icon flaticon-questions-circular-button"></i>
                        <span class="m-menu__link-text">
                            Knowledge Base
                        </span>
                        <i class="m-menu__ver-arrow la la-angle-right"></i>
                    </a>
                    <div class="m-menu__submenu ">
                        <span class="m-menu__arrow"></span>
                        <ul class="m-menu__subnav">
                            <li class="m-menu__item  m-menu__item--parent" aria-haspopup="true">
                                <span class="m-menu__link">
                                    <span class="m-menu__link-text">
                                        Knowledge Base
                                    </span>
                                </span>
                            </li>
                            <router-link tag="li" to="/kb/articles" class="m-menu__item " aria-haspopup="true">
                                <a class="m-menu__link ">
                                    <i class="m-menu__link-bullet m-menu__link-bullet--dot">
                                        <span></span>
                                    </i>
                                    <span class="m-menu__link-text">
                                        Articles
                                    </span>
                                </a>
                            </router-link>
                            <!--<router-link tag="li" to="/kb/series" class="m-menu__item " aria-haspopup="true">
                    <a class="m-menu__link ">
                        <i class="m-menu__link-bullet m-menu__link-bullet--dot">
                            <span></span>
                        </i>
                        <span class="m-menu__link-text">
                            Series
                        </span>
                    </a>
                </router-link>-->
                            <router-link tag="li" to="/kb/categories" class="m-menu__item " aria-haspopup="true">
                                <a class="m-menu__link ">
                                    <i class="m-menu__link-bullet m-menu__link-bullet--dot">
                                        <span></span>
                                    </i>
                                    <span class="m-menu__link-text">
                                        Categories
                                    </span>
                                </a>
                            </router-link>
                            <router-link tag="li" to="/kb/tags" class="m-menu__item " aria-haspopup="true">
                                <a class="m-menu__link ">
                                    <i class="m-menu__link-bullet m-menu__link-bullet--dot">
                                        <span></span>
                                    </i>
                                    <span class="m-menu__link-text">
                                        Tags
                                    </span>
                                </a>
                            </router-link>
                        </ul>
                    </div>
                </li>
                <li class="m-menu__section">
                    <h4 class="m-menu__section-text">
                        Settings
                    </h4>
                    <i class="m-menu__section-icon flaticon-more-v3"></i>
                </li>
                <router-link tag="li" to="/plugins" class="m-menu__item">
                    <a class="m-menu__link m-menu__toggle">
                        <i class="m-menu__link-icon flaticon-squares-4"></i>
                        <span class="m-menu__link-text">
                            Plugins
                        </span>
                    </a>
                </router-link>
                <router-link tag="li" to="/themes" class="m-menu__item">
                    <a class="m-menu__link m-menu__toggle">
                        <i class="m-menu__link-icon flaticon-layers"></i>
                        <span class="m-menu__link-text">
                            Themes
                        </span>
                    </a>
                </router-link>
                <li class="m-menu__item  m-menu__item--submenu" aria-haspopup="true" data-menu-submenu-toggle="hover">
                    <a href="#" class="m-menu__link m-menu__toggle">
                        <i class="m-menu__link-icon fa fa-envelope-o"></i>
                        <span class="m-menu__link-text">
                            Email
                        </span>
                        <i class="m-menu__ver-arrow la la-angle-right"></i>
                    </a>
                    <div class="m-menu__submenu ">
                        <span class="m-menu__arrow"></span>
                        <ul class="m-menu__subnav">
                            <li class="m-menu__item  m-menu__item--parent" aria-haspopup="true">
                                <span class="m-menu__link">
                                    <span class="m-menu__link-text">
                                        Email
                                    </span>
                                </span>
                            </li>
                            <router-link tag="li" to="/email/smtp" class="m-menu__item " aria-haspopup="true">
                                <a class="m-menu__link ">
                                    <i class="m-menu__link-bullet m-menu__link-bullet--dot">
                                        <span></span>
                                    </i>
                                    <span class="m-menu__link-text">
                                        SMTP Servers
                                    </span>
                                </a>
                            </router-link>
                            <router-link tag="li" to="/email/pop" class="m-menu__item " aria-haspopup="true">
                                <a class="m-menu__link ">
                                    <i class="m-menu__link-bullet m-menu__link-bullet--dot">
                                        <span></span>
                                    </i>
                                    <span class="m-menu__link-text">
                                        POP Servers
                                    </span>
                                </a>
                            </router-link>
                            <router-link tag="li" to="/email/responders" class="m-menu__item " aria-haspopup="true">
                                <a class="m-menu__link ">
                                    <i class="m-menu__link-bullet m-menu__link-bullet--dot">
                                        <span></span>
                                    </i>
                                    <span class="m-menu__link-text">
                                        Autoresponders
                                    </span>
                                </a>
                            </router-link>
                        </ul>
                    </div>
                </li>
                <li class="m-menu__section">
                    <h4 class="m-menu__section-text">
                        Uptime Monitoring
                    </h4>
                    <i class="m-menu__section-icon flaticon-more-v3"></i>
                </li>
                <router-link tag="li" to="/uptime/servers" class="m-menu__item">
                    <a class="m-menu__link m-menu__toggle">
                        <i class="m-menu__link-icon flaticon-squares-4"></i>
                        <span class="m-menu__link-text">
                            Servers
                        </span>
                    </a>
                </router-link>
                <li class="m-menu__section">
                    <h4 class="m-menu__section-text">
                        Notification Configuration
                    </h4>
                    <i class="m-menu__section-icon flaticon-more-v3"></i>
                </li>
                <li class="m-menu__item  m-menu__item--submenu" aria-haspopup="true" data-menu-submenu-toggle="hover">
                    <a href="#" class="m-menu__link m-menu__toggle">
                        <i class="m-menu__link-icon fa fa-envelope-o"></i>
                        <span class="m-menu__link-text">
                            Email
                        </span>
                        <i class="m-menu__ver-arrow la la-angle-right"></i>
                    </a>
                    <div class="m-menu__submenu ">
                        <span class="m-menu__arrow"></span>
                        <ul class="m-menu__subnav">
                            <li class="m-menu__item  m-menu__item--parent" aria-haspopup="true">
                                <span class="m-menu__link">
                                    <span class="m-menu__link-text">
                                        Email
                                    </span>
                                </span>
                            </li>
                            <router-link tag="li" to="/email/smtp" class="m-menu__item " aria-haspopup="true">
                                <a class="m-menu__link ">
                                    <i class="m-menu__link-bullet m-menu__link-bullet--dot">
                                        <span></span>
                                    </i>
                                    <span class="m-menu__link-text">
                                        SMTP Servers
                                    </span>
                                </a>
                            </router-link>
                           
                            <router-link tag="li" to="/email/responders" class="m-menu__item " aria-haspopup="true">
                                <a class="m-menu__link ">
                                    <i class="m-menu__link-bullet m-menu__link-bullet--dot">
                                        <span></span>
                                    </i>
                                    <span class="m-menu__link-text">
                                        Templates
                                    </span>
                                </a>
                            </router-link>
                        </ul>
                    </div>
                </li>


            </ul>
        </div>
        <!-- END: Aside Menu -->
    </div>
</template>

<script>
    import { mapGetters } from "vuex"

    export default {
        computed: mapGetters({
            summary: "ticketSummary"
        }),

        methods: {
            update() {
                this.$store.dispatch("updateSummary");
                var self = this;

                setTimeout(function () { self.update(); }, 10000);
            },
        },

        created() {
            this.update();
        }
    }
</script>
