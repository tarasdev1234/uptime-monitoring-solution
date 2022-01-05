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

    async get(id, cb, err) {
        await axios.get(AppSettings.ApiUrl + "api/company/" + id)
            .then(function (response) {
                cb(response.data);
            })
            .catch(function (error) {
                err(this.getErr(ex));
            });
    },

    async save(value, cb, err) {
        await axios.put(AppSettings.ApiUrl + "api/company", value)
            .then(function (response) {
                cb(response.data);
            })
            .catch(function (error) {
                err(this.getErr(ex));
            });
    },

    async delete(id, cb, err) {
        await axios.delete(AppSettings.ApiUrl + "api/company/" + id)
            .then(function (response) {
                cb(response.data);
            })
            .catch(function (error) {
                err(this.getErr(ex));
            });
    },

    async getAll(params, cb, err) {
        params = $.extend(
            true,
            {
                pageSize: 10,
                pageIndex: 1
            },
            params
        );

        await axios.get(AppSettings.ApiUrl + "api/company", { params: params })
            .then(function (response) {
                cb(response.data);
            })
            .catch(function (error) {
                err(this.getErr(ex));
            });
    },

    async create(value, cb, err) {
        await axios.post(AppSettings.ApiUrl + "api/company", value)
            .then(function (response) {
                cb(response.data);
            })
            .catch(function (error) {
                err(this.getErr(ex));
            });
    },
}
