<template>
    <div class="m-grid__item m-grid__item--fluid m-wrapper">
        <div class="m-content">
            <div class="m-portlet m-portlet--mobile">
                <div class="m-portlet__head">
                    <div class="m-portlet__head-caption">
                        <div class="m-portlet__head-title">
                            <h3 class="m-portlet__head-text">
                                Departments
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
                                    <div class="col-md-3">
                                        <div class="form-group m-form__group">
                                            <select v-model="selectedBrand" @change="search" class="form-control m-input">
                                                <option value="">
                                                    All Brands
                                                </option>
                                                <option v-for="brnd in brands" v-bind:value="brnd.id">
                                                    {{ brnd.name }}
                                                </option>
                                            </select>
                                        </div>
                                    </div>

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
                                            New Department
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
                                        <span style="width: 150px;">Brand</span>
                                    </th>
                                    <th class="m-datatable__cell">
                                        <span style="width: 150px;">Name</span>
                                    </th>
                                    <th class="m-datatable__cell">
                                        <span style="width: 150px;">Users</span>
                                    </th>
                                    <th class="m-datatable__cell">
                                        <span style="width: 150px;">Tickets</span>
                                    </th>
                                    <th class="m-datatable__cell">
                                        <span style="width: 110px;">Actions</span>
                                    </th>
                                </tr>
                            </thead>
                            <tbody class="m-datatable__body">
                                <span v-if="!deps" class="m-datatable--error">No records found</span>
                                <tr :class="'m-datatable__row ' + (index % 2 == 1 ? 'm-datatable__row--even' : '')" v-for="(dep, index) in deps" :key="index">
                                    <td class="m-datatable__cell"><span style="width: 150px;">{{ dep.brandName }}</span></td>
                                    <td class="m-datatable__cell"><span style="width: 150px;">{{ dep.name }}</span></td>
                                    <td class="m-datatable__cell"><span style="width: 150px;">{{ dep.usersCount }}</span></td>
                                    <td class="m-datatable__cell"><span style="width: 150px;">{{ dep.ticketsCount }}</span></td>
                                    <td data-field="Actions" class="m-datatable__cell">
                                        <span style="overflow: visible; width: 110px;">
                                            <a @click="editDep(dep.id)" href="#" class="m-portlet__nav-link btn m-btn m-btn--hover-accent m-btn--icon m-btn--icon-only m-btn--pill" title="Edit details">
                                                <i class="la la-edit"></i>
                                            </a>
                                            <a @click="deleteDep(dep.id)" href="#" class="m-portlet__nav-link btn m-btn m-btn--hover-danger m-btn--icon m-btn--icon-only m-btn--pill" title="Delete">
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
                            Edit Department
                        </h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">
                                &times;
                            </span>
                        </button>
                    </div>

                    <form @submit.prevent="saveDep()" id="create_frm">
                        <div class="modal-body">
                            <div class="form-group m-form__group">
                                <label>
                                    Brand
                                </label>
                                <select v-model="currentDepartment.brandId" class="form-control m-input">
                                    <option v-for="brnd in brands" v-bind:value="brnd.id">
                                        {{ brnd.name }}
                                    </option>
                                </select>
                            </div>
                            <div class="form-group">
                                <label class="form-control-label">
                                    Name:
                                </label>
                                <input required v-model="currentDepartment.name" type="text" class="form-control">
                            </div>
                            <div class="form-group m-form__group">
                                <label>
                                    SMTP Server
                                </label>
                                <select v-model="currentDepartment.smtpServerId" class="form-control m-input">
                                    <option :value="null">
                                        Use Brand's
                                    </option>
                                    <option v-for="srv in smtpServers" v-bind:value="srv.id">
                                        {{ srv.serverName }}
                                    </option>
                                </select>
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
                            New Department
                        </h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">
                                &times;
                            </span>
                        </button>
                    </div>

                    <form @submit.prevent="createDepartment()" id="create_frm">
                        <div class="modal-body">
                            <div class="form-group m-form__group">
                                <label>
                                    Brand
                                </label>
                                <select required v-model="currentDepartment.brandId" class="form-control m-input">
                                    <option v-for="brnd in brands" v-bind:value="brnd.id">
                                        {{ brnd.name }}
                                    </option>
                                </select>
                            </div>
                            <div class="form-group">
                                <label class="form-control-label">
                                    Name:
                                </label>
                                <input required v-model="currentDepartment.name" type="text" class="form-control">
                            </div>
                            <div class="form-group m-form__group">
                                <label>
                                    SMTP Server
                                </label>
                                <select v-model="currentDepartment.smtpServerId" class="form-control m-input">
                                    <option :value="null">
                                        Use Brand's
                                    </option>
                                    <option v-for="srv in smtpServers" v-bind:value="srv.id">
                                        {{ srv.serverName }}
                                    </option>
                                </select>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-dismiss="modal">
                                Cancel
                            </button>
                            <input type="submit" value="Create" class="btn btn-primary">
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    import DepartmentApi from "../services/departments";
    import { mapGetters } from "vuex"

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
                selectedBrand: "",
                newDepBrndId: "",
                brands: [],
                smtpServers: [],
                deps: [],
                total: 0,
                currentPage: 1,
                newDepartment: {
                    brandId: null,
                    smtpServerId: null,
                    name: ""
                },
                currentDepartment: {
                    brandId: null,
                    smtpServerId: null,
                    name: ""
                },
            }
        },

        methods: {
            setPageSize(size) {
                this.pageSize = size;
                alert(this.pageSize);
            },

            showModal(id) {
                this.currentDepartment = Object.assign({}, this.newDepartment);
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

            async editDep(id) {
                this.showModal("edit_modal");
                
                await DepartmentApi.get(
                    id,
                    (data) => { this.currentDepartment = data },
                    (err) => notification(err, "danger")
                );
            },

            async deleteDep(id) {
                try {
                    if (confirm("Are you sure you want to delete this department?")) {
                        let response = await this.$http.delete(AppSettings.ApiUrl + `api/departments/${id}`);

                        this.notification("Department deleted.", "success");
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

            async saveDep() {
                try {
                    let response = await this.$http.put(AppSettings.ApiUrl + "api/departments", this.currentDepartment);

                    this.notification("Department updated.", "success");

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

            async createDepartment() {
                try {
                    var fd = new FormData();

                    fd.append("name", this.currentDepartment.name);
                    fd.append("smtpServerId", this.currentDepartment.smtpServerId);
                    fd.append("brandId", this.currentDepartment.brandId);

                    let response = await this.$http.post(AppSettings.ApiUrl + "api/departments", fd);

                    this.notification("Department created.", "success");

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

            async loadPage(page) {
                if (page < 1 || page > this.totalPages) {
                    return;
                }

                this.block();

                this.currentPage = page;
                
                await DepartmentApi.getAll(
                    {
                        pageSize: this.pageSize,
                        pageIndex: this.currentPage,
                        s: this.searchQuery,
                        brandId: this.selectedBrand
                    },
                    (data) => {
                        this.deps = data.data;
                        this.total = data.count;
                    },
                    (err) => notification(err, "danger")
                );

                this.unblock();
            },

            async loadBrands() {
                try {
                    let response = await this.$http.get(AppSettings.ApiUrl + "api/brand")
                    this.brands = response.data.data;
                } catch (err) {
                    alert(err);
                    console.log(err)
                }
            },

            async loadSmtps() {
                try {
                    let response = await this.$http.get(AppSettings.ApiUrl + "api/settings/email/smtp")
                    this.smtpServers = response.data.data;
                } catch (err) {
                    alert(err);
                    console.log(err)
                }
            },
        },

        mounted: function () {
            this.loadBrands();
            this.loadSmtps();
            this.loadPage(1);
        },
    }
</script>
