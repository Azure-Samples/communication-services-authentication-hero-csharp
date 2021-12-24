using Xunit;
using Moq;
using Microsoft.Graph;
using System.Threading;
using System.Threading.Tasks;

namespace TokenApi.Test
{
    public class TokenControllerUnitTests
    {
        [Fact]
        public void Valid_UserPrincipalName_Returned()
        {
            // Setup dependencies
            var mockAuthProvider = new Mock<IAuthenticationProvider>();
            var mockHttpProvider = new Mock<IHttpProvider>();
            var mockGraphClient = new Mock<GraphServiceClient>(mockAuthProvider.Object, mockHttpProvider.Object);
            var graphClient = mockGraphClient.Object;
            var tokenController = new AcsTokenApi.Controllers.TokenController(graphClient);

            const string USER_PRINCIPAL_NAME = "Bob";

            // Act
            User testUser = new User
            {
                UserPrincipalName = USER_PRINCIPAL_NAME,
            };
            mockGraphClient.Setup(g => g.Me.Request().GetAsync(CancellationToken.None)).Returns(Task.Run(() => testUser)).Verifiable();
            var returnedUserPrincipalName = tokenController.Get();

            // Assert name returned is valid
            Assert.Equal(USER_PRINCIPAL_NAME, returnedUserPrincipalName);

        }
    }
}
