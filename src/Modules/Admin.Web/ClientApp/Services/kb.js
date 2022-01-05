import axios from "../http/axios-config";

export default {
    getErr(ex) {
        console.log(ex.response);

        var errMsg = ex.response.data;
        
        for (var key in ex.response.data) {
            if (ex.response.data.hasOwnProperty(key)) {
                errMsg = ex.response.data[key];
                break;
            }
        }

        return errMsg;
    },

    async getArticles(params, cb, err) {
        try {
            params = $.extend(
                true,
                {
                    pageSize: 10,
                    pageIndex: 1
                },
                params
            );

            let response = await axios.get(AppSettings.ApiUrl + "api/kb", { params: params });

            cb(response.data);
        } catch (ex) {
            err(this.getErr(ex));
        }
    },

    async getArticle(id, cb, err) {
        try {
            let response = await axios.get(AppSettings.ApiUrl + "api/kb/" + id);

            cb(response.data);
        } catch (ex) {
            err(this.getErr(ex));
        }
    },

    async saveArticle(value, cb, err) {
        try {
            let response = await axios.put(AppSettings.ApiUrl + "api/kb", value);

            cb(response.data);
        } catch (ex) {
            err(this.getErr(ex));
        }
    },

    async createArticle(value, cb, err) {
        try {
            let response = await axios.post(AppSettings.ApiUrl + "api/kb", value);

            cb(response.data);
        } catch (ex) {
            err(this.getErr(ex));
        }
    },

    async getCategories(params, cb, err) {
        try {
            params = $.extend(
                true,
                {
                    pageSize: 10,
                    pageIndex: 1
                },
                params
            );

            let response = await axios.get(AppSettings.ApiUrl + "api/kb/categories", { params: params });

            cb(response.data);
        } catch (ex) {
            err(this.getErr(ex));
        }
    },

    async createCat(cat, cb, err) {
        try {
            let response = await axios.post(AppSettings.ApiUrl + "api/kb/categories", cat);

            cb(response.data);
        } catch (ex) {
            err(this.getErr(ex));
        }
    },

    async saveCat(cat, cb, err) {
        try {
            let response = await axios.put(AppSettings.ApiUrl + "api/kb/categories", cat);

            cb(response.data);
        } catch (ex) {
            err(this.getErr(ex));
        }
    },

    async getCat(id, cb, err) {
        try {
            let response = await axios.get(AppSettings.ApiUrl + "api/kb/categories/" + id);

            cb(response.data);
        } catch (ex) {
            err(this.getErr(ex));
        }
    },

    async deleteCat(id, cb, err) {
        await axios.delete(AppSettings.ApiUrl + "api/kb/categories/" + id)
            .then(function (response) {
                cb(response.data);
            })
            .catch(function (error) {
                err(this.getErr(ex));
            });
    },

    // tags
    async getTags(params, cb, err) {
        params = $.extend(
            true,
            {
                pageSize: 10,
                pageIndex: 1
            },
            params
        );

        await axios.get(AppSettings.ApiUrl + "api/kb/tags", { params: params })
            .then(function (response) {
                cb(response.data);
            })
            .catch(function (error) {
                err(this.getErr(ex));
            });
    },

    async createTag(tag, cb, err) {
        await axios.post(AppSettings.ApiUrl + "api/kb/tags", tag)
            .then(function (response) {
                cb(response.data);
            })
            .catch(function (error) {
                err(this.getErr(ex));
            });
    },

    async saveTag(tag, cb, err) {
        await axios.put(AppSettings.ApiUrl + "api/kb/tags", tag)
            .then(function (response) {
                cb(response.data);
            })
            .catch(function (error) {
                err(this.getErr(ex));
            });
    },

    async getTag(id, cb, err) {
        await axios.get(AppSettings.ApiUrl + "api/kb/tags/" + id)
            .then(function (response) {
                cb(response.data);
            })
            .catch(function (error) {
                err(this.getErr(ex));
            });
    },

    async deleteTag(id, cb, err) {
        await axios.delete(AppSettings.ApiUrl + "api/kb/tags/" + id)
            .then(function (response) {
                cb(response.data);
            })
            .catch(function (error) {
                err(this.getErr(ex));
            });
    },
}
