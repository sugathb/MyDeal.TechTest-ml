using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;
using MyDeal.TechTest.Core.Models;
using MyDeal.TechTest.Core.Infrastructure;
using Serilog;

namespace MyDeal.TechTest.Core.Middleware
{
    public class ErrorHandlingMiddleware : IMiddleware
    {
        private readonly ILogger _logger = Log.ForContext<UserDetailsClient>();

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var problemDetail = new ProblemDetail
            {
                Type = "Server Error",
                Title = "Server Error",
                Status = 500,
                Detail = "An internal server error has occurred"
            };

            var json = JsonConvert.SerializeObject(problemDetail);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsync(json);
        }
    }
}
