/* Delete user by guid */
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Text;
using Darkspede.Sycamore;
using Newtonsoft.Json;
using Darkspede;
using Darkspede.User;
using Darkspede.Player;


public partial class OnGetPlayerPosition : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        StringBuilder response = new StringBuilder();
        string key = Request["key"];
        string playerID = Request["playerID"];
        string session = Request["session"];

        //Darkspede_User package_item = new Darkspede_User();

        APIPlayer package_item = new APIPlayer();
        //PlayerController playerController = new PlayerController();

        // Security Check
        if (!Config.CheckKey(key))
        {
            response.AppendFormat("{{\"success\":\"false\", \"message\":\"{0}\", \"package\":{1}}}",
                "Authentication fail", APIPlayer.ToJson(package_item));
            Response.Write(response.ToString());
            return;
        }

        // Data input Check
        if (playerID == "" || playerID == null || session == ""|| session == null)
        {
            response.AppendFormat("{{\"success\":\"false\", \"message\":\"{0}\", \"package\":{1}}}",
                 "Data input fail", APIPlayer.ToJson(package_item));
            Response.Write(response.ToString());
            return;
        }

        try
        {
            package_item = new PlayerController().FindPlayer(playerID, session);

            if (package_item != null)
            {
                response.AppendFormat("{{\"success\":\"true\", \"message\":\"{0}\", \"package\":{1}}}",
                "Get Player Position Successfully", APIPlayer.ToJson(package_item));
            }
            else
            {
                response.AppendFormat("{{\"success\":\"false\", \"message\":\"{0}\", \"package\":{1}}}",
                "No Item Found", APIPlayer.ToJson(new APIPlayer()));
            }
        }
        catch (Exception ex)
        {
            response.AppendFormat("{{\"success\":\"false\", \"message\":\"{0}\"}}", "API Error: " + ex.Message);
        }

        Response.Write(response.ToString());
        return;
    }
}
