// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ACS.Solution.Authentication.Server.Interfaces;
using ACS.Solution.Authentication.Server.Models;
using Microsoft.Extensions.Options;
using Microsoft.Graph;

namespace ACS.Solution.Authentication.Server.Services
{
    /// <summary>
    /// Represents the set of methods for Microsoft Graph manipulation.
    /// </summary>
    public sealed class GraphService : IGraphService
    {
        private readonly GraphSettingsModel _graphSettingsOptions;
        private readonly GraphServiceClient _graphServiceClient;

        /// <summary>
        /// Initializes a new instance of Microsoft Graph service client.
        /// </summary>
        /// <param name="graphServiceClient">An instance of <c>GraphServiceClient</c>.</param>
        /// <param name="graphSettingsOptions">The Microsoft Graph Services settings object in appsettings file.</param>
        public GraphService(GraphServiceClient graphServiceClient, IOptionsMonitor<GraphSettingsModel> graphSettingsOptions)
        {
            _graphSettingsOptions = graphSettingsOptions.CurrentValue;
            _graphServiceClient = graphServiceClient;
        }

        /// <summary>
        /// Get an Communication Services identity by expanding the extension navigation property.
        /// </summary>
        /// <param name="accessToken">The token issued by the Microsoft identity platform.</param>
        /// <returns>An Communication Services identity if existing in Microsoft Graph.</returns>
        public async Task<string> GetACSUserId()
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

        /// <summary>
        /// Add an identity mapping to a user resource using Graph open extension.
        /// </summary>
        /// <param name="acsUserId">The Communication Services identity.</param>
        /// <returns>An awaitable <see cref="Task"/>.</returns>
        public async Task AddIdentityMapping(string acsUserId)
        {
            // Initialize an OpenTypeExtension instance.
            var extension = new OpenTypeExtension
            {
                ExtensionName = _graphSettingsOptions.ExtensionName,
                AdditionalData = new Dictionary<string, object>() { { IdentityMapping.IdentityMappingKeyName, acsUserId } },
            };

            await _graphServiceClient.Me
                                     .Extensions
                                     .Request()
                                     .AddAsync(extension);
        }

        /// <summary>
        /// Delete an identity mapping information from a user's roaming profile.
        /// </summary>
        /// <returns>An awaitable <see cref="Task"/>.</returns>
        public async Task DeleteIdentityMapping()
        {
            await _graphServiceClient.Me
                                     .Extensions[_graphSettingsOptions.ExtensionName]
                                     .Request()
                                     .DeleteAsync();
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
                if (string.Equals(openExtension.ExtensionName, _graphSettingsOptions.ExtensionName, StringComparison.Ordinal))
                {
                    identityMappingOpenExtension = openExtension;
                }
            }

            return identityMappingOpenExtension;
        }
    }
}
