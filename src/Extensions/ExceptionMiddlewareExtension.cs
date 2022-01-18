// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE.md in the project root for license information.

using ACS.Solution.Authentication.Server.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace ACS.Solution.Authentication.Server.Extensions
{
    /// <summary>
    /// Registered the UseExceptionHandler middleware.
    /// Then, populated the status code and the content type of our response, logged the error message,
    /// and finally returned the response with the custom created object.
    /// </summary>
    public static class ExceptionMiddlewareExtension
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
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
                        await context.Response.WriteAsync(new ErrorDetails()
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = exception.Error.Message,
                        }.ToString());
                    }
                });
            });
        }
    }
}
