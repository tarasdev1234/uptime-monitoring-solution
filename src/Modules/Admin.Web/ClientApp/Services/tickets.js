import axios from "../http/axios-config";

export default {
    async createTicket(value, cb, err) {
        await axios.post(AppSettings.ApiUrl + "api/tickets/create", value)
            .then(function (response) {
                cb(response.data);
            })
            .catch(function (error) {
                err(error);
            });
    },

    async getStatuses(cb, err) {
        try {
            let response = await axios.get(AppSettings.ApiUrl + "api/tickets/statuses");

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

    async getSummary(cb, err) {
        try {
            let response = await axios.get(AppSettings.ApiUrl + "api/tickets/summary");

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

    async addComment(id, params, cb, err) {
        try {
            let response = await axios.post(AppSettings.ApiUrl + `api/tickets/${id}/comment`, params);

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

    async reply(id, params, cb, err) {
        try {
            let response = await axios.post(AppSettings.ApiUrl + `api/tickets/reply/${id}`, params);

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

    async update(id, params, cb, err) {
        try {
            let response = await axios.put(AppSettings.ApiUrl + `api/tickets/update/${id}`, params);

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

    async take(id, cb, err) {
        try {
            let response = await axios.post(AppSettings.ApiUrl + `api/tickets/take/${id}`);

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
