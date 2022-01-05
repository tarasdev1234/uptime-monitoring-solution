<template>
    <div class="m-grid__item m-grid__item--fluid m-wrapper">
        <div class="m-content">
            <div class="m-portlet m-portlet--mobile">
                <div class="m-portlet__head">
                    <div class="m-portlet__head-caption">
                        <div class="m-portlet__head-title">
                            <h3 class="m-portlet__head-text">
                                Autoresponders
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
                                            New Responder
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
                                        <span style="width: 300px;">Department</span>
                                    </th>
                                    <th class="m-datatable__cell m-datatable__cell--center">
                                        <span style="width: 100px;">Enabled</span>
                                    </th>
                                    <th class="m-datatable__cell">
                                        <span style="width: 110px;">Actions</span>
                                    </th>
                                </tr>
                            </thead>
                            <tbody class="m-datatable__body">
                                <span v-if="!deps" class="m-datatable--error">No records found</span>
                                <tr :class="'m-datatable__row ' + (index % 2 == 1 ? 'm-datatable__row--even' : '')" v-for="(dep, index) in deps" :key="index">
                                    <td class="m-datatable__cell"><span style="width: 300px;">{{ dep.name }}</span></td>
                                    <td class="m-datatable__cell--center m-datatable__cell m-datatable__cell--check">
                                        <span style="width: 100px;">
                                            <label class="m-checkbox m-checkbox--single m-checkbox--solid m-checkbox--brand">
                                                <input type="checkbox" onclick="return false;" :checked="dep.responderEnabled"><span></span>
                                            </label>
                                        </span>
                                    </td>
                                    <td data-field="Actions" class="m-datatable__cell">
                                        <span style="overflow: visible; width: 110px;">
                                            <a @click="editResponder(dep.id)" href="#" class="m-portlet__nav-link btn m-btn m-btn--hover-accent m-btn--icon m-btn--icon-only m-btn--pill" title="Edit details">
                                                <i class="la la-edit"></i>
                                            </a>
                                            <a @click="deleteResponder(dep.id)" href="#" class="m-portlet__nav-link btn m-btn m-btn--hover-danger m-btn--icon m-btn--icon-only m-btn--pill" title="Delete">
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
                            Edit Responder
                        </h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">
                                &times;
                            </span>
                        </button>
                    </div>
                    <form @submit.prevent="saveResponder()" id="create_frm">
                        <div class="modal-body">
                            <div class="form-group m-form__group row">
                                <div class="col-lg-2 col-sm-12"></div>
                                <div class="col-lg-9 col-md-9 col-sm-12">
                                    <label class="m-checkbox m-checkbox--single m-checkbox--solid m-checkbox--brand">
                                        <input type="checkbox" v-model="currentResponder.isEnabled"><span></span> Enabled
                                    </label>
                                </div>
                            </div>
                            <div class="form-group m-form__group row m--margin-top-10">
                                <label class="col-form-label col-lg-2 col-sm-12">
                                    Template:
                                </label>
                                <div class="col-lg-9 col-md-9 col-sm-12">
                                    <textarea v-model="currentResponder.template" class="form-control" data-provide="markdown" rows="10">{{ currentResponder.template }}</textarea>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-dismiss="modal">
                                Cancel
                            </button>
                            <input type="submit" value="Save" class="btn btn-primary">
                        </div>
                    </form>
                </div>
            </div>
        </div>

        <div class="modal fade" id="create_modal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-lg" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="exampleModalLabel">
                            New Responder
                        </h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">
                                &times;
                            </span>
                        </button>
                    </div>
                    <form @submit.prevent="createResponder()" id="create_frm">
                        <div class="modal-body">
                            <div class="form-group m-form__group row">
                                <div class="col-lg-2 col-sm-12"></div>
                                <div class="col-lg-9 col-md-9 col-sm-12">
                                    <label class="m-checkbox m-checkbox--single m-checkbox--solid m-checkbox--brand">
                                        <input type="checkbox" v-model="currentResponder.isEnabled"><span></span> Enabled
                                    </label>
                                </div>
                            </div>
                            <div class="form-group m-form__group row m--margin-top-10">
                                <label class="col-form-label col-lg-2 col-sm-12">
                                    Template:
                                </label>
                                <div class="col-lg-9 col-md-9 col-sm-12">
                                    <textarea v-model="currentResponder.template" class="form-control" data-provide="markdown" rows="10">{{ currentResponder.template }}</textarea>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-dismiss="modal">
                                Cancel
                            </button>
                            <input type="submit" value="Save" class="btn btn-primary">
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    //import autoresponder from "../../Services/autoresponder";
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
                smtpServers: [],
                deps: [],
                total: 0,
                currentPage: 1,
                NewDepartment: {
                    id: ""
                },
                currentDepartment: {
                    id: ""
                },
                NewResponder: {
                    id: null,
                    template: null,
                    isEnabled: ""
                },
                currentResponder: {
                    id: null,
                    template: null,
                    isEnabled: ""
                },
            }
        },

        methods: {
            setPageSize(size) {
                this.pageSize = size;
                alert(this.pageSize);
            },

            showModal(id) {
                $("#" + id).modal();
                clearForm();
                this.currentResponder = Object.assign({}, this.NewResponder);
                $("#" + id).modal();
            },

            search() {
                this.currentPage = 1;
                this.loadPage(this.currentPage);
            },

            block() {
                mApp.block("#datatable", {
                    message: "Please wait",
                });
            },

            unblock() {
                mApp.unblock("#datatable");
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

            async editResponder(id) {
                this.showModal('edit_modal');

                try {
                    let response = await this.$http.get(AppSettings.ApiUrl + `api/departments/${id}/responder`);
                    console.log(response.data);

                    this.currentDepartment.id = id;
                    this.currentResponder = response.data;
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

            async saveResponder() {
                try {
                    let response = await this.$http.put(AppSettings.ApiUrl + `api/departments/${this.currentDepartment.id}/responder`, this.currentResponder);

                    this.notification("Responder updated.", "success");

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

            async loadPage(page) {
                if (page < 1 || page > this.totalPages) {
                    return;
                }

                this.block();

                this.currentPage = page;

                try {
                    let response = await this.$http.get(AppSettings.ApiUrl + `api/departments?pageSize=${this.pageSize}&pageIndex=${this.currentPage}&s=${this.searchQuery}`)
                    this.deps = response.data.data;
                    this.total = response.data.count;
                } catch (err) {
                    alert(err);
                    console.log(err)
                }

                this.unblock();
            },
        },

        mounted: function () {
            $("textarea").markdown();

            this.loadPage(1);
        },
    }
</script>
