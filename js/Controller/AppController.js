function AppController() {
    var _ = this;
    var proxy = new DataProxy();
    var event = new UIEvent();
    var mobile;
    var code;
    var isLogin = false;
    var accesstoken;
    var savedToken;
    var AppNumber;

    _.INIT = function () {
        _.event = event;
        _.event.parent = _;
        _.proxy = proxy;
        _.proxy.parent = _;
        _.OnRequestByLocalToken();
        _.event.bindUIEvent();
        isLogin = false;
        //_.loadAppInfo();
        // _.generateCard();
    }



    // login function part
    //login with local token automatically
    _.OnRequestByLocalToken = function () {
        var localToken = localStorage.getItem("savedToken");
        debug("localtoken :" + localToken);
        if (localToken != "null") {
            console.log("check token");
            var LoginByToken = _.proxy.OnRequestLoginByToken(localToken);
            _.isLogin = LoginByToken.isLogin;
            _.savedToken = LoginByToken.savedToken;
            _.guid = LoginByToken.guid;
            _.fullName = LoginByToken.fullName;
            debug("tokenaccess-login:" + _.isLogin);
            _.CheckLogin();
        }
        // console.log(_.isLogin);

    }


    _.OnRequestAuthCodeByMobile = function () {
        clearNoteOnLogin();
        mobile = document.getElementById("Phone_Number").value;
        mobile = UconvertPhoneNumber(mobile);

        if (mobile != "wrongNum") {
            addGreenNoteOnLogin("The code message is sent to this mobile");
            clearNoteOnLoginIn10S();
            _.CountDown();
            _.accesstoken = _.proxy.OnRequestAuthCodeByMobile(mobile);
        }
    }

    _.OnRequestLoginByMobile = function () {
        clearNoteOnLogin();
        mobile = document.getElementById("Phone_Number").value;
        mobile = UconvertPhoneNumber(mobile);
        code = document.getElementById("SMS_Code").value;
        var LoginByMobile = _.proxy.OnRequestLoginByMobile(mobile, code, _.accesstoken);
        console.log(LoginByMobile.isLogin);
        _.isLogin = LoginByMobile.isLogin === 'true';
        _.guid = LoginByMobile.guid;
        console.log(_.isLogin);
        if (!(_.isLogin)) {
            addRedNoteOnLogin("Mobile and Security Code do not match!Please insert right number!");
            // addRedNoteOnLogin(LoginByMobile.info);
            redBorderOnMobileInput();
            redBorderOnCodeInput();
            clearNoteOnLoginIn10S();
        } else {
            _.savedToken = LoginByMobile.savedToken;
            _.CheckLogin();
        }

    }

    _.CheckLogin = function () {
        // _.isLogin = _.isLogin === true;
        // console.log(_.isLogin);
        if (_.isLogin) {
            _.event.hideLoginPage();
            debug("save to local" + _.savedToken);
            localStorage.setItem('savedToken', _.savedToken);
            _.checkInfoForm();
        } else {
            _.event.showLoginPage();
            localStorage.setItem("savedToken", null);

        }
    }

    //logout part
    _.Logout = function () {
        _.isLogin = false;
        $("#logout-confirm").attr('style', 'display:none');
        $("#logout-bg").attr('style', 'display:none');
        code = null;
        document.getElementById("SMS_Code").value = '';
        localStorage.setItem("savedToken", null);
        _.CheckLogin();
    }
    //double check logout
    _.CheckLogout = function () {
        $("#logout-bg").attr('style', 'display:block');
        $("#logout-confirm").attr('style', 'display:block');
    }
    _.closeLogout = function () {
        $("#logout-bg").attr('style', 'display:none');
        $("#logout-confirm").attr('style', 'display:none');

    }


    // App info part
    _.loadAppInfo = function () {
        // AppNumber = 4;
        _.getBugs();
        _.getLastestAccess();
    }

    _.getBugs = function (id) {
        var TodayBugsNumber = _.proxy.getBugs(id, 'today');
        $("#bug_number_today").append(`${TodayBugsNumber}`);
        var ThisWeekBugsNumber = _.proxy.getBugs(id, 'a week');
        $("#bug_number_thisweek").append(`${ThisWeekBugsNumber}`);
        var ThisMonthBugsNumber = _.proxy.getBugs(id, 'a month');
        $("#bug_number_thismonth").append(`${ThisMonthBugsNumber}`);

    }

    _.getLastestAccess = function (id) {
        var last_access_data = _.proxy.getLatestAccess(id);
        $("#last_access_time").append(`<h4>${last_access_data}</h4>`);
    }

    _.showAddNewAppPage = function () {
        $("#AddNewApp").attr('style', 'display:block');
    }

    _.AddNewApp = function () {
        var Name = document.getElementById("AppName").value;
        var JSONOutput = ObjectToJsonString({ appName: Name });
        _.proxy.addNewApp(JSONOutput);
        $("#AddNewApp").attr('style', 'display:none');
        _.generateCard();
    }

    _.CancelAddNewApp = function () {
        $("#AddNewApp").attr('style', 'display:none');
    }

    _.CountDown = function () {
        var timeCount = Time_Count;
        $("#SMS_btn").attr('disabled', 'disabled');
        $("#SMS_btn").attr('cursor', 'default');
        $("#SMS_btn").attr('background-color', 'gray');
        var timeStop = setInterval(function () {
            timeCount--;
            if (timeCount > 0) {
                $("#SMS_btn").text('Please resend in ' + timeCount + ' s');
            } else {
                timeCount = 0;
                $("#SMS_btn").text('Get SMS Code');
                clearInterval(timeStop);
                $("#SMS_btn").removeAttr('disabled');
            }
            localStorage.setItem("timestop", timeCount);
        }, 1000);
    }

    _.onChcekTimeStop = function () {
        var timeCount = localStorage.getItem("timestop");
        if (timeCount > 0) {
            $("#SMS_btn").attr('cursor', 'default');
            $("#SMS_btn").attr('background-color', 'gray');
            var timeStop = setInterval(function () {
                timeCount--;
                if (timeCount > 0) {
                    $("#SMS_btn").text('Please resend in ' + timeCount + ' s');
                    $("#SMS_btn").attr('disabled', 'disabled');
                    $("#SMS_btn").css('cursor', 'default');
                } else {
                    timeCount = 0;
                    $("#SMS_btn").text('Get SMS Code');
                    clearInterval(timeStop);
                    $("#SMS_btn").removeAttr('disabled');
                }
                localStorage.setItem("timestop", timeCount);
            }, 1000);
        }
    }


    _.generateCard = function () {
        _.loadAppInfo();
        var appList = _.proxy.loadApp("status", "dev");
        debug(appList);
        var appNum = appList.length;
        debug(appNum);
        for (ele of appList) {
            debug("add one app");
            $("#AppList").append(`
            <div class="col-xl-3 col-lg-6">
                <section class="card">
                    <div class="twt-feed blue-bg">
                        <div class="corner-ribon black-ribon">
                            <i class="fa"></i>
                        </div>
                        <!-- <div class="fa fa-twitter wtt-mark"></div> -->

                        <div class="media">
                            <a href="#">
                                <img class="align-self-center rounded-circle mr-3" style="width:80px; height:80px;" alt=""
                                    src="resources\logo\allink.png">
                            </a>
                            <div class="media-body">
                                <h2 class="text-white display-6">${ele.appName}</h2>
                                <p class="text-light">Last Access: </p>
                                <p id="last_access_time" class="text-light"> </p>
                            </div>
                        </div>
                    </div>
                    <div class="weather-category twt-category">
                        <ul>
                            <li class="active">
                                <h5 id="bug_number_today"></h5>
                                Bugs Today
                            </li>
                            <li>
                                <h5 id="bug_number_thisweek"></h5>
                                Bugs This Week
                            </li>
                            <li>
                                <h5 id="bug_number_thismonth"></h5>
                                Bugs This Month
                            </li>
                        </ul>
                    </div>

                    <div class="card-function">
                        <div class="fa fa-refresh functionIcon" style="width: 40px;"></div>
                        <div class="fa fa-wrench functionIcon" style="width: 50px;"></div>
                    </div>

                </section>
            </div>
       `);

        }
    }


    _.checkInfoForm = function () {
        console.log("guid:" + _.guid);
        console.log("Name:" + _.fullName);
    }

    _.ModifyUserInfo = function () {
        $("#form_page").attr('style', 'display:none');
    }
}