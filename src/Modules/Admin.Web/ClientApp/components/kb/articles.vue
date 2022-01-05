<template>
    <div class="m-grid__item m-grid__item--fluid m-wrapper">
        <div class="m-content">
            <div class="m-portlet m-portlet--mobile">
                <div class="m-portlet__head">
                    <div class="m-portlet__head-caption">
                        <div class="m-portlet__head-title">
                            <h3 class="m-portlet__head-text">
                                Knowledge Base
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
                                            New Article
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
                                        <span style="width: 150px;">Title</span>
                                    </th>
                                    <th class="m-datatable__cell">
                                        <span style="width: 150px;">Author</span>
                                    </th>
                                    <th class="m-datatable__cell">
                                        <span style="width: 150px;">Category</span>
                                    </th>
                                    <th class="m-datatable__cell">
                                        <span style="width: 150px;">Tags</span>
                                    </th>
                                    <th class="m-datatable__cell">
                                        <span style="width: 150px;">Date</span>
                                    </th>
                                    <th class="m-datatable__cell m-datatable__cell--center">
                                        <span style="width: 90px;">Comments</span>
                                    </th>
                                    <th class="m-datatable__cell">
                                        <span style="width: 70px;">Actions</span>
                                    </th>
                                </tr>
                            </thead>
                            <tbody class="m-datatable__body">
                                <span v-if="!articles" class="m-datatable--error">No records found</span>
                                <tr :class="'m-datatable__row ' + (index % 2 == 1 ? 'm-datatable__row--even' : '')" v-for="(article, index) in articles" :key="index">
                                    <td class="m-datatable__cell"><span style="width: 150px;">{{ article.title }}</span></td>
                                    <td class="m-datatable__cell"><span style="width: 150px;">{{ article.authorName }}</span></td>
                                    <td class="m-datatable__cell"><span style="width: 150px;">{{ article.kbCategoryName }}</span></td>
                                    <td class="m-datatable__cell">
                                        <div class="m-list-badge">
                                            <div style="width: 150px;" class="m-list-badge__items">
                                                <span v-for="(tag, index) in article.tagNames" class="m-list-badge__item m-list-badge__item--success">
                                                    {{ tag }}
                                                </span>
                                            </div>
                                        </div>
                                    </td>
                                    <td class="m-datatable__cell"><span style="width: 150px;">{{ article.dateCreated }}</span></td>
                                    <td class="m-datatable__cell m-datatable__cell--center"><span style="width: 90px;">{{ article.commentsCount }}</span></td>
                                    <td data-field="Actions" class="m-datatable__cell">
                                        <span style="overflow: visible; width: 70px;">
                                            <a @click="editArticle(article.id)" href="#" class="m-portlet__nav-link btn m-btn m-btn--hover-accent m-btn--icon m-btn--icon-only m-btn--pill" title="Edit details">
                                                <i class="la la-edit"></i>
                                            </a>
                                            <a @click="deleteArticle(article.id)" href="#" class="m-portlet__nav-link btn m-btn m-btn--hover-danger m-btn--icon m-btn--icon-only m-btn--pill" title="Delete">
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
                            Edit Article
                        </h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">
                                &times;
                            </span>
                        </button>
                    </div>
                    <form @submit.prevent="saveArticle()">
                        <div class="modal-body">
                            <ul class="nav nav-tabs nav-fill" role="tablist">
                                <li class="nav-item">
                                    <a class="nav-link active show" data-toggle="tab" href="#edit_tabs_info">
                                        Edit
                                    </a>
                                </li>
                                <li class="nav-item">
                                    <a class="nav-link" data-toggle="tab" href="#edit_tabs_options">
                                        Options
                                    </a>
                                </li>
                            </ul>
                            <div class="tab-content">
                                <div class="tab-pane active" id="edit_tabs_info" role="tabpanel">
                                    <div class="m-portlet__body">
                                        <div class="form-group m-form__group row">
                                            <label for="example-text-input" class="col-2 col-form-label">
                                                Title
                                            </label>
                                            <div class="col-10">
                                                <input v-model="currentArticle.title" class="form-control m-input" type="text">
                                            </div>
                                        </div>
                                        <div class="form-group m-form__group row">
                                            <label for="example-text-input" class="col-2 col-form-label">
                                                Category
                                            </label>
                                            <div class="col-10">
                                                <select v-model="currentArticle.kbCategoryId" class="form-control m-input">
                                                    <option v-for="cat in categories" v-bind:value="cat.id">
                                                        {{ cat.name }}
                                                    </option>
                                                </select>
                                            </div>
                                        </div>
                                        <div class="form-group m-form__group row">
                                            <label for="example-text-input" class="col-2 col-form-label">
                                                Status
                                            </label>
                                            <div class="col-10">
                                                <label class="m-checkbox">
                                                    <input v-model="currentArticle.isPublished" type="checkbox">
                                                    Published
                                                    <span></span>
                                                </label>
                                            </div>
                                        </div>
                                        <div class="form-group m-form__group row">
                                            <label for="example-text-input" class="col-2 col-form-label">
                                                Author
                                            </label>
                                            <div class="col-10">
                                                <select v-model="currentArticle.authorId" class="form-control m-input">
                                                    <option v-for="usr in users" v-bind:value="usr.id">
                                                        {{ usr.userName }}
                                                    </option>
                                                </select>
                                            </div>
                                        </div>
                                        <div class="form-group m-form__group row">
                                            <label for="example-text-input" class="col-2 col-form-label">
                                                Content
                                            </label>
                                            <div class="col-10">
                                                <textarea required data-provide="markdown" v-model="currentArticle.content" class="form-control m-input" rows="3">{{ currentArticle.content }}</textarea>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="tab-pane" id="edit_tabs_options" role="tabpanel">
                                    <div class="m-portlet__body">
                                        <div class="form-group m-form__group row">
                                            <label for="example-text-input" class="col-3 col-form-label">
                                                Brands
                                            </label>
                                            <div class="col-9">
                                                <div class="m-checkbox-list">
                                                    <label class="m-checkbox">
                                                        <input v-model="currentArticle.showInAll" type="checkbox">
                                                        Show In All
                                                        <span></span>
                                                    </label>
                                                    <label v-for="(brand, index) in brands" class="m-checkbox" v-bind:class="{ 'm-checkbox--disabled': currentArticle.showInAll }">
                                                        <input :value="brand.id" :disabled="currentArticle.showInAll" v-model="currentArticle.brands" type="checkbox" checked="checked">
                                                        {{ brand.name }}
                                                        <span></span>
                                                    </label>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="form-group m-form__group row">
                                            <label for="example-text-input" class="col-3 col-form-label">
                                                Tags
                                            </label>
                                            <div class="col-9">
                                                <div class="m-checkbox-list">
                                                    <label v-for="(tag, index) in tags" class="m-checkbox">
                                                        <input :value="tag.id" v-model="currentArticle.tags" type="checkbox">
                                                        {{ tag.name }}
                                                        <span></span>
                                                    </label>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
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

        <div class="modal fade" id="create_modal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-lg" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="exampleModalLabel">
                            New Article
                        </h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">
                                &times;
                            </span>
                        </button>
                    </div>
                    <form @submit.prevent="createArticle()">
                        <div class="modal-body">
                            <ul class="nav nav-tabs nav-fill" role="tablist">
                                <li class="nav-item">
                                    <a class="nav-link active show" data-toggle="tab" href="#m_tabs_info">
                                        Edit
                                    </a>
                                </li>
                                <li class="nav-item">
                                    <a class="nav-link" data-toggle="tab" href="#m_tabs_options">
                                        Options
                                    </a>
                                </li>
                            </ul>
                            <div class="tab-content">
                                <div class="tab-pane active" id="m_tabs_info" role="tabpanel">
                                    <div class="m-portlet__body">
                                        <div class="form-group m-form__group row">
                                            <label for="example-text-input" class="col-2 col-form-label">
                                                Title
                                            </label>
                                            <div class="col-10">
                                                <input v-model="currentArticle.title" class="form-control m-input" type="text">
                                            </div>
                                        </div>
                                        <div class="form-group m-form__group row">
                                            <label for="example-text-input" class="col-2 col-form-label">
                                                Category
                                            </label>
                                            <div class="col-10">
                                                <select v-model="currentArticle.kbCategoryId" class="form-control m-input">
                                                    <option v-for="cat in categories" v-bind:value="cat.id">
                                                        {{ cat.name }}
                                                    </option>
                                                </select>
                                            </div>
                                        </div>
                                        <div class="form-group m-form__group row">
                                            <label for="example-text-input" class="col-2 col-form-label">
                                                Status
                                            </label>
                                            <div class="col-10">
                                                <label class="m-checkbox">
                                                    <input v-model="currentArticle.isPublished" type="checkbox">
                                                    Published
                                                    <span></span>
                                                </label>
                                            </div>
                                        </div>
                                        <div class="form-group m-form__group row">
                                            <label for="example-text-input" class="col-2 col-form-label">
                                                Author
                                            </label>
                                            <div class="col-10">
                                                <select v-model="currentArticle.authorId" class="form-control m-input">
                                                    <option v-for="usr in users" v-bind:value="usr.id">
                                                        {{ usr.userName }}
                                                    </option>
                                                </select>
                                            </div>
                                        </div>
                                        <div class="form-group m-form__group row">
                                            <label for="example-text-input" class="col-2 col-form-label">
                                                Content
                                            </label>
                                            <div class="col-10">
                                                <textarea required data-provide="markdown" v-model="currentArticle.content" class="form-control m-input" rows="3">{{ currentArticle.content }}</textarea>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="tab-pane" id="m_tabs_options" role="tabpanel">
                                    <div class="m-portlet__body">
                                        <div class="form-group m-form__group row">
                                            <label for="example-text-input" class="col-3 col-form-label">
                                                Brands
                                            </label>
                                            <div class="col-9">
                                                <div class="m-checkbox-list">
                                                    <label class="m-checkbox">
                                                        <input v-model="currentArticle.showInAll" type="checkbox">
                                                        Show In All
                                                        <span></span>
                                                    </label>
                                                    <label v-for="(brand, index) in brands" class="m-checkbox" v-bind:class="{ 'm-checkbox--disabled': currentArticle.showInAll }">
                                                        <input :value="brand.id" :disabled="currentArticle.showInAll" v-model="currentArticle.brands" type="checkbox" checked="checked">
                                                        {{ brand.name }}
                                                        <span></span>
                                                    </label>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
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
    import KbApi from "../../Services/kb"
    import BrandApi from "../../Services/brands"
    import UserApi from "../../Services/users"
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
                total: 0,
                currentPage: 1,
                articles: [],
                newArticle: {
                    name: "",
                    brands: [],
                },
                currentArticle: {
                    brands: [],
                },
                categories: [],
                users: [],
                brands: [],
                tags: [],
                categories: []
            }
        },

        methods: {
            showModal(id) {
                this.currentArticle = Object.assign({}, this.newArticle);
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

            async editArticle(id) {
                this.showModal("edit_modal");

                KbApi.getArticle(
                    id,
                    (data) => {
                        this.currentArticle = data;
                    },
                    (err) => this.notification(err, "danger")
                );
            },

            async saveArticle() {
                await KbApi.saveArticle(
                    this.currentArticle,
                    (data) => {
                        this.notification("Item updated", "success");
                        $("#edit_modal").modal("toggle");
                        this.loadPage(this.currentPage);
                    },
                    (err) => this.notification(err, "danger")
                );
            },

            async deleteArticle(id) {
                try {
                    if (confirm("Are you sure you want to delete this item?")) {
                        let response = await this.$http.delete(AppSettings.ApiUrl + `api/settings/email/smtp/${id}`);

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

            async createArticle() {
                KbApi.createArticle(
                    this.currentArticle,
                    (data) => {
                        this.notification("Article created", "success");
                        $("#create_modal").modal("toggle");
                        this.loadPage(this.currentPage);
                    },
                    (err) => this.notification(err, "danger")
                );
            },

            async loadPage(page) {
                if (page < 1 || page > this.totalPages) {
                    return;
                }

                mApp.block("#datatable", {
                    message: "Please wait",
                });

                this.currentPage = page;

                KbApi.getArticles(
                    {
                        pageSize: this.pageSize,
                        pageIndex: this.currentPage,
                        s: this.searchQuery
                    },
                    (data) => {
                        this.articles = data.data;
                        this.total = data.count;
                    },
                    (err) => notification(err, "danger")
                );

                mApp.unblock("#datatable");
            },

            async loadDeps() {
                await KbApi.getCategories(
                    {},
                    (data) => {
                        this.categories = data.data;
                    },
                    (err) => this.notification(err, "danger")
                );

                await BrandApi.getBrands(
                    {},
                    (data) => {
                        this.brands = data.data;
                    },
                    (err) => this.notification(err, "danger")
                );

                await KbApi.getTags(
                    {},
                    (data) => {
                        this.tags = data.data;
                    },
                    (err) => this.notification(err, "danger")
                );

                await UserApi.getAll(
                    {},
                    (data) => {
                        this.users = data.data;
                    },
                    (err) => this.notification(err, "danger")
                );
            },
        },

        mounted: function () {
            this.loadPage(1);
            this.loadDeps();

            $("textarea").markdown();
        },
    }
</script>
