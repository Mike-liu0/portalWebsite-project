using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Text;
using Darkspede;
using Amazon.DynamoDBv2.DocumentModel;

public partial class OnApplicationValidatingWithKey : System.Web.UI.Page
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
            response.AppendFormat("{{\"success\":\"false\", \"message\":\"{0}\", \"package\":{1}}}", "Incorrect data package", Darkspede_Application.ToJson(packageItem));
            Response.Write(response.ToString());
            return;
        }

        packageItem = Darkspede_Application.FromJson(package);

        try
        {
            AWSController aws = new AWSController();
            Darkspede_Application item = Darkspede_Application.FromJson(package);

            List<Document> searchResult = aws.OnDynamoDB_GetAllApplicationDocuments("appKey", item.appKey);



            if(searchResult.Count != 1 && searchResult != null) // there should be only one result
            {

                // System Log
                SystemLog log = new SystemLog(key, this.GetType().Name, "application activate with key, key not find", "false", Darkspede_Application.ToJson(item));
                LogController.WriteToLog(Server.MapPath(Config.Resource_SystemLog), log);

                item.appKey = "default";
                response.AppendFormat("{{\"success\":\"false\", \"message\":\"{0}\", \"package\":{1}}}", "License key not registed ["+ searchResult.Count + "]", Darkspede_Application.ToJson(item));

            }
            else
            {
                Darkspede_Application foundItem = Darkspede_Application.FromJson(searchResult[0].ToJson());
                foundItem.updated = DateTime.Now.Ticks.ToString();

                if (item.appIdentifer != foundItem.appIdentifer || item.apiKey != foundItem.apiKey || item.appKey != foundItem.appKey)
                {

                        item.appKey = "default";
                        response.AppendFormat("{{\"success\":\"false\", \"message\":\"{0}\", \"package\":{1}}}", "License key is not valid", Darkspede_Application.ToJson(item));

                        // System Log
                        SystemLog log = new SystemLog(key, this.GetType().Name, "application activate with key, License key is not valid", "false", Darkspede_Application.ToJson(item));
                        LogController.WriteToLog(Server.MapPath(Config.Resource_SystemLog), log);
                }
                else
                {
                    if (foundItem.IsCodeExpired())
                    {

                        item.appKey = "default";
                        response.AppendFormat("{{\"success\":\"false\", \"message\":\"{0}\", \"package\":{1}}}", "License key is expired", Darkspede_Application.ToJson(item));

                        // System Log
                        SystemLog log = new SystemLog(key, this.GetType().Name, "application activate with key, License key is expired", "false", Darkspede_Application.ToJson(item));
                        LogController.WriteToLog(Server.MapPath(Config.Resource_SystemLog), log);
                    }
                    else
                    {
                        foundItem.RenewToken();
                        foundItem.accessEndPoint = item.accessEndPoint;
                        foundItem.appStatus = "active";
                        aws.OnDynamoDB_ModifyApplication(foundItem);

                        response.AppendFormat("{{\"success\":\"true\", \"message\":\"{0}\", \"package\":{1}}}", "License activated", Darkspede_Application.ToJson(foundItem));

                        // System Log
                        SystemLog log = new SystemLog(key, this.GetType().Name, "application activate with key, license activated", "true", Darkspede_Application.ToJson(foundItem));
                        LogController.WriteToLog(Server.MapPath(Config.Resource_SystemLog), log);

                        if (foundItem.guid != item.guid)
                        {
                            //aws.OnDynamoDB_DeleteApplicationByGuid(item.guid);
                        }
                    }


                }

            }


        }
        catch (Exception ex)
        {
            response.AppendFormat("{{\"success\":\"false\", \"message\":\"{0}\", \"package\":{1}}}", ex.Message, package);

            // System Log
            SystemLog log = new SystemLog(key, this.GetType().Name, "application activate with key", "false", ex.Message);
            LogController.WriteToLog(Server.MapPath(Config.Resource_SystemLog), log);
        }

        Response.Write(response.ToString());
        return;


    }
}
