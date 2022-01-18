// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ACS.Solution.Authentication.Server.Interfaces;
using ACS.Solution.Authentication.Server.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;

namespace ACS.Solution.Authentication.Server.Services
{
    /// <summary>
    /// Represents the set of methods for Microsoft Graph manipulation.
    /// </summary>
    public sealed class GraphService : IGraphService
    {
        private readonly ILogger<GraphService> _logger;
        private readonly GraphServiceClient _graphServiceClient;

        // Error messages
        private const string RetrieveIdentityMappingError = "An error occured when retrieving the identity mapping information";
        private const string AddIdentityMappingError = "An error occured when adding the identity mapping information";
        private const string DeleteIdentityMappingError = "An error occured when deleting the identity mapping information";

        /// <summary>
        /// Initializes a new instance of Microsoft Graph service client.
        /// </summary>
        /// <param name="graphServiceClient">An instance of <c>GraphServiceClient</c>.</param>
        /// <param name="logger">Used to perform logging.</param>
        public GraphService(GraphServiceClient graphServiceClient, ILogger<GraphService> logger)
        {
            _logger = logger;
            _graphServiceClient = graphServiceClient;
        }

        /// <summary>
        /// Get an Communication Services identity by expanding the extension navigation property.
        /// </summary>
        /// <param name="accessToken">The token issued by the Microsoft identity platform.</param>
        /// <returns>An Communication Services identity if existing in Microsoft Graph.</returns>
        public async Task<string> GetACSUserId()
        {
            try
            {
                User roamingProfileInfoResponse = await _graphServiceClient.Me
                                                                      .Request()
                                                                      .Expand("extensions")
                                                                      .Select("id")
                                                                      .GetAsync();

                IList<Extension> openExtensionsData = roamingProfileInfoResponse.Extensions.CurrentPage;

                OpenTypeExtension identityMappingOpenExtension = GetIdentityMappingOpenExtension(openExtensionsData);

                if (openExtensionsData.Count == 0 || identityMappingOpenExtension == null)
                {
                    return null;
                }

                // An Communication Services identity mapping information existing in Microsoft Graph.
                return identityMappingOpenExtension.AdditionalData[IdentityMapping.IdentityMappingKeyName].ToString();
            }
            catch (Exception ex)
            {
                // Fail to retrieve an Communication Services identity from Microsoft Graph.
                _logger.LogWarning($"{RetrieveIdentityMappingError}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Add an identity mapping to a user resource using Graph open extension.
        /// </summary>
        /// <param name="acsUserId">The Communication Services identity.</param>
        /// <returns>A <see cref="IdentityMappingModel"> object.</returns>.
        public async Task<string> AddIdentityMapping(string acsUserId)
        {
            // Initialize an OpenTypeExtension instance.
            var extension = new OpenTypeExtension
            {
                ExtensionName = Configurations.Constants.ExtensionName,
                AdditionalData = new Dictionary<string, object>() { { IdentityMapping.IdentityMappingKeyName, acsUserId } },
            };

            try
            {
                Extension response = await _graphServiceClient.Me
                                     .Extensions
                                     .Request()
                                     .AddAsync(extension);

                return response.AdditionalData[IdentityMapping.IdentityMappingKeyName].ToString();
            }
            catch (Exception ex)
            {
                // Fail to add an Communication Services identity mapping information to Microsoft Graph.
                _logger.LogWarning($"{AddIdentityMappingError}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Delete an identity mapping information from a user's roaming profile.
        /// </summary>
        /// <returns>An awaitable <see cref="Task"/>.</returns>
        public async Task DeleteIdentityMapping()
        {
            try
            {
                await _graphServiceClient.Me
                                     .Extensions[Configurations.Constants.ExtensionName]
                                     .Request()
                                     .DeleteAsync();
            }
            catch (Exception ex)
            {
                // Fail to remove an Communication Services identity mapping information from Microsoft Graph.
                _logger.LogWarning($"{DeleteIdentityMappingError}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get the identity mapping extension from Graph exthensions.
        /// </summary>
        /// <param name="openExtensionsData">Microsoft Graph Open Extensions.</param>
        /// <returns>An identity mapping extension if existing, otherwise null.</returns>
        private OpenTypeExtension GetIdentityMappingOpenExtension(IList<Extension> openExtensionsData)
        {
            OpenTypeExtension identityMappingOpenExtension = null;

            foreach (OpenTypeExtension openExtension in openExtensionsData)
            {
                if (string.Equals(openExtension.ExtensionName, Configurations.Constants.ExtensionName, StringComparison.Ordinal))
                {
                    identityMappingOpenExtension = openExtension;
                }
            }

            return identityMappingOpenExtension;
        }
    }
}
