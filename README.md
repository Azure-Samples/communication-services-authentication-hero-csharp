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

<!--[![CI build status](https://github.com/Azure-Samples/communication-services-authentication-hero-nodejs/workflows/CI/badge.svg?branch=main)](https://github.com/Azure-Samples/communication-services-authentication-hero-nodejs/actions/workflows/ci.yml?query=branch%3Amain)-->
[![C#](https://img.shields.io/badge/%3C%2F%3E-C%23-blue)](https://dotnet.microsoft.com/en-us/languages/csharp)
[![.Net 5.0](https://img.shields.io/badge/%3C%2F%3E-.Net 5.0-%230074c1.svg)](https://dotnet.microsoft.com/en-us/)
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
5. [Resources](#resources)
6. [Known Issues](#known-issues)
7. [Contributing](#contributing)
8. [Trademark](#trademark)
9. [License](#license)

## Overview

In order to properly implement Azure Communication Services solutions, developers must start by putting in place the correct infrastructure to perform key actions for the communications lifecycle. These actions include authenticating users since the Azure Communication Services are identity-agnostic.

This is an ACS solution server sample to provide a guidance establishing best practices on a simple use case to build trusted backend service that will manage ACS identities by mapping them 1:1 with Azure Active Directory identities (for Teams Interop or native ACS calling/chat) and issue ACS tokens . There are two scenarios:

1. As a developer, you need to enable authentication flow for joining native ACS and Teams Interop calling/chat by mapping ACS Identity to Azure Active Directory identity and using same ACS identity for the user to fetch ACS tokens in every session.
2. As a developer, you need to enable authentication flow for Custom Teams Endpoint by using Azure Active Directory identity of Teams' user to fetch ACS tokens to join Teams calling/chat.

> :loudspeaker: An ACS Solutions - Authentication Sample (Nodejs version) can be found [here](https://github.com/Azure-Samples/communication-services-authentication-hero-javascript).

Additional documentation for this sample can be found on [Microsoft Docs](https://docs.microsoft.com/azure/communication-services/samples/calling-hero-sample).

Since the sample only focuses on the server apis, the client application is not part of the sample. If you want to add the client application to login user using Azure AD, then please follow the MSAL samples [here](https://github.com/AzureAD/microsoft-authentication-library-for-js).

Before contributing to this sample, please read our [contribution guidelines](./CONTRIBUTING.md).

## Features

This ACS Solutions - Authentication server sample provides the following features:

* **/deleteUser** - Delete the identity mapping information from the user's roaming profile including the ACS identity.

* **/getToken** - Get / refresh a token for an ACS user.

* **/exchangeToken** - Exchange an M365 token of a Teams user for an ACS token.

  > :information_source: Teams users are authenticated via the MSAL library against Azure Active Directory in the client application. Authentication tokens are exchanged for Microsoft 365 Teams token via the Communication Services Identity SDK. Developers are encouraged to implement an exchange of tokens in your backend services as exchange requests are signed by credentials for Azure Communication Services. In backend services, you can require any additional authentication. Learn more information [here](https://docs.microsoft.com/en-ca/azure/communication-services/concepts/teams-interop#microsoft-365-teams-identity)


(Add a workflow diagram here...)

## Getting Started

### Prerequisites

- Register a Client and Web Api application in Azure Active Directory (AAD) as part of [On Behalf Of workflow](https://docs.microsoft.com/en-us/azure/active-directory/develop/v2-oauth2-on-behalf-of-flow). See instructions below
- Update the TokenApi applications with information from the app registrations

#### Server App Registration

- go to https://portal.azure.com/
- select Azure Active Directory
- select App Registrations
- select new registration 
- Name it AuthServer and select Default Directory Only - single tenant
- For redirect uri select web and enter http://localhost:44351/
- select certificates and secrets, and create a new client secret (save this for later)
- under API permissions select grant admin access for the graph api call
- go to expose an API and select set an application id uri
- now select add a scope
- Scope name = access_as_user
- select admin and users to who can consent.
- add info for the descriptions and add the scope

#### Client App Registration
**Note** - This client app registration will be used to manually generate the AAD Token required to call AAD protected Web Api as there is no client application in the sample.
- go to https://portal.azure.com/
- select Azure Active Directory
- select App Registrations
- select new registration 
- name it AuthClient and select Default Directory Only - single tenant
- for redirect uri select Web (Choose SPA in case you add a client application)  and enter http://localhost:3000/
- select add permission, my API, and select the AuthServer and select access_as_user
- now go to certificates & secrets ,create a secret. This will be used later to generate the AAD token.
- now go back to the AuthServer app registration, under manifest, select known applications, and add the app registration id for the client.

### Code Structure

- ...

### Locally deploying the sample app

- Open TokenApi/appsettings.json.template and follow the comments on configuration. Afterwards, rename it to appsettings.json.
- open TokenApi, run dotnet build. then run dotnet run

### Locally testing the api
1. You will need an access token using client app registration to call the api. In order to get the access token, open browser in private mode and visit below link
**Note:** The full scope name of the server api should be used, e.g."api://1234-5678-abcd-efgh...../access_as_user" for the scope parameter in below request
```
https://login.microsoftonline.com/<tenantid>.onmicrosoft.com/oauth2/v2.0/authorize?response_type=code&client_id=<client_appid>&redirect_uri=<put url encoded redirect_uri from client app>&scope=<put url encoded server scope>
```
This will prompt you to perform authentication and consent, and it will return a code in the query string. 
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
   3. Once you get the access token, make a GET request to `http://localhost:44351/api/token` with the access token as a Authorization Bearer header. Verify that you get an output similar to the below. The values marked as ..removed.. will have actual values in your output.

 ``` SHELL
curl --location --request GET 'http://localhost:44351/api/token' \

--header 'Authorization: Bearer <put access token here>
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
