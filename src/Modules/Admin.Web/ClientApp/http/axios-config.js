import axios from "axios"

const http = axios.create();

const maxAttempts = 5;

// TODO: move interceptors to separate plugin

http.interceptors.request.use(function (config) {
    config.headers = {
        "RequestVerificationToken": AppSettings.AfToken
    }

    return config;
}, null);

//http.interceptors.request.use(function (config) {
//    config.headers = {
//        "Authorization": `Bearer ${AppSettings.AccessToken}`
//    }

//    return config;
//}, null);

//http.interceptors.response.use(null, (error) => {
//    if (error.config && error.response && error.response.status === 401) {
//        return http.get("account/refresh").then((rspns) => {
//            error.config.retryCount = error.config.retryCount >= 0
//                ? error.config.retryCount + 1
//                : 0;

//            if (!rspns || !rspns.data.accessToken || error.config.retryCount > maxAttempts) {
//                window.location.href = AppSettings.AppUrl + "home/logout";
//                return;
//            }

//            AppSettings.AccessToken = rspns.data.accessToken;

//            return http.request(error.config);
//        });
//    }

//    return Promise.reject(error);
//});

export default http;
