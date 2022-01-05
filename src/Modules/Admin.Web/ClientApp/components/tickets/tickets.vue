<template>
    <div class="m-grid__item m-grid__item--fluid m-wrapper">
        <div class="m-content">
            <div class="m-portlet m-portlet--mobile">
                <div class="m-portlet__head">
                    <div class="m-portlet__head-caption">
                        <div class="m-portlet__head-title">
                            <h3 class="m-portlet__head-text">
                                Support Tickets
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
                                            Open Ticket
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
                        <table class="m-datatable__table" id="datatable" style="display: block; min-height: 300px; overflow: visible;">
                            <thead class="m-datatable__head">
                                <tr class="m-datatable__row">
                                    <th class="m-datatable__cell">
                                        <span style="width: 150px;">Email</span>
                                    </th>
                                    <th class="m-datatable__cell">
                                        <span style="width: 150px;">Subject</span>
                                    </th>
                                    <th class="m-datatable__cell">
                                        <span style="width: 150px;">Department</span>
                                    </th>
                                    <th class="m-datatable__cell">
                                        <span style="width: 150px;">Created Date</span>
                                    </th>
                                    <th class="m-datatable__cell">
                                        <span style="width: 110px;">Actions</span>
                                    </th>
                                </tr>
                            </thead>
                            <tbody class="m-datatable__body">
                                <span v-if="!tickets" class="m-datatable--error">No records found</span>
                                <tr :class="'m-datatable__row ' + (index % 2 == 1 ? 'm-datatable__row--even' : '')" v-for="(ticket, index) in tickets" :key="index">
                                    <td class="m-datatable__cell"><span style="width: 150px;">{{ ticket.customerEmail }}</span></td>
                                    <td class="m-datatable__cell"><span style="width: 150px;">{{ ticket.subject }}</span></td>
                                    <td class="m-datatable__cell"><span style="width: 150px;">{{ ticket.departmentName }}</span></td>
                                    <td class="m-datatable__cell"><span style="width: 150px;">{{ ticket.dateOpened }}</span></td>
                                    <td data-field="Actions" class="m-datatable__cell">
                                        <span style="overflow: visible; width: 110px;">
                                            <div class="dropdown">
                                                <a href="#" class="btn m-btn m-btn--hover-accent m-btn--icon m-btn--icon-only m-btn--pill" data-toggle="dropdown">
                                                    <i class="la la-ellipsis-h"></i>
                                                </a>
                                                <div class="dropdown-menu dropdown-menu-right">
                                                    <a v-if="isQueue" @click="take(ticket.id)" class="dropdown-item" href="#"><i class="la la-check"></i> Take</a>
                                                    <a class="dropdown-item" @click="assign(ticket.id)" href="#"><i class="la la-mail-forward"></i> Assign</a>
                                                </div>
                                            </div>
                                            <a @click="ticketDetails(ticket.id)" href="#" class="m-portlet__nav-link btn m-btn m-btn--hover-accent m-btn--icon m-btn--icon-only m-btn--pill" title="Edit details">
                                                <i class="la la-edit"></i>
                                            </a>
                                            <a @click="deleteTicket(ticket.id)" href="#" class="m-portlet__nav-link btn m-btn m-btn--hover-danger m-btn--icon m-btn--icon-only m-btn--pill" title="Delete">
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
                            Ticket Details
                        </h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">
                                &times;
                            </span>
                        </button>
                    </div>
                    <div class="modal-body" id="ticket_details">
                        <!--begin::m-widget5-->
                        <div class="m-widget5">
                            <div class="m-widget5__item">
                                <div class="m-widget5__content">
                                    <h4 class="m-widget5__title">
                                        {{ currentTicket.subject }}
                                    </h4>
                                    <span class="m-widget5__desc">
                                        {{ currentTicket.customerEmail }}
                                    </span>
                                    <div class="m-widget5__info">
                                        <span class="m-widget5__author">
                                            {{ currentTicket.department.name }}
                                        </span>
                                    </div>
                                </div>
                                <div class="m-widget5__stats1">
                                    <span class="m-widget3__status m--font-success">
                                        {{ currentTicket.status.displayName }}
                                    </span>
                                </div>
                            </div>
                        </div>
                        <!--end::m-widget5-->
                        <ul class="nav nav-tabs nav-fill" role="tablist">
                            <li class="nav-item">
                                <a class="nav-link active show" data-toggle="tab" href="#m_tabs_messages">
                                    Conversation
                                </a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link" data-toggle="tab" href="#m_tabs_comments">
                                    Comments
                                </a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link" data-toggle="tab" href="#m_tabs_options">
                                    Options
                                </a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link" data-toggle="tab" href="#m_tabs_history">
                                    History
                                </a>
                            </li>
                        </ul>
                        <div class="tab-content">
                            <div class="tab-pane active" id="m_tabs_messages" role="tabpanel">
                                <div class="m-portlet m-portlet--collapsed m-portlet--head-sm m-portlet--unair" data-portlet="true" id="reply_form">
                                    <div class="m-portlet__head">
                                        <div class="m-portlet__head-caption">
                                            <div class="m-portlet__head-title">
                                                <span class="m-portlet__head-icon">
                                                    <i class="flaticon-speech-bubble-1"></i>
                                                </span>
                                                <h3 class="m-portlet__head-text">
                                                    Reply
                                                </h3>
                                            </div>
                                        </div>
                                        <div class="m-portlet__head-tools">
                                            <ul class="m-portlet__nav">
                                                <li class="m-portlet__nav-item">
                                                    <a id="reply_toggle" href="#" data-portlet-tool="toggle" class="m-portlet__nav-link m-portlet__nav-link--icon">
                                                        <i class="la la-angle-down"></i>
                                                    </a>
                                                </li>
                                            </ul>
                                        </div>
                                    </div>
                                    <form @submit.prevent="ticketReply(currentTicket.id)" class="m-form m-form--fit m-form--label-align-right" style="">
                                        <div class="m-portlet__body">
                                            <div class="form-group m-form__group row">
                                                <label for="example-text-input" class="col-2 col-form-label">
                                                    New Status
                                                </label>
                                                <div class="col-10">
                                                    <select v-model="reply.newStatus.id" class="form-control m-input">
                                                        <option v-for="status in statuses" v-bind:value="status.id">
                                                            {{ status.displayName }}
                                                        </option>
                                                    </select>
                                                </div>
                                            </div>
                                            <div class="form-group m-form__group row">
                                                <label for="example-text-input" class="col-2 col-form-label">
                                                    To
                                                </label>
                                                <div class="col-10">
                                                    <input v-model="reply.to" class="form-control m-input" type="text">
                                                </div>
                                            </div>
                                            <div class="form-group m-form__group row">
                                                <label for="example-text-input" class="col-2 col-form-label">
                                                    Subject
                                                </label>
                                                <div class="col-10">
                                                    <input v-model="reply.subject" class="form-control m-input" type="text">
                                                </div>
                                            </div>
                                            <div class="form-group m-form__group row">
                                                <label for="example-text-input" class="col-2 col-form-label">
                                                    Message
                                                </label>
                                                <div class="col-10">
                                                    <textarea required data-provide="markdown" v-model="reply.message" class="form-control m-input" id="exampleTextarea" rows="3">{{ reply.message }}</textarea>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="m-portlet__foot m-portlet__foot--fit">
                                            <div class="m-form__actions m-form__actions">
                                                <div class="row">
                                                    <div class="col-2"></div>
                                                    <div class="col-10">
                                                        <button type="submit" class="btn btn-primary">
                                                            Submit
                                                        </button>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </form>
                                </div>
                                <div class="m-portlet m-portlet--full-height m-portlet--unair">
                                    <div class="m-portlet__body no_top_bot_padding">
                                        <div class="m-widget3">
                                            <div v-for="(message, index) in currentTicket.messages" class="m-widget3__item">
                                                <div class="m-widget3__header">
                                                    <div class="m-widget3__user-img">
                                                        <img class="m-widget3__img" src="https://www.gravatar.com/avatar/1a9b4fabea79ed763b435793887fb67b" alt="">
                                                    </div>
                                                    <div class="m-widget3__info">
                                                        <span class="m-widget3__username">
                                                            {{ message.subject }}
                                                        </span>
                                                        <br>
                                                        <span class="m-widget3__time">
                                                            {{ message.from }} / {{ message.date }}
                                                        </span>
                                                    </div>
                                                </div>
                                                <div class="m-widget3__body">
                                                    <p class="m-widget3__text">
                                                        {{ message.body }}
                                                    </p>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="tab-pane" id="m_tabs_comments" role="tabpanel">
                                <div class="tab-pane active" id="m_tabs_messages" role="tabpanel">
                                    <div class="m-portlet m-portlet--collapsed m-portlet--head-sm m-portlet--unair" data-portlet="true" id="add_comment_form">
                                        <div class="m-portlet__head">
                                            <div class="m-portlet__head-caption">
                                                <div class="m-portlet__head-title">
                                                    <span class="m-portlet__head-icon">
                                                        <i class="flaticon-speech-bubble-1"></i>
                                                    </span>
                                                    <h3 class="m-portlet__head-text">
                                                        Add Comment
                                                    </h3>
                                                </div>
                                            </div>
                                            <div class="m-portlet__head-tools">
                                                <ul class="m-portlet__nav">
                                                    <li class="m-portlet__nav-item">
                                                        <a href="#" id="comment_toggle" data-portlet-tool="toggle" class="m-portlet__nav-link m-portlet__nav-link--icon">
                                                            <i class="la la-angle-down"></i>
                                                        </a>
                                                    </li>
                                                </ul>
                                            </div>
                                        </div>
                                        <form @submit.prevent="addComment(currentTicket.id)" class="m-form m-form--fit m-form--label-align-right" style="">
                                            <div class="m-portlet__body">
                                                <div class="form-group m-form__group">
                                                    <textarea required v-model="newComment.text" class="form-control m-input" id="exampleTextarea" rows="3">{{ newComment.text }}</textarea>
                                                </div>
                                            </div>
                                            <div class="m-portlet__foot m-portlet__foot--fit">
                                                <div class="m-form__actions">
                                                    <button type="submit" class="btn btn-primary">
                                                        Submit
                                                    </button>
                                                </div>
                                            </div>
                                        </form>
                                    </div>
                                    <div class="m-portlet m-portlet--full-height m-portlet--unair">
                                        <div class="m-portlet__body no_top_bot_padding">
                                            <div class="m-widget3">
                                                <div v-for="(comment, index) in currentTicket.comments" class="m-widget3__item">
                                                    <div class="m-widget3__header">
                                                        <div class="m-widget3__user-img">
                                                            <img class="m-widget3__img" src="https://www.gravatar.com/avatar/1a9b4fabea79ed763b435793887fb67b" alt="">
                                                        </div>
                                                        <div class="m-widget3__info">
                                                            <span class="m-widget3__username">
                                                                {{ comment.user.userName }}
                                                            </span>
                                                            <br>
                                                            <span class="m-widget3__time">
                                                                {{ comment.date }}
                                                            </span>
                                                        </div>
                                                    </div>
                                                    <div class="m-widget3__body">
                                                        <p class="m-widget3__text">
                                                            {{ comment.text }}
                                                        </p>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="tab-pane" id="m_tabs_history" role="tabpanel">
                                <div class="m-scrollable mCustomScrollbar _mCS_5 mCS-autoHide" data-scrollbar-shown="true" data-scrollable="true" data-max-height="380" style="overflow: visible; height: 380px; max-height: 380px; position: relative;">
                                    <div class="m-portlet m-portlet--unair">
                                        <div class="m-portlet__body">
                                            <div class="m_datatable m-datatable m-datatable--brand m-datatable--default m-datatable--loaded">
                                                <table class="m-datatable__table" id="datatable" style="display: block; min-height: 300px;">
                                                    <thead class="m-datatable__head">
                                                        <tr class="m-datatable__row">
                                                            <th class="m-datatable__cell">
                                                                <span style="width: 150px;">Date</span>
                                                            </th>
                                                            <th class="m-datatable__cell">
                                                                <span style="width: 150px;">Description</span>
                                                            </th>
                                                        </tr>
                                                    </thead>
                                                    <tbody class="m-datatable__body">
                                                        <span v-if="!tickets" class="m-datatable--error">No records found</span>
                                                        <tr :class="'m-datatable__row ' + (index % 2 == 1 ? 'm-datatable__row--even' : '')" v-for="(event, index) in currentTicket.events" :key="index">
                                                            <td class="m-datatable__cell"><span style="width: 150px;">{{ event.date }}</span></td>
                                                            <td class="m-datatable__cell"><span style="width: 150px;">{{ event.eventType.displayName }}</span></td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="tab-pane" id="m_tabs_options" role="tabpanel">
                                <div class="m-portlet m-portlet--unair" id="update_form">
                                    <form @submit.prevent="updateTicket(currentTicket.id)" class="m-form">
                                        <div class="m-portlet__body">
                                            <div class="m-form__section m-form__section--first">
                                                <div class="form-group m-form__group row">
                                                    <label class="col-lg-3 col-form-label">
                                                        Department:
                                                    </label>
                                                    <div class="col-lg-9">
                                                        <select v-model="currentTicket.department.id" class="form-control m-input">
                                                            <option v-for="dep in departments" v-bind:value="dep.id">
                                                                {{ dep.brandName + "/" + dep.name }}
                                                            </option>
                                                        </select>
                                                    </div>
                                                </div>
                                                <div class="form-group m-form__group row">
                                                    <label class="col-lg-3 col-form-label">
                                                        Agent:
                                                    </label>
                                                    <div class="col-lg-9">
                                                        <select v-model="currentTicket.userId" class="form-control m-input">
                                                            <option :value="null">Unassigned</option>
                                                            <option v-for="usr in users" v-bind:value="usr.id">
                                                                {{ usr.userName }}
                                                            </option>
                                                        </select>
                                                    </div>
                                                </div>
                                                <div class="form-group m-form__group row">
                                                    <label class="col-lg-3 col-form-label">
                                                        Status:
                                                    </label>
                                                    <div class="col-lg-9">
                                                        <select v-model="currentTicket.status.id" class="form-control m-input">
                                                            <option v-for="status in statuses" v-bind:value="status.id">
                                                                {{ status.displayName }}
                                                            </option>
                                                        </select>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="m-portlet__foot m-portlet__foot--fit">
                                            <div class="m-form__actions m-form__actions">
                                                <div class="row">
                                                    <div class="col-lg-3"></div>
                                                    <div class="col-lg-9">
                                                        <button type="submit" class="btn btn-primary">
                                                            Submit
                                                        </button>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </form>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">
                            Close
                        </button>
                        <!--<button type="button" class="btn btn-primary">
                        Save
                    </button>-->
                    </div>
                </div>
            </div>
        </div>

        <div class="modal fade" id="create_modal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-lg" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="exampleModalLabel">
                            New Ticket
                        </h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">
                                &times;
                            </span>
                        </button>
                    </div>
                    <form @submit.prevent="createTicket()" id="create_frm">
                        <div class="modal-body">
                            <div class="form-group" id="Email">
                                <label class="form-control-label">
                                    Email:
                                </label>
                                <input required v-model="startTicket.email" type="text" class="form-control">
                            </div>
                            <div class="form-group m-form__group">
                                <label class="form-control-label">
                                    Department:
                                </label>
                                <select v-model="startTicket.departmentId" class="form-control m-input">
                                    <option v-for="dep in departments" v-bind:value="dep.id">
                                        {{ dep.brandName + "/" + dep.name }}
                                    </option>
                                </select>
                            </div>
                            <div class="form-group">
                                <label class="form-control-label">
                                    Name:
                                </label>
                                <input required v-model="startTicket.name" type="text" class="form-control">
                            </div>
                            <div class="form-group">
                                <label class="form-control-label">
                                    Subject:
                                </label>
                                <input required v-model="startTicket.subject" type="text" class="form-control">
                            </div>
                            <div class="form-group">
                                <label class="form-control-label">
                                    Message:
                                </label>

                                <textarea required v-model="startTicket.message" class="form-control m-input" rows="3">{{ startTicket.message }}</textarea>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-dismiss="modal">
                                Cancel
                            </button>
                            <input type="submit" class="btn btn-primary" value="Create" />
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    import DepartmentApi from "../../services/departments";
    import UserApi from "../../services/users";
    import TicketsApi from "../../services/tickets";
    import { mapGetters } from "vuex";

    export default {
        computed: {
            totalPages: function () {
                return Math.max(Math.ceil(this.total / this.pageSize), 1);
            },
            isQueue: function () {
                return this.status == "queue";
            },
            ...mapGetters({
                pageSize: "itemsPerPage",
            })
        },

        watch: {
            status: function (o, n) {
                this.loadPage(1);
            },
            isLoading: function (o, n) {
                if (!this.isLoading) {
                    mApp.unblock('#ticket_details');
                }
            },
            pageSize: function (value) {
                this.currentPage = 1;
                this.loadPage(this.currentPage);
            }
        },

        props: [
            "status",
            "global",
            "open",
            "active"
        ],

        data() {
            return {
                isLoading: false,
                searchQuery: "",
                total: 0,
                currentPage: 1,
                selectedBrand: 0,
                startTicket: {
                },
                currentTicket: {
                    status: {},
                    department: {}
                },
                newComment: {
                    text: "",
                },
                reply: {
                    to: "",
                    message: "",
                    newStatus: {},
                    subject: ""
                },
                tickets: [],
                brands: [],
                departments: [],
                users: [],
                statuses: [],
            }
        },

        methods: {
            setPageSize(size) {
                this.pageSize = size;
                alert(this.pageSize);
            },

            showModal(id) {
                clearForm();
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

            async createTicket() {
                clearForm();
                mApp.block("#create_frm", {
                    message: "Please wait",
                });

                await TicketsApi.createTicket(
                    this.startTicket,
                    (data) => {
                        this.notification("Item created", "success");
                        $("#create_modal").modal("toggle");
                        this.loadPage(this.currentPage);
                        this.startTicket = {};
                    },
                    (err) => {
                        this.notification("Please fix the errors", "danger");
                        addErrors(err);
                    }
                );

                mApp.unblock("#create_frm");
            },

            async ticketDetails(id) {
                this.isLoading = true;

                $("#edit_modal").modal();

                try {
                    let response = await this.$http.get(AppSettings.ApiUrl + `api/tickets/${id}`);

                    this.currentTicket = response.data;
                    this.reply.newStatus = this.currentTicket.status;
                    this.reply.to = this.currentTicket.customerEmail;
                    this.reply.subject = this.currentTicket.subject;
                } catch (err) {
                    var errMsg = "";
                    for (var key in err.response.data) {
                        if (err.response.data.hasOwnProperty(key)) {
                            errMsg = err.response.data[key];
                            break;
                        }
                    }

                    this.notification(errMsg, "danger");
                }

                this.isLoading = false;
            },

            async ticketReply(id) {
                mApp.block("#reply_form", {
                    message: "Please wait",
                    overlayColor: "#000000",
                    opacity: 0.33,
                });

                var fd = new FormData();

                fd.append("newStatusId", this.reply.newStatus.id);
                fd.append("to", this.reply.to);
                fd.append("subject", this.reply.subject);
                fd.append("message", this.reply.message);

                TicketsApi.reply(id, fd, (data) => {
                    this.ticketDetails(id);
                    this.$store.dispatch("updateSummary");
                    this.notification("Message Sent.", "success");

                    $("#reply_toggle").click();
                    this.reply.message = "";
                    this.reply.to = this.currentTicket.customerEmail;
                    this.reply.subject = this.currentTicket.subject;
                }, (err) => this.notification(err, "danger"));

                mApp.unblock('#reply_form');
            },

            async updateTicket(id) {
                mApp.block("#update_form", {
                    message: "Please wait",
                    overlayColor: "#000000",
                    opacity: 0.33,
                });

                var fd = new FormData();

                fd.append("statusId", this.currentTicket.status.id);
                fd.append("departmentId", this.currentTicket.department.id);
                fd.append("agentId", this.currentTicket.userId);

                TicketsApi.update(id, fd, (data) => {
                    this.notification("Ticket updated.", "success");
                    this.$store.dispatch("updateSummary");
                }, (err) => this.notification(err, "danger"));

                mApp.unblock('#update_form');
            },

            async addComment(id) {
                mApp.block("#add_comment_form", {
                    message: "Please wait",
                    overlayColor: "#000000",
                    opacity: 0.33,
                });

                var fd = new FormData();

                fd.append("text", this.newComment.text);

                TicketsApi.addComment(id, fd, (data) => this.ticketDetails(id), (err) => this.notification(err, "danger"));

                mApp.unblock('#add_comment_form');

                $("#comment_toggle").click();
                this.notification("Message Sent.", "success");
                this.newComment.text = "";
            },

            async take(id) {
                TicketsApi.take(id, (data) => {
                    this.loadPage(this.currentPage);
                    this.$store.dispatch("updateSummary");
                }, (err) => this.notification(err, "danger"));
            },

            async assign(id) {
                await this.ticketDetails(id);
                $('a[href="#m_tabs_options"]').tab("show");
            },

            async deleteTicket(id) {
                try {
                    if (confirm("Are you sure you want to delete this item?")) {
                        let response = await this.$http.delete(AppSettings.ApiUrl + `api/tickets/${id}`);

                        this.notification("Ticket deleted.", "success");
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
                }
            },

            buildFilters() {
                var filters = {};

                switch (this.status) {
                    case "queue":
                        filters.queue = true;
                        break;

                    case "active":
                    case "gactive":
                        filters.open = true;
                        filters.active = true;
                        break;

                    case "waiting":
                    case "gwaiting":
                        filters.open = true;
                        filters.active = false;
                        break;

                    case "closed":
                    case "gclosed":
                        filters.open = false;
                        filters.active = false;
                        break;

                    default:
                }

                filters.global = typeof this.global !== 'undefined' ? this.global : true;
                filters.pageSize = this.pageSize;
                filters.pageIndex = this.currentPage;
                filters.s = this.searchQuery;

                return filters;
            },
            
            async loadPage(page) {
                if (page < 1 || page > this.totalPages) {
                    return;
                }

                var filters = this.buildFilters();

                mApp.block("#datatable", {
                    message: "Please wait",
                });

                this.currentPage = page;

                var url = AppSettings.ApiUrl + "api/tickets";

                try {
                    let response = await this.$http.get(url, { params: filters });
                    this.tickets = response.data.data;
                    this.total = response.data.count;
                } catch (err) {
                    alert(err);
                }

                mApp.unblock("#datatable");
            },
        },

        mounted: async function () {
            await DepartmentApi.getAll({}, (data) => this.departments = data.data, (err) => this.notification(err, "danger"));
            await UserApi.getAgents({}, (data) => { this.users = data.data }, (err) => this.notification(err, "danger"));
            await TicketsApi.getStatuses((data) => this.statuses = data, (err) => this.notification(err, "danger"));

            this.loadPage(1);

            var _this = this;
            
            $('#edit_modal').on('shown.bs.modal', function (event) {
                if (_this.isLoading) {
                    mApp.block("#ticket_details", {
                        message: "Please wait",
                        overlayColor: "#000000",
                        opacity: 0.33,
                    });
                }
            });

            $("#add_comment_form").mPortlet();
            $("#reply_form").mPortlet();
            $("textarea").markdown();
        },

        beforeDestroy: function () {
            $(".mCustomScrollbar").mCustomScrollbar("destroy");
            $(".mCustomScrollbar").removeClass('mCS_destroyed');
        }
    }
</script>

<style>
    /*.m-widget5 .m-widget5__item .m-widget5__content {
        padding-left: 0 !important;
        padding-top: 0 !important;
    }
    .m-widget5 .m-widget5__item {
        margin-bottom: 0 !important;
    }
    .no_top_bot_padding {
        padding: 0 !important;
    }
    .no_shadow {
        -webkit-box-shadow: none !important;
        -moz-box-shadow: none !important;
        box-shadow: none !important;
    }*/
    .m-widget5 .m-widget5__item .m-widget5__stats1 {
        padding-top: 0 !important;
    }
    .m-timeline-2:before {
        left: 10rem !important;
    }
    .m-timeline-2 .m-timeline-2__items .m-timeline-2__item .m-timeline-2__item-cricle {
        left: 9.2rem !important;
    }
    .m-timeline-2 .m-timeline-2__items .m-timeline-2__item .m-timeline-2__item-text {
        padding-left: 10rem !important;
    }
</style>
