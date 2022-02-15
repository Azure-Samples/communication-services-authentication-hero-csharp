# Test deployed service

1. Testing with manually generated Azure Active Directory Token

    - [Generate Azure Active Directory token manually](../test-tools/generate_aad_token_manually.md) to call secure Apis of Azure Communication Services Authentication Hero sample.

    - Invoke the API
    Once you get the access token, make a GET request to `/api/token` endpoint with the access token as a Authorization Bearer header. Verify you get a successful status code (i.e. 200).

        ```shell
        curl --location --request GET 'https://<replace with URL on your provisioned App Service>/api/token  OR http://localhost:5000/api/token' \

        --header 'Authorization: Bearer <put access token here>'
        ```
        > Note: If you are facing issues running the curl command, then try importing (File -> import -> raw text, paste the curl command and continue) the curl command in [Postman](https://www.postman.com/downloads/) and running it there

2. Test the APIs using the MinimalClient
    -  Please take a look at the MinimalClient README.md [MinimalClient](../../MinimalClient/README.md)