---
page_type: sample
languages:
- csharp
- .Net 5
products:
- azure
- azure-communication-services

---

[![Deploy to Azure](https://aka.ms/deploytoazurebutton)]()

# ACS Solutions - Authentication Server Sample

[![CI](https://github.com/Azure-Samples/communication-services-authentication-hero-csharp/actions/workflows/ci.yml/badge.svg)](https://github.com/Azure-Samples/communication-services-authentication-hero-csharp/actions/workflows/ci.yml)
[![CodeQL](https://github.com/Azure-Samples/communication-services-authentication-hero-csharp/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/Azure-Samples/communication-services-authentication-hero-csharp/actions/workflows/codeql-analysis.yml)
[![C#](https://img.shields.io/badge/%3C%2F%3E-C%23-blue)](https://dotnet.microsoft.com/en-us/languages/csharp)
[![.Net 5.0](https://img.shields.io/badge/%3C%2F%3E-.Net5.0-%230074c1.svg)](https://dotnet.microsoft.com/en-us/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

1. [Overview](#overview)
2. [Features](#features)
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

In order to properly implement Azure Communication Services solutions, developers must start by putting in place the correct infrastructure to perform key actions for the communications lifecycle. These actions include authenticating users since the Azure Communication Services are identity-agnostic.

This is an ACS solution server sample to provide a guidance establishing best practices on a simple use case to build trusted backend service that will manage ACS identities by mapping them 1:1 with Azure Active Directory identities (for Teams Interop or native ACS calling/chat) and issue ACS tokens.

There are two scenarios:
1. As a developer, you need to enable authentication flow for joining native ACS and Teams Interop calling/chat by mapping an ACS Identity to an Azure Active Directory identity and using this same ACS identity for the user to fetch an ACS token in every session.
2. As a developer, you need to enable authentication flow for Custom Teams Endpoint by using an Azure Active Directory identity of Teams' user to fetch an ACS token to be able to join Teams calling/chat.

> :loudspeaker: An ACS Solutions - Authentication Sample (Nodejs version) can be found [here](https://github.com/Azure-Samples/communication-services-authentication-hero-javascript).

Additional documentation for this sample can be found on [Microsoft Docs](https://docs.microsoft.com/azure/communication-services/samples/calling-hero-sample).

Since the sample only focuses on the Web Server Apis, the client application is not part of the sample. If you want to add the client application to login user using Azure AD, then please follow the MSAL samples [here](https://github.com/AzureAD/microsoft-authentication-library-for-js).

Before contributing to this sample, please read our [contribution guidelines](./CONTRIBUTING.md).

## Features

This ACS Solutions - Authentication server sample provides the following features:

* **/deleteUser** - Delete the identity mapping information from the user's roaming profile including the ACS identity.

* **/getToken** - Get / refresh a token for an ACS user.

* **/exchangeToken** - Exchange an M365 token of a Teams user for an ACS token.

  > :information_source: Teams users are authenticated via the MSAL library against Azure Active Directory in the client application. Authentication tokens are exchanged for Microsoft 365 Teams token via the Communication Services Identity SDK. Developers are encouraged to implement an exchange of tokens in their backend services as exchange requests are signed by credentials for Azure Communication Services. In backend services, developers can require any additional authentication. Learn more [here](https://docs.microsoft.com/en-ca/azure/communication-services/concepts/teams-interop#microsoft-365-teams-identity)


(Add a workflow diagram here...)

## Getting Started

### Prerequisites

- Register a Client and Web Api application in Azure Active Directory (AAD) as part of [On Behalf Of workflow](https://docs.microsoft.com/en-us/azure/active-directory/develop/v2-oauth2-on-behalf-of-flow). See instructions below
- Update the TokenApi applications with information from the app registrations

#### Server App Registration

- go to https://portal.azure.com/
- open Azure Active Directory service
- on the Azure Active Directory page:
  - navigate to and click on 'App Registrations' menu item
  - click on 'New registration' 
  - on the 'Register an application' page:
    - name your application `AuthServer`
    - select the 'Accounts in this organizational directory only (Microsoft only - Single tenant)' option for who can use or access this application
    - redirect the URI to 'Web' platform with `http://localhost:44351/` as link
    - click on 'Register' and it will open your application page once registration is sucessful
- on your AuthServer application page:
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
- go to https://portal.azure.com/
- open Azure Active Directory service
- on the Azure Active Directory page:
  - navigate to and click on 'App Registrations' menu item
  - click on 'New registration' 
  - on the 'Register an application' page:
    - name your application `AuthClient`
    - select the 'Accounts in this organizational directory only (Microsoft only - Single tenant)' option for who can use or access this application
    - redirect the URI to 'Web' platform with `http://localhost:3000/` as link  (Choose SPA in case you add a client application)
    - click on 'Register' and it will open your application page once registration is sucessful
- on your AuthClient page:
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

- Open TokenApi/appsettings.json.template and follow the comments on configuration. Afterwards, rename it to appsettings.json.
- open TokenApi, run `dotnet build`, then run `dotnet run`.

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

### Testing

To run unit tests in vscode, open the TokenApi.Test or the TokenApi folder, and run the command 'dotnet test'.
To run unit tests in Visual Studio, open the solution, open up test explorer, and run the tests via the UI.

- [Unit testing best practices](https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices) - Find more about unit testing best practices.
- [Unit testing with xUnit](https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-with-dotnet-test) - Find more about how to use xUnit with C#.

## Resources

- [Azure Communication Services Documentation](https://docs.microsoft.com/en-us/azure/communication-services/) - Find more about how to add voice, video, chat, and telephony on our official documentation.
- [Azure Communication Services Hero Samples](https://docs.microsoft.com/en-us/azure/communication-services/samples/overview) - Find more ACS samples and examples on our samples overview page.
- [On-Behalf-Of workflow](https://docs.microsoft.com/en-us/azure/active-directory/develop/v2-oauth2-on-behalf-of-flow) - Find more about the OBO workflow
- [Creating a protected API](https://github.com/Azure-Samples/active-directory-dotnet-native-aspnetcore-v2/tree/master/2.%20Web%20API%20now%20calls%20Microsoft%20Graph) - Detailed example of creating a protected API
## Known Issues

* ...

## Contributing

Join us by making a contribution. To get you started check out our [making a contribution](<.>) guide.

We look forward to building an amazing open source ACS sample with you!

## Trademark

**Trademarks** This project may contain trademarks or logos for projects, products, or services. Authorized use of Microsoft trademarks or logos is subject to and must follow [Microsoft’s Trademark & Brand Guidelines](https://www.microsoft.com/en-us/legal/intellectualproperty/trademarks/usage/general). Use of Microsoft trademarks or logos in modified versions of this project must not cause confusion or imply Microsoft sponsorship. Any use of third-party trademarks or logos are subject to those third-party’s policies.

## License

[MIT](LICENSE.md)
