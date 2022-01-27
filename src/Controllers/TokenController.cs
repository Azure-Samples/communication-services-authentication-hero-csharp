// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE.md in the project root for license information.

using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ACS.Solution.Authentication.Server.Interfaces;
using ACS.Solution.Authentication.Server.Models;
using Azure.Communication;
using Azure.Communication.Identity;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        private const string NoAuthorizationCodeError = "Fail to get the authorization code from the request header";

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
        /// <returns>An ACS token with an ACS identity.</returns>
        [Authorize]
        [RequiredScope("access_as_user")] // This is the scope we gave the AuthService when registering the application.
        [HttpGet]
        [ProducesResponseType(typeof(CommunicationUserIdentifierAndTokenResponse), StatusCodes.Status201Created)]
        public async Task<ActionResult> GetACSTokenAsync()
        {
            CommunicationUserIdentifierAndTokenResponse acsIdentityTokenResponse;

            // Retrieve ACS Identity from Microsoft Graph
            string acsUserId = await _graphService.GetACSUserId();

            if (acsUserId == null)
            {
                // User doesn't exist
                CommunicationUserIdentifierAndToken acsIdentityTokenObject = await _acsService.CreateACSUserIdentityAndToken();

                // Store the identity mapping information
                await _graphService.AddIdentityMapping(acsIdentityTokenObject.User.Id);

                // This LoC below should be excuted after AddIdentityMapping excuted successfully
                // because the acsToken can not be returned if failing to add the identity mapping information to Microsoft Graph
                acsIdentityTokenResponse = new CommunicationUserIdentifierAndTokenResponse(acsIdentityTokenObject.AccessToken, acsIdentityTokenObject.User);
            }
            else
            {
                // User exists
                AccessToken acsToken = await _acsService.CreateACSToken(acsUserId);
                acsIdentityTokenResponse = new CommunicationUserIdentifierAndTokenResponse(acsToken, new CommunicationUserIdentifier(acsUserId));
            }

            return StatusCode(StatusCodes.Status201Created, acsIdentityTokenResponse);
        }

        /// <summary>
        /// Eexchange AAD token for an ACS access token of Teams user using the Azure Communication Services Identity SDK.
        /// 1. Get an AAD user access token passed through request header
        /// 2. Initialize a Communication Identity Client and then issue an ACS access token for the Teams user.
        /// </summary>
        /// <param name="authorization">The authorization string to validate.</param>
        /// <returns>If authorizing successfully, an ACS access token for the Teams user. Otherwise, an unauthorized error message.</returns>
        [HttpGet]
        [Route("aad")]
        public async Task<ActionResult> ExchangeAADTokenAsync([FromHeader] string authorization)
        {
            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                // Get an AAD token passed through request header
                string aadTokenViaRequest = headerValue.Parameter;
                // Exchange the AAD user token for the Teams access token
                AccessToken acsTokenForTeamsUser = await _acsService.GetACSTokenForTeamsUser(aadTokenViaRequest);

                return StatusCode(StatusCodes.Status201Created, acsTokenForTeamsUser);
            }
            else
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new ErrorDetails()
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Message = NoAuthorizationCodeError,
                });
            }
        }
    }
}
