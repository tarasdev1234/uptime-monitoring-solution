<template>
    <div class="m-grid__item m-grid__item--fluid m-wrapper">
        <!-- END: Subheader -->
        <div class="m-content">
            <div class="m-portlet m-portlet--mobile">
                <div class="m-portlet__head">
                    <div class="m-portlet__head-caption">
                        <div class="m-portlet__head-title">
                            <h3 class="m-portlet__head-text">
                                Brands
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
                                            New Brand
                                        </span>
                                    </span>
                                </a>
                                <div class="m-separator m-separator--dashed d-xl-none"></div>
                            </div>
                        </div>
                    </div>
                    <!--end: Search Form -->
                    <!--begin: Datatable -->
                    <div class="m_datatable m-datatable m-datatable--brand m-datatable--default m-datatable--loaded"">
                        <table class="m-datatable__table" id="datatable" style="display: block; min-height: 300px;">
                            <thead class="m-datatable__head">
                                <tr class="m-datatable__row">
                                    <th class="m-datatable__cell">
                                        <span style="width: 300px;">Name</span>
                                    </th>
                                    <th class="m-datatable__cell">
                                        <span style="width: 300px;">Url</span>
                                    </th>
                                    <th class="m-datatable__cell">
                                        <span style="width: 110px;">Actions</span>
                                    </th>
                                </tr>
                            </thead>
                            <tbody class="m-datatable__body">
                                <span v-if="!brands" class="m-datatable--error">No records found</span>
                                <tr :class="'m-datatable__row ' + (index % 2 == 1 ? 'm-datatable__row--even' : '')" v-for="(brand, index) in brands" :key="index">
                                    <td class="m-datatable__cell"><span style="width: 300px;">{{ brand.name }}</span></td>
                                    <td class="m-datatable__cell"><span style="width: 300px;">{{ brand.url }}</span></td>
                                    <td data-field="Actions" class="m-datatable__cell">
                                        <span style="overflow: visible; width: 110px;">
                                            <a @click="editBrand(brand.id)" href="#" class="m-portlet__nav-link btn m-btn m-btn--hover-accent m-btn--icon m-btn--icon-only m-btn--pill" title="Edit details">
                                                <i class="la la-edit"></i>
                                            </a>
                                            <a @click="deleteBrand(brand.id)" href="#" class="m-portlet__nav-link btn m-btn m-btn--hover-danger m-btn--icon m-btn--icon-only m-btn--pill" title="Delete">
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

        <div class="modal fade" id="edit_modal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-lg" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="exampleModalLabel">
                            Edit brand
                        </h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">
                                &times;
                            </span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <ul class="nav nav-tabs nav-fill" role="tablist">
                            <li class="nav-item">
                                <a class="nav-link active show" data-toggle="tab" href="#m_tabs_info">
                                    Brand Info
                                </a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link" data-toggle="tab" href="#m_tabs_plugins">
                                    Addons
                                </a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link" data-toggle="tab" href="#m_tabs_theme">
                                    Theme
                                </a>
                            </li>
                        </ul>
                        <div class="tab-content">
                            <div class="tab-pane active" id="m_tabs_info" role="tabpanel">
                                <form>
                                    <div class="form-group">
                                        <label class="form-control-label">
                                            Name:
                                        </label>
                                        <input required v-model="currentBrand.name" type="text" class="form-control">
                                    </div>
                                    <div class="form-group">
                                        <label class="form-control-label">
                                            Url:
                                        </label>
                                        <input required v-model="currentBrand.url" type="text" class="form-control">
                                    </div>
                                    <div class="form-group m-form__group">
                                        <label>
                                            SMTP Server
                                        </label>
                                        <select v-model="currentBrand.smtpServerId" class="form-control m-input">
                                            <option :value="null">
                                                Use Default
                                            </option>
                                            <option v-for="srv in servers" v-bind:value="srv.id">
                                                {{ srv.name }}
                                            </option>
                                        </select>
                                    </div>
                                </form>
                            </div>
                            <div class="tab-pane" id="m_tabs_plugins" role="tabpanel">
                                <form class="m-form">
                                    <div class="m-form__group form-group">
                                        <span v-if="plugins.length == 0" class="m-datatable--error">No custom plugins found</span>
                                        <div v-for="(plg, index) in plugins" class="m-checkbox-list">
                                            <label class="m-checkbox">
                                                <input :value="plg.name" v-model="currentBrand.plugins" type="checkbox">
                                                {{ plg.name }}
                                                <span></span>
                                            </label>
                                        </div>
                                    </div>
                                </form>
                            </div>
                            <div class="tab-pane" id="m_tabs_theme" role="tabpanel">
                                <form class="m-form">
                                    <div class="m-form__group form-group">
                                        <span v-if="themes.length == 0" class="m-datatable--error">No themes found</span>
                                        <div v-for="(theme, index) in themes" class="m-checkbox-list">
                                            <label class="m-checkbox">
                                                <input :value="theme.name" :id="theme.id" v-model="currentBrand.theme" type="radio">
                                                {{ theme.name }}
                                                <span></span>
                                            </label>
                                        </div>
                                    </div>
                                </form>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">
                            Cancel
                        </button>
                        <button @click="saveBrand()" type="button" class="btn btn-primary">
                            Save
                        </button>
                    </div>
                </div>
            </div>
        </div>

        <div class="modal fade" id="create_modal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-lg" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="exampleModalLabel">
                            New brand
                        </h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">
                                &times;
                            </span>
                        </button>
                    </div>
                    <form @submit.prevent="createBrand()" id="create_frm">
                        <div class="modal-body">
                            <div class="form-group">
                                <label class="form-control-label">
                                    Name:
                                </label>
                                <input required v-model="newBrand.name" type="text" class="form-control">
                            </div>
                            <div class="form-group">
                                <label class="form-control-label">
                                    Url:
                                </label>
                                <input required v-model="newBrand.url" type="text" class="form-control" />
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-dismiss="modal">
                                Cancel
                            </button>
                            <input type="submit" class="btn btn-primary" value="Create"/>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    import { mapGetters } from "vuex"
    import BrandApi from "../Services/brands"
    import PluginApi from "../Services/plugins"
    import ThemeApi from "../Services/themes"

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
                searchRole: "",
                brands: [],
                servers: [],
                plugins: [],
                themes: [],
                total: 0,
                currentPage: 1,
                newBrand: {
                    name: "",
                    url: ""
                },
                currentBrand: {
                    id: "",
                    name: "",
                    url: "",
                    smtpServerId: "",
                    plugins: []
                }
            }
        },

        methods: {
            setPageSize(size) {
                this.pageSize = size;
                alert(this.pageSize);
            },

            showModal(id) {
                $("#" + id).modal();
            },

            block() {
                mApp.block("#datatable", {
                    message: "Please wait",
                });
            },

            unblock() {
                mApp.unblock("#datatable");
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

            async editBrand(id) {
                this.showModal('edit_modal');

                try {
                    let response = await this.$http.get(AppSettings.ApiUrl + `api/brand/${id}`);

                    this.currentBrand = response.data;
                } catch (err) {
                    var errMsg = "";
                    for (var key in err.response.data) {
                        if (err.response.data.hasOwnProperty(key)) {
                            errMsg = err.response.data[key];
                            break;
                        }
                    }

                    this.notification(errMsg, "danger");
                    console.log(err)
                }
            },

            async deleteBrand(id) {
                try {
                    if (confirm("Are you sure you want to delete this brand?")) {
                        let response = await this.$http.delete(AppSettings.ApiUrl + `api/brand/${id}`);
                        console.log(response.data);

                        this.notification("Brand deleted.", "success");
                        this.loadPage(this.currentPage);
                    }
                } catch (err) {
                    var errMsg = "";
                    for (var key in err.response.data) {
                        if (err.response.data.hasOwnProperty(key)) {
                            errMsg = err.response.data[key];
                            break;
                        }
                    }

                    this.notification(errMsg, "danger");
                    console.log(err)
                }
            },

            async saveBrand() {
                try {
                    let response = await this.$http.put(AppSettings.ApiUrl + `api/brand`, this.currentBrand);

                    this.notification("Brand updated.", "success");

                    this.loadPage(this.currentPage);

                    $("#edit_modal").modal("toggle");
                } catch (err) {
                    var errMsg = "";

                    for (var key in err.response.data) {
                        if (err.response.data.hasOwnProperty(key)) {
                            errMsg = err.response.data[key];
                            break;
                        }
                    }

                    this.notification(errMsg, "danger");
                    console.log(err)
                }
            },

            async createBrand() {
                try {
                    var fd = new FormData();

                    fd.append("name", this.newBrand.name);
                    fd.append("url", this.newBrand.url);

                    let response = await this.$http.post(AppSettings.ApiUrl + `api/brand`, fd);
                    console.log(response.data);

                    this.notification("Brand created.", "success");

                    this.loadPage(this.currentPage);

                    $("#create_modal").modal("toggle");
                } catch (err) {
                    var errMsg = "";
                    for (var key in err.response.data) {
                        if (err.response.data.hasOwnProperty(key)) {
                            errMsg = err.response.data[key];
                            break;
                        }
                    }

                    this.notification(errMsg, "danger");
                    console.log(err)
                }
            },

            async loadDeps() {
                try {
                    let response = await this.$http.get(AppSettings.ApiUrl + "api/settings/email/smtp")
                    this.servers = response.data.data;
                } catch (err) {
                    alert(err);
                    console.log(err)
                }

                await PluginApi.getAll(
                    {},
                    (data) => {
                        this.plugins = data.data;
                    },
                    (err) => notification(err, "danger")
                );

                await ThemeApi.getThemes(
                    {},
                    (data) => {
                        this.themes = data.data;
                    },
                    (err) => notification(err, "danger")
                );
            },

            async loadPage(page) {
                if (page < 1 || page > this.totalPages) {
                    return;
                }

                this.block();

                this.currentPage = page;

                await BrandApi.getBrands(
                    {
                        pageSize: this.pageSize,
                        pageIndex: this.currentPage,
                        s: this.searchQuery
                    },
                    (data) => {
                        this.brands = data.data;
                        this.total = data.count;
                    },
                    (err) => this.notification(err, "danger")
                );

                this.unblock();
            },
        },

        mounted: function () {
            this.loadDeps();
            this.loadPage(1);
        },
    }
</script>
