using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;



namespace Darkspede
{
    [Serializable]
    public class Darkspede_Application
    {
        public string guid = Guid.NewGuid().ToString();
        public string created = DateTime.Now.Ticks.ToString();
        public string updated = DateTime.Now.Ticks.ToString();
        public string status = Config.DevelopmentStage;       // dev/production/pending/testing/other

        // Application Information
        public string appName = "default";
        public string appDescription = "default";
        public string appPlatform = "default";
        public string appIdentifer = "default";
        public string appDomain = "default";

        // Developer Detial
        public string developerName = "default";
        public string developerUrl = "default";
        public string developerEmail = "default";

        // System Status
        public string developmentGroup = "dev";                    // dev/testing/release
        public string version = "1.0";
        public string updateMessage = "default";
        public string redirectUrl = "default";


        // Application Access detail
        public string apiKey = "default";
        public string appKey = "default";                       // jQTHqBgLA0aNSNoVUbU0NQ
        public string appToken = "default";
        public string appStatus = "active";
        public string appTokenExpire = DateTime.Now.Ticks.ToString(); // time Tick
        public string appNextCheck = DateTime.Now.AddDays(Config.NextLicenseCheckInterval).Ticks.ToString(); // time Tick

        // Publisher information
        public string publisherName = "default";     // agent
        public string publishUrl = "default";
        public string publishEmail = "default";

        // Client Information
        public string clientName = "default";
        public string clientID = "default";
        public string clientPostcode = "default";
        public string clientEmail = "default";

        // Extra Information
        public string accessEndPoint = "default";
        public string attachedUser = "default";

        public Darkspede_Application()
        {

        }

        public static string ToJson(Darkspede_Application _item)
        {
            return JsonConvert.SerializeObject(_item);
        }

        public static Darkspede_Application FromJson(string _json)
        {
            return JsonConvert.DeserializeObject<Darkspede_Application>(_json);
        }



        public bool IsCodeExpired()
        {
            if (DateTime.Compare(DateTime.Now, new DateTime(long.Parse(appTokenExpire))) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string RenewToken()
        {
            appToken = Guid.NewGuid().ToString();
            //appTokenExpire = DateTime.Now.AddDays(Config.TokenExpire).Ticks.ToString();
            return appToken;
        }

    }
}