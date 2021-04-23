using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Drawing;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using Darkspede;

public partial class OnUploadImage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        StringBuilder response = new StringBuilder();

        string key = Request["key"];
        string itemGuid = Request["itemGuid"];
        string action = Request["action"];
        string payload = Request["payload"];
        string attachmentExtension = Request["attachmentExtension"];
        string attachmentSize = Request["attachmentSize"];


        // more data need

        // Security Check
        if (!Config.CheckKey(key))
        {
            response.AppendFormat("{{\"success\":\"false\", \"message\":\"{0}\", \"package\":\"{0}\"}}", "Authentication fail", "package");
            Response.Write(response.ToString());
            return;
        }

        // Data input Check
        if (payload == "" || payload == null)
        {
            response.AppendFormat("{{\"success\":\"false\", \"message\":\"{0}\", \"package\":\"{0}\"}}", "No data input", "package");
            Response.Write(response.ToString());
            return;
        }


        try
        {
            // Save to file

            // ======== This is not safe way to do it, MARK ====
            string[] list = payload.Split(',');
            string base64 = "";
            if (list.Length == 2)
            {
                base64 = payload.Split(',')[1];
            }
            else
            {
                base64 = payload.Split(',')[0];
            }
            // ======== END =================


            byte[] bytes = Convert.FromBase64String(base64);
            /*
            MemoryStream ms = new MemoryStream(bytes);

            Image newImage = Image.FromStream(ms);

            #region Resize
            float height = newImage.Height;
            float width = newImage.Width;


            float ratio = height / width;
            float newWidth = width;
            float newHeight = height;


            if (width > height)
            {
                if (width > Config.ResizeImageMaxWidth)
                {
                    newWidth = Config.ResizeImageMaxWidth;
                    newHeight = Config.ResizeImageMaxWidth / width * height;
                }
            }
            else
            {
                if (height > Config.ResizeImageMaxHeight)
                {
                    newHeight = Config.ResizeImageMaxHeight;
                    newWidth = Config.ResizeImageMaxHeight / height * width;
                }
            }
            #endregion
            */

            string filePath = "";

            if(action == "post" || action == "postedit")
            {
                filePath = Server.MapPath(Config.Resource_PostImage) + itemGuid;
            }
            else if (action == "user")
            {
                filePath = Server.MapPath(Config.Resource_UserImage) + itemGuid;
            }
            else
            {
                filePath = Server.MapPath(Config.Resource_MiscImage) + itemGuid;
            }

            string filename = "";

            // check folder exsits
            if (!Directory.Exists(filePath))
            {
                DirectoryInfo di = Directory.CreateDirectory(filePath);
            }

            if(attachmentExtension == "")
            {
                attachmentExtension = "jpg";
            }

            if (attachmentExtension.Contains("/"))
            {
                string[] ext = attachmentExtension.Split('/');

                if (ext.Length != 2 || ext[0] != "image")
                {
                    response.AppendFormat("{{\"success\":\"false\", \"message\":\"{0}\", \"package\":\"{1}\"}}", "Not an image", "false");
                    Response.Write(response.ToString());
                    return;
                }

                // Hardcoded extension, not godd
                string fileUrl = filePath + "\\" + itemGuid + "." + ext[1];
                filename = itemGuid + "." + ext[1];
                File.WriteAllBytes(fileUrl, bytes);
            }
            else
            {
                string fileUrl = filePath + "\\" + itemGuid + "." + attachmentExtension.Replace(".","");
                filename = itemGuid + "." + attachmentExtension.Replace(".", "");
                File.WriteAllBytes(fileUrl, bytes);
            }

            response.AppendFormat("{{\"success\":\"true\", \"message\":\"{0}\", \"package\":\"{1}\"}}", "API Complete", filename);

            // System Log
            SystemLog log = new SystemLog(key, "OnUploadImage.aspx", "upload image", "true");
            LogController.WriteToLog(Server.MapPath(Config.Resource_SystemLog), log);
        }
        catch (Exception ex)
        {
            response.AppendFormat("{{\"success\":\"false\", \"message\":\"{0}\", \"package\":\"{1}\"}}", attachmentExtension + ex.Message, "false");

            // System Log
            SystemLog log = new SystemLog(key, "OnUploadImage.aspx", "upload image", "false", ex.Message);
            LogController.WriteToLog(Server.MapPath(Config.Resource_SystemLog), log);
        }

        Response.Write(response.ToString());
        return;
    }
}