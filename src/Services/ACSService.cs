// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE.md in the project root for license information.

using System;
using System.Threading.Tasks;
using ACS.Solution.Authentication.Server.Interfaces;
using ACS.Solution.Authentication.Server.Models;
using Azure;
using Azure.Communication;
using Azure.Communication.Identity;
using Azure.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ACS.Solution.Authentication.Server.Services
{
    /// <summary>
    /// Represents the set of methods for Azure Communication Services Identity manipulation.
    /// </summary>
    public sealed class ACSService : IACSService
    {
        private readonly ILogger<ACSService> _logger;
        private readonly CommunicationServicesSettingsModel _communicationServicesSettings;
        private readonly CommunicationIdentityClient _identityClient;

        // Error messages
        private const string CreateACSUserIdentityError = "An error occured when creating an ACS user id";
        private const string CreateACSUserTokenError = "An error occured when creating an ACS token";
        private const string CreateACSUserIdentityTokenError = "An error occured when creating an ACS user id and issuing an access token for it in one go";
        private const string DeleteACSUserIdentityError = "An error occured when deleting an ACS user id";

        /// <summary>
        /// Initializes a new instance of Azure.Communication.Identity.CommunicationIdentityClient.
        /// </summary>
        /// <param name="communicationServicesSettingsOptions">The Communication Services settings object in appsettings file.</param>
        /// <param name="logger">Used to perform logging.</param>
        public ACSService(IOptionsMonitor<CommunicationServicesSettingsModel> communicationServicesSettingsOptions, ILogger<ACSService> logger)
        {
            _logger = logger;
            _communicationServicesSettings = communicationServicesSettingsOptions.CurrentValue;
            _identityClient = new CommunicationIdentityClient(_communicationServicesSettings.ConnectionString);
        }

        /// <summary>
        /// Create a Communication Servicesidentity using the client authenticated with Azure AD.
        /// </summary>
        /// <returns>The unique Communication Services identity.</returns>
        public async Task<string> CreateACSUserIdentity()
        {
            try
            {
                // Create an identity
                Response<CommunicationUserIdentifier> identityResponse = await _identityClient.CreateUserAsync();
                // Retrieve ACS identity from the response.
                string identity = identityResponse.Value.Id;

                return identity;
            }
            catch (Exception ex)
            {
                // Fail to create an unique Communication Services identity.
                _logger.LogWarning($"{CreateACSUserIdentityError}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Issue an access token for an already existing Communication Services identity.
        /// </summary>
        /// <param name="acsUserId">The unique Communication Services identity.</param>
        /// <returns>An ACS access token with the given scope for a given ACS identity.</returns>
        public async Task<AccessToken> CreateACSToken(string acsUserId)
        {
            try
            {
                CommunicationTokenScope[] tokenScopes = GetCommunicationTokenScopes();
                // Issue an access token with the given scope for an identity
                CommunicationUserIdentifier identity = new CommunicationUserIdentifier(acsUserId);
                Response<AccessToken> tokenResponse = await _identityClient.GetTokenAsync(identity, scopes: tokenScopes);

                // Get the token from the response
                string token = tokenResponse.Value.Token;
                DateTimeOffset expiresOn = tokenResponse.Value.ExpiresOn;

                return tokenResponse.Value;
            }
            catch (Exception ex)
            {
                // Fail to issue an ACS access token with the given scope for a given ACS identity.
                _logger.LogWarning($"{CreateACSUserTokenError}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Create a Communication Services identity and issue an access token for it in one go.
        /// </summary>
        /// <returns>A Communication Services identity and an access token for it at the same time.</returns>
        public async Task<CommunicationUserIdentifierAndToken> CreateACSUserIdentityAndToken()
        {
            try
            {
                CommunicationTokenScope[] tokenScopes = GetCommunicationTokenScopes();
                // Issue an identity and an access token with the "voip" scope for the new identity
                Response<CommunicationUserIdentifierAndToken> identityAndTokenResponse = await _identityClient.CreateUserAndTokenAsync(scopes: tokenScopes);

                // Retrieve the identity, token, and expiration date from the response
                CommunicationUserIdentifier identity = identityAndTokenResponse.Value.User;
                string token = identityAndTokenResponse.Value.AccessToken.Token;
                DateTimeOffset expiresOn = identityAndTokenResponse.Value.AccessToken.ExpiresOn;

                return identityAndTokenResponse.Value;
            }
            catch (Exception ex)
            {
                // Fail to create a Communication Services identity and an access token for it at the same time.
                _logger.LogWarning($"{CreateACSUserIdentityTokenError}: {ex.Message}");
                throw;
            }
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
            try
            {
                CommunicationUserIdentifier identity = new CommunicationUserIdentifier(acsUserId);
                await _identityClient.DeleteUserAsync(identity);
            }
            catch (Exception ex)
            {
                // Fail to delete a Communication Services identity.
                _logger.LogWarning($"{DeleteACSUserIdentityError}: {ex.Message}");
                throw;
            }
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
