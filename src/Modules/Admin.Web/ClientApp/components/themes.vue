<template>
    <div class="m-grid__item m-grid__item--fluid m-wrapper">
        <div class="m-content">
            <div class="m-portlet m-portlet--mobile">
                <div class="m-portlet__head">
                    <div class="m-portlet__head-caption">
                        <div class="m-portlet__head-title">
                            <h3 class="m-portlet__head-text">
                                Themes
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
                            <div class="col-xl-4 order-1 order-xl-2 m--align-right">
                                <a href="#" @click="showModal('create_modal')" class="btn btn-focus m-btn m-btn--custom m-btn--icon m-btn--air m-btn--pill">
                                    <span>
                                        <i class="la la-plus-circle"></i>
                                        <span>
                                            New
                                        </span>
                                    </span>
                                </a>
                                <div class="m-separator m-separator--dashed d-xl-none"></div>
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
                                        <span style="width: 70px;">Actions</span>
                                    </th>
                                </tr>
                            </thead>
                            <tbody class="m-datatable__body">
                                <span v-if="themes.length == 0" class="m-datatable--error">No records found</span>
                                <tr :class="'m-datatable__row ' + (index % 2 == 1 ? 'm-datatable__row--even' : '')" v-for="(theme, index) in themes" :key="index">
                                    <td class="m-datatable__cell"><span style="width: 150px;">{{ theme.name }}</span></td>
                                    <td data-field="Actions" class="m-datatable__cell">
                                        <span style="overflow: visible; width: 70px;">
                                            <a @click="editTheme(theme.id)" href="#" class="m-portlet__nav-link btn m-btn m-btn--hover-accent m-btn--icon m-btn--icon-only m-btn--pill" title="Edit details">
                                                <i class="la la-edit"></i>
                                            </a>
                                            <a @click="deleteTheme(theme.id)" href="#" class="m-portlet__nav-link btn m-btn m-btn--hover-danger m-btn--icon m-btn--icon-only m-btn--pill" title="Delete">
                                                <i class="la la-trash"></i>
                                            </a>
                                        </span>
                                    </td>
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

        <div class="modal fade" id="create_modal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-lg" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="exampleModalLabel">
                            Add Theme
                        </h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">
                                &times;
                            </span>
                        </button>
                    </div>
                    <form @submit.prevent="createTheme()" class="m-form m-form--fit m-form--label-align-right" style="">
                        <div class="modal-body" id="create_frm">
                            <div class="form-group m-form__group row">
                                <label class="col-3 col-form-label">
                                    Name
                                </label>
                                <div class="col-9">
                                    <input v-model="currentTheme.name" class="form-control m-input" type="text">
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-dismiss="modal">
                                Cancel
                            </button>
                            <button type="submit" class="btn btn-primary">
                                Create
                            </button>
                        </div>
                    </form>
                </div>
            </div>
        </div>

        <div class="modal fade" id="edit_modal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-lg" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="exampleModalLabel">
                            Edit Theme
                        </h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">
                                &times;
                            </span>
                        </button>
                    </div>
                    <form @submit.prevent="saveTheme()" class="m-form m-form--fit m-form--label-align-right" style="">
                        <div class="modal-body" id="create_frm">
                            <div class="form-group m-form__group row">
                                <label class="col-3 col-form-label">
                                    Name
                                </label>
                                <div class="col-9">
                                    <input v-model="currentTheme.name" class="form-control m-input" type="text">
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-dismiss="modal">
                                Cancel
                            </button>
                            <button type="submit" class="btn btn-primary">
                                Save
                            </button>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    import ThemeApi from "../Services/themes";
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
                themes: [],
                newTheme: {},
                currentTheme: {},
            }
        },

        methods: {
            showModal(id) {
                this.currentTheme = {};
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

            async createTheme() {
                mApp.block("#create_frm", {
                    message: "Please wait",
                });

                await ThemeApi.createTheme(
                    this.currentTheme,
                    (data) => {
                        this.notification("Item created", "success");
                        $("#create_modal").modal("toggle");
                        this.loadPage(this.currentPage);
                    },
                    (err) => this.notification(err, "danger")
                );

                mApp.unblock("#create_frm");
            },

            async editTheme(id) {
                showModal("edit_modal");

                await ThemeApi.getTheme(
                    id,
                    (data) => {
                        this.currentTheme = data;
                    },
                    (err) => this.notification(err, "danger")
                );
            },

            async saveTheme() {
                await ThemeApi.saveTheme(
                    this.currentTheme,
                    (data) => {
                        this.notification("Item updated", "success");
                        $("#edit_modal").modal("toggle");
                        this.loadPage(this.currentPage);
                    },
                    (err) => this.notification(err, "danger")
                );
            },

            async deleteTheme(id) {
                if (confirm("Are you sure you want to delete this item?")) {
                    await ThemeApi.deleteTheme(
                        id,
                        (data) => {
                            this.notification("Item delete", "success");
                            this.loadPage(this.currentPage);
                        },
                        (err) => this.notification(err, "danger")
                    );
                }
            },

            async loadPage(page) {
                if (page < 1 || page > this.totalPages) {
                    return;
                }

                mApp.block("#datatable", {
                    message: "Please wait",
                });

                this.currentPage = page;

                await ThemeApi.getThemes(
                    {
                        pageSize: this.pageSize,
                        pageIndex: this.currentPage,
                        s: this.searchQuery
                    },
                    (data) => {
                        this.themes = data.data;
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
