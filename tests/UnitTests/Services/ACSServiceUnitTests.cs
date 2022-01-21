using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ACS.Solution.Authentication.Server.Interfaces;
using ACS.Solution.Authentication.Server.Models;
using ACS.Solution.Authentication.Server.Services;
using Azure;
using Azure.Communication;
using Azure.Communication.Identity;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
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
            var communicationUserIdentifierResponse = Response.FromValue(new CommunicationUserIdentifier(ACS_USER_ID), null);

            mockACSClient.Setup(g => g.CreateUserAsync(CancellationToken.None)).Returns(Task.Run(() => communicationUserIdentifierResponse)).Verifiable();

            var ACSService = new ACSService(optionsMonitorMock.Object, mockACSClient.Object );
            var returnedACSUserIdentity = ACSService.CreateACSUserIdentity();

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
            var accessTokenResponse = Response.FromValue<AccessToken>(new AccessToken(TOKEN_VALUE, DateTime.Now), null);

            optionsMonitorMock.SetupGet(g => g.CurrentValue).Returns(communicationServiceSettingsModel);
            var scopes = GetCommunicationTokenScopes(optionsMonitorMock.Object.CurrentValue);
            mockACSClient.Setup(g => g.GetTokenAsync(new CommunicationUserIdentifier(ACS_USER_ID), scopes, CancellationToken.None)).Returns(Task.Run(() => accessTokenResponse)).Verifiable();

            var ACSService = new ACSService(optionsMonitorMock.Object, mockACSClient.Object);
            var returnedACSUserIdentity = ACSService.CreateACSToken(ACS_USER_ID);

            Assert.Equal(TOKEN_VALUE, returnedACSUserIdentity.Result.Token);
        }

    }
}
