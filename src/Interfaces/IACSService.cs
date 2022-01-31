// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE.md in the project root for license information.

using System.Threading.Tasks;
using Azure.Communication.Identity;
using Azure.Core;

namespace ACS.Solution.Authentication.Server.Interfaces
{
    /// <summary>
    /// Represents the set of methods for Azure Communication Services Identity manipulation.
    /// </summary>
    public interface IACSService
    {
        /// <summary>
        /// Create a Communication Services identity using the client.
        /// </summary>
        /// <returns>The unique Communication Services identity.</returns>
        Task<string> CreateACSUserIdentity();

        /// <summary>
        /// Issue an access token for an already existing Communication Services identity.
        /// </summary>
        /// <param name="acsUserId">The unique Communication Services identity.</param>
        /// <returns>An ACS access token with the given scope for a given ACS identity.</returns>
        Task<AccessToken> CreateACSToken(string acsUserId);

        /// <summary>
        /// Exchange an AAD access token of a Teams user for a new Communication Services AccessToken with a matching expiration time.
        /// </summary>
        /// <param name="aadTokan">The Azure AD token of the Teams user.</param>
        /// <returns>An ACS access token with the given scope for a given ACS identity.</returns>
        Task<AccessToken> GetACSTokenForTeamsUser(string aadTokan);

        /// <summary>
        /// Create a Communication Services identity and issue an access token for it in one go.
        /// </summary>
        /// <returns>A Communication Services identity and an access token for it at the same time.</returns>
        Task<CommunicationUserIdentifierAndToken> CreateACSUserIdentityAndToken();

        /// <summary>
        /// Delete a Communication Services identity which will revokes all active access tokens
        /// and prevents the user from issuing access tokens for the identity.
        /// It also removes all the persisted content associated with the identity.
        /// </summary>
        /// <param name="acsUserId">The unique Communication Services identity.</param>
        /// <returns>An awaitable <see cref="Task"/>.</returns>
        Task DeleteACSUserIdentity(string acsUserId);
    }
}
