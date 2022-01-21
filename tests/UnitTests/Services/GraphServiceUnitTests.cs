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
            var mockAuthProvider = new Mock<IAuthenticationProvider>();
            var mockHttpProvider = new Mock<IHttpProvider>();
            var mockGraphClient = new Mock<GraphServiceClient>(mockAuthProvider.Object, mockHttpProvider.Object);

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


            var mockGraphService = new GraphService(mockGraphClient.Object, new NullLogger<GraphService>());
            var returnedACSUserId = mockGraphService.GetACSUserId();

            Assert.Equal(ACS_USER_ID, returnedACSUserId.Result);
        }


        [Fact]
        public void GetACSUserId_With_No_ACS_Extension_Returns_Null()
        {
            var mockAuthProvider = new Mock<IAuthenticationProvider>();
            var mockHttpProvider = new Mock<IHttpProvider>();
            var mockGraphClient = new Mock<GraphServiceClient>(mockAuthProvider.Object, mockHttpProvider.Object);

            User testUser = new User
            {
                Extensions = new UserExtensionsCollectionPage() { },
            };

            mockGraphClient.Setup(g => g.Me.Request().Expand("extensions").Select("id").GetAsync(CancellationToken.None)).Returns(Task.Run(() => testUser)).Verifiable();


            var mockGraphService = new GraphService(mockGraphClient.Object, new NullLogger<GraphService>());
            var returnedToken = mockGraphService.GetACSUserId();

            Assert.Null(returnedToken.Result);
        }

        [Fact]
        public void AddIdentityMapping_Returns_ACSUserId()
        {
            var mockAuthProvider = new Mock<IAuthenticationProvider>();
            var mockHttpProvider = new Mock<IHttpProvider>();
            var mockGraphClient = new Mock<GraphServiceClient>(mockAuthProvider.Object, mockHttpProvider.Object);

            const string ACS_USER_ID = "John";

            var extension = new OpenTypeExtension
            {
                ExtensionName = Configurations.Constants.ExtensionName,
                AdditionalData = new Dictionary<string, object>() { { IdentityMapping.IdentityMappingKeyName, ACS_USER_ID } },
            };
            mockGraphClient.Setup(g => g.Me.Extensions.Request().AddAsync(It.IsAny<OpenTypeExtension>(), CancellationToken.None)).Returns(Task.Run(() => (Extension)extension));


            var mockGraphService = new GraphService(mockGraphClient.Object, new NullLogger<GraphService>());
            var returnedACSUserId = mockGraphService.AddIdentityMapping(ACS_USER_ID);

            Assert.Equal(ACS_USER_ID, returnedACSUserId.Result);
        }



    }
}
