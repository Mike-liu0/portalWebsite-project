function DataProxy() {
    var _ = this;
    var parent;

    var accesstoken;
    var flag = false;

    _.OnRequestAuthCodeByMobile = function (mobile) {
        debug("ajax" + mobile);
        $.ajax({
            type: 'POST',
            async: false,
            url: API_ROOT + get_SMS_api,
            data: {
                key: API_KEY,
                mobile: mobile,
            },
            success: function (response) {
                debug(response);
                var package_info = JSON.parse(response).package;
                accesstoken = package_info.accesstoken;
            },
            error: function (response) {
                login = false;
                console.log("API error Response :" + response);
            }
        });

        return accesstoken;
    }

    _.OnRequestLoginByMobile = function (mobile, code, accesstoken) {
        var return_Token;
        var flag = false;
        var info;
        $.ajax({
            type: 'POST',
            async: false,
            url: API_ROOT + LoginByMobile_api,
            data: {
                key: API_KEY,
                mobile: mobile,
                code: code,
                token: accesstoken,
            },
            success: function (response) {
                var package_info = JSON.parse(response).success;
                flag = package_info;
                guid = JSON.parse(response).package.guid;
                info = JSON.parse(response).message;
                return_Token = JSON.parse(response).package.accesstoken;

            },
            error: function (response) {
                login = false;
                console.log("API error Response :" + response);
            }
        });
        return {
            isLogin: flag,
            info: info,
            guid: guid,
            savedToken: return_Token,
        };

    }


    _.OnRequestLoginByToken = function (accesstoken) {
        var flag;
        var return_Token;
        $.ajax({
            type: 'POST',
            async: false,
            url: API_ROOT + LoginByToken_api,
            data: {
                key: API_KEY,
                token: accesstoken,
            },

            success: function (response) {
                flag = JSON.parse(response).success;
                guid = JSON.parse(response).package.guid;
                fullName = JSON.parse(response).package.fullName;
                return_Token = JSON.parse(response).package.accesstoken;
            },
            error: function (response) {
                login = false;
                return_Token = null;
                console.log("API error Response :" + response);

            }
        });
        return {
            isLogin: flag,
            guid: guid,
            fullName: fullName,
            savedToken: return_Token,
        };

    }

    _.getBugs = function (id, time) {
        var bug_number;
        $.ajax({
            type: 'POST',
            async: false,
            url: "https://dev.allinks.com.au/api/" + check_bugs_api,
            data: {
                key: "jQTHqBgLA0aNSNoVUbU0NQ",
                time: time,
            },
            success: function (response) {
                var message_info = JSON.parse(response).message;
                var bug_list = message_info.split("|");
                bug_number = bug_list.length;
                console.log(response);

            },
            error: function (response) {
                console.log("API error Response :" + response);
                bug_number = NaN;
            }
        });
        return bug_number;
    }

    _.getLatestAccess = function (id) {
        var last_access_data;
        $.ajax({
            type: 'POST',
            async: false,
            url: "https://dev.allinks.com.au/api/" + check_latest_log_api,
            data: {
                key: "jQTHqBgLA0aNSNoVUbU0NQ",
            },
            success: function (response) {
                last_access_data = JSON.parse(response).message;
            },
            error: function (response) {
                console.log("API error Response :" + response);
            }
        });
        return last_access_data;
    }

    _.addNewApp = function (appInfo) {
        $.ajax({
            type: 'POST',
            async: false,
            url: API_ROOT + add_new_app_api,
            data: {
                key: API_KEY,
                package: appInfo,
            },
            success: function (response) {
                flag = JSON.parse(response).success;
            },
            error: function (response) {
                console.log("API error Response :" + response);
            }
        });
    }

    //get all apps
    _.loadApp = function (filterName, filterValue) {
        var package_info;
        $.ajax({
            type: 'POST',
            async: false,
            url: API_ROOT + get_all_app_api,
            data: {
                key: API_KEY,
                filterName: filterName,
                filterValue: filterValue,
            },
            success: function (response) {
                package_info = JSON.parse(response).package;
                // console.log(package_info);

                // var AppList = JsonStringToObject(list)
                // console.log("get app :" + list[1]);
                // $("#last_access_time").append(`<h4>${last_access_data}</h4>`);

            },
            error: function (response) {
                console.log("API error Response :" + response);
            }
        });
        return package_info;
    }




}