// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE.md in the project root for license information.

using System;

namespace ACS.Solution.Authentication.Server.Models
{
    /// <summary>
    /// The Communication Services settings object in appsettings file.
    /// </summary>
    public class CommunicationServicesSettingsModel
    {
        /// <summary>
        /// The Key name of Communication Services settings.
        /// </summary>
        public const string CommunicationServicesSettingsName = "CommunicationServices";

        /// <summary>
        /// Gets or sets conection string of Communication Services.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// List of scopes for a Communication services access token.
        /// </summary>
        public string[] Scopes { get; set; }
    }
}
