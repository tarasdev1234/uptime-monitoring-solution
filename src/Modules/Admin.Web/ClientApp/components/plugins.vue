<template>
    <div class="m-grid__item m-grid__item--fluid m-wrapper">
        <div class="m-content">
            <div class="m-portlet m-portlet--mobile">
                <div class="m-portlet__head">
                    <div class="m-portlet__head-caption">
                        <div class="m-portlet__head-title">
                            <h3 class="m-portlet__head-text">
                                Plugins
                            </h3>
                        </div>
                    </div>
                </div>
                <div class="m-portlet__body">
                    <!--begin: Search Form -->
                    <div class="m-form m-form--label-align-right m--margin-top-20 m--margin-bottom-30">
                        <div class="row align-items-center">
                            <div class="col-xl-8 order-2 order-xl-1">
                                <div class="form-group m-form__group row align-items-center">
                                    <div class="col-md-4">
                                        <div class="m-input-icon m-input-icon--left">
                                            <input @change="search" v-model="searchQuery" type="text" class="form-control m-input m-input--solid" placeholder="Search..." id="generalSearch">
                                            <span class="m-input-icon__icon m-input-icon__icon--left">
                                                <span>
                                                    <i class="la la-search"></i>
                                                </span>
                                            </span>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <!--end: Search Form -->
                    <!--begin: Datatable -->
                    <div class="m_datatable m-datatable m-datatable--brand m-datatable--default m-datatable--loaded">
                        <table class="m-datatable__table" id="datatable" style="display: block; min-height: 300px;">
                            <thead class="m-datatable__head">
                                <tr class="m-datatable__row">
                                    <th class="m-datatable__cell">
                                        <span style="width: 150px;">Name</span>
                                    </th>
                                    <th class="m-datatable__cell">
                                        <span style="width: 150px;">Author</span>
                                    </th>
                                    <th class="m-datatable__cell">
                                        <span style="width: 150px;">Website</span>
                                    </th>
                                    <th class="m-datatable__cell">
                                        <span style="width: 150px;">Description</span>
                                    </th>
                                </tr>
                            </thead>
                            <tbody class="m-datatable__body">
                                <span v-if="plugins.length == 0" class="m-datatable--error">No records found</span>
                                <tr :class="'m-datatable__row ' + (index % 2 == 1 ? 'm-datatable__row--even' : '')" v-for="(plg, index) in plugins" :key="index">
                                    <td class="m-datatable__cell"><span style="width: 150px;">{{ plg.name }}</span></td>
                                    <td class="m-datatable__cell"><span style="width: 150px;">{{ plg.pluginInfo.author }}</span></td>
                                    <td class="m-datatable__cell"><span style="width: 150px;">{{ plg.pluginInfo.website }}</span></td>
                                    <td class="m-datatable__cell"><span style="width: 150px;">{{ plg.pluginInfo.description }}</span></td>
                                </tr>
                            </tbody>
                        </table>

                        <div class="m-datatable__pager clearfix">
                            <ul class="m-datatable__pager-nav">
                                <li>
                                    <a href="#" title="Previous" @click="loadPage(currentPage - 1)" :class="'m-datatable__pager-link m-datatable__pager-link--prev' + (currentPage == 1 ? ' m-datatable__pager-link--disabled' : '')" :disabled="currentPage == 1">
                                        <i class="la la-angle-left"></i>
                                    </a>
                                </li>

                                <li v-for="(n, index) in totalPages" :key="index">
                                    <a :class="'m-datatable__pager-link m-datatable__pager-link-number' + (n == currentPage ? ' m-datatable__pager-link--active' : '')" href="#" @click="loadPage(n)">{{n}}</a>
                                </li>

                                <li>
                                    <a title="Next" @click="loadPage(currentPage + 1)" :class="'m-datatable__pager-link m-datatable__pager-link--next' + (currentPage < totalPages ? '' : ' m-datatable__pager-link--disabled')" :disabled="currentPage < totalPages">
                                        <i class="la la-angle-right"></i>
                                    </a>
                                </li>
                            </ul>
                            <div class="m-datatable__pager-info">
                                <div class="btn-group bootstrap-select m-datatable__pager-size" style="width: 70px;">
                                    <select-picker :value="pageSize"></select-picker>
                                </div>
                                <span class="m-datatable__pager-detail">Items per page</span>
                            </div>
                        </div>
                    </div>
                    <!--end: Datatable -->
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    import PluginApi from "../Services/plugins";
    import { mapGetters } from "vuex";

    export default {
        computed: {
            totalPages: function () {
                return Math.max(Math.ceil(this.total / this.pageSize), 1);
            },
            ...mapGetters({
                pageSize: "itemsPerPage",
            })
        },

        watch: {
            pageSize: function (value) {
                this.currentPage = 1;
                this.loadPage(this.currentPage);
            }
        },

        data() {
            return {
                searchQuery: "",
                total: 0,
                currentPage: 1,
                plugins: [],
            }
        },

        methods: {
            showModal(id) {
                $("#" + id).modal();
            },

            search() {
                this.currentPage = 1;
                this.loadPage(this.currentPage);
            },

            notification(message, type) {
                let title = type == "success" ? "" : "Error";

                $.notify({
                    title: title,
                    message: message
                }, {
                    type: type,
                    z_index: 20000
                });
            },

            async loadPage(page) {
                if (page < 1 || page > this.totalPages) {
                    return;
                }

                mApp.block("#datatable", {
                    message: "Please wait",
                });

                this.currentPage = page;

                await PluginApi.getAll(
                    {
                        pageSize: this.pageSize,
                        pageIndex: this.currentPage,
                        s: this.searchQuery
                    },
                    (data) => {
                        this.plugins = data.data;
                        this.total = data.count;
                    },
                    (err) => notification(err, "danger")
                );

                mApp.unblock("#datatable");
            },

            async loadDeps() {
            },
        },

        mounted: function () {
            this.loadPage(1);
            this.loadDeps();
        },
    }
</script>
