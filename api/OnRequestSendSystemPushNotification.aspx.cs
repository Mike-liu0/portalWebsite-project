using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Text;
using Darkspede.Sycamore;


//https://dev.allinks.com.au/api/OnRequestSendSystemPushNotification.aspx?key=jQTHqBgLA0aNSNoVUbU0NQ&message=f9O-sGYn_krutwKdXuQZNR:APA91bEjTTmEbILLnWpDM07f-eErs2py0QG3NXaYYKD06YEUzCUQ9qxwpEzrQIOBA2yDjz2seKxKaU02Ho2kpI79xFuuDHQC_TjBKDo6z_YcjSNCCZdHCF4Ct-FRDctp_fGUvNWsqglU&message=welcome to all link
public partial class OnRequestSendSystemPushNotification : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        StringBuilder response = new StringBuilder();
        string key = Request["key"];
        string message = Request["message"];
        string group = Request["group"];



        // Security Check
        if (!Config.CheckKey(key))
        {
            response.AppendFormat("{{\"success\":\"false\", \"message\":\"{0}\", \"package\":{1}}}", "Authentication failed", FCM_SMS.ToJson(new FCM_SMS()));
            Response.Write(response.ToString());
            return;
        }


        // Data input Check
        if (message == "" || message == null || group == "" || group == null)
        {
            response.AppendFormat("{{\"success\":\"false\", \"message\":\"{0}\", \"package\":{1}}}",
                "Data input fail", Sycamore_Request.ToJson(new Sycamore_Request()));
            Response.Write(response.ToString());
            return;
        }

        string result = "";
        
        try
        {

            string userResult = new AWSController().OnDynamoDB_GetAllUsers("group", group);


            FCMController fcm = new FCMController();
       
            string message1 = "Welcome to Allink";
            result = fcm.OnSendFCMNotifucation_System(message1);

            if (result != "")
            {
                response.AppendFormat("{{\"success\":\"true\", \"message\":\"{0}\", \"package\":\"{1}\"}}", "API.RequestSendSMS Complete", "NONE");

                // System Log
                SystemLog log = new SystemLog(key, "OnRequestSendSystemPushNotification.aspx", "request send system push notification", "true");
                LogController.WriteToLog(Server.MapPath(Config.Resource_SystemLog), log);
            }
            else
            {
                response.AppendFormat("{{\"success\":\"false\", \"message\":\"{0}\", \"package\":\"{1}\"}}", "AWS Error: " + result, "NONE");

                // System Log
                SystemLog log = new SystemLog(key, "OnRequestSendSystemPushNotification.aspx", "request send system push notification", "false", result);
                LogController.WriteToLog(Server.MapPath(Config.Resource_SystemLog), log);
            }
        }
        catch (Exception ex)
        {
            response.AppendFormat("{{\"success\":\"false\", \"message\":\"{0}\", \"package\":\"{1}\"}}", "API Error: " + ex.Message, "NONE");

            // System Log
            SystemLog log = new SystemLog(key, "OnRequestSendSystemPushNotification.aspx", "request send system push notification", "false", ex.Message);
            LogController.WriteToLog(Server.MapPath(Config.Resource_SystemLog), log);
        }


        
        Response.Write(response.ToString());
        return;
    }
}