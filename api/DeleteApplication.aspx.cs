﻿using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Text;
using Darkspede.Sycamore;
using Newtonsoft.Json;
using Darkspede;

public partial class DeleteApplication : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        StringBuilder response = new StringBuilder();
        string key = Request["key"];
        string guid = Request["guid"];

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
        if (guid == "" || guid == null)
        {
            response.AppendFormat("{{\"success\":\"false\", \"message\":\"{0}\", \"package\":{1}}}",
                 "Data input fail", Darkspede_Application.ToJson(package_item));
            Response.Write(response.ToString());
            return;
        }


        try
        {
            string result = new AWSController().OnDynamoDB_DeleteApplicationByGuid(guid);

            if (result == "success")
            {
                response.AppendFormat("{{\"success\":\"true\", \"message\":\"{0}\", \"package\":{1}}}",
                    "API.RemoveEarthworkByGuid  Complete", Darkspede_Application.ToJson(package_item));

                // System Log
                SystemLog log = new SystemLog(key, this.GetType().Name, "AWS delete item", "true");
                LogController.WriteToLog(Server.MapPath(Config.Resource_SystemLog), log);
            }
            else
            {
                response.AppendFormat("{{\"success\":\"false\", \"message\":\"{0}\", \"package\":{1}}}",
                    "Database Error: " + result, Darkspede_Application.ToJson(package_item));

                // System Log
                SystemLog log = new SystemLog(key, this.GetType().Name, "AWS delete item", "false", result);
                LogController.WriteToLog(Server.MapPath(Config.Resource_SystemLog), log);
            }


            // update the rest

        }
        catch (Exception ex)
        {
            response.AppendFormat("{{\"success\":\"false\", \"message\":\"{0}\"}}", "API Error: " + ex.Message);

            // System Log
            SystemLog log = new SystemLog(key, this.GetType().Name, "AWS delete item", "false", ex.Message);
            LogController.WriteToLog(Server.MapPath(Config.Resource_SystemLog), log);
        }

        Response.Write(response.ToString());
        return;
    }
}