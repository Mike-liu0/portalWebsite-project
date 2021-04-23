/*Model for application user*/
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Darkspede.User
{
    [Serializable]
    public class Darkspede_User
    {
        public string guid = Guid.NewGuid().ToString();
        public string created = DateTime.Now.Ticks.ToString();
        public string updated = DateTime.Now.Ticks.ToString();
        public string status = Config.DevelopmentStage;

        //Important info
        public string mobile = "614000000";
        public string accesstoken = Guid.NewGuid().ToString();
        public string tokenExpire = DateTime.Now.Ticks.ToString();

        //Security Code
        public string securityCode = Guid.NewGuid().ToString();
        public string codeExpire = DateTime.Now.Ticks.ToString();

        //Update later basic info about user
        public string username = "N";
        public string fullName = "Default User";
        public string email = "N";
        public string password = "N"; //stored it as SHA
        public string location = "Melbourne";
        public string companyName = "N";
        public string companyType = "Not Stated";

        public string permission = "C1";
        public string iconUrl = "default";
        public string fcmToken = "Not provided";

        //Constructor
        public Darkspede_User()
        {

        }

        public string RenewCode()
        {
            if (!Config.CheckUserException(mobile))  //Send security code to user
            {
                securityCode = Utility.GenerateSecurityCode();
            }

            //Update code expire date
            codeExpire = DateTime.Now.AddMinutes(Config.CodeExpire).Ticks.ToString(); //Add new expire date by minutes
            return securityCode;
        }

        //Is current code expired?
        public bool IsCodeExpired()
        {
            if(DateTime.Now.Ticks < long.Parse(codeExpire))
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        //return a new token
        public string RenewToken()
        {
            accesstoken = Guid.NewGuid().ToString();
            tokenExpire = DateTime.Now.AddDays(Config.TokenExpire).Ticks.ToString(); //Add new expire date by days
            return accesstoken;
        }

        //Is current token expired?
        public bool IsTokenExpired()
        {
            if(DateTime.Now.Ticks < long.Parse(tokenExpire))
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        public static string ToJson(Darkspede_User _item)
        {
            return JsonConvert.SerializeObject(_item);
        }

        public static Darkspede_User FromJson(string _json)
        {
            return JsonConvert.DeserializeObject<Darkspede_User>(_json);
        }


    }
}
