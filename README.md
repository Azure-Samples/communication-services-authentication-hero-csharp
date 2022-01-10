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

- [Overview](#overview)
- [Features](#features)
- [Code Structure](#code-structure)
- [Getting Started](#getting-started)
- [Guidance](#guidance)
  - [Identity Storage Options](#identity-storage-options)
  - [Bring Your Own Identity (BYOI)](#bring-your-own-identity-byoi)
- [Known Issues](#known-issues)
- [Contributing](#contributing)
- [Trademark](#trademark)
- [License](#license)

## Overview

In order to properly implement Azure Communication Services solutions, developers must start by putting in place the correct infrastructure to perform key actions for the communications lifecycle. These actions include authenticating users since the Azure Communication Services are identity-agnostic.

This is an ACS solution server sample to provide a guidance establishing best practices on a simple use case to build trusted backend service that will manage ACS identities by mapping them 1:1 with Azure Active Directory identities (for Teams Interop or native ACS calling/chat) and issue ACS tokens.

There are two scenarios:
1. As a developer, you need to enable authentication flow for joining native ACS and Teams Interop calling/chat by mapping an ACS Identity to an Azure Active Directory identity and using this same ACS identity for the user to fetch an ACS token in every session.
2. As a developer, you need to enable authentication flow for Custom Teams Endpoint by using an Azure Active Directory identity of Teams' user to fetch an ACS token to be able to join Teams calling/chat.

> :loudspeaker: An ACS Solutions - Authentication Sample (Nodejs version) can be found [here](https://github.com/Azure-Samples/communication-services-authentication-hero-javascript).

![ACS Authentication Server Sample Overview Flow](docs/images/ACS-Authentication-Server-Sample_Overview-Flow.png)

Additional documentation for this sample can be found on [Microsoft Docs](https://docs.microsoft.com/azure/communication-services/samples/calling-hero-sample).

Since the sample only focuses on the server APIs, the client application is not part of the sample. If you want to add the client application to login user using Azure AD, then please follow the MSAL samples [here](https://github.com/AzureAD/microsoft-authentication-library-for-js).

## Features

This ACS Solutions - Authentication server sample provides the following features:

* **/deleteUser** - Delete the identity mapping information from the user's roaming profile including the ACS identity.

* **/getToken** - Get / refresh a token for an ACS user.

* **/exchangeToken** - Exchange an M365 token of a Teams user for an ACS token.

  > :information_source: Teams users are authenticated via the MSAL library against Azure Active Directory in the client application. Authentication tokens are exchanged for Microsoft 365 Teams token via the Communication Services Identity SDK. Developers are encouraged to implement an exchange of tokens in their backend services as exchange requests are signed by credentials for Azure Communication Services. In backend services, developers can require any additional authentication. Learn more [here](https://docs.microsoft.com/en-ca/azure/communication-services/concepts/teams-interop#microsoft-365-teams-identity)

## Code Structure

(Add after code freezing.)

## Getting Started

If you're wondering where to get started, here are a few scenarios to help you get going:

* "How does the ACS Authentication Server sample work?"
  * Take a look at our conceptual documentation on 
    - [ACS Authentication Server Sample Architecture Design]().
    - [Secured Web API Architecture Design]().
    - [Identity Mapping Architecture Design]().
    - [AAD Token Exchange Architecture Design]().
* "I want to see what this ACS Authentication Server sample can do by running!"
  * Check out our [Run Authentication Sample](docs/contribution-guides/3. run-authentication-sample.md) guide.
* "I want to submit a fix or a feature for this project"
  * Check out our [making a contribution](CONTRIBUTING.md) guide first.
  * Check out following guides in sequence after coding.
    * [Test Your Changes](docs/contribution-guides/4. test-your-changes.md)
    * [Write Unit Tests](docs/contribution-guides/5. write-unit-tests.md)
    * [Submit a PR](docs/contribution-guides/6. submit-a-pr.md)
    * [Publish Your Changes](docs/contribution-guides/7. publish-your-changes.md)

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
