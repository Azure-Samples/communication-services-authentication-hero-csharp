// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE.md in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ACS.Solution.Authentication.Server.Models;
using ACS.Solution.Authentication.Server.Services;
using Microsoft.Extensions.Options;
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
            Mock<IAuthenticationProvider> mockAuthProvider = new();
            Mock<IHttpProvider> mockHttpProvider = new();
            Mock<GraphServiceClient> mockGraphClient = new(mockAuthProvider.Object, mockHttpProvider.Object);
            Mock<IOptionsMonitor<GraphSettingsModel>> optionsMonitorMock = new();

            const string ACS_USER_ID = "John";

            GraphSettingsModel graphSettingsModel = new();
            graphSettingsModel.ExtensionName = "com.contoso.identityMapping";

            optionsMonitorMock.SetupGet(g => g.CurrentValue).Returns(graphSettingsModel);
            string extensionNameMock = optionsMonitorMock.Object.CurrentValue.ExtensionName;

            var extension = new OpenTypeExtension
            {
                ExtensionName = extensionNameMock,
                Id = extensionNameMock,
                AdditionalData = new Dictionary<string, object>() { { IdentityMapping.IdentityMappingKeyName, ACS_USER_ID } },
            };

            User testUser = new()
            {
                Extensions = new UserExtensionsCollectionPage() { extension },
            };

            mockGraphClient.Setup(g => g.Me.Request().Expand("extensions").Select("id").GetAsync(CancellationToken.None)).Returns(Task.Run(() => testUser)).Verifiable();


            GraphService mockGraphService = new(mockGraphClient.Object, optionsMonitorMock.Object);
            Task<string> returnedACSUserId = mockGraphService.GetACSUserId();

            Assert.Equal(ACS_USER_ID, returnedACSUserId.Result);
        }


        [Fact]
        public void GetACSUserId_With_No_ACS_Extension_Returns_Null()
        {
            Mock<IAuthenticationProvider> mockAuthProvider = new();
            Mock<IHttpProvider> mockHttpProvider = new();
            Mock<GraphServiceClient> mockGraphClient = new(mockAuthProvider.Object, mockHttpProvider.Object);
            Mock<IOptionsMonitor<GraphSettingsModel>> optionsMonitorMock = new();

            User testUser = new()
            {
                Extensions = new UserExtensionsCollectionPage() { },
            };

            GraphSettingsModel graphSettingsModel = new();

            mockGraphClient.Setup(g => g.Me.Request().Expand("extensions").Select("id").GetAsync(CancellationToken.None)).Returns(Task.Run(() => testUser)).Verifiable();
            optionsMonitorMock.SetupGet(g => g.CurrentValue).Returns(graphSettingsModel);

            GraphService mockGraphService = new(mockGraphClient.Object, optionsMonitorMock.Object);
            Task<string> returnedToken = mockGraphService.GetACSUserId();

            Assert.Null(returnedToken.Result);
        }

    }
}
