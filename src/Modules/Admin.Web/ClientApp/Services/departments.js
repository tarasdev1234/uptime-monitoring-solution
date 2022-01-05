import axios from "../http/axios-config";

export default {
    async get(id, cb, err) {
        try {
            let response = await axios.get(AppSettings.ApiUrl + `api/departments/${id}`);

            cb(response.data);
        } catch (err) {
            var errMsg = "";

            for (var key in err.response.data) {
                if (err.response.data.hasOwnProperty(key)) {
                    errMsg = err.response.data[key];
                    break;
                }
            }

            err(errMsg);
        }
    },

    async getAll(params, cb, err) {
        try {
            params = $.extend(
                true,
                {
                    pageSize: 10,
                    pageIndex: 1
                },
                params
            );

            let response = await axios.get(AppSettings.ApiUrl + "api/departments", { params: params });

            cb(response.data);
        } catch (err) {
            console.log(err);

            var errMsg = "";

            for (var key in err.response.data) {
                if (err.response.data.hasOwnProperty(key)) {
                    errMsg = err.response.data[key];
                    break;
                }
            }

            err(errMsg);
        }
    },
}
