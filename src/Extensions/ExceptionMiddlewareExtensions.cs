// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE.md in the project root for license information.

using System.Text.Json;
using ACS.Solution.Authentication.Server.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ACS.Solution.Authentication.Server.Extensions
{
    /// <summary>
    /// The extension class is used to register the UseExceptionHandler middleware, which
    /// populates the status code and the content type of our response, logs the error message,
    /// and returns the response with the custom created object.
    /// </summary>
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app, ILogger logger)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";

                    IExceptionHandlerFeature exception = context.Features.Get<IExceptionHandlerFeature>();

                    if (exception != null)
                    {
                        logger.LogError($"Something went wrong: {exception.Error}");

                        await context.Response.WriteAsync(JsonSerializer.Serialize(new ErrorDetails()
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = exception.Error.Message,
                        }));
                    }
                });
            });
        }
    }
}
