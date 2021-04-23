using Darkspede.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for DataSet
/// </summary>
public class DataSet
{


    public static List<APIPlayer> PlayerList = new List<APIPlayer>()
    {
        new APIPlayer("1", "0"),
        new APIPlayer("2", "0"),  // used for general mockup

        new APIPlayer("1", "1"),
        new APIPlayer("2", "1"),  // used on wac fy

        new APIPlayer("1", "2"),
        new APIPlayer("2", "2"),  // used on clinic demo

        new APIPlayer("1", "3"),
        new APIPlayer("2", "3"),  // reservied for, well, nothing
    };




}
