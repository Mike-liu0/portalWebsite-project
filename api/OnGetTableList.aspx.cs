using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Text;
using Darkspede.Sycamore;


//http://sycamore.darkspede.space/api/GetTableList.aspx?key=jQTHqBgLA0aNSNoVUbU0NQ
public partial class GetTableList : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        StringBuilder response = new StringBuilder();

        string key = Request["key"];




        // Security Check
        if (!Config.CheckKey(key))
        {
            response.AppendFormat("{{\"success\":\"false\", \"message\":\"{0}\"",
                "Authentication fail");
            Response.Write(response.ToString());
            return;
        }

        try
        {
            //the result is a list for object jsons
            string result = new AWSController().GetTableList();

            response.AppendFormat("{{\"success\":\"true\", \"message\":\"{0}\", \"package\":\"{1}\"}}",
                     "API.GetTableList Complete", result);

            // System Log
            SystemLog log = new SystemLog(key, "GetTableList.aspx", "Get table list", "true");
            LogController.WriteToLog(Server.MapPath(Config.Resource_SystemLog), log);

        }
        catch (Exception ex)
        {
            response.AppendFormat("{{\"success\":\"false\", \"message\":\"{0}\"}}", "API Error: " + ex.Message);

            // System Log
            SystemLog log = new SystemLog(key, "GetTableList.aspx", "Get table list", "false", ex.Message);
            LogController.WriteToLog(Server.MapPath(Config.Resource_SystemLog), log);
        }

        Response.Write(response.ToString());
        return;


    }
}