using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Text;
using Darkspede;

// https://portal.darkspede.space/api/AddNewApplication.aspx?key=aef14f62-9eb8-4c02-b642-e5faafe30f95

public partial class AddNewApplication : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        StringBuilder response = new StringBuilder();
        string key = Request["key"];
        string package = Request["package"];

        Darkspede_Application package_item = new Darkspede_Application();

        // Security Check
        if (!Config.CheckKey(key))
        {
            response.AppendFormat("{{\"success\":\"false\", \"message\":\"{0}\", \"package\":{1}}}", 
                "Authentication fail", Darkspede_Application.ToJson(package_item));
            Response.Write(response.ToString());
            return;
        }
      

        // Data input Check
        if (package == "" || package == null)
        {
            response.AppendFormat("{{\"success\":\"false\", \"message\":\"{0}\", \"package\":{1}}}", 
                "Data input fail", Darkspede_Application.ToJson(package_item));
            Response.Write(response.ToString());
            return;
        }

        // TODO
        // Checking for permission


        try
        {
            // remove excape from json encoder (unity JsonUtility)
            package.Replace("\\", "");

            Darkspede_Application item = Darkspede_Application.FromJson(package);

            string result = new AWSController().OnDynamoDB_AddNewApplication(item);
            Guid guidResult;
            bool isValid = Guid.TryParse(result, out guidResult);

            if (isValid)
            {
                response.AppendFormat("{{\"success\":\"true\", \"message\":\"{0}\", \"package\":{1}}}",
                    "AddNewApplication Complete", Darkspede_Application.ToJson(item));

                // System Log
                SystemLog log = new SystemLog(key, this.GetType().Name, "AWS add item", "true");
                LogController.WriteToLog(Server.MapPath(Config.Resource_SystemLog), log);
            }
            else
            {
                response.AppendFormat("{{\"success\":\"false\", \"message\":\"{0}\", \"package\":{1}}}",
                    "Database Error: " + result, Darkspede_Application.ToJson(package_item));

                // System Log
                SystemLog log = new SystemLog(key, this.GetType().Name, "AWS add item", "false", result);
                LogController.WriteToLog(Server.MapPath(Config.Resource_SystemLog), log);
            }
        }
        catch (Exception ex)
        {
            response.AppendFormat("{{\"success\":\"false\", \"message\":\"{0}\", \"package\":{1}}}", 
                "API Error: " + ex.Message, Darkspede_Application.ToJson(package_item));

            // System Log
            SystemLog log = new SystemLog(key, this.GetType().Name, "AWS add item", "false", ex.Message);
            LogController.WriteToLog(Server.MapPath(Config.Resource_SystemLog), log);
        }

        Response.Write(response.ToString());
        return;
    }
}
