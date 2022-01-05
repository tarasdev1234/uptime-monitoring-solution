<template>
    <div class="m-grid__item m-grid__item--fluid m-wrapper">
        <!-- END: Subheader -->
        <div class="m-content">
            <div class="m-portlet m-portlet--mobile">
                <div class="m-portlet__head">
                    <div class="m-portlet__head-caption">
                        <div class="m-portlet__head-title">
                            <h3 class="m-portlet__head-text">
                                Users
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
                                        <i class="la la-user-plus"></i>
                                        <span>
                                            New User
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
                                        <span style="width: 150px;">Email</span>
                                    </th>
                                    <th class="m-datatable__cell">
                                        <span style="width: 150px;">Phone Number</span>
                                    </th>
                                    <th class="m-datatable__cell">
                                        <span style="width: 150px;">Name</span>
                                    </th>
                                    <th class="m-datatable__cell">
                                        <span style="width: 110px;">Actions</span>
                                    </th>
                                </tr>
                            </thead>
                            <tbody class="m-datatable__body">
                                <span v-if="!users" class="m-datatable--error">No records found</span>
                                <tr :class="'m-datatable__row ' + (index % 2 == 1 ? 'm-datatable__row--even' : '')" v-for="(user, index) in users" :key="index">
                                    <td class="m-datatable__cell"><span style="width: 150px;">{{ user.email }}</span></td>
                                    <td class="m-datatable__cell"><span style="width: 150px;">{{ user.phoneNumber }}</span></td>
                                    <td class="m-datatable__cell"><span style="width: 150px;">{{ user.userName }}</span></td>
                                    <td data-field="Actions" class="m-datatable__cell">
                                        <span style="overflow: visible; width: 110px;">
                                            <a @click="editUser(user.id)" href="#" class="m-portlet__nav-link btn m-btn m-btn--hover-accent m-btn--icon m-btn--icon-only m-btn--pill" title="Edit details">
                                                <i class="la la-edit"></i>
                                            </a>
                                            <a @click="deleteUser(user.id)" href="#" class="m-portlet__nav-link btn m-btn m-btn--hover-danger m-btn--icon m-btn--icon-only m-btn--pill" title="Delete">
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
                            Edit user
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
                                    User Info
                                </a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link" data-toggle="tab" href="#m_tabs_roles">
                                    Roles
                                </a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link" data-toggle="tab" href="#m_tabs_deps">
                                    Dpartments
                                </a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link" data-toggle="tab" href="#m_tabs_comps">
                                    Company
                                </a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link" data-toggle="tab" href="#m_tabs_perms">
                                    Permissions
                                </a>
                            </li>
                        </ul>
                        <div class="tab-content">
                            <div class="tab-pane active" id="m_tabs_info" role="tabpanel">
                                <form>
                                    <div class="form-group">
                                        <label for="recipient-name" class="form-control-label">
                                            Email:
                                        </label>
                                        <input required v-model="currentUser.email" type="text" class="form-control" id="recipient-name">
                                    </div>
                                    <div class="form-group">
                                        <label for="user_phoneNumber" class="form-control-label">
                                            Phone Number:
                                        </label>
                                        <input required v-model="currentUser.phoneNumber" type="text" class="form-control" id="user_phoneNumber">
                                    </div>
                                    <div class="form-group">
                                        <label for="user_sign" class="form-control-label">
                                            Signature:
                                        </label>
                                        <textarea v-model="currentUser.signature" class="form-control m-input" rows="3">{{ currentUser.signature }}</textarea>
                                    </div>
                                    <div class="form-group">
                                        <label for="message-text" class="form-control-label">
                                            Password:
                                        </label>
                                        <input required v-model="currentUser.password" type="password" class="form-control" id="pass" />
                                    </div>
                                    <div class="form-group">
                                        <label for="message-text" class="form-control-label">
                                            Confirm Password:
                                        </label>
                                        <input required v-model="currentUser.confirmPass" type="password" class="form-control" id="pass_confirm" />
                                    </div>
                                </form>
                            </div>
                            <div class="tab-pane" id="m_tabs_roles" role="tabpanel">
                                <form class="m-form">
                                    <div class="m-form__group form-group">
                                        <div v-for="(role, index) in roles" class="m-checkbox-list">
                                            <label class="m-checkbox">
                                                <input :value="role.name" v-model="currentUser.roles" type="checkbox">
                                                {{ role.name }}
                                                <span></span>
                                            </label>
                                        </div>
                                    </div>
                                </form>
                            </div>
                            <div class="tab-pane" id="m_tabs_deps" role="tabpanel">
                                <form class="m-form">
                                    <div class="m-form__group form-group">
                                        <div v-for="(dep, index) in departments" class="m-checkbox-list">
                                            <label class="m-checkbox">
                                                <input :value="dep.id" v-model="currentUser.departments" type="checkbox">
                                                {{ dep.name }}
                                                <span></span>
                                            </label>
                                        </div>
                                    </div>
                                </form>
                            </div>
                            <div class="tab-pane" id="m_tabs_comps" role="tabpanel">
                                <form class="m-form">
                                    <div class="form-group">
                                        <select v-model="currentUser.company.companyId" class="form-control m-input">
                                            <option v-for="cmpn in companies" v-bind:value="cmpn.id">
                                                {{ cmpn.name }}
                                            </option>
                                        </select>
                                    </div>
                                    <div class="form-group">
                                        <label class="m-checkbox">
                                            <input v-model="currentUser.company.isAdmin" type="checkbox">
                                            Is Admin
                                            <span></span>
                                        </label>
                                    </div>
                                    <div class="form-group">
                                        <label class="m-checkbox">
                                            <input v-model="currentUser.company.isOwner" type="checkbox">
                                            Is Owner
                                            <span></span>
                                        </label>
                                    </div>
                                </form>
                            </div>
                            <div class="tab-pane" id="m_tabs_perms" role="tabpanel">
                                <form class="m-form">
                                    <div v-for="(prmsn, index) in permissions" class="m-form__group form-group">
                                        <h3>{{ prmsn.type }}</h3> 
                                        <div class="col-9">
                                            <div class="m-checkbox-inline">
                                                <label v-for="(ptype, index) in prmsn.permissions" class="m-checkbox">
                                                    <input :value="ptype.name" v-model="currentUser.permissions" type="checkbox">
                                                    {{ ptype.description }}
                                                    <span></span>
                                                </label>
                                            </div>
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
                        <button @click="saveUser()" type="button" class="btn btn-primary">
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
                            New user
                        </h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">
                                &times;
                            </span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <form>
                            <div class="form-group">
                                <label for="edit_user_name" class="form-control-label">
                                    Email:
                                </label>
                                <input required v-model="currentUser.email" type="text" class="form-control" id="edit_user_name">
                            </div>
                            <div class="form-group">
                                <label for="edit_user_phoneNumber" class="form-control-label">
                                    Phone:
                                </label>
                                <input required v-model="currentUser.phoneNumber" type="text" class="form-control" id="edit_user_phoneNumber">
                            </div>
                            <div class="form-group">
                                <label for="edit_pass" class="form-control-label">
                                    Password:
                                </label>
                                <input required v-model="currentUser.password" type="password" class="form-control" id="edit_pass" />
                            </div>
                            <div class="form-group">
                                <label for="edit_pass_confirm" class="form-control-label">
                                    Confirm Password:
                                </label>
                                <input required v-model="currentUser.confirmPass" type="password" class="form-control" id="edit_pass_confirm" />
                            </div>
                        </form>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">
                            Cancel
                        </button>
                        <button @click="createUser()" type="button" class="btn btn-primary">
                            Create
                        </button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    import UserApi from "../../services/users";
    import RoleApi from "../../services/roles";
    import DprtmntsApi from "../../services/departments";
    import CompaniesApi from "../../services/companies";
    import PrmsnApi from "../../services/permissions";
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
                searchRole: "",
                users: [],
                total: 0,
                currentPage: 1,
                roles: [],
                permissions: [],
                departments: [],
                companies: [],
                newUser: {
                    email: "",
                    phoneNumber: "",
                    signature: "",
                    password: "",
                    confirmPass: "",
                    company: {}
                },
                currentUser: {
                    company: {}
                }
            }
        },

        methods: {
            setPageSize(size) {
                this.pageSize = size;
                alert(this.pageSize);
            },

            showModal(id) {
                this.currentUser = Object.assign({}, this.newUser);
                $("#" + id).modal();
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

            async editUser(id) {
                this.showModal('edit_modal');

                try {
                    let response = await this.$http.get(AppSettings.ApiUrl + `api/users/${id}`);
                    console.log(response.data);

                    let company = await this.$http.get(AppSettings.ApiUrl + `api/users/${id}/company`);

                    this.currentUser = {
                        ...response.data,
                        ...company.data
                    };

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

            async deleteUser(id) {
                try {
                    if (confirm("Are you sure you want to delete this user?")) {
                        let response = await this.$http.delete(AppSettings.ApiUrl + `api/users/${id}`);
                        console.log(response.data);

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

            async saveUser() {
                try {
                    let response = await this.$http.put(AppSettings.ApiUrl + "api/users", this.currentUser);
                    console.log(response.data);

                    await this.$http.put(AppSettings.ApiUrl + `api/users/${this.currentUser.id}/company`, this.currentUser);

                    this.notification("User updated.", "success");

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

            async createUser() {
                try {
                    var fd = new FormData();

                    fd.append("Email", this.currentUser.email);
                    fd.append("Password", this.currentUser.password);
                    fd.append("ConfirmPassword", this.currentUser.confirmPass);
                    fd.append("PhoneNumber", this.currentUser.phoneNumber);

                    let response = await this.$http.post(AppSettings.ApiUrl + "api/users", fd);
                    console.log(response.data);

                    this.notification("User created.", "success");

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

                await UserApi.getAll(
                    {
                        pageSize: this.pageSize,
                        pageIndex: this.currentPage,
                        s: this.searchQuery
                    },
                    (data) => {
                        this.users = data.data;
                        this.total = data.count;
                    },
                    (err) => notification(err, "danger")
                );

                this.unblock();
            },

            async loadDeps() {
                await RoleApi.getAll(
                    {},
                    (data) => {
                        this.roles = data.data;
                    },
                    (err) => notification(err, "danger")
                );

                await DprtmntsApi.getAll(
                    {},
                    (data) => {
                        this.departments = data.data;
                    },
                    (err) => notification(err, "danger")
                );

                await CompaniesApi.getAll(
                    {},
                    (data) => {
                        this.companies = data.data;
                    },
                    (err) => notification(err, "danger")
                );

                await PrmsnApi.getAll(
                    {},
                    (data) => {
                        this.permissions = data;
                    },
                    (err) => notification(err, "danger")
                );
            },
        },

        mounted: function () {
            this.loadDeps();
            this.loadPage(1);
        },
    }
</script>

<style>
    .nav-item a.active, .nav-item a.active:hover, .nav-item a.active:focus {
        background-color: #fff;
        color: #495057;
        text-decoration: none;
    }
    .loading-data {
        position: relative;
        zoom: 1;
    }
</style>
