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

    async getCurrencies(params, cb, err) {
        try {
            params = $.extend(
                true,
                {
                    pageSize: 10,
                    pageIndex: 1
                },
                params
            );

            let response = await axios.get(AppSettings.ApiUrl + "api/billing/currencies", { params: params });

            cb(response.data);
        } catch (ex) {
            err(this.getErr(ex));
        }
    },

    async getCurrencyFormats(cb, err) {
        try {
            let response = await axios.get(AppSettings.ApiUrl + "api/billing/currencies/formats");

            cb(response.data);
        } catch (ex) {
            err(this.getErr(ex));
        }
    },

    async createCurrency(value, cb, err) {
        try {
            let response = await axios.post(AppSettings.ApiUrl + "api/billing/currencies", value);

            cb(response.data);
        } catch (ex) {
            err(this.getErr(ex));
        }
    },

    async saveCurrency(value, cb, err) {
        try {
            let response = await axios.put(AppSettings.ApiUrl + "api/billing/currencies", value);

            cb(response.data);
        } catch (ex) {
            err(this.getErr(ex));
        }
    },

    async getCurrency(id, cb, err) {
        try {
            let response = await axios.get(AppSettings.ApiUrl + "api/billing/currencies/" + id);

            cb(response.data);
        } catch (ex) {
            err(this.getErr(ex));
        }
    },

    async deleteCurrency(id, cb, err) {
        await axios.delete(AppSettings.ApiUrl + "api/billing/currencies/" + id)
            .then(function (response) {
                cb(response.data);
            })
            .catch(function (error) {
                err(this.getErr(ex));
            });
    },
    
    async getProductGroups(params, cb, err) {
        try {
            params = $.extend(
                true,
                {
                    pageSize: 10,
                    pageIndex: 1
                },
                params
            );

            let response = await axios.get(AppSettings.ApiUrl + "api/billing/productgroups", { params: params });

            cb(response.data);
        } catch (ex) {
            err(this.getErr(ex));
        }
    },

    async getProductGroup(id, cb, err) {
        try {
            let response = await axios.get(AppSettings.ApiUrl + "api/billing/productgroups/" + id);

            cb(response.data);
        } catch (ex) {
            err(this.getErr(ex));
        }
    },

    async createProductGroup(value, cb, err) {
        try {
            let response = await axios.post(AppSettings.ApiUrl + "api/billing/productgroups", value);

            cb(response.data);
        } catch (ex) {
            err(this.getErr(ex));
        }
    },

    async saveProductGroup(value, cb, err) {
        try {
            let response = await axios.put(AppSettings.ApiUrl + "api/billing/productgroups", value);

            cb(response.data);
        } catch (ex) {
            err(this.getErr(ex));
        }
    },

    async deleteProductGroup(id, cb, err) {
        try {
            let response = await axios.delete(AppSettings.ApiUrl + "api/billing/productgroups/" + id);

            cb(response.data);
        } catch (ex) {
            err(this.getErr(ex));
        }
    },

    async getProducts(params, cb, err) {
        try {
            params = $.extend(
                true,
                {
                    pageSize: 10,
                    pageIndex: 1
                },
                params
            );

            let response = await axios.get(AppSettings.ApiUrl + "api/billing/products", { params: params });

            cb(response.data);
        } catch (ex) {
            err(this.getErr(ex));
        }
    },

    async getProduct(id, cb, err) {
        try {
            let response = await axios.get(AppSettings.ApiUrl + "api/billing/products/" + id);

            cb(response.data);
        } catch (ex) {
            err(this.getErr(ex));
        }
    },

    async createProduct(value, cb, err) {
        try {
            let response = await axios.post(AppSettings.ApiUrl + "api/billing/products", value);

            cb(response.data);
        } catch (ex) {
            err(this.getErr(ex));
        }
    },

    async saveProduct(value, cb, err) {
        try {
            let response = await axios.put(AppSettings.ApiUrl + "api/billing/products", value);

            cb(response.data);
        } catch (ex) {
            err(this.getErr(ex));
        }
    },

    async deleteProduct(id, cb, err) {
        try {
            let response = await axios.delete(AppSettings.ApiUrl + "api/billing/products/" + id);

            cb(response.data);
        } catch (ex) {
            err(this.getErr(ex));
        }
    },
}
