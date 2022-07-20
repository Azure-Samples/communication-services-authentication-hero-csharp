// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE.md in the project root for license information.

using System.Threading.Tasks;
using ACS.Solution.Authentication.Server.Interfaces;
using ACS.Solution.Authentication.Server.Models;
using Azure;
using Azure.Communication;
using Azure.Communication.Identity;
using Azure.Core;
using Microsoft.Extensions.Options;

namespace ACS.Solution.Authentication.Server.Services
{
    /// <summary>
    /// Represents the set of methods for Azure Communication Services Identity manipulation.
    /// </summary>
    public sealed class ACSService : IACSService
    {
        private readonly CommunicationServicesSettingsModel _communicationServicesSettings;
        private readonly AzureActiveDirectorySettingsModel _azureActiveDirectorySettingsOptions;
        private readonly CommunicationIdentityClient _identityClient;

        /// <summary>
        /// Initializes a new instance of Azure.Communication.Identity.CommunicationIdentityClient.
        /// </summary>
        /// <param name="communicationServicesSettingsOptions">The Communication Services settings object in appsettings file.</param>
        /// <param name="azureActiveDirectorySettingsOptions">The Azure Active Directory settings object in appsettings file.</param>
        /// <param name="identityClient">The Azure Communication Services Identity client.</param>
        public ACSService(IOptionsMonitor<CommunicationServicesSettingsModel> communicationServicesSettingsOptions, IOptionsMonitor<AzureActiveDirectorySettingsModel> azureActiveDirectorySettingsOptions = null, CommunicationIdentityClient identityClient = null)
        {
            _communicationServicesSettings = communicationServicesSettingsOptions.CurrentValue;
            _azureActiveDirectorySettingsOptions = azureActiveDirectorySettingsOptions.CurrentValue;
            _identityClient = identityClient ?? new CommunicationIdentityClient(_communicationServicesSettings.ConnectionString);
        }

        /// <summary>
        /// Create a Communication Services identity using the client.
        /// </summary>
        /// <returns>The unique Communication Services identity.</returns>
        public async Task<string> CreateACSUserIdentity()
        {
            // Create an identity
            Response<CommunicationUserIdentifier> identityResponse = await _identityClient.CreateUserAsync();

            return identityResponse.Value.Id;
        }

        /// <summary>
        /// Issue an access token for an already existing Communication Services identity.
        /// </summary>
        /// <param name="acsUserId">The unique Communication Services identity.</param>
        /// <returns>An ACS access token with the given scope for a given ACS identity.</returns>
        public async Task<AccessToken> CreateACSToken(string acsUserId)
        {
            CommunicationTokenScope[] tokenScopes = GetCommunicationTokenScopes();
            // Issue an access token with the given scope for an identity
            CommunicationUserIdentifier identity = new (acsUserId);
            Response<AccessToken> tokenResponse = await _identityClient.GetTokenAsync(identity, scopes: tokenScopes);

            return tokenResponse.Value;
        }

        /// <summary>
        /// Exchange an AAD access token of a Teams user for a new Communication Services AccessToken with a matching expiration time.
        /// </summary>
        /// <param name="teamsUserAadToken">The Azure AD token of the Teams user.</param>
        /// <param name="userObjectId">Object ID of an Azure AD user (Teams User) to be verified against the OID claim in the Azure AD access token.</param>
        /// <returns>An ACS access token with the given scope for a given ACS identity.</returns>
        public async Task<AccessToken> GetACSTokenForTeamsUser(string teamsUserAadToken, string userObjectId)
        {
            // Issue an access token for the Teams user that can be used with the Azure Communication Services SDKs.
            Response<AccessToken> tokenResponse = await _identityClient.GetTokenForTeamsUserAsync(new GetTokenForTeamsUserOptions(teamsUserAadToken, _azureActiveDirectorySettingsOptions.ClientId, userObjectId));

            return tokenResponse.Value;
        }

        /// <summary>
        /// Create a Communication Services identity and issue an access token for it in one go.
        /// </summary>
        /// <returns>A Communication Services identity and an access token for it at the same time.</returns>
        public async Task<CommunicationUserIdentifierAndToken> CreateACSUserIdentityAndToken()
        {
            CommunicationTokenScope[] tokenScopes = GetCommunicationTokenScopes();
            // Issue an identity and an access token with the given scope for the new identity
            Response<CommunicationUserIdentifierAndToken> identityAndTokenResponse = await _identityClient.CreateUserAndTokenAsync(scopes: tokenScopes);

            return identityAndTokenResponse.Value;
        }

        /// <summary>
        /// Delete a Communication Services identity which will revokes all active access tokens
        /// and prevents the user from issuing access tokens for the identity.
        /// It also removes all the persisted content associated with the identity.
        /// </summary>
        /// <param name="acsUserId">The unique Communication Services identity.</param>
        /// <returns>An awaitable <see cref="Task"/>.</returns>
        public async Task DeleteACSUserIdentity(string acsUserId)
        {
            CommunicationUserIdentifier identity = new (acsUserId);
            await _identityClient.DeleteUserAsync(identity);
        }

        // Generate CommunicationTokenScopes object based on string scopes in appsettings.
        private CommunicationTokenScope[] GetCommunicationTokenScopes()
        {
            string[] scopes = _communicationServicesSettings.Scopes;
            CommunicationTokenScope[] tokenScopes = new CommunicationTokenScope[scopes.Length];

            for (int i = 0; i < scopes.Length; i++)
            {
                tokenScopes[i] = new CommunicationTokenScope(scopes[i]);
            }

            return tokenScopes;
        }
    }
}
