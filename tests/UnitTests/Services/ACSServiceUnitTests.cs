// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE.md in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using ACS.Solution.Authentication.Server.Models;
using ACS.Solution.Authentication.Server.Services;
using Azure;
using Azure.Communication;
using Azure.Communication.Identity;
using Azure.Core;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace ACS.Solution.Authentication.Server.UnitTests.Service
{
    public class ACSServiceUnitTests
    {

        private CommunicationTokenScope[] GetCommunicationTokenScopes(CommunicationServicesSettingsModel settings)
        {
            string[] scopes = settings.Scopes;
            CommunicationTokenScope[] tokenScopes = new CommunicationTokenScope[scopes.Length];

            for (int i = 0; i < scopes.Length; i++)
            {
                tokenScopes[i] = new CommunicationTokenScope(scopes[i]);
            }

            return tokenScopes;
        }

        [Fact]
        public void CreateACSUserIdentity_Returns_ACSUserId()
        {
            Mock<CommunicationIdentityClient> mockACSClient = new Mock<CommunicationIdentityClient>();
            Mock<IOptionsMonitor<CommunicationServicesSettingsModel>> optionsMonitorMock = new Mock<IOptionsMonitor<CommunicationServicesSettingsModel>>();

            const string ACS_USER_ID = "John";
            Response<CommunicationUserIdentifier> communicationUserIdentifierResponse = Response.FromValue(new CommunicationUserIdentifier(ACS_USER_ID), null);

            mockACSClient.Setup(g => g.CreateUserAsync(CancellationToken.None)).Returns(Task.Run(() => communicationUserIdentifierResponse)).Verifiable();

            ACSService ACSService = new ACSService(optionsMonitorMock.Object, mockACSClient.Object);
            Task<string> returnedACSUserIdentity = ACSService.CreateACSUserIdentity();

            Assert.Equal(ACS_USER_ID, returnedACSUserIdentity.Result);
        }

        [Fact]
        public void CreateACSToken_Returns_AccessToken()
        {
            Mock<CommunicationIdentityClient> mockACSClient = new Mock<CommunicationIdentityClient>();
            Mock<IOptionsMonitor<CommunicationServicesSettingsModel>> optionsMonitorMock = new Mock<IOptionsMonitor<CommunicationServicesSettingsModel>>();

            const string ACS_USER_ID = "John";
            const string TOKEN_VALUE = "John_TOKEN117";

            CommunicationServicesSettingsModel communicationServiceSettingsModel = new CommunicationServicesSettingsModel();
            communicationServiceSettingsModel.Scopes = new string[] { "chat", "voip" };
            Response<AccessToken> accessTokenResponse = Response.FromValue(new AccessToken(TOKEN_VALUE, DateTime.Now), null);

            optionsMonitorMock.SetupGet(g => g.CurrentValue).Returns(communicationServiceSettingsModel);
            CommunicationTokenScope[] scopes = GetCommunicationTokenScopes(optionsMonitorMock.Object.CurrentValue);
            mockACSClient.Setup(g => g.GetTokenAsync(new CommunicationUserIdentifier(ACS_USER_ID), scopes, CancellationToken.None)).Returns(Task.Run(() => accessTokenResponse)).Verifiable();

            ACSService ACSService = new ACSService(optionsMonitorMock.Object, mockACSClient.Object);
            Task<AccessToken> returnedACSUserIdentity = ACSService.CreateACSToken(ACS_USER_ID);

            Assert.Equal(TOKEN_VALUE, returnedACSUserIdentity.Result.Token);
        }

    }
}
