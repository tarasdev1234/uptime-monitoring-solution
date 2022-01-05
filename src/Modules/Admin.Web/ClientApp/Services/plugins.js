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

        await axios.get(AppSettings.ApiUrl + "api/plugins", { params: params })
            .then(function (response) {
                cb(response.data);
            })
            .catch(function (error) {
                err(this.getErr(ex));
            });
    },

    async getCustom(params, cb, err) {
        params = $.extend(
            true,
            {
                pageSize: 10,
                pageIndex: 1
            },
            params
        );

        await axios.get(AppSettings.ApiUrl + "api/plugins/custom", { params: params })
            .then(function (response) {
                cb(response.data);
            })
            .catch(function (error) {
                err(this.getErr(ex));
            });
    },
}
