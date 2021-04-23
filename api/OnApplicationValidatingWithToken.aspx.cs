using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Text;
using Darkspede;

public partial class OnApplicationValidatingWithToken : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        StringBuilder response = new StringBuilder();

        string key = Request["key"];
        string package = Request["package"];


        Response.Clear();
        Darkspede_Application packageItem = new Darkspede_Application();
        // Security Check
        if (!Config.CheckKey(key))
        {
            response.AppendFormat("{{\"success\":\"false\", \"message\":\"{0}\", \"package\":{1}}}", "Api Key Error", Darkspede_Application.ToJson(packageItem));
            Response.Write(response.ToString());
            return;
        }

        // Data input Check
        if (package == "" || package == null)
        {
            response.AppendFormat("{{\"success\":\"false\", \"message\":\"{0}\", \"package\":{1}}}", "date invalid", Darkspede_Application.ToJson(packageItem));
            Response.Write(response.ToString());
            return;
        }

        packageItem = Darkspede_Application.FromJson(package);

        try
        {
   
            AWSController aws = new AWSController();
            Darkspede_Application item = Darkspede_Application.FromJson(package);

            string searchResult = aws.OnDynamoDB_GetApplicationByGuid(item.guid);

            if (searchResult == "none")
            {
                // no item found add new
                // adding modified information
                item.created = DateTime.Now.Ticks.ToString();
                item.updated = DateTime.Now.Ticks.ToString();
                item.appToken = "default";
                item.appKey = "default";
                aws.OnDynamoDB_AddNewApplication(item);
                response.AppendFormat("{{\"success\":\"false\", \"message\":\"{0}\", \"package\":{1}}}", "License key not registed", Darkspede_Application.ToJson(item));

                // System Log
                SystemLog log = new SystemLog(key, this.GetType().Name, "application activate with token, add new key register", "false", Darkspede_Application.ToJson(item));
                LogController.WriteToLog(Server.MapPath(Config.Resource_SystemLog), log);
            }
            else
            {
                Darkspede_Application foundItem = Darkspede_Application.FromJson(searchResult);
                foundItem.updated = DateTime.Now.Ticks.ToString();

                // checking for client

                
                if (item.appIdentifer != foundItem.appIdentifer || item.apiKey != foundItem.apiKey)
                {
                    response.AppendFormat("{{\"success\":\"false\", \"message\":\"{0}\", \"package\":{1}}}", "Incorrect application", Darkspede_Application.ToJson(item));

                    // System Log
                    SystemLog log = new SystemLog(key, this.GetType().Name, "application activate with token, incorrect application", "false", Darkspede_Application.ToJson(item));
                    LogController.WriteToLog(Server.MapPath(Config.Resource_SystemLog), log);
                }
                else
                {

                    if (foundItem.IsCodeExpired())
                    {                        // System Log
                        SystemLog log = new SystemLog(key, this.GetType().Name, "application activate with token, token expired", "false", Darkspede_Application.ToJson(item));
                        LogController.WriteToLog(Server.MapPath(Config.Resource_SystemLog), log);

                        item.appToken = "default";
                        item.appKey = "default";

                        response.AppendFormat("{{\"success\":\"false\", \"message\":\"{0}\", \"package\":{1}}}", "Token expired", Darkspede_Application.ToJson(item));

                    }
                    else if (item.appToken != foundItem.appToken)
                    {
                        // System Log
                        SystemLog log = new SystemLog(key, this.GetType().Name, "application activate with token, token not match", "false", Darkspede_Application.ToJson(item));
                        LogController.WriteToLog(Server.MapPath(Config.Resource_SystemLog), log);

                        item.appToken = "default";
                        item.appKey = "default";

                        response.AppendFormat("{{\"success\":\"false\", \"message\":\"{0}\", \"package\":{1}}}", "Token not match", Darkspede_Application.ToJson(item));

                    }
                    else if (item.appKey != foundItem.appKey)
                    {

                        // System Log
                        SystemLog log = new SystemLog(key, this.GetType().Name, "application activate with token, appKey not match", "false", Darkspede_Application.ToJson(item));
                        LogController.WriteToLog(Server.MapPath(Config.Resource_SystemLog), log);

                        item.appToken = "default";
                        item.appKey = "default";

                        response.AppendFormat("{{\"success\":\"false\", \"message\":\"{0}\", \"package\":{1}}}", "appKey not match", Darkspede_Application.ToJson(item));
                    }
                    else
                    {
                        foundItem.RenewToken();
                        foundItem.accessEndPoint = item.accessEndPoint;
                        aws.OnDynamoDB_ModifyApplication(foundItem);
                        response.AppendFormat("{{\"success\":\"true\", \"message\":\"{0}\", \"package\":{1}}}", "Token updated", Darkspede_Application.ToJson(foundItem));

                        // System Log
                        SystemLog log = new SystemLog(key, this.GetType().Name, "application activate with token, new token updated", "true", Darkspede_Application.ToJson(foundItem));
                        LogController.WriteToLog(Server.MapPath(Config.Resource_SystemLog), log);
                    }

                }
            }

        }
        catch (Exception ex)
        {
            response.AppendFormat("{{\"success\":\"false\", \"message\":\"{0}\", \"package\":{1}}}", ex.Message, package);

            // System Log
            SystemLog log = new SystemLog(key, this.GetType().Name, "application activate with token", "false", ex.Message);
            LogController.WriteToLog(Server.MapPath(Config.Resource_SystemLog), log);
        }

        Response.Write(response.ToString());
        return;


    }
}