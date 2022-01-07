// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE.md in the project root for license information.

namespace ACS.Solution.Authentication.Server.Models
{
    /// <summary>
    /// The identity mapping object storted in Microsoft Graph Open Extension.
    /// </summary>
    public class IdentityMapping
    {
        /// <summary>
        /// Represent the name of identity mapping key.
        /// </summary>
        public const string IdentityMappingKeyName = "acsUserIdentity";

        /// <summary>
        /// Creates a new instance of IdentityMapping using the provided acsUserIdentity.
        /// </summary>
        /// <param name="acsUserIdentity">Identifier representing a user in Azure Communication Services.</param>
        public IdentityMapping(string acsUserIdentity)
        {
            ACSUserIdentity = acsUserIdentity;
        }

        /// <summary>
        /// Gets or sets unique Azure Communication Services identity.
        /// </summary>
        public string ACSUserIdentity { get; set; }
    }
}
