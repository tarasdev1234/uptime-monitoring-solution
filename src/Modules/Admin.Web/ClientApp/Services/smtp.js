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

        await axios.get(AppSettings.ApiUrl + "api/settings/email/smtp", { params: params })
            .then(function (response) {
                cb(response.data);
            })
            .catch(function (error) {
                err(that.getErr(error));
            });
    },

    async save(value, cb, err) {
        await axios.put(AppSettings.ApiUrl + "api/settings/email/smtp", value)
            .then(function (response) {
                cb(response.data);
            })
            .catch(function (error) {
                err(error);
            });
    }
}
