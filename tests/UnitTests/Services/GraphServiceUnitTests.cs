using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ACS.Solution.Authentication.Server.Models;
using ACS.Solution.Authentication.Server.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Graph;
using Moq;
using Xunit;

namespace ACS.Solution.Authentication.Server.UnitTests.Service
{
    public class GraphServiceUnitTests
    {

        [Fact]
        public void GetACSUserId_With_Existing_User_Returns_ACSUserId()
        {
            Mock<IAuthenticationProvider> mockAuthProvider = new Mock<IAuthenticationProvider>();
            Mock<IHttpProvider> mockHttpProvider = new Mock<IHttpProvider>();
            Mock<GraphServiceClient> mockGraphClient = new Mock<GraphServiceClient>(mockAuthProvider.Object, mockHttpProvider.Object);

            const string ACS_USER_ID = "John";

            var extension = new OpenTypeExtension
            {
                ExtensionName = Configurations.Constants.ExtensionName,
                Id = Configurations.Constants.ExtensionName,
                AdditionalData = new Dictionary<string, object>() { { IdentityMapping.IdentityMappingKeyName, ACS_USER_ID } },
            };

            User testUser = new User
            {
                Extensions = new UserExtensionsCollectionPage() { extension },
            };

            mockGraphClient.Setup(g => g.Me.Request().Expand("extensions").Select("id").GetAsync(CancellationToken.None)).Returns(Task.Run(() => testUser)).Verifiable();


            GraphService mockGraphService = new GraphService(mockGraphClient.Object);
            Task<string> returnedACSUserId = mockGraphService.GetACSUserId();

            Assert.Equal(ACS_USER_ID, returnedACSUserId.Result);
        }


        [Fact]
        public void GetACSUserId_With_No_ACS_Extension_Returns_Null()
        {
            Mock<IAuthenticationProvider> mockAuthProvider = new Mock<IAuthenticationProvider>();
            Mock<IHttpProvider> mockHttpProvider = new Mock<IHttpProvider>();
            Mock<GraphServiceClient> mockGraphClient = new Mock<GraphServiceClient>(mockAuthProvider.Object, mockHttpProvider.Object);

            User testUser = new User
            {
                Extensions = new UserExtensionsCollectionPage() { },
            };

            mockGraphClient.Setup(g => g.Me.Request().Expand("extensions").Select("id").GetAsync(CancellationToken.None)).Returns(Task.Run(() => testUser)).Verifiable();


            GraphService mockGraphService = new GraphService(mockGraphClient.Object);
            Task<string> returnedToken = mockGraphService.GetACSUserId();

            Assert.Null(returnedToken.Result);
        }

    }
}
