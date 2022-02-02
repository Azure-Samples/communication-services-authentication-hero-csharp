# Manually generating AAD Token to test secure ACS Authentication Server Sample Apis

> **TIPS:** If you are facing issues running curl commands in #2 and #3, then try importing(File -> import -> raw text, paste the curl command and continue) the curl command in Postman and running it there.
1. You will need an access token using client app registration to call the api. In order to get the access token, open your browser in private mode and visit the link below

> **Note:** The full scope name of the server api should be used for the scope parameter in the below request (e.g.: "api://1234-5678-abcd-efgh...../access_as_user")
```
https://login.microsoftonline.com/<tenantid>.onmicrosoft.com/oauth2/v2.0/authorize?response_type=code&client_id=<client_appid>&redirect_uri=<put url encoded redirect_uri from client app>&scope=<put url encoded server scope>
```
2. This will prompt you to perform authentication and consent, and it will return a code in the query string. 
Use that code in the following request to get an access token, remember to put in the code and client secret.
``` SHELL
curl -X POST \
  https://login.microsoftonline.com/<tenantid>.onmicrosoft.com/oauth2/v2.0/token \
  -H 'Accept: */*' \
  -H 'Cache-Control: no-cache' \
  -H 'Connection: keep-alive' \
  -H 'Content-Type: application/x-www-form-urlencoded' \
  -H 'Host: login.microsoftonline.com' \
  -H 'accept-encoding: gzip, deflate' \
  -H 'cache-control: no-cache' \
  -d 'redirect_uri=<url encoded redirect_uri from client app>&client_id=<appid>&grant_type=authorization_code&code=<put code here>&client_secret=<put secret generated in client app registration>&scope=<url encoded server scope>
  ```