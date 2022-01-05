<template>
    <div class="m-grid__item m-grid__item--fluid m-wrapper">
        <div class="m-content">
            <div class="m-portlet m-portlet--mobile">
                <div class="m-portlet__head">
                    <div class="m-portlet__head-caption">
                        <div class="m-portlet__head-title">
                            <h3 class="m-portlet__head-text">
                                POP Servers
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
                                            New Server
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
                                        <span style="width: 150px;">Server</span>
                                    </th>
                                    <th class="m-datatable__cell">
                                        <span style="width: 150px;">Email</span>
                                    </th>
                                    <th class="m-datatable__cell">
                                        <span style="width: 150px;">Department</span>
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
                                <span v-if="!servers" class="m-datatable--error">No records found</span>
                                <tr :class="'m-datatable__row ' + (index % 2 == 1 ? 'm-datatable__row--even' : '')" v-for="(srv, index) in servers" :key="index">
                                    <td class="m-datatable__cell"><span style="width: 150px;">{{ srv.serverName }}</span></td>
                                    <td class="m-datatable__cell"><span style="width: 150px;">{{ srv.emailAddress }}</span></td>
                                    <td class="m-datatable__cell"><span style="width: 150px;">{{ srv.departmentName }}</span></td>
                                    <td class="m-datatable__cell--center m-datatable__cell m-datatable__cell--check">
                                        <span style="width: 100px;">
                                            <label class="m-checkbox m-checkbox--single m-checkbox--solid m-checkbox--brand">
                                                <input type="checkbox" onclick="return false;" :checked="srv.enabled"><span></span>
                                            </label>
                                        </span>
                                    </td>
                                    <td data-field="Actions" class="m-datatable__cell">
                                        <span style="overflow: visible; width: 110px;">
                                            <a @click="editSrv(srv.id)" href="#" class="m-portlet__nav-link btn m-btn m-btn--hover-accent m-btn--icon m-btn--icon-only m-btn--pill" title="Edit details">
                                                <i class="la la-edit"></i>
                                            </a>
                                            <a @click="deleteSrv(srv.id)" href="#" class="m-portlet__nav-link btn m-btn m-btn--hover-danger m-btn--icon m-btn--icon-only m-btn--pill" title="Delete">
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
                            Edit Server
                        </h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">
                                &times;
                            </span>
                        </button>
                    </div>
                    <form @submit.prevent="saveSrv()">
                        <div class="modal-body">
                            <div class="form-group m-form__group">
                                <label>
                                    Department
                                </label>
                                <select required v-model="currentServer.departmentId" class="form-control m-input">
                                    <option v-for="dep in departments" v-bind:value="dep.id">
                                        {{ dep.name }}
                                    </option>
                                </select>
                            </div>
                            <div class="form-group">
                                <label class="form-control-label">
                                    Email Address:
                                </label>
                                <input required v-model="currentServer.emailAddress" type="text" class="form-control">
                            </div>
                            <div class="form-group">
                                <label class="form-control-label">
                                    Server Address:
                                </label>
                                <input required v-model="currentServer.serverName" type="text" class="form-control">
                            </div>
                            <div class="form-group">
                                <label class="form-control-label">
                                    Server Port:
                                </label>
                                <input required v-model="currentServer.serverPort" type="text" class="form-control">
                            </div>
                            <div class="form-group">
                                <label class="m-checkbox m-checkbox--single m-checkbox--solid m-checkbox--brand">
                                    <input type="checkbox" v-model="currentServer.enabled"><span></span> Enabled
                                </label>
                            </div>
                            <div class="form-group m-form__group">
                                <label>
                                    Encryption
                                </label>
                                <select v-model="currentServer.encryption" class="form-control m-input">
                                    <option v-for="type in encTypes" v-bind:value="type.value">
                                        {{ type.key }}
                                    </option>
                                </select>
                            </div>
                            <div class="form-group">
                                <label class="form-control-label">
                                    Login:
                                </label>
                                <input required v-model="currentServer.login" type="text" class="form-control">
                            </div>
                            <div class="form-group">
                                <label class="form-control-label">
                                    Password:
                                </label>
                                <input v-model="currentServer.password" type="password" class="form-control">
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

        <div class="modal fade" id="create_modal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-lg" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="exampleModalLabel">
                            New Server
                        </h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">
                                &times;
                            </span>
                        </button>
                    </div>
                    <form @submit.prevent="createSrv()">
                        <div class="modal-body">
                            <div class="form-group m-form__group">
                                <label>
                                    Department
                                </label>
                                <select required v-model="currentServer.departmentId" class="form-control m-input">
                                    <option v-for="dep in departments" v-bind:value="dep.id">
                                        {{ dep.name }}
                                    </option>
                                </select>
                            </div>
                            <div class="form-group" id="EmailAddress">
                                <label class="form-control-label">
                                    Email Address:
                                </label>
                                <input required v-model="currentServer.emailAddress" type="text" class="form-control">
                            </div>
                            <div class="form-group">
                                <label class="form-control-label">
                                    Server Address:
                                </label>
                                <input required v-model="currentServer.serverName" type="text" class="form-control">
                            </div>
                            <div class="form-group" id="serverPort">
                                <label class="form-control-label">
                                    Server Port:
                                </label>
                                <input required v-model="currentServer.serverPort" type="text" class="form-control">
                            </div>
                            <div class="form-group" id="updateInterval">
                                <label class="form-control-label">
                                    Update Interval:
                                </label>
                                <input required v-model="currentServer.updateInterval" type="text" class="form-control">
                            </div>
                            <div class="form-group">
                                <label class="m-checkbox m-checkbox--single m-checkbox--solid m-checkbox--brand">
                                    <input type="checkbox" v-model="currentServer.enabled"><span></span> Enbled
                                </label>
                            </div>
                            <div class="form-group m-form__group">
                                <label>
                                    Encryption
                                </label>
                                <select v-model="currentServer.encryption" class="form-control m-input">
                                    <option v-for="type in encTypes" v-bind:value="type.value">
                                        {{ type.key }}
                                    </option>
                                </select>
                            </div>
                            <div class="form-group">
                                <label class="form-control-label">
                                    Login:
                                </label>
                                <input required v-model="currentServer.login" type="text" class="form-control">
                            </div>
                            <div class="form-group">
                                <label class="form-control-label">
                                    Password:
                                </label>
                                <input required v-model="currentServer.password" type="password" class="form-control">
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
    </div>
