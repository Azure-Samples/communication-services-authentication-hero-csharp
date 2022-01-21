using System;
using System.Threading.Tasks;
using ACS.Solution.Authentication.Server.Controllers;
using ACS.Solution.Authentication.Server.Interfaces;
using ACS.Solution.Authentication.Server.Models;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace ACS.Solution.Authentication.Server.UnitTests.Controllers
{
    public class TokenControllerUnitTests
    {
        [Fact]
        public void GetACSTokenAsync_IdentityMappingExists_Returns_Status201()
        {
            var mockGraphService = new Mock<IGraphService>();
            var mockAcsService = new Mock<IACSService>();

            const string EXISTING_USERID = "UserId123";
            const string EXISTING_USERID_TOKENVAL = "UserId123_TokenVal";

            mockGraphService.Setup(mg => mg.GetACSUserId()).Returns(Task.Run(() => EXISTING_USERID));
            mockAcsService.Setup(ma => ma.CreateACSToken(EXISTING_USERID)).Returns(Task.Run(() => new AccessToken(EXISTING_USERID_TOKENVAL, DateTime.Now)));

            var tokenController = new TokenController(mockAcsService.Object, mockGraphService.Object);
            var returnedTokenResult = tokenController.GetACSTokenAsync();
            var returnedToken = returnedTokenResult.Result as ObjectResult;

            CommunicationUserIdentifierAndTokenResponse accessToken = (CommunicationUserIdentifierAndTokenResponse)returnedToken.Value;
            Assert.Equal(EXISTING_USERID_TOKENVAL, accessToken.Token);
            Assert.Equal(EXISTING_USERID, accessToken.User.Id);
            Assert.Equal(201, returnedToken.StatusCode);

        }
    }
}
