using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Text;
using Darkspede;
using Darkspede.User;
using Amazon.DynamoDBv2.DocumentModel;

//Use securityCode to vlidate user

public partial class OnUserValidatingWithCode : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        StringBuilder response = new StringBuilder();

        string key = Request["key"];
        string package = Request["package"];


        Response.Clear();
        Darkspede_User packageItem = new Darkspede_User();

        // Security Check
        if (!Config.CheckKey(key))
        {
            response.AppendFormat("{{\"success\":\"false\", \"message\":\"{0}\", \"package\":{1}}}", "Api Key Error", Darkspede_User.ToJson(packageItem));
            Response.Write(response.ToString());
            return;
        }

        // Data input Check
        if (package == "" || package == null)
        {
            response.AppendFormat("{{\"success\":\"false\", \"message\":\"{0}\", \"package\":{1}}}", "Incorrect data package", Darkspede_User.ToJson(packageItem));
            Response.Write(response.ToString());
            return;
        }

        //Transform json to string
        packageItem = Darkspede_User.FromJson(package);

        try
        {
            AWSController aws = new AWSController();
            Darkspede_User item = Darkspede_User.FromJson(package);      //Json to string

            List<Document> searchResult = aws.OnDynamoDB_GetAllUserDocuments("mobile", item.mobile);


            //It should return only one result
            if(searchResult.Count != 1 || searchResult == null) 
            {
                response.AppendFormat("{{\"success\":\"false\", \"message\":\"{0}\", \"package\":{1}}}", "No Mobile Number found!", Darkspede_User.ToJson(item));

                // System Log
                SystemLog log = new SystemLog(key, this.GetType().Name, "No mobile number found", "false", Darkspede_User.ToJson(item));
                LogController.WriteToLog(Server.MapPath(Config.Resource_SystemLog), log);
            }
            else
            {
                Darkspede_User foundItem = Darkspede_User.FromJson(searchResult[0].ToJson());   //transfer first item to string
                foundItem.updated = DateTime.Now.Ticks.ToString();

                if (foundItem.securityCode != item.securityCode)
                {

                        response.AppendFormat("{{\"success\":\"false\", \"message\":\"{0}\", \"package\":{1}}}", "Code is not valid", Darkspede_User.ToJson(item));

                        // System Log
                        SystemLog log = new SystemLog(key, this.GetType().Name, "User activate with key, License key is not valid", "false", Darkspede_User.ToJson(item));
                        LogController.WriteToLog(Server.MapPath(Config.Resource_SystemLog), log);
                }
                else
                {
                    if (foundItem.IsCodeExpired())
                    {
                        response.AppendFormat("{{\"success\":\"false\", \"message\":\"{0}\", \"package\":{1}}}", "Code is expired", Darkspede_User.ToJson(item));

                        // System Log
                        SystemLog log = new SystemLog(key, this.GetType().Name, "User activate with key, License key is expired", "false", Darkspede_User.ToJson(item));
                        LogController.WriteToLog(Server.MapPath(Config.Resource_SystemLog), log);
                    }
                    else
                    {
                        foundItem.RenewToken();
                        foundItem.status = "active";
                        aws.OnDynamoDB_ModifyUser(foundItem);

                        response.AppendFormat("{{\"success\":\"true\", \"message\":\"{0}\", \"package\":{1}}}", "Code is valid", Darkspede_User.ToJson(foundItem));

                        // System Log
                        SystemLog log = new SystemLog(key, this.GetType().Name, "user activate with key, license activated", "true", Darkspede_User.ToJson(foundItem));
                        LogController.WriteToLog(Server.MapPath(Config.Resource_SystemLog), log);

                        aws.OnDynamoDB_DeleteUserByGuid(item.guid);
                    }


                }

            }


        }
        catch (Exception ex)
        {
            response.AppendFormat("{{\"success\":\"false\", \"message\":\"{0}\", \"package\":{1}}}", ex.Message, package);

            // System Log
            SystemLog log = new SystemLog(key, this.GetType().Name, "user activate with key", "false", ex.Message);
            LogController.WriteToLog(Server.MapPath(Config.Resource_SystemLog), log);
        }

        Response.Write(response.ToString());
        return;


    }
}