</template>

<script>
    import PopApi from "../../Services/pop"
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
                isLoading: false,
                searchQuery: "",
                encTypes: [
                    { key: "None", value: 0 },
                    { key: "SSL", value: 1 },
                    { key: "TLS", value: 2 },
                ],
                servers: [],
                departments: [],
                total: 0,
                currentPage: 1,
                newServer: {
                    encryption: 0,
                    updateInterval: 0,
                    emailAddress: "",
                    serverName: "",
                    serverPort: "",
                    login: "",
                    enabled: false,
                    password: "",
                },
                currentServer: {
                    encryption: 0,
                    updateInterval: 0,
                    emailAddress: "",
                    serverName: "",
                    serverPort: "",
                    login: "",
                    enabled: false,
                    password: "",
                },
            }
        },

        methods: {
            setPageSize(size) {
                this.pageSize = size;
                alert(this.pageSize);
            },

            showModal(id) {
                clearForm();
                this.currentServer = Object.assign({}, this.newServer);
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

            async editSrv(id) {
                this.showModal("edit_modal");

                try {
                    let response = await this.$http.get(AppSettings.ApiUrl + `api/settings/email/pop/${id}`);
                    console.log(response.data);

                    this.currentServer = response.data;
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

            async deleteSrv(id) {
                try {
                    if (confirm("Are you sure you want to delete this server?")) {
                        let response = await this.$http.delete(AppSettings.ApiUrl + `api/settings/email/pop/${id}`);

                        this.notification("Server deleted.", "success");
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

            async saveSrv() {
                try {
                    let response = await this.$http.put(AppSettings.ApiUrl + "api/settings/email/pop", this.currentServer);

                    this.notification("Server updated.", "success");

                    this.loadPage(this.currentPage);

                    $("#edit_modal").modal("toggle");
                } catch (err) {
                    this.notification("Please fix the errors.", "danger");
                    addErrors(err);
                }
            },

            async createSrv() {
                clearForm();

                try {
                    let response = await this.$http.post(AppSettings.ApiUrl + "api/settings/email/pop", this.currentServer);

                    this.notification("Server created.", "success");

                    this.loadPage(this.currentPage);

                    $("#create_modal").modal("toggle");
                } catch (err) {
                    this.notification("Please fix the errors.", "danger");
                    addErrors(err);
                }
            },

            async loadPage(page) {
                if (page < 1 || page > this.totalPages) {
                    return;
                }

                this.block();

                this.currentPage = page;
                
                await PopApi.getAll(
                    {
                        pageSize: this.pageSize,
                        pageIndex: this.currentPage,
                        s: this.searchQuery
                    },
                    (data) => {
                        this.servers = data.data;
                        this.total = data.count;
                    },
                    (err) => {
                        this.notification(err, "danger");
                    }
                );

                this.unblock();
            },

            async loadDepartments() {
                try {
                    let response = await this.$http.get(AppSettings.ApiUrl + "api/departments");
                    this.departments = response.data.data;
                } catch (err) {
                    alert(err);
                    console.log(err)
                }
            },
        },

        mounted: function () {
            this.loadDepartments();
            this.loadPage(1);
        },
    }
</script>
