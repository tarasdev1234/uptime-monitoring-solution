import axios from "../../http/axios-config";

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

    async getAll(params, cb, err) {
        params = $.extend(
            true,
            {
                pageSize: 10,
                pageIndex: 1
            },
            params
        );
        var that = this;

        await axios.get(AppSettings.ApiUrl + "api/uptime/servers", { params: params })
            .then(function (response) {
                cb(response.data);
            })
            .catch(function (error) {
                err(that.getErr(error));
            });
    },

    async create(value, cb, err) {
        var that = this;
        await axios.post(AppSettings.ApiUrl + "api/uptime/servers", value)
            .then(function (response) {
                cb(response.data);
            })
            .catch(function (error) {
                err(that.getErr(error));
            });
    },

    async get(id, cb, err) {
        var that = this;
        await axios.get(AppSettings.ApiUrl + "api/uptime/servers/" + id)
            .then(function (response) {
                cb(response.data);
            })
            .catch(function (error) {
                err(that.getErr(error));
            });
    },

    async save(value, cb, err) {
        var that = this;
        await axios.put(AppSettings.ApiUrl + "api/uptime/servers", value)
            .then(function (response) {
                cb(response.data);
            })
            .catch(function (error) {
                err(that.getErr(error));
            });
    },

    async delete(id, cb, err) {
        var that = this;
        await axios.delete(AppSettings.ApiUrl + "api/uptime/servers/" + id)
            .then(function (response) {
                cb(response.data);
            })
            .catch(function (error) {
                err(that.getErr(error));
            });
    },
}
