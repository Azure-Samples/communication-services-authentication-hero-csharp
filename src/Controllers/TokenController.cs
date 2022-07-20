// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE.md in the project root for license information.

using System;
using System.Threading.Tasks;
using ACS.Solution.Authentication.Server.Interfaces;
using ACS.Solution.Authentication.Server.Models;
using Azure.Communication;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;

namespace ACS.Solution.Authentication.Server.Controllers
{
    /// <summary>
    /// Token controller.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly IACSService _acsService;
        private readonly IGraphService _graphService;

        // Error message
        private const string NoIdentityMappingError = "There is no identity mapping information stored in Microsoft Graph";

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenController"/> class.
        /// </summary>
        /// <param name="acsService"> An instance representing the set of methods for Azure Communication Services Identity manipulation.</param>
        /// <param name="graphService">An instance representing the set of methods for Microsoft Graph manipulation.</param>
        /// <exception cref="ArgumentNullException">The exception that is thrown when a null reference (Nothing in Visual Basic) is passed to a method that does not accept it as a valid argument.</exception>
        public TokenController(IACSService acsService, IGraphService graphService)
        {
            _acsService = acsService ?? throw new ArgumentNullException(nameof(acsService));
            _graphService = graphService ?? throw new ArgumentNullException(nameof(graphService));
        }

        /// <summary>
        /// GET: api/token
        /// Get or refresh a Communication Services access token.
        /// <list type="number">
        /// <item>
        /// <description>If the identity mapping information existing in the user's roaming profile, then issue an access token for an already existing Communication Services identity</description>
        /// </item>
        /// <item>
        /// <description>If not, create a Communication Services identity and then</description>
        /// </item>
        /// <item>
        /// <list type="number">
        /// <item>
        /// <description>If successfully adding the identity mapping information, then issue an access token.</description>
        /// </item>
        /// <item>
        /// <description>If not, return an error message.</description>
        /// </item>
        /// </list>
        /// </item>
        /// </list>
        /// </summary>
        /// <response code="201">ACS token is successfully generated.</response>
        /// <response code="404">ACS UserID is not found.</response>
        /// <returns>An ACS token with an ACS identity.</returns>
        [Authorize]
        [RequiredScope("access_as_user")] // This is the scope we gave the AuthService when registering the application.
        [HttpGet]
        [ProducesResponseType(typeof(CommunicationUserIdentifierAndTokenResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetACSTokenAsync()
        {
            CommunicationUserIdentifierAndTokenResponse acsIdentityTokenResponse;

            // Retrieve ACS Identity from Microsoft Graph
            string acsUserId = await _graphService.GetACSUserId();

            if (acsUserId == null)
            {
                // User doesn't exist
                return StatusCode(StatusCodes.Status404NotFound, NoIdentityMappingError);
            }

            // User exists
            AccessToken acsToken = await _acsService.CreateACSToken(acsUserId);
            acsIdentityTokenResponse = new CommunicationUserIdentifierAndTokenResponse(acsToken, new CommunicationUserIdentifier(acsUserId));

            return StatusCode(StatusCodes.Status201Created, acsIdentityTokenResponse);
        }

        /// <summary>
        /// Exchange Azure AD token of a Teams user for an ACS access token using the Azure Communication Services Identity SDK.
        /// 1. Get an AAD user access token passed through request header
        /// 2. Initialize a Communication Identity Client and then issue an ACS access token for the Teams user.
        /// </summary>
        /// <param name="teamsUserAadToken">An Azure AD token with Teams.ManageCalls and Teams.ManageChats delegated permissions.</param>
        /// <response code="201">ACS token is successfully generated.</response>
        /// <response code="401">Fail to get an authorization code from the request header.</response>
        /// <returns>If authorizing successfully, an ACS access token for the Teams user. Otherwise, an unauthorized error message.</returns>
        [Authorize]
        [RequiredScope("access_as_user")] // This is the scope we gave the AuthService when registering the application.
        [HttpGet]
        [Route("teams")]
        public async Task<ActionResult> ExchangeAADTokenAsync([FromBody] string teamsUserAadToken)
        {
            // Exchange the Azure AD token of a Teams user for a Communication token
            AccessToken acsTokenForTeamsUser = await _acsService.GetACSTokenForTeamsUser(teamsUserAadToken, User.GetObjectId());

            return StatusCode(StatusCodes.Status201Created, acsTokenForTeamsUser);
        }
    }
}
