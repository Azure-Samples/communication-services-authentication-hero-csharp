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
        private readonly CommunicationIdentityClient _identityClient;

        /// <summary>
        /// Initializes a new instance of Azure.Communication.Identity.CommunicationIdentityClient.
        /// </summary>
        /// <param name="communicationServicesSettingsOptions">The Communication Services settings object in appsettings file.</param>
        /// <param name="identityClient">The Azure Communication Services Identity client.</param>
        public ACSService(IOptionsMonitor<CommunicationServicesSettingsModel> communicationServicesSettingsOptions, CommunicationIdentityClient identityClient = null)
        {
            _communicationServicesSettings = communicationServicesSettingsOptions.CurrentValue;
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
            CommunicationUserIdentifier identity = new CommunicationUserIdentifier(acsUserId);
            Response<AccessToken> tokenResponse = await _identityClient.GetTokenAsync(identity, scopes: tokenScopes);

            return tokenResponse.Value;
        }

        /// <summary>
        /// Exchange an AAD access token of a Teams user for a new Communication Services AccessToken with a matching expiration time.
        /// </summary>
        /// <param name="aadTokan">The Azure AD token of the Teams user.</param>
        /// <returns>An ACS access token with the given scope for a given ACS identity.</returns>
        public async Task<AccessToken> GetACSTokenForTeamsUser(string aadTokan)
        {
            // Issue an access token for the Teams user that can be used with the Azure Communication Services SDKs.
            // Notice: the function name will be renamed to exchangeTeamsUserAadToken
            // Know more, please read this https://github.com/Azure/azure-sdk-for-net/pull/24846#issuecomment-948489542
            Response<AccessToken> tokenResponse = await _identityClient.GetTokenForTeamsUserAsync(aadTokan);

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
            CommunicationUserIdentifier identity = new CommunicationUserIdentifier(acsUserId);
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
