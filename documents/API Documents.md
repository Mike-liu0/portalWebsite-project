# API Document

## 1 - OnParseUserMobile

### API Name
OnParseUserMobile.aspx


### API Base

http://portaldev.darkspede.space/

### Usage

Get security code by mobile

### Query

Key: API key, could be selected from:

 - 1b949f90-90d1-4310-85b4-67177bc0c6f5
 - d1b88c27-36ca-4cbf-abf8-a830d7f1f968 
 - aef14f62-9eb8-4c02-b642-e5faafe30f95
 - 585a01a5-4824-4f37-9624-75efd23b8d65
 - fa9ead86-4e73-4211-b972-834fcca8fb76
 - d1106961-6c9d-4150-9452-dbc09f2c2117

Mobile: User mobile. Should start with 61 & 4 & 04, example:
 - 61400000000
 - 400000000
 - 0400000000


### Example API Request

http://portaldev.darkspede.space/api/OnParseUserMobile.aspx?key=1b949f90-90d1-4310-85b4-67177bc0c6f5&mobile=0400000000


### Response

- API key check not passed
  
{"success":"false", "message":" API Key Error! Authentication fail", "package":NONE}

- No Mobile Input 

{"success":"false", "message":" Please Input Mobile Number", "package":NONE}

- Mobile format is invalid

{"success":"false", "message":" Invalid Mobile Format", "package":NONE}

- Successfully send

{"success":"true","message":"Code Sent",
"package":{"guid":"0002","created":"637509918667440102","updated":"637517590341322445","status":"dev","mobile":"61400000000","accesstoken":"4a84529e-1ff7-45fe-828d-cbb2d8f8693a","tokenExpire":"637521910341322445","securityCode":"354279","codeExpire":"637517596341322445","username":"N","fullName":"Default
User","email":"N","password":"N","location":"Melbourne","companyName":"N","companyType":"Not
Stated","permission":"C1","iconUrl":"default","fcmToken":"Not provided"}}