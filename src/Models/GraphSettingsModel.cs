// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE.md in the project root for license information.

namespace ACS.Solution.Authentication.Server.Models
{
    /// <summary>
    /// The Graph settings object in appsettings file.
    /// </summary>
    public class GraphSettingsModel
    {
        /// <summary>
        /// The Key name of Graph settings.
        /// </summary>
        public const string GraphSettingsName = "Graph";

        /// <summary>
        /// Microsoft Graph Open Extension Name.
        /// The name of Graph open extension representing some roaming profile information about users.
        /// </summary>
        public string ExtensionName { get; set; }

        /// <summary>
        /// The URL of the Graph API.
        /// </summary>
        public string BaseUrl { get; set; }
    }
}
