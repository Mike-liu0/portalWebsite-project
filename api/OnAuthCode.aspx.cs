using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Text;
using Darkspede.Sycamore;

public partial class OnAuthCode : System.Web.UI.Page
{
    //http://darkspede.mynetgear.com/api/schiavello/GetAllProjects.aspx

    protected void Page_Load(object sender, EventArgs e)
    {
        StringBuilder response = new StringBuilder();

        string key = Request["key"];
        string mobile = Request["mobile"];


        // Security Check
        if (!Config.CheckKey(key))
        {
            response.AppendFormat("{{\"success\":\"false\", \"message\":\"{0}\", \"package\":{1}}}", "Authentication fail", "NONE");
            Response.Write(response.ToString());
            return;
        }

        // Data input Check
        if (mobile == "")
        {
            response.AppendFormat("{{\"success\":\"false\", \"message\":\"{0}\", \"package\":{1}}}", "filter not found");
            Response.Write(response.ToString());
            return;
        }

        string newMobile = Utility.ValidateMobile(mobile);

        if(newMobile == "")
        {
            response.AppendFormat("{{\"success\":\"false\", \"message\":\"{0}\", \"package\":{1}}}", "Invalid Mobile Format");
            Response.Write(response.ToString());
            return;
        }

        try
        {
            Sycamore_User user;
            AWSController aws = new AWSController();
            string result = aws.OnDynamoDB_GetUserByMobile("mobile", newMobile);

            if (result != "")
            {
                // find exist user
                user = Sycamore_User.FromJson(result);
            }
            else
            {
                // create new user place holder
                user = new Sycamore_User();
                user.mobile = newMobile;
            }

            user.RenewCode();
            aws.OnDynamoDB_UpdateUser(user);


            // Sending Code

            string security_codeString = user.securityCode + Config.AUTH_CODE_SURFIX;
            string TargetMobile;

            if (!newMobile.StartsWith("+"))
            {
                TargetMobile = "+" + newMobile;
            }
            else
            {
                TargetMobile = newMobile;
            }

            aws.OnLambda_RequestSendSMS(security_codeString, TargetMobile);

            response.AppendFormat("{{\"success\":\"true\", \"message\":\"{0}\", \"package\":\"{1}\"}}", "Code Sent", user.securityCode);

            // System Log
            SystemLog log = new SystemLog(key, "OnUserRequestAuthCode.aspx", "user request authorize code", "true");
            LogController.WriteToLog(Server.MapPath(Config.Resource_SystemLog), log);

        }
        catch (Exception ex)
        {
            response.AppendFormat("{{\"success\":\"false\", \"message\":\"{0}\", \"package\":\"{1}\"}}", "API Error: ", ex.Message);

            // System Log
            SystemLog log = new SystemLog(key, "OnUserRequestAuthCode.aspx", "user request authorize code", "false", ex.Message);
            LogController.WriteToLog(Server.MapPath(Config.Resource_SystemLog), log);
        }

        Response.Write(response.ToString());
        return;


    }
}