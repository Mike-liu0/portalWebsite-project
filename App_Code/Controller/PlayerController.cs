using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Darkspede.Player;


public class PlayerController
{
    public PlayerController()
    {

    }


    public void AddNewPlayer(APIPlayer player)
    {
        DataSet.PlayerList.Add(player);
    }

    public APIPlayer FindPlayer(string id, string sessionId)
    {
        return DataSet.PlayerList.Find(item => item.playerID == id && item.session == sessionId);
    }

    public APIPlayer SetPlayer(APIPlayer newPlayer)
    {
        APIPlayer result = FindPlayer(newPlayer.playerID, newPlayer.session);


        if (result != null)
        {
            newPlayer.updated = DateTime.Now.Ticks.ToString();

            DataSet.PlayerList[DataSet.PlayerList.IndexOf(result)] =
                APIPlayer.FromJson(APIPlayer.ToJson(newPlayer));
        }

        return result;
    }
}

