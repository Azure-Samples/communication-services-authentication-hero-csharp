// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ACS.Solution.Authentication.Server.Exceptions;
using ACS.Solution.Authentication.Server.Interfaces;
using ACS.Solution.Authentication.Server.Models;
using Microsoft.Graph;

namespace ACS.Solution.Authentication.Server.Services
{
    /// <summary>
    /// Represents the set of methods for Microsoft Graph manipulation.
    /// </summary>
    public sealed class GraphService : IGraphService
    {
        private readonly GraphServiceClient _graphServiceClient;

        // Error messages
        private const string GetACSUserIdentityError = "An error occured when retrieving the ACS user id";
        private const string AddIdentityMappingError = "An error occured when adding the identity mapping information";
        private const string DeleteIdentityMappingError = "An error occured when deleting the identity mapping information";
        private const string IdentityMappingNotFoundError = "No identity mapping information stored in Microsoft Graph";

        /// <summary>
        /// Initializes a new instance of Microsoft Graph service client.
        /// </summary>
        /// <param name="graphServiceClient">An instance of <c>GraphServiceClient</c>.</param>
        public GraphService(GraphServiceClient graphServiceClient)
        {
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

                if (openExtensionsData.Count == 0)
                {
                    throw new IdentityMappingNotFoundException(IdentityMappingNotFoundError);
                }
                else
                {
                    // An Communication Services identity mapping information existing in Microsoft Graph.
                    return openExtensionsData[0].AdditionalData[IdentityMappingModel.IdentityMappingKeyName].ToString();
                }
            }
            catch (IdentityMappingNotFoundException identityMappingNotFoundException)
            {
                // No identity mapping information stored in Microsoft Graph
                Console.WriteLine($"{GetACSUserIdentityError}: {identityMappingNotFoundException.Message}");
                throw;
            }
            catch (Exception ex)
            {
                // Fail to retrieve an Communication Services identity from Microsoft Graph.
                Console.WriteLine($"{GetACSUserIdentityError}: {ex.Message}");
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
                ExtensionName = Configuration.Constants.ExtensionName,
                AdditionalData = new Dictionary<string, object>() { { IdentityMappingModel.IdentityMappingKeyName, acsUserId } },
            };

            try
            {
                Extension response = await _graphServiceClient.Me
                                     .Extensions
                                     .Request()
                                     .AddAsync(extension);

                return response.AdditionalData[IdentityMappingModel.IdentityMappingKeyName].ToString();
            }
            catch (Exception ex)
            {
                // Fail to add an Communication Services identity mapping information to Microsoft Graph.
                Console.WriteLine($"{AddIdentityMappingError}: {ex.Message}");
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
                                     .Extensions[Configuration.Constants.ExtensionName]
                                     .Request()
                                     .DeleteAsync();
            }
            catch (Exception ex)
            {
                // Fail to remove an Communication Services identity mapping information from Microsoft Graph.
                Console.WriteLine($"{DeleteIdentityMappingError}: {ex.Message}");
                throw;
            }
        }
    }
}
