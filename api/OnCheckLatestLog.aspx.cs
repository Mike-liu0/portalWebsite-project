using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Text;
using Darkspede.Sycamore;


//https://dev.allinks.com.au/api/OnRequestContact.aspx?key=jQTHqBgLA0aNSNoVUbU0NQ&target=61433280815
public partial class OnCheckLatestLog : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        StringBuilder response = new StringBuilder();
        string key = Request["key"];

        // Security Check
        if (!Config.CheckKey(key))
        {
            response.AppendFormat("{{\"success\":\"false\", \"message\":\"{0}\"}}", "Authentication failed");
            Response.Write(response.ToString());
            return;
        }

        try
        {
            string result = new LogController().CheckLatestLog(Server.MapPath(Config.Resource_SystemLog));

            if (result != "none")
            {

                response.AppendFormat("{{\"success\":\"true\", \"message\":\"{0}\"}}", result);

                // System Log
                //SystemLog log = new SystemLog(key, "OnCheckLatestLog.aspx", "check latest access", "true", result);
                //LogController.WriteToLog(Server.MapPath(Config.Resource_SystemLog), log);
            }
            else
            {
                response.AppendFormat("{{\"success\":\"false\", \"message\":\"{0}\"}}", "AWS Error: Unknow Error");

                // System Log
                //SystemLog log = new SystemLog(key, "OnCheckLatestLog.aspx", "check latest access", "false", result);
                //LogController.WriteToLog(Server.MapPath(Config.Resource_SystemLog), log);
            }

        }
        catch (Exception ex)
        {
            response.AppendFormat("{{\"success\":\"false\", \"message\":\"{0}\"}}", "API Error: " + ex.Message);

            // System Log
            //SystemLog log = new SystemLog(key, "OnCheckLatestLog.aspx", "check latest bugsaccess "false", ex.Message);
            //LogController.WriteToLog(Server.MapPath(Config.Resource_SystemLog), log);
        }

        Response.Write(response.ToString());
        return;
    }
}