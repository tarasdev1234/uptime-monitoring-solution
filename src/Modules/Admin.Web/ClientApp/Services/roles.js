import axios from "../http/axios-config";

export default {
    getErr(ex) {
        console.log(ex);
        
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

        await axios.get(AppSettings.ApiUrl + "api/roles", { params: params })
            .then(function (response) {
                cb(response.data);
            })
            .catch(function (error) {
                err(that.getErr(error));
            });
    },
    
    async save(value, cb, err) {
        await axios.put(AppSettings.ApiUrl + "api/roles", value)
            .then(function (response) {
                cb(response.data);
            })
            .catch(function (error) {
                err(this.getErr(ex));
            });
    },
}
