// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE.md in the project root for license information.

using System;
using System.Threading.Tasks;
using ACS.Solution.Authentication.Server.Interfaces;
using ACS.Solution.Authentication.Server.Models;
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
    public class UserController : ControllerBase
    {
        private readonly IACSService _acsService;
        private readonly IGraphService _graphService;

        // Error message
        private const string NoIdentityMappingError = "There is no identity mapping information stored in Microsoft Graph";

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="acsService"> An instance representing the set of methods for Azure Communication Services Identity manipulation.</param>
        /// <param name="graphService">An instance representing the set of methods for Microsoft Graph manipulation.</param>
        /// <exception cref="ArgumentNullException">The exception that is thrown when a null reference (Nothing in Visual Basic) is passed to a method that does not accept it as a valid argument.</exception>
        public UserController(IACSService acsService, IGraphService graphService)
        {
            _acsService = acsService ?? throw new ArgumentNullException(nameof(acsService));
            _graphService = graphService ?? throw new ArgumentNullException(nameof(graphService));
        }

        /// <summary>
        /// GET: api/user
        /// Get a Communication Services identity through Graph open extensions.
        /// </summary>
        /// <response code="200">ACS user id successfully retrieved.</response>
        /// <response code="404">Specified acs user id doesn't exist.</response>
        /// <returns>An awaitable <see cref="Task"/>.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetACSUser()
        {
            string acsUserId = await _graphService.GetACSUserId();

            return acsUserId == null ? StatusCode(StatusCodes.Status404NotFound, NoIdentityMappingError) : Ok(new IdentityMapping(acsUserId));
        }

        /// <summary>
        /// POST: api/user
        /// Create a Communication Services identity and then add the roaming identity mapping information to the user resource.
        /// </summary>
        /// <response code="201">ACS user successfully created.</response>
        /// <response code="200">ACS user already exists.</response>
        /// <returns>An awaitable <see cref="Task"/>.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(IdentityMapping), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IdentityMapping), StatusCodes.Status201Created)]
        public async Task<ActionResult> CreateACSUser()
        {

            string acsUserId = await _graphService.GetACSUserId();

            if (acsUserId == null)
            {
                // Create a Communication Services identity.
                acsUserId = await _acsService.CreateACSUserIdentity();
                await _graphService.AddIdentityMapping(acsUserId);

                return StatusCode(StatusCodes.Status201Created, new IdentityMapping(acsUserId));
            }

            return StatusCode(StatusCodes.Status200OK, new IdentityMapping(acsUserId));
        }

        /// <summary>
        /// DELETE: api/user
        /// Delete a Communication Services identity and then remove an identity mapping from Microsoft Graph
        /// The strategy of deleting users applied here can avoid creating an ACS token using the ACS identity already deleted which
        /// will cause the error(Provided identity doesn't exist).
        /// <list type="number">
        /// <item>
        /// <description>Delete the identity mapping information from Microsoft Graph.</description>
        /// </item>
        /// <item>
        /// <description>Delete the ACS user identity.</description>
        /// </item>
        /// </list>
        /// </summary>
        /// <response code="204">ACS user id is deleted successfully.</response>
        /// <returns>An awaitable <see cref="Task"/>.</returns>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteACSUser()
        {
            string acsUserId = await _graphService.GetACSUserId();
            // Delete the identity mapping from the user's roaming profile information using Microsoft Graph Open Extension
            await _graphService.DeleteIdentityMapping();
            // Delete the ACS user identity which revokes all active access tokens
            // and prevents users from issuing access tokens for the identity.
            // It also removes all the persisted content associated with the identity.
            await _acsService.DeleteACSUserIdentity(acsUserId);

            return NoContent();
        }
    }
}
