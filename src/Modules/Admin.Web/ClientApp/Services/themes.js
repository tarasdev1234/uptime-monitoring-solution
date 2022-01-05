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
    
    async getThemes(params, cb, err) {
        params = $.extend(
            true,
            {
                pageSize: 10,
                pageIndex: 1
            },
            params
        );

        await axios.get(AppSettings.ApiUrl + "api/themes", { params: params })
            .then(function (response) {
                cb(response.data);
            })
            .catch(function (error) {
                err(this.getErr(ex));
            });
    },

    async createTheme(value, cb, err) {
        await axios.post(AppSettings.ApiUrl + "api/themes/", value)
            .then(function (response) {
                cb(response.data);
            })
            .catch(function (error) {
                err(this.getErr(ex));
            });
    },

    async saveTheme(value, cb, err) {
        await axios.put(AppSettings.ApiUrl + "api/themes/", value)
            .then(function (response) {
                cb(response.data);
            })
            .catch(function (error) {
                err(this.getErr(ex));
            });
    },

    async getTheme(id, cb, err) {
        await axios.get(AppSettings.ApiUrl + "api/themes/" + id)
            .then(function (response) {
                cb(response.data);
            })
            .catch(function (error) {
                err(this.getErr(ex));
            });
    },

    async deleteTheme(id, cb, err) {
        await axios.delete(AppSettings.ApiUrl + "api/themes/" + id)
            .then(function (response) {
                cb(response.data);
            })
            .catch(function (error) {
                err(this.getErr(ex));
            });
    },
}
