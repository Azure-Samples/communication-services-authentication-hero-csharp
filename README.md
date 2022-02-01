---
page_type: sample
languages:
- csharp
- .Net 5
products:
- azure
- azure-communication-services
---

Deploy to Azure using instructions [here](./docs/deploy/deploy_test-sample-on-azure.md).

# ACS Solutions - Authentication Server Sample

[![CI](https://github.com/Azure-Samples/communication-services-authentication-hero-csharp/actions/workflows/ci.yml/badge.svg)](https://github.com/Azure-Samples/communication-services-authentication-hero-csharp/actions/workflows/ci.yml)
[![CodeQL](https://github.com/Azure-Samples/communication-services-authentication-hero-csharp/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/Azure-Samples/communication-services-authentication-hero-csharp/actions/workflows/codeql-analysis.yml)
[![C#](https://img.shields.io/badge/%3C%2F%3E-C%23-blue)](https://dotnet.microsoft.com/en-us/languages/csharp)
[![.Net 5.0](https://img.shields.io/badge/%3C%2F%3E-.Net5.0-%230074c1.svg)](https://dotnet.microsoft.com/en-us/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

1. [Overview](#overview)
2. [Endpoints](#endpoints)
3. [Code Structure](#code-structure)
4. [Getting Started](#getting-started)
5. [Guidance](#guidance)
   1. [Identity Storage Options](#identity-storage-options)
   2. [Bring Your Own Identity (BYOI)](#bring-your-own-identity-byoi)
6. [Known Issues](#known-issues)
7. [Contributing](#contributing)
8. [Trademark](#trademark)
9. [License](#license)

## Overview

In order to properly implement a secure Azure Communication Services solutions, developers must start by putting in place the correct infrastructure to properly generate user and access token credentials for Azure Communication Services. Azure Communication Services is identity-agnostic, to learn more check out our [conceptual documentation](https://docs.microsoft.com/azure/communication-services/concepts/identity-model).

This repository provides a sample of a server implementation of an authentication service for Azure Communication Services. It uses best practices to build a trusted backend service that issues Azure Communication Services credentials and maps them to Azure Active Direction identities. 

This sample can help you in the following scenarios:
1. As a developer, you need to enable an authentication flow for joining native ACS and Teams Interop calling/chat by mapping an ACS identity to an Azure Active Directory identity and using this same ACS identity for the user to fetch an ACS token in every session.
2. As a developer, you need to enable an authentication flow for Custom Teams Endpoint by using an M365 Azure Active Directory identity of a Teams' user to fetch an ACS token to be able to join Teams calling/chat.

If you are looking to get started with Azure Communication Services, but are still in learning / prototyping phases, check out our [quickstarts for getting started with azure communication services users and access tokens](https://docs.microsoft.com/en-us/azure/communication-services/quickstarts/access-tokens?pivots=programming-language-csharp).

> :loudspeaker: An ACS Solutions - Authentication Sample (NodeJS version) can be found [here](https://github.com/Azure-Samples/communication-services-authentication-hero-nodejs).

![ACS Authentication Server Sample Overview Flow](docs/images/ACS-Authentication-Server-Sample_Overview-Flow.png)

Additional documentation for this sample can be found on [Microsoft Docs](https://docs.microsoft.com/azure/communication-services/samples/calling-hero-sample). !!! TODO: change link?

Since this sample only focuses on the server APIs, the client application is not part of it. If you want to add the client application to login user using Azure AD, then please follow the MSAL samples [here](https://github.com/AzureAD/microsoft-authentication-library-for-js).

Before contributing to this sample, please read our [contribution guidelines](./CONTRIBUTING.md).

## Endpoints

This ACS Solutions - Authentication server sample provides the following endpoints:

* **/deleteUser** - Delete the identity mapping information from the user's roaming profile including the ACS identity.

* **/getToken** - Get / refresh a token for an ACS user.

* **/exchangeToken** - Exchange an M365 token of a Teams user for an ACS token.

  > :information_source: Teams users are authenticated via the MSAL library against Azure Active Directory in the client application. Authentication tokens are exchanged for Microsoft 365 Teams token via the Communication Services Identity SDK. Developers are encouraged to implement an exchange of tokens in their backend services as exchange requests are signed by credentials for Azure Communication Services. In backend services, developers can require any additional authentication. Learn more [here](https://docs.microsoft.com/en-ca/azure/communication-services/concepts/teams-interop#microsoft-365-teams-identity)

The below section is WIP...

## Getting Started
### Code Structure
### Architecture Overview
## Building off of the sample
## Publish to Azure
## Troubleshooting
## Known Issues

## Bring Your Own Identity (BYOI)
(AAD B2C)


## Contributing

Join us by making a contribution. To get you started check out our [making a contribution](CONTRIBUTING.md) guide.

We look forward to building an amazing open source ACS Authentication Server sample with you!

## Trademark

**Trademarks** This project may contain trademarks or logos for projects, products, or services. Authorized use of Microsoft trademarks or logos is subject to and must follow [Microsoft’s Trademark & Brand Guidelines](https://www.microsoft.com/en-us/legal/intellectualproperty/trademarks/usage/general). Use of Microsoft trademarks or logos in modified versions of this project must not cause confusion or imply Microsoft sponsorship. Any use of third-party trademarks or logos are subject to those third-party’s policies.

## License

[MIT](LICENSE.md)
