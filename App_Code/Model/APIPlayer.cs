using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;


namespace Darkspede.Player
{
    [Serializable]
    public class APIPlayer
    {
        public string playerID = "0";
        public string session = "0";

        public string created = DateTime.Now.Ticks.ToString();
        public string updated = DateTime.Now.AddDays(-1).Ticks.ToString();  // move back a day to deactivate the object

        public float positionX;
        public float positionY;
        public float positionZ;
        public float rotationX;
        public float rotationY;
        public float rotationZ;

        public string status = "active";

        public APIPlayer(string ID, string SessionId)
        {
            this.playerID = ID;
            this.session = SessionId;
        }

        public APIPlayer(){}

        public static string ToJson(APIPlayer _item)
        {
            return JsonConvert.SerializeObject(_item);
        }

        public static APIPlayer FromJson(string _json)
        {
            return JsonConvert.DeserializeObject<APIPlayer>(_json);
        }
    }
}
