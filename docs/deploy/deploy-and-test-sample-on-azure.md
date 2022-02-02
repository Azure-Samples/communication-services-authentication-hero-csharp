# Deploy & Test sample on Azure

1. Set up App Registrations

   To register your Client and Server applications, please refer to our [registrations set up guideline](./set-up-app-registrations.md)

2. Deploy to Azure

    1. Follow button [![Deploy to Azure](https://aka.ms/deploytoazurebutton)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FAzure-Samples%2Fcommunication-services-authentication-hero-csharp%2Fmain%2Fdeploy%2Fazuredeploy.json) to deploy to Azure through [ARM template](https://docs.microsoft.com/en-us/azure/azure-resource-manager/templates/overview).

    2. The template provisions an instance of Communication Services and App Service with deployed code.

    3. When the deployment is completed successfully, a few configurations need to be updated on Application settings within App Service using the information from server app registration.

        Edit the values of following keys by visiting the server app registration:

       - "AzureActiveDirectory__ClientId": "<Application Id from 'Overview page of the server app>"

       - "AzureActiveDirectory__ClientSecret": "<Client Secret Value from 'Certifactes & secrets' of server app>"

       - "AzureActiveDirectory__TenantId": "<Tenant Id from 'Overview' page of the server app>"

    > :bangbang: For the multiple deployments of ACS Authentication Server sample, there might be issue on "/api/token" with mismatched Communication Services identity not belonging to the instance of Communication Services if using same Azure Active Directory instance for user sign in. The sample perists only single mapping of ACS identity within Active Directory user instance through Graph extensions endpoint. So if a different ACS resource is used within subsequent deployments, the persisted ACS identity within Active Directory user instance will not match the ACS resource.

    **Solutions**

    1. Swap the "CommunicationServices__ConnectionString" value from earlier created deployment.

    2. Follow the Troubleshooting section to resolve the issue of "Mismatched ACS identity and ACS Resource" (To Be discussed...) with particular user 

3. Test the deployed APIs

    a. Testing with manually generated AAD Token

     - [Generate AAD token manually](../test-tools/generate_aad_token_manually.md) to call secure Apis of ACS Authentication Hero sample.

     - Invoke the Api
        Once you get the access token, make a GET request to `/api/token` endpoint with the access token as a Authorization Bearer header. Verify you get a successful status code i.e. 200.

        ```shell
        curl --location --request GET 'http://<replace with URL on your provisioned App Service>/api/token' \

        --header 'Authorization: Bearer <put access token here>
        ```
    
    b. Test the APIs using Test Client (To Be discussed...) 


**[Proceed to Architecture Overview ...](../design-guides/architecture-overview.md)**

**[Setting up for Local Development ...](<../contribution-guides/1. get-set-up.md>)**
