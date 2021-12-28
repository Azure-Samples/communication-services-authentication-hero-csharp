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
        /// Create a Communication Servicesidentity using the client authenticated with Azure AD.
        /// </summary>
        /// <returns>The unique Communication Services identity.</returns>
        /// <exception cref="RequestFailedException">If failing to create an unique Communication Services identity.</exception>
        Task<string> CreateACSUserIdentity();

        /// <summary>
        /// Issue an access token for an already existing Communication Services identity.
        /// </summary>
        /// <param name="acsUserId">The unique Communication Services identity.</param>
        /// <returns>An ACS access token with the given scope for a given ACS identity.</returns>
        /// <exception cref="RequestFailedException">If failing to issue an ACS access token with the given scope for a given ACS identity.</exception>
        Task<AccessToken> CreateACSToken(string acsUserId);

        /// <summary>
        /// Create a Communication Services identity and issue an access token for it in one go.
        /// </summary>
        /// <returns>A Communication Services identity and an access token for it at the same time.</returns>
        /// <exception cref="RequestFailedException">If failing to create a Communication Services identity and an access token for it at the same time.</exception>
        Task<CommunicationUserIdentifierAndToken> CreateACSUserIdentityAndToken();

        /// <summary>
        /// Delete a Communication Services identity which will revokes all active access tokens
        /// and prevents the user from issuing access tokens for the identity.
        /// It also removes all the persisted content associated with the identity.
        /// </summary>
        /// <param name="acsUserId">The unique Communication Services identity.</param>
        /// <returns>An awaitable <see cref="Task"/>.</returns>
        /// <exception cref="RequestFailedException">If failing to delete a Communication Services identity.</exception>
        Task DeleteACSUserIdentity(string acsUserId);
    }
}
