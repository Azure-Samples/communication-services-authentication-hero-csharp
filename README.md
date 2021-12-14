---
page_type: sample
languages:
- csharp
- .Net 3.1
products:
- azure
- azure-communication-services

---

[![Deploy to Azure](https://aka.ms/deploytoazurebutton)]()

# ACS Solutions - Authentication Server Sample

<!--[![CI build status](https://github.com/Azure-Samples/communication-services-authentication-hero-nodejs/workflows/CI/badge.svg?branch=main)](https://github.com/Azure-Samples/communication-services-authentication-hero-nodejs/actions/workflows/ci.yml?query=branch%3Amain)-->
[![C#](https://img.shields.io/badge/%3C%2F%3E-C%23-blue)](https://dotnet.microsoft.com/en-us/languages/csharp)
[![.Net 3.1](https://img.shields.io/badge/%3C%2F%3E-.Net%203.1-blue.svg)](https://dotnet.microsoft.com/en-us/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

1. [Overview](#overview)
2. [Features](#features)
3. [Getting Started](#getting-started)
   1. [Prerequisites](#prerequisites)
   2. [Code Structure](#code-structure)
   3. [Before running the sample for the first time](#before-running-the-sample-for-the-first-time)
   4. [Locally deploying the sample app](#locally-deploying-the-sample-app)
   5. [Troubleshooting](#troubleshooting)
   6. [Publish to Azure](#publish-to-azure)
   7. [Building off of the sample](#building-off-of-the-sample)
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

- Register a client and server application in Azure Active Directory (AAD) * See instructions below
- Download Quickstart single page application (SPA) * See instructions below
- Update the client(SPA) and server(TokenApi) applications with information from the app registrations

### Server Registration

- go to https://portal.azure.com/
- select Azure Active Directory
- select App Registrations
- select new registration 
- Name it AuthServer and select Default Directory Only - single tenant
- For redirect uri select web and enter https://localhost:44351/
- select certificates and secrets, and create a new client secret (save this for later)
- go to expose an API and select set an application id uri
- now select add a scope
- Scope name = access_as_user
- select admin and users to who can consent.
- add info for the descriptions and add the scope

### Client Registraiton
- go to https://portal.azure.com/
- select Azure Active Directory
- select App Registrations
- select new registration 
- Name it AuthClient and select Default Directory Only - single tenant
- For redirect uri select single page application and enter http://localhost:3000/
- under API permissions remove the existing graph API call.
- select add permission, my API, and select the server and select access_as_user
- select grant admin access for default directory
- now select add a client application and add the auth client app id.
- now under manifest, select known applications, and add the app id for the client.

### Downloading the client (SPA)
- open the client app registration from the previous step
- select quickstart
- select single-page application -> JavaScript (auth code flow)
- select "Make these changes for me" and then download the code sample.


### Server config
- Open TokenApi/appsettings.json.template and follow the comments on configuration. Afterwards, rename it to appsettings.json.

### Client config
- under loginRequest, update the scope to be the API we added during the client registration. Example "api://1234-5678-abcd-efgh...../access_as_user"

### Code Structure

- ...

### Locally deploying the sample app

- open AuthMiddleApi, run dotnet build. then run dotnet run
- open yourClientApplication, run npm install, then npm start.

## Demo

A demo app is included to show how to use the project.

To run the demo, follow these steps:

1. after starting both applications, proceed to localhost:3000, and sign in. Add break point and intercept the token.
    - add the breakpoint in authPopup.js, function "seeProfile" and the line that calls "callMSGraph".
    - now click the "See Profile" button, and intercept the token from the accessToken field in the response. 
2. Use postman to test the newly generated token with your API
    - get request to https://localhost:44351/api/token
    - set the authorization to Bearer Token and enter the token you intercepted previously.
    - it should return your email address by calling graph
3. Should successfully complete a graph call.
4. if you look at the web client, you can see the graph call from that side failed. Meaning it has no access to graph, but the middleware does on behalf of the user.

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
