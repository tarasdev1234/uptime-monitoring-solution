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
        try {
            let response = await axios.get(AppSettings.ApiUrl + "api/permissions");

            cb(response.data);
        } catch (ex) {
            err(this.getErr(ex));
        }
    },
}
