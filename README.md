# Authentication Hero C#

This project contains the Authentication sample using c#. We include a simple webpage that will be used to generate an access token. This access token will be used with the on-behalf-of workflow by the middleware, to gain access to Microsoft graph API. We will then send a simple request to the graph API.

## Features

This project framework provides the following features:

* Middleware authentication API that uses the on behalf of workflow.

## Getting Started

### Prerequisites

- Register the client and server application (see instructions)
- In the AuthClient app registration, go to QuickStart, select single page application, and download the JavaScript spa code. 
- Update the AuthMiddleApi code and AuthSamplePage with the information from app registrations.

### Server Registration

- go to https://portal.azure.com/
- select Azure Active Directory
- select App Registrations
- select new registration 
- Name it AuthServer and select Default Directory Only - single tenant
- For redirect uri select web and enter https://localhost:44351/
- select certificates and secrets, and create a new client secret
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


### Server config
- Open AuthMiddleApi/appsettings.json and follow the comments on configuration.

### Client config
- under loginRequest, update the scope to be the API we added during the client registration. Example "api://05f25e1d-2905-4b3a-a907-b5f942f62c22/access_as_user"

### Installation

- open AuthMiddleApi, run dotnet build. then run dotnet run
- open AuthSamplePage, run npm install, then npm start.



## Demo

A demo app is included to show how to use the project.

To run the demo, follow these steps:

1. proceed to localhost:3000, and sign in. Add break point and intercept token.
2. User postman to test the newly generated token with your API
3. Should successfully complete a graph call.
4. if you look at the web client, you can see the graph call from that side failed. Meaning it has no access to graph, but the middleware does on behalf of the user.


## Resources

- https://docs.microsoft.com/en-us/azure/active-directory/develop/v2-oauth2-on-behalf-of-flow
- https://github.com/Azure-Samples/ms-identity-aspnet-webapi-onbehalfof


- The following was used to help construct the server portion, for the client portion an auto generated single page app via azure portal was used.
- https://github.com/Azure-Samples/active-directory-dotnet-native-aspnetcore-v2/tree/master/2.%20Web%20API%20now%20calls%20Microsoft%20Graph
