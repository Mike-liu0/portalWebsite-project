using Amazon;
using Amazon.CognitoIdentity;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda;
using Amazon.Lambda.Model;
using Amazon.Runtime;
using Amazon.S3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Darkspede.User;

namespace Darkspede
{
    /// <summary>
    /// Summary description for AWSController
    /// </summary>
    public class AWSController
    {

        #region AWS Instance
        public string IdentityPoolId = Config.AWS_IdentityPoolId;
        public string CognitoIdentityRegion = RegionEndpoint.APSoutheast2.SystemName;
        public string Region = RegionEndpoint.APSoutheast2.SystemName;
        private RegionEndpoint _CognitoIdentityRegion
        {
            get { return RegionEndpoint.GetBySystemName(CognitoIdentityRegion); }
        }
        private RegionEndpoint _LambdaRegion
        {
            get { return RegionEndpoint.GetBySystemName(Region); }
        }

        private AWSCredentials _credentials;
        private AWSCredentials Credentials
        {
            get
            {
                if (_credentials == null)
                    _credentials = new CognitoAWSCredentials(IdentityPoolId, RegionEndpoint.APSoutheast2);
                return _credentials;
            }
        }


        public AWSController()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        /* Client Intance  */
        private IAmazonLambda _lambdaClient;
        private IAmazonDynamoDB _ddbClient;
        private IAmazonS3 _s3Client;
        private IAmazonLambda Lambda_Client
        {
            get
            {
                if (_lambdaClient == null)
                {
                    _lambdaClient = new AmazonLambdaClient(Credentials, RegionEndpoint.APSoutheast2);
                }
                return _lambdaClient;
            }
        }
        private IAmazonDynamoDB Dyanmo_Client
        {
            get
            {
                if (_ddbClient == null)
                {
                    _ddbClient = new AmazonDynamoDBClient(Credentials, RegionEndpoint.APSoutheast2);
                }
                return _ddbClient;
            }
        }
        private IAmazonS3 S3_Client
        {
            get
            {
                if (_s3Client == null)
                {
                    _s3Client = new AmazonS3Client(Credentials, RegionEndpoint.APSoutheast2);
                }
                //test comment
                return _s3Client;
            }
        }

        public object ScanFilter { get; private set; }


        #endregion


        // Table Operations
        #region System
        public string OnLambda_RequestSendSMS(string _message, string _target)
        {
            FCM_SMS sms = new FCM_SMS();
            sms.FCM_To = _target;
            sms.FCM_Message = _message;

            string jsonPackage = FCM_SMS.ToJson(sms);

            try
            {
                InvokeRequest Request = new InvokeRequest
                {
                    FunctionName = "OnRequestSendSMS",
                    InvocationType = InvocationType.Event,
                    Payload = jsonPackage,
                };

                InvokeResponse response = Lambda_Client.Invoke(Request);
            }
            catch (Exception e)
            {
                return "";
            }


            return jsonPackage;
        }

        public string OnDynamoDB_GetSystemConfig()
        {
            return "";
        }

        public string GetTableList()
        {
            string tableListString = "";
            foreach (string table in Config.TableList.Keys)
            {
                if(tableListString != "")
                {
                    tableListString += ",";
                }

                tableListString += Config.TableList[table];
            }
            return tableListString;
        }

        #endregion


        //======================================================================
        //======Application Service DARKSPEDE ==================================
        //======================================================================

        #region Darkspede Application

        // General Database Access
        public string OnDynamoDB_AddNewApplication(Darkspede_Application _item)
        {

            Table table = Table.LoadTable(Dyanmo_Client, Config.Site + Config.TableList["Darkspede_Application"]);
            Document item = Document.FromJson(Darkspede_Application.ToJson(_item));
            PutItemOperationConfig config = new PutItemOperationConfig();
            bool reuslt = table.TryPutItem(item, config);

            if (reuslt)
            {
                return _item.guid;
            }
            else
            {
                return config.ReturnValues.ToString();
            }
        }

        public string OnDynamoDB_ModifyApplication(Darkspede_Application _item)
        {
            Table table = Table.LoadTable(Dyanmo_Client, Config.Site + Config.TableList["Darkspede_Application"]);
            Document item = Document.FromJson(Darkspede_Application.ToJson(_item));
            UpdateItemOperationConfig config = new UpdateItemOperationConfig();
            bool reuslt = table.TryUpdateItem(item, config);

            if (reuslt)
            {
                return _item.guid;
            }
            else
            {
                return config.ReturnValues.ToString();
            }
        }

        public string OnDynamoDB_DeleteApplicationByGuid(string _guid)
        {
            Table table = Table.LoadTable(Dyanmo_Client, Config.Site + Config.TableList["Darkspede_Application"]);
            Darkspede_Application item = new Darkspede_Application();
            item.guid = _guid;
            Document doc = Document.FromJson(Darkspede_Application.ToJson(item));
            DeleteItemOperationConfig config = new DeleteItemOperationConfig();
            bool result = table.TryDeleteItem(doc, config);

            if (result)
            {
                return "success";
            }
            else
            {
                return "none";
            }
        }

        public string OnDynamoDB_GetApplicationByGuid(string _guid)
        {
            Table table = Table.LoadTable(Dyanmo_Client, Config.Site + Config.TableList["Darkspede_Application"]);
            Document document = table.GetItem(_guid);

            if (document != null)
            {
                return document.ToJson();
            }
            else
            {
                return "none";
            }
        }

        public string OnDynamoDB_GetAllApplications(string _condition, string _value)
        {
            Table table = Table.LoadTable(Dyanmo_Client, Config.Site + Config.TableList["Darkspede_Application"]);
            ScanFilter scanFilter = new ScanFilter();
            scanFilter.AddCondition(_condition, ScanOperator.Equal, _value);

            Search search = table.Scan(scanFilter);

            List<Document> documentList = new List<Document>();

            string jsonList = "[";
            int index = 0;

            while (!search.IsDone)
            {

                documentList = search.GetNextSet();

                foreach (var document in documentList)
                {
                    if (index != 0)
                    {
                        jsonList = jsonList + "," + document.ToJson();
                    }
                    else
                    {
                        jsonList = jsonList + document.ToJson();
                    }
                    index++;
                }
            }

            return jsonList + "]";
        }

        // Extra Database Access
        public string OnDynamoDB_GetAllApplicationFilters(Dictionary<string, string> _filterPair)
        {
            Table table = Table.LoadTable(Dyanmo_Client, Config.Site + Config.TableList["Darkspede_Application"]);
            ScanFilter scanFilter = new ScanFilter();

            foreach (KeyValuePair<string, string> pair in _filterPair)
            {
                if (pair.Value != "N")
                {
                    scanFilter.AddCondition(pair.Key, ScanOperator.Equal, pair.Value);
                }
            }



            Search search = table.Scan(scanFilter);

            List<Document> documentList = new List<Document>();
            string jsonList = "[";
            int index = 0;

            while (!search.IsDone)
            {

                documentList = search.GetNextSet();

                foreach (var document in documentList)
                {
                    if (index != 0)
                    {
                        jsonList = jsonList + "," + document.ToJson();
                    }
                    else
                    {
                        jsonList = jsonList + document.ToJson();
                    }
                    index++;
                }
            }

            return jsonList + "]";
        }

        public List<Document> OnDynamoDB_GetAllApplicationDocuments(string _condition, string _value)
        {
            Table table = Table.LoadTable(Dyanmo_Client, Config.Site + Config.TableList["Darkspede_Application"]);
            ScanFilter scanFilter = new ScanFilter();
            scanFilter.AddCondition(_condition, ScanOperator.Equal, _value);

            Search search = table.Scan(scanFilter);

            List<Document> documentList = new List<Document>();


            while (!search.IsDone)
            {
                documentList = search.GetNextSet();

            }

            return documentList;


        }

        #endregion


        /*Operation to users in DB*/
        #region Darkspede User

        //Add new user
        public string OnDynamoDB_AddNewUser(Darkspede_User _item)
        {
            Table table = Table.LoadTable(Dyanmo_Client, Config.Site + Config.TableList["Darkspede_User"]);
            Document item = Document.FromJson(Darkspede_User.ToJson(_item));
            PutItemOperationConfig config = new PutItemOperationConfig();
            bool result = table.TryPutItem(item, config);    //if put in item success

            if (result)
            {
                return _item.guid;
            }
            else
            {
                return "Failed: " + config.ReturnValues.ToString();
            }

        }


