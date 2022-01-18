// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE.md in the project root for license information.

using System;
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
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [RequiredScope("access_as_user")] // This is the scope we gave the AuthService when registering the application.
    public class TokenController : ControllerBase
    {
        private readonly IACSService _acsService;
        private readonly IGraphService _graphService;

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
        /// <response code="200">ACS token is successfully retrieved.</response>
        /// <response code="404">Fail to get or refresh an ACS token.</response>
        /// <response code="500">Internal server error.</response>
        /// <returns>An ACS token.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(CommunicationUserIdentifierAndTokenResponse), 201)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> GetACSTokenAsync()
        {
            CommunicationUserIdentifierAndTokenResponse acsIdentityTokenResponse;

            try
            {
                // Retrieve ACS Identity from Microsoft Graph
                string acsUserId = await _graphService.GetACSUserId();

                if (acsUserId == null)
                {
                    // User doesn't exist

                    // No identity mapping information stored in Microsoft Graph
                    Console.WriteLine("There is no identity mapping info stored in Microsoft Graph. Creating now...");

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
            }
            catch (Exception)
            {
                throw;
            }

            return StatusCode(StatusCodes.Status201Created, acsIdentityTokenResponse);
        }
    }
}
