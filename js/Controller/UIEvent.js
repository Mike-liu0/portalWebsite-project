function UIEvent() {
    var _ = this;
    var parent;
    var leftSecond;

    _.bindUIEvent = function () {
        _.parent.onChcekTimeStop();
        // _.manageButtons();

        $("#SMS_btn").bind("click", _.parent.OnRequestAuthCodeByMobile);
        $("#login_btn").bind("click", _.parent.OnRequestLoginByMobile);
        $("#form_btn").bind("click", _.parent.ModifyUserInfo);
        $("#logout_btn").bind("click", _.parent.CheckLogout);
        $("#confirmLogout_btn").bind("click", _.parent.Logout);
        $("#closeLogout").bind("click", _.parent.closeLogout);
        $("#AddNewApp_btn").bind("click", _.parent.showAddNewAppPage);
        $("#AddApp_sub_btn").bind("click", _.parent.AddNewApp);
        $("#AddApp_can_btn").bind("click", _.parent.CancelAddNewApp);



    }

    _.hideLoginPage = function () {
        $("#login_page").attr('style', 'display:none');
    }

    _.showLoginPage = function () {
        $("#login_page").attr('style', 'display:block');
    }
    _.manageButtons = function () {
        if (document.getElementById("Phone_Number").value == null) {
            $("#SMS_btn").attr('disabled', 'disabled');
            $("#login_btn").attr('disabled', 'disabled');
        }

        if (document.getElementById("Phone_Number").value != null) {
            $("#SMS_btn").removeAttr('disabled', 'disabled');
        }

        if (document.getElementById("SMS_Code").value == null) {
            $("#login_btn").attr('disabled', 'disabled');
        }

        if (document.getElementById("SMS_Code").value != null) {
            $("#login_btn").removeAttr('disabled', 'disabled');
        }
    }



}