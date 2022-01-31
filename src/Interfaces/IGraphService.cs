// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE.md in the project root for license information.

using System.Threading.Tasks;

namespace ACS.Solution.Authentication.Server.Interfaces
{
    /// <summary>
    /// Represents the set of methods for Microsoft Graph manipulation.
    /// </summary>
    public interface IGraphService
    {
        /// <summary>
        /// Get an Communication Services identity by expanding the extension navigation property.
        /// </summary>
        /// <param name="accessToken">The token issued by the Microsoft identity platform.</param>
        /// <returns>An Communication Services identity if existing in Microsoft Graph.</returns>
        Task<string> GetACSUserId();

        /// <summary>
        /// Add an identity mapping to a user resource using Graph open extension.
        /// </summary>
        /// <param name="acsUserId">The Communication Services identity.</param>
        /// <returns>A <see cref="IdentityMapping"> object.</returns>.
        Task AddIdentityMapping(string acsUserId);

        /// <summary>
        /// Delete an identity mapping information from a user's roaming profile.
        /// </summary>
        /// <returns>An awaitable <see cref="Task"/>.</returns>
        Task DeleteIdentityMapping();
    }
}
