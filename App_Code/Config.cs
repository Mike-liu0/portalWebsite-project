#define DEV // Remove this for production server

using Amazon.DynamoDBv2.Model;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Config
/// </summary>
public class Config
{
    public static string ProductName = "Darkspede Portal";
#if (!DEV)
    public static bool IsDebugEnabled = true;
    public static string Site = "";
    public static string AWS_IdentityPoolId = "ap-southeast-2:c3475673-cbd0-4902-b7ce-298d750e229a";  // IdentityPool Id From darkspede.api@gmail.com
    public static string DevelopmentStage = "production";   // change to production when deploy
    public static string DevelopmentGroup = "production";   // change to admin when deploy

#else
    // Service production
    public static bool IsDebugEnabled = false;
    public static string Site = "Dev_";// currently the dev site is the production site for this project
    public static string AWS_IdentityPoolId = "ap-southeast-2:c3475673-cbd0-4902-b7ce-298d750e229a";  // IdentityPool Id From darkspede.api@gmail.com
    public static string DevelopmentStage = "dev";  
    public static string DevelopmentGroup = "dev";
#endif

    // Table list of current design, add new table in this list only
    public static Dictionary<string, string> TableList = new Dictionary<string, string>()
    {
        { "Darkspede_Application","Darkspede_Application" },
        { "Darkspede_User","Darkspede_User" },

    };

    public static string securityCode = "********"; //Hide securityCode
    public static string smsLink = "https://dev.allinks.com.au/api/OnRequestSendSMS.aspx?key=jQTHqBgLA0aNSNoVUbU0NQ&target="; //Send sms link

    public static string AUTH_CODE_SURFIX = " is your Darkspede Portal security code. it expires in 10 minutes. DO NOT share this code with anyone.";
    public static string REQUEST_PREFIX = "Darkspede User [";
    public static string REQUEST_SURFIX = "] is agreed to contact with you via mobile number -  ";

    public static int TokenExpire = 5; // days, added when renew token
    public static int CodeExpire = 10; // minutes, added when renew code
    public static string Resource_PostImage = "/resource/post/";
    public static string Resource_UserImage = "/resource/user/";
    public static string Resource_MiscImage = "/resource/misc/";
    public static string Resource_SystemLog = "/resource/systemLog/";


    public static string API_MapboxGeoCoding = "https://api.mapbox.com/geocoding/v5/mapbox.places/";
    public static string API_MapboxAccessToken = "pk.eyJ1IjoieXVoZTA5MjUiLCJhIjoiY2tkbnJobWM2MHY2ZTJycWltNG1lbm5xNyJ9.OPHhWDcFai-mdAqNLzhATA";
    public static string API_GoogleGeoCoding = "https://maps.googleapis.com/maps/api/place/textsearch/json?query=";
    public static string API_GoogleAPIKEY = "AIzaSyAqtQk-7uH6izNU-mC3zby9nGvrHxMDjuI"; // using sycamore.master@gmail.com

    public static string API_FCM_API = "https://fcm.googleapis.com/fcm/send";
    public static string API_FCM_AuthorizationKey = "AAAAdVWtPJs:APA91bG94plrR3q76vnz-fDhnQh35bbVPUgSTn0ckrDWQSwQUBZLFNKV17QECbGTXuLOfTQVzh7OBcTatyS2q_kOy4mfnfdtXbPuqztLTBmhogt17YZURJQVXsroYzOXVMrRzAzPDuCz";
    public static string API_FCM_SenderID = "503948590235";

    public static int ResizeImageMaxWidth = 1024;
    public static int ResizeImageMaxHeight = 1024;

    public static string LogFileNameDateFormat = "yyyyMMdd";
    public static string LogDateTimeFormat = "yyyy-MM-dd HH-mm-ss";
    public static int OneSecondTick = 10000000;  // 10 million ticks is a second


    // Authentication
    public static int NextLicenseCheckInterval = 7; // In days

    public Config()
    {
        //
        // TODO: Add constructor logic here
        //
    }




    // HardCoded Key, only for dev

    private static List<string> SecureKeyList = new List<string>()
    {
        "1b949f90-90d1-4310-85b4-67177bc0c6f5",  // BigBox Internal
        "d1b88c27-36ca-4cbf-abf8-a830d7f1f968",  // BigBox External
        "aef14f62-9eb8-4c02-b642-e5faafe30f95",  // BigBox Developer
        "585a01a5-4824-4f37-9624-75efd23b8d65",
        "fa9ead86-4e73-4211-b972-834fcca8fb76",
        "d1106961-6c9d-4150-9452-dbc09f2c2117"
    };

    private static Dictionary<string, string> ExceptionUserGroup = new Dictionary<string, string>()
    {
        { "61400000001","000000"}
    };


    public static bool CheckKey(string _key)
    {
        return SecureKeyList.Contains(_key);
    }


    public static bool CheckUserException(string _key)
    {
        return ExceptionUserGroup.ContainsKey(_key);
    }




}
