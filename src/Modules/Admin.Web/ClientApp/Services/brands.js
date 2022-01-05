import axios from "../http/axios-config";

export default {
    getErr(ex) {
        console.log(ex);

        var errMsg = "";

        for (var key in ex.response.data) {
            if (ex.response.data.hasOwnProperty(key)) {
                errMsg = ex.response.data[key];
                break;
            }
        }

        return errMsg;
    },

    async getBrands(params, cb, err) {
        params = $.extend(
            true,
            {
                pageSize: 10,
                pageIndex: 1
            },
            params
        );

        await axios.get(AppSettings.ApiUrl + "api/brand", { params: params })
            .then(function (response) {
                cb(response.data);
            })
            .catch(function (error) {
                err(this.getErr(ex));
            });
    },
}
