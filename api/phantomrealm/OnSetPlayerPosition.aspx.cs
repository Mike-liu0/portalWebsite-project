using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Text;
using Darkspede;
using Darkspede.User;
using Darkspede.Player;


public partial class OnSetPlayerPosition : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        StringBuilder response = new StringBuilder();
        string key = Request["key"];
        string package = Request["package"];

        //Darkspede_User package_item = new Darkspede_User();

        APIPlayer package_item = new APIPlayer();
        PlayerController playerController = new PlayerController();

        // Security Check
        if (!Config.CheckKey(key))
        {
            response.AppendFormat("{{\"success\":\"false\", \"message\":\"{0}\", \"package\":{1}}}",
                "Authentication fail", APIPlayer.ToJson(new APIPlayer()));
            Response.Write(response.ToString());
            return;
        }

        // Data input Check
        if (package == "" || package == null)
        {
            response.AppendFormat("{{\"success\":\"false\", \"message\":\"{0}\", \"package\":{1}}}",
                 "Data input false", APIPlayer.ToJson(new APIPlayer()));
            Response.Write(response.ToString());
            return;
        }

        try
        {

            // Parse json package
            APIPlayer newPlayer = APIPlayer.FromJson(package);


            // Set Item
            package_item = playerController.SetPlayer(newPlayer);

            if(package_item == null)
            {
                response.AppendFormat("{{\"success\":\"false\", \"message\":\"{0}\", \"package\":{1}}}",
                    "Player not found", APIPlayer.ToJson(new APIPlayer()));
            }
            else
            {
                response.AppendFormat("{{\"success\":\"true\", \"message\":\"{0}\", \"package\":{1}}}",
                    "Set Player Done", APIPlayer.ToJson(package_item));
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
