# Deploy Locally

1. [Get Set up](#get-set-up)
   1. [Prerequisites for Development Environment Setup](#prerequisites-for-development-environment-setup)
   2. [Cloning the repo](#cloning-the-repo)
   2. [Installing dependencies](#installing-dependencies)
2. [Build Authentication Server Sample](#build-authentication-server-sample)
3. [Run Authentication Server Sample](#run-authentication-server-sample)
   1. [Prerequisites to run the sample](#prerequisites-to-run-the-sample)
   2. [Update the appSettings.json file](#update-the-appsettingsjson-file)
   3. [Generate an Azure Active Directory Token Manually](#generate-an-azure-active-directory-token-manually)
   4. [Run the App](#run-the-app)

## Get Set up

### Prerequisites for Development Environment Setup

- Install [.Net](https://dotnet.microsoft.com/).
- (Recommended) Install [Visual Studio](https://visualstudio.microsoft.com/).

### Cloning the repo

Clone the repo: https://github.com/Azure-Samples/communication-services-authentication-hero-csharp

```shell
# HTTPS
git clone https://github.com/Azure-Samples/communication-services-authentication-hero-csharp.git

# SSH
git clone git@github.com:Azure-Samples/communication-services-authentication-hero-csharp.git
```

### Installing dependencies

Navigate to the project `src` directory and install dependencies for all packages and samples:

```shell
# Navigate to the src directory
cd communication-services-authentication-hero-csharp/src

# restore dependencies
dotnet restore
```

>**Note:** this may take some time the first time it runs

## Build Authentication Server Sample

Once set up, you can run the following commands to build the repo.

```shell
# Navigate to the project root directory
cd communication-services-authentication-hero-csharp/

# Builds the project and its dependencies into a set of binaries
dotnet build
```

>**Note:**
>
> 1. This may take some time the first time it runs.
> 2. To learn more dotnet commands, please visit [.NET CLI overview](https://docs.microsoft.com/dotnet/core/tools/)
> 3. You can also build the repo using Visual Studio:
>    1. Right click on the solution `ACSAuthenticationSolution.sln` in **Solution Explorer**.
>    2. Click on **Build Solution**

## Run Authentication Server Sample

### Prerequisites to run the sample
To be able to run this sample locally, you will first need to follow those [prerequisites](../../README.md#prerequisites).

### Update the `appSettings.json` File

Before running the sample, you will need to replace the values in the  `appSettings.json` file:

1. Replace `connectionString` and `scopes` for the Communication Services
2. Replace `clientId`, `tenantId` and `clientSecret` for the Azure Active Directory.

>**Note:** Values of `clientId`, `tenantId` and `clientSecret` are all from your `auther-server-sample-webApi`.

### Generate an Azure Active Directory Token Manually

To make calls to Azure Active Directory protected backend Web APIs in the sample, you will need an access token using client app registration. In order to get the access token manually, please follow the steps [here](../test-tools/generate_aad_token_manually.md). 

### Testing this backend with the sample client (A sample to show the endpoints being called)

Please take a look at the MinimalClient README.md [MinimalClient](../../MinimalClient/README.md)

>**Note:** If you are integrating a client application, then please ignore these steps as you could test directly via user signing through client application.

Once you get the `access_token` in the response, you can jump to the next step to start the server and call `http://localhost:5000/api/token` or `https://localhost:5001/api/token` using the  `access_token`.

### Run the App

In order to run the Azure Communication Services Authentication Server sample,

1. Go to the project `src` directory.

   ```shell
   # navigate to the src directory
   cd communication-services-authentication-hero-csharp/src
   ```

2. Run the following command.

   ```shell
   # Start the server
   dotnet run
   ```

   > Note: You can also run the selected project using the Visual Studio.

3. Make a GET request to `http://localhost:5000/api/token` with the `access_token` generated at **step 2** of **Generate an Azure Active Directory Token manually** guide as a Authorization Bearer header. Verify you get a response with a successful status code (i.e. 200).

   ```shell
   curl --location --request GET 'http://localhost:5000/api/token' --header 'Authorization: Bearer <access_token>'
   ```

   > Note: If you are facing issues running the curl command, then try importing (File -> import -> raw text, paste the curl command and continue) the curl command in [Postman](https://www.postman.com/downloads/) and running it there.

4. During local development/testing, if the identity mapping needs to be verified in Graph for `/api/user` and `/api/token` endpoint, please use [Graph Explorer](https://developer.microsoft.com/graph/graph-explorer). Sign in with your Azure Active Directory Identity and verify the response on GET `https://graph.microsoft.com/v1.0/me/extensions` endpoint.

>**Note:** Want to contribute to this sample and help us make it even better? Check our [contribution guide](../contribution-guides/1.get-set-up.md).