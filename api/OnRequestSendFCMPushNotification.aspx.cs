using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Text;
using Darkspede.Sycamore;


//https://dev.allinks.com.au/api/OnRequestSendPushNotification.aspx?key=jQTHqBgLA0aNSNoVUbU0NQ&targetToken=f9O-sGYn_krutwKdXuQZNR:APA91bEjTTmEbILLnWpDM07f-eErs2py0QG3NXaYYKD06YEUzCUQ9qxwpEzrQIOBA2yDjz2seKxKaU02Ho2kpI79xFuuDHQC_TjBKDo6z_YcjSNCCZdHCF4Ct-FRDctp_fGUvNWsqglU&message=welcome to all link
public partial class OnRequestSendFCMPushNotification : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        StringBuilder response = new StringBuilder();
        string key = Request["key"];
        string targetToken = Request["target"];
        string message = Request["message"];
        //string targetGuid = Request["targetGuid"];
        //string userGuid = Request["userGuid"];

        // Security Check
        if (!Config.CheckKey(key))
        {
            response.AppendFormat("{{\"success\":\"false\", \"message\":\"{0}\", \"package\":{1}}}", "Authentication failed", FCM_SMS.ToJson(new FCM_SMS()));
            Response.Write(response.ToString());
            return;
        }


        string result = "";
        
        try
        {
            FCMController fcm = new FCMController();
            //string token = "f9O-sGYn_krutwKdXuQZNR:APA91bEjTTmEbILLnWpDM07f-eErs2py0QG3NXaYYKD06YEUzCUQ9qxwpEzrQIOBA2yDjz2seKxKaU02Ho2kpI79xFuuDHQC_TjBKDo6z_YcjSNCCZdHCF4Ct-FRDctp_fGUvNWsqglU";
            string token = targetToken;
            string message1 = message;
            result = fcm.OnSendFCMNotifucation(token, message1, "system","dev");

            if (result != "")
            {
                response.AppendFormat("{{\"success\":\"true\", \"message\":\"{0}\", \"package\":\"{1}\"}}", "API.RequestSendSMS Complete", result);

                // System Log
                SystemLog log = new SystemLog(key, "OnRequestSendFCMPushNotification.aspx", "request send FCM push notification", "true");
                LogController.WriteToLog(Server.MapPath(Config.Resource_SystemLog), log);
            }
            else
            {
                response.AppendFormat("{{\"success\":\"false\", \"message\":\"{0}\", \"package\":\"{0}\"}}", "AWS Error: " + result, result);

                // System Log
                SystemLog log = new SystemLog(key, "OnRequestSendFCMPushNotification.aspx", "request send FCM push notification", "false", result);
                LogController.WriteToLog(Server.MapPath(Config.Resource_SystemLog), log);
            }
        }
        catch (Exception ex)
        {
            response.AppendFormat("{{\"success\":\"false\", \"message\":\"{0}\", \"package\":\"{0}\"}}", "API Error: " + ex.Message);

            // System Log
            SystemLog log = new SystemLog(key, "OnRequestSendFCMPushNotification.aspx", "request send FCM push notification", "false", ex.Message);
            LogController.WriteToLog(Server.MapPath(Config.Resource_SystemLog), log);
        }


        
        Response.Write(response.ToString());
        return;
    }
}