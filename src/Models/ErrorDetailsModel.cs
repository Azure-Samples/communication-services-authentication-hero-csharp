// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE.md in the project root for license information.

namespace ACS.Solution.Authentication.Server.Models
{
    /// <summary>
    /// Error Object.
    /// </summary>
    public class ErrorDetails
    {
        /// <summary>
        /// HTTP status code.
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Error Message.
        /// </summary>
        public string Message { get; set; }
    }
}
