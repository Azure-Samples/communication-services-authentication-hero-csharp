// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE.md in the project root for license information.

using System;
using ACS.Solution.Authentication.Server.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace ACS.Solution.Authentication.Server
{
    // Configuration and setup.
    public class Startup
    {
        public IConfiguration configuration { get; }

        // Initializes new instance of Startup.
        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Register services

            // Add app settings
            services.AddAppSettings(configuration);

            // Add core services- e.g. ACS service | Graph service
            services.AddCoreServices();

            // Add downstream apis
            services.AddDownstreamApis(configuration);

            // Add custom CORS
            services.AddCustomCors();

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ACS Solution Authentication Server API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // For more the Middleware Order information, see https://docs.microsoft.com/en/aspnet/core/fundamentals/middleware/?view=aspnetcore-6.0#middleware-order
            if (env.IsDevelopment())
            {
                /*
                  Since IdentityModel version 5.2.1 (or since Microsoft.AspNetCore.Authentication.JwtBearer version 2.2.0),
                  PII hiding in log files is enabled by default for GDPR concerns.
                  For debugging/development purposes, one can enable additional detail in exceptions by setting IdentityModelEventSource.ShowPII to true.
                  Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
                */
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(context => context.SwaggerEndpoint("/swagger/v1/swagger.json", "ACS Solution Authentication Server API v1"));
            }
            else
            {
                // The best practice is to manage exception handling in one place instead of add try-catch everywhere in actions - A.K.A controllers (Keep action cleaner and simpler)
                app.UseExceptionHandler(config =>
                {
                    config.Run(async context =>
                    {
                        IExceptionHandlerPathFeature exception = context.Features.Get<IExceptionHandlerPathFeature>();

                        if (exception != null)
                        {
                            Exception error = exception.Error;
                            var response = new { code = StatusCodes.Status500InternalServerError, message = error.Message, stack_trace = error.StackTrace };
                            await context.Response.WriteAsJsonAsync(response);
                        }
                    });
                });
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
