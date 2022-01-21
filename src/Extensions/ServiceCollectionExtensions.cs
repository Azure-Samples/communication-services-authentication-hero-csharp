﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE.md in the project root for license information.

using ACS.Solution.Authentication.Server.Interfaces;
using ACS.Solution.Authentication.Server.Models;
using ACS.Solution.Authentication.Server.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;

namespace ACS.Solution.Authentication.Server.Extensions
{
    /// <summary>
    /// Combine service collection by moving related groups of registrations to an extension method to register services.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add cross-origin resource sharing services to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
        /// </summary>
        /// <param name="services">A collection of service descriptors.</param>
        public static void AddCustomCors(this IServiceCollection services)
        {
            // Refer to this article if you require more information on CORS
            // https://docs.microsoft.com/en-us/aspnet/core/security/cors
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });
        }

        /// <summary>
        /// Add all setting models realted to the appsettings to the service container with Configure and bound to configuration.
        /// </summary>
        /// <param name="services">A collection of service descriptors.</param>
        /// <param name="configuration">Represents a set of key/value application configuration properties.</param>
        public static void AddAppSettings(
             this IServiceCollection services, IConfiguration configuration)
        {
            // Add CommunicationServicesSettingsModel to the service container with Configure and bound to configuration
            services.Configure<CommunicationServicesSettingsModel>(
                configuration.GetSection(CommunicationServicesSettingsModel.CommunicationServicesSettingsName));
        }

        /// <summary>
        /// Add core services (e.g. ACS service | Graph service) to the service container.
        /// </summary>
        /// <param name="services">A collection of service descriptors.</param>
        public static void AddCoreServices(this IServiceCollection services)
        {
            /*
             * In the case of Singleton service, only one instance is created and shared across applications.
             * If we click on refresh button or load the UI on the different tab of a browser (which is nothing but Request 2),
             * those ids will remain the same.
             */
            // Add ACS service
            services.AddSingleton<IACSService, ACSService>();
            /*
             * In the case of Scoped service, a single instance is created per request and the same instance is shared across the request.
             * That is why operation Ids are the same for first instance as well as second instance of Request 1.
             * But if we click on refresh button or load the UI on different tab of a browser (which is nothing but Request 2), new ids are generated.
             */
            // Add Graph service
            services.AddScoped<IGraphService, GraphService>();
        }

        /// <summary>
        /// Add all downstream apis to the service container.
        /// </summary>
        /// <param name="services">A collection of service descriptors.</param>
        /// <param name="configuration">Represents a set of key/value application configuration properties.</param>
        public static void AddDownstreamApis(this IServiceCollection services, IConfiguration configuration)
        {
            // Add the Microsoft Graph api as one of downstream apis
            // For more information, see https://docs.microsoft.com/en-us/azure/active-directory/develop/scenario-web-api-call-api-app-configuration?tabs=aspnetcore#option-1-call-microsoft-graph
            services.AddMicrosoftIdentityWebApiAuthentication(configuration, AzureActiveDirectorySettingsModel.AzureActiveDirectorySettingsName)
                    .EnableTokenAcquisitionToCallDownstreamApi()
                    .AddMicrosoftGraph(configuration.GetSection(GraphSettingsModel.GraphSettingsName))
                    .AddInMemoryTokenCaches();
        }
    }
}