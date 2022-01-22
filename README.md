---
page_type: sample
languages:
- csharp
- .Net 5
products:
- azure
- azure-communication-services

---

Deploy to Azure using instructions as [here](./docs/deploy/deploy_test-sample-on-azure.md).

# ACS Solutions - Authentication Server Sample

[![CI](https://github.com/Azure-Samples/communication-services-authentication-hero-csharp/actions/workflows/ci.yml/badge.svg)](https://github.com/Azure-Samples/communication-services-authentication-hero-csharp/actions/workflows/ci.yml)
[![CodeQL](https://github.com/Azure-Samples/communication-services-authentication-hero-csharp/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/Azure-Samples/communication-services-authentication-hero-csharp/actions/workflows/codeql-analysis.yml)
[![C#](https://img.shields.io/badge/%3C%2F%3E-C%23-blue)](https://dotnet.microsoft.com/en-us/languages/csharp)
[![.Net 5.0](https://img.shields.io/badge/%3C%2F%3E-.Net5.0-%230074c1.svg)](https://dotnet.microsoft.com/en-us/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

1. [Overview](#overview)
2. [Endpoints](#endpoints)
3. [Getting Started](#getting-started)
   1. [Prerequisites](#prerequisites)
   2. [Code Structure](#code-structure)
   3. [Before running the sample for the first time](#before-running-the-sample-for-the-first-time)
   4. [Locally deploying the sample app](#locally-deploying-the-sample-app)
   5. [Locally testing the api](#locally-testing-the-api)
   6. [Troubleshooting](#troubleshooting)
   7. [Publish to Azure](#publish-to-azure)
   8. [Building off of the sample](#building-off-of-the-sample)
4. [Guidance](#guidance)
   1. [Identity Storage Options](#Iidentity-storage-options)
   2. [Bring Your Own Identity (BYOI)](#bring-your-own-identity-byoi)
5. [Unit Tests](#testing)
6. [Resources](#resources)
7. [Known Issues](#known-issues)
8. [Contributing](#contributing)
9. [Trademark](#trademark)
10. [License](#license)

## Overview

In order to properly implement a secure Azure Communication Services solutions, developers must start by putting in place the correct infrastructure to properly generate user and access token credentials for Azure Communication Services. Azure Communication Services is identity-agnostic, to learn more check out our [conceptual documentation](https://docs.microsoft.com/azure/communication-services/concepts/identity-model).

This repository provides a sample of a server implementation of an authentication service for Azure Communication Services. It uses best practices to build a trusted backend service that issues Azure Communication Services credentials and maps them to Azure Active Direction identities. 

This sample can help you in the following scenarios:
1. As a developer, you need to enable authentication flow for joining native ACS and Teams Interop calling/chat by mapping an ACS Identity to an Azure Active Directory identity and using this same ACS identity for the user to fetch an ACS token in every session.
2. As a developer, you need to enable authentication flow for Custom Teams Endpoint by using an M365 Azure Active Directory identity of a Teams' user to fetch an ACS token to be able to join Teams calling/chat.

If you are looking to get started with Azure Communication Services, but are still in learning / prototyping phases, check out our [quickstarts for getting started with azure communication services users and access tokens](https://docs.microsoft.com/azure/communication-services/quickstarts/access-tokens?pivots=programming-language-csharp).

> :loudspeaker: An ACS Solutions - Authentication Sample (Nodejs version) can be found [here](https://github.com/Azure-Samples/communication-services-authentication-hero-javascript).

![ACS Authentication Server Sample Overview Flow](docs/images/ACS-Authentication-Server-Sample_Overview-Flow.png)

Additional documentation for this sample can be found on [Microsoft Docs](https://docs.microsoft.com/azure/communication-services/samples/calling-hero-sample).

Since the sample only focuses on the Web Server Apis, the client application is not part of the sample. If you want to add the client application to login user using Azure AD, then please follow the MSAL samples [here](https://github.com/AzureAD/microsoft-authentication-library-for-js).

Before contributing to this sample, please read our [contribution guidelines](./CONTRIBUTING.md).

## Endpoints

This ACS Solutions - Authentication server sample provides the following endpoints:

* **/deleteUser** - Delete the identity mapping information from the user's roaming profile including the ACS identity.

* **/getToken** - Get / refresh a token for an ACS user.

* **/exchangeToken** - Exchange an M365 token of a Teams user for an ACS token.

  > :information_source: Teams users are authenticated via the MSAL library against Azure Active Directory in the client application. Authentication tokens are exchanged for Microsoft 365 Teams token via the Communication Services Identity SDK. Developers are encouraged to implement an exchange of tokens in their backend services as exchange requests are signed by credentials for Azure Communication Services. In backend services, developers can require any additional authentication. Learn more [here](https://docs.microsoft.com/en-ca/azure/communication-services/concepts/teams-interop#microsoft-365-teams-identity)

## Code Structure

(Add after code freezing.)

## Getting Started

If you're wondering where to get started, here are a few scenarios to help you get going:

- Register a Client and Web Api application in Azure Active Directory (AAD) as part of [On Behalf Of workflow](https://docs.microsoft.com/en-us/azure/active-directory/develop/v2-oauth2-on-behalf-of-flow). See instructions below
- Create an ACS resource through [Azure Portal](https://docs.microsoft.com/en-us/azure/communication-services/quickstarts/create-communication-resource?tabs=linux&pivots=platform-azp).
- Update the TokenApi applications with information from the app registrations

#### Server App Registration

Follow instructions on how to register your server application with Azure Active Directory [here](https://docs.microsoft.com/azure/active-directory/develop/quickstart-register-app)

When registering use the following information:
    - name your application `AuthServer`
    - select the 'Accounts in this organizational directory only (Microsoft only - Single tenant)' option for who can use or access this application
    - redirect the URI to 'Web' platform with `http://localhost:44351/` as link
    - click on 'Register' and it will open your application page once registration is sucessful

On your application page
  - navigate to and click on 'Certificates & Secrets' menu item
    - on the 'Client secrets' tab, click on 'New client secret' to create a new one
    - add a description, select an expiration time and click 'Add'
    - this will be used later on
  - navigate to and click on 'API permissions' menu item
    - select 'Grant admin consent' for the Microsoft Graph api call
  - navigate to and click on 'Expose an API' menu item
    - click on 'Set' beside 'Application ID URI'
      - this will automatically set an ID URI for your application
      - click on 'Save'
    - now click on 'Add a scope'
      - your scope should be `access_as_user`. Please remember the full scope name for later use (e.g.: "api://1234-5678-abcd-efgh...../access_as_user").
      - select the 'Admin and users' option for who can consent
      - fill out the consent display name and description for both admin and user
      - select the 'Enabled' state
      - click on 'Add scope'

#### Client App Registration

**Note** - This client app registration should be used to generate the AAD Token manually to call AAD protected Web Apis in the sample, if you do not add a client application extending the backend Web Apis sample.

Follow instructions on how to register your client application with Azure Active Directory [here](https://docs.microsoft.com/azure/active-directory/develop/quickstart-register-app)

When registering use the following information:
    - name your application `AuthClient`
    - select the 'Accounts in this organizational directory only (Microsoft only - Single tenant)' option for who can use or access this application
    - redirect the URI to 'Web' platform with `http://localhost:3000/` as link  (Choose SPA in case you add a client application)
    - click on 'Register' and it will open your application page once registration is sucessful
    
On your AuthClient page:
  - navigate to and click on 'API permissions' menu item
    - click on 'Add a permission'
      - navigate and click on 'My APIs' tab
      - select your 'AuthServer' application
      - check 'access_as_user' box for permissions
  - now navigate to and click on 'Certificates & Secrets' menu item
    - on the 'Client secrets' tab, click on 'New client secret' to create a new one
    - add a description, select an expiration time and click 'Add'
    - this will be used later on to generate the AAD token
- now go back to your 'AuthServer' app
  - navigate to and click on 'Expose an API'
    - click on 'Add client applications
      - past your 'AuthClient' application ID
      - check the corresponding authorized scope box
      - click on 'Add application'

### Code Structure

- ...

### Before running the sample for the first time

1. ...

### Locally deploying the sample app

1. Open TokenApi/appsettings.json and update the configurations.
   
    a. Update the CommunicationServices:ConnectionString from ACS resource.
    
    b. Update the AzureActiveDirectory settings from your server app registration. The ClientId and TenantId should be used from Overview Page and use the previously recorded ClientSecret as from the set up step.

2. open TokenApi, run `dotnet build`, then run `dotnet run`.

### Locally testing the api
**TIPS:** If you are facing issues running curl commands in #2 and #3, then try importing(File -> import -> raw text, paste the curl command and continue) the curl command in [Postman](https://www.postman.com/downloads/) and running it there. 

Since the sample does not have a client application, you need to generate Client AAD Token manually to make calls to AAD protected backend Web Apis in the sample. You will need an access token using client app registration to call an api. In order to get the access token manually, please follow below steps. If you are integrating a client application, then please ignore these steps as you could test directly via user signing through client application.

**Note:** The <client app id> is the application id of the client app registration referred in below requests. The client app in below requests refers the client app registration generally. You can get the <tenantid> from the app registration overview page as well. The full scope name of the server api should be used for the scope parameter in the below request (e.g.: "api://1234-5678-abcd-efgh...../access_as_user").

1. Open your browser in private mode and visit the link below 
```
https://login.microsoftonline.com/<tenantid>/oauth2/v2.0/authorize?response_type=code&client_id=<client appid>&redirect_uri=<redirect_uri from client app>&scope=<server api scope>
```
2. This will prompt you to perform authentication and consent, and it will return a code(which is short lived for 10 minutes) and session_state in the query string. 
Use that code and session_state in the following request to get an access token.

``` SHELL
curl --location --request POST 'https://login.microsoftonline.com/<tenantid>/oauth2/v2.0/token' \
--header 'Accept: */*' \
--header 'Cache-Control: no-cache' \
--header 'Connection: keep-alive' \
--header 'Content-Type: application/x-www-form-urlencoded' \
--header 'Host: login.microsoftonline.com' \
--header 'accept-encoding: gzip, deflate' \
--header 'cache-control: no-cache' \
--data-urlencode 'redirect_uri=<redirect_uri from client app>' \
--data-urlencode 'client_id=<client appid>' \
--data-urlencode 'grant_type=authorization_code' \
--data-urlencode 'code=<code>' \
--data-urlencode 'session_state=<session_state>' \
--data-urlencode 'client_secret=<secret gererated in client app>' \
--data-urlencode 'scope=<server api scope>'
```
3. Once you get the access_token in the response, make a GET request to `http://localhost:44351/api/token` with the access token as a Authorization Bearer header. Verify you get a successful status code i.e. 200.

``` SHELL
curl --location --request GET 'http://localhost:44351/api/token' --header 'Authorization: Bearer <access_token>'
```
4. During local development/testing, if the identity mapping needs to be verified in Graph for `/api/user` and `/api/token` endpoint the please use [Graph Explorer](https://developer.microsoft.com/graph/graph-explorer). Sign in with your Azure Active Directory Identity and verify the response on GET `https://graph.microsoft.com/v1.0/me/extensions` endpoint.

### Troubleshooting

1. ...

### Publish to Azure

1. ...

### Building off of the sample

1. ...

## Guidance

### Identity Storage Options

(Add privacy to provide links to data protection of ACS user Id)

(Add a comparison table here...)

### Bring Your Own Identity (BYOI)

(AAD B2C)

## Known Issues

* ...

## Contributing

Join us by making a contribution. To get you started check out our [making a contribution](<.>) guide.

We look forward to building an amazing open source ACS sample with you!

## Trademark

**Trademarks** This project may contain trademarks or logos for projects, products, or services. Authorized use of Microsoft trademarks or logos is subject to and must follow [Microsoft’s Trademark & Brand Guidelines](https://www.microsoft.com/en-us/legal/intellectualproperty/trademarks/usage/general). Use of Microsoft trademarks or logos in modified versions of this project must not cause confusion or imply Microsoft sponsorship. Any use of third-party trademarks or logos are subject to those third-party’s policies.

## License

[MIT](LICENSE.md)
