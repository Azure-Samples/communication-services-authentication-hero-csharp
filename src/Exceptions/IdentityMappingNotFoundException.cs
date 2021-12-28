// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE.md in the project root for license information.

using System;

namespace ACS.Solution.Authentication.Server.Exceptions
{
    /// <summary>
    /// Create a serializable class that inherits from <see cref="Exception">
    /// </summary>.
    [Serializable]
    public class IdentityMappingNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <c>IdentityMappingNotFoundException</c> class.
        /// </summary>
        public IdentityMappingNotFoundException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>IdentityMappingNotFoundException</c> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public IdentityMappingNotFoundException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>IdentityMappingNotFoundException</c> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public IdentityMappingNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