        //Modify user
        public string OnDynamoDB_ModifyUser(Darkspede_User _item)
        {
            Table table = Table.LoadTable(Dyanmo_Client, Config.Site + Config.TableList["Darkspede_User"]);
            Document item = Document.FromJson(Darkspede_User.ToJson(_item));
            UpdateItemOperationConfig config = new UpdateItemOperationConfig();
            bool reuslt = table.TryUpdateItem(item, config);

            if (reuslt)
            {
                return _item.guid;
            }
            else
            {
                return config.ReturnValues.ToString();
            }
        }



        //Delete user by guid
        public string OnDynamoDB_DeleteUserByGuid(string _guid)
        {
            Table table = Table.LoadTable(Dyanmo_Client, Config.Site + Config.TableList["Darkspede_User"]);
            Darkspede_User item = new Darkspede_User();
            item.guid = _guid;
            Document document = Document.FromJson(Darkspede_User.ToJson(item));
            DeleteItemOperationConfig config = new DeleteItemOperationConfig();
            bool result = table.TryDeleteItem(document, config);

            if (result)
            {
                return "success";
            }
            else
            {
                return "none";
            }
        }


        //Get user by guid
        public string OnDynamoDB_GetUserByGuid(string _guid)
        {
            Table table = Table.LoadTable(Dyanmo_Client, Config.Site + Config.TableList["Darkspede_User"]);
            Document document = table.GetItem(_guid);

            if (document != null)
            {
                return document.ToJson();
            }
            else
            {
                return "none";
            }
        }


        //Get all users
        public string OnDynamoDB_GetAllUsers(string _condition, string _value)
        {
            Table table = Table.LoadTable(Dyanmo_Client, Config.Site + Config.TableList["Darkspede_User"]);
            ScanFilter scanFilter = new ScanFilter();
            scanFilter.AddCondition(_condition, ScanOperator.Equal, _value);

            Search search = table.Scan(scanFilter);

            List<Document> docuementList = new List<Document>();

            string jsonList = "[";  //Empty json list
            //string jsonList = "none";  //Empty json list
            int index = 0;

            while (!search.IsDone)    //Still searching
            {
                docuementList = search.GetNextSet();

                foreach (var doc in docuementList)
                {
                    if (index != 0)    //first item in documentlist
                    {
                        jsonList += "," + doc.ToJson();
                    }
                    else
                    {
                        jsonList += doc.ToJson();
                    }
                    index++;
                }
            }
            return jsonList + "]";
            //return jsonList;
        }


        public string OnDynamoDB_GetAllUserFilters(Dictionary<string, string> _filterPair)
        {
            Table table = Table.LoadTable(Dyanmo_Client, Config.Site + Config.TableList["Darkspede_User"]);
            ScanFilter scanFilter = new ScanFilter();

            foreach (KeyValuePair<string, string> pair in _filterPair)
            {
                if (pair.Value != "N")
                {
                    scanFilter.AddCondition(pair.Key, ScanOperator.Equal, pair.Value);
                }
            }



            Search search = table.Scan(scanFilter);

            List<Document> documentList = new List<Document>();
            string jsonList = "[";
            int index = 0;

            while (!search.IsDone)
            {

                documentList = search.GetNextSet();

                foreach (var document in documentList)
                {
                    if (index != 0)
                    {
                        jsonList = jsonList + "," + document.ToJson();
                    }
                    else
                    {
                        jsonList = jsonList + document.ToJson();
                    }
                    index++;
                }
            }

            return jsonList + "]";
        }


        public List<Document> OnDynamoDB_GetAllUserDocuments(string _condition, string _value)
        {
            Table table = Table.LoadTable(Dyanmo_Client, Config.Site + Config.TableList["Darkspede_User"]);
            ScanFilter scanFilter = new ScanFilter();
            scanFilter.AddCondition(_condition, ScanOperator.Equal, _value);

            Search search = table.Scan(scanFilter);

            List<Document> documentList = new List<Document>();

            while (!search.IsDone)
            {
                documentList = search.GetNextSet();
            }

            return documentList;
        }


        public string OnDynamoDB_GetUserPermission(string guid)
        {
            Table table = Table.LoadTable(Dyanmo_Client, Config.Site + Config.TableList["Darkspede_User"]);
            Document document = table.GetItem(guid);

            if (document != null)
            {
                return document.ToJson();
            }
            else
            {
                return "none";
            }
        }

        #endregion
    }
}
