// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE.md in the project root for license information.

using System;
using Azure.Communication;
using Azure.Core;

namespace ACS.Solution.Authentication.Server.Models
{
    /// <summary>
    /// The Communication User Identifier and Token Response object.
    /// </summary>
    public class CommunicationUserIdentifierAndTokenResponse
    {
        /// <summary>
        /// Get the access token value.
        /// </summary>
        public string Token { get; }

        /// <summary>
        /// Get the time when the provided token expires.
        /// </summary>
        public DateTimeOffset ExpiresOn { get; }

        /// <summary>
        /// Get a communication user.
        /// </summary>
        public CommunicationUserIdentifier User { get; }

        /// <summary>
        /// Creates a new instance of CommunicationUserIdentifierAndTokenResponse using the provided ACS AccessToken and User.
        /// </summary>
        /// <param name="accessToken">The value of the Azure.Communication.Identity.CommunicationUserIdentifierAndToken.AccessToken property.</param>
        /// <param name="user">The value of the Azure.Communication.Identity.CommunicationUserIdentifierAndToken.User property.</param>
        public CommunicationUserIdentifierAndTokenResponse(AccessToken accessToken, CommunicationUserIdentifier user)
        {
            Token = accessToken.Token;
            ExpiresOn = accessToken.ExpiresOn;
            User = user;
        }
    }
}
