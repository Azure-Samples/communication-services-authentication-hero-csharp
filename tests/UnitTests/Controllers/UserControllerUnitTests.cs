// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE.md in the project root for license information.

using System.Threading.Tasks;
using ACS.Solution.Authentication.Server.Interfaces;
using ACS.Solution.Authentication.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ACS.Solution.Authentication.Server.UnitTests.Controllers
{
    public class UserControllerUnitTests
    {

        [Fact]
        public void GetACSUser_UserExists_Returns_Status200()
        {
            var mockGraphService = new Mock<IGraphService>();
            var mockACSService = new Mock<IACSService>();

            const string EXISTING_USERID = "UserId123";

            mockGraphService.Setup(mg => mg.GetACSUserId()).Returns(Task.Run(() => EXISTING_USERID));

            var userController = new Server.Controllers.UserController(mockACSService.Object, mockGraphService.Object);
            var returnedACSUserResult = userController.GetACSUser();
            var returnedUser = returnedACSUserResult.Result as OkObjectResult;

            IdentityMapping map = (IdentityMapping)returnedUser.Value;
            Assert.Equal(EXISTING_USERID, map.ACSUserIdentity);
            Assert.Equal(200, returnedUser.StatusCode);

        }

        [Fact]
        public void GetACSUser_UserNotExists_Returns_Status404()
        {
            var mockGraphService = new Mock<IGraphService>();
            var mockACSService = new Mock<IACSService>();

            var userController = new Server.Controllers.UserController(mockACSService.Object, mockGraphService.Object);
            var returnedACSUserResult = userController.GetACSUser();
            var returnedUser = returnedACSUserResult.Result as ObjectResult;

            Assert.Equal(404, returnedUser.StatusCode);
        }

        [Fact]
        public void CreateACSUser_Returns_Status201()
        {
            var mockGraphService = new Mock<IGraphService>();
            var mockACSService = new Mock<IACSService>();

            const string NEW_USERID = "UserId123";

            mockACSService.Setup(mg => mg.CreateACSUserIdentity()).Returns(Task.Run(() => NEW_USERID));
            mockGraphService.Setup(mg => mg.AddIdentityMapping(NEW_USERID)).Returns(Task.Run(() => NEW_USERID));

            var userController = new Server.Controllers.UserController(mockACSService.Object, mockGraphService.Object);
            var returnedACSUserResult = userController.CreateACSUser();
            var returnedUser = returnedACSUserResult.Result as ObjectResult;

            Assert.Equal(NEW_USERID, (returnedUser.Value as IdentityMapping).ACSUserIdentity);
            Assert.Equal(201, returnedUser.StatusCode);

        }

        [Fact]
        public void DeleteACSUser_Returns_Status204()
        {
            var mockGraphService = new Mock<IGraphService>();
            var mockACSService = new Mock<IACSService>();

            const string NEW_USERID = "UserId123";

            mockGraphService.Setup(mg => mg.GetACSUserId()).Returns(Task.Run(() => NEW_USERID));

            var userController = new Server.Controllers.UserController(mockACSService.Object, mockGraphService.Object);
            var deleteACSUserResult = userController.DeleteACSUser();
            var returnedStatusCode = deleteACSUserResult.Result as StatusCodeResult;

            Assert.Equal(204, returnedStatusCode.StatusCode);
        }

    }
}
