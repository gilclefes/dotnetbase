using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Spark.Library.Logging;
using dotnetbase.Application.Models;
using ILogger = Spark.Library.Logging.ILogger;

namespace dotnetbase.Application.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;


        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }



        public async Task InvokeAsync(HttpContext context)
        {
            var _logger = context.RequestServices.GetService<ILogger>();
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger?.Error($"Something went wrong: {ex}");
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var response = context.Response;
            ResponseModel exModel = new ResponseModel();

            switch (exception)
            {
                case ApplicationException ex:
                    exModel.responseCode = (int)HttpStatusCode.BadRequest;
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    exModel.responseMessage = "Application Exception Occured, please retry after sometime.";
                    break;
                case FileNotFoundException ex:
                    exModel.responseCode = (int)HttpStatusCode.NotFound;
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    exModel.responseMessage = "The requested resource is not found.";
                    break;
                default:
                    exModel.responseCode = (int)HttpStatusCode.InternalServerError;
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    exModel.responseMessage = "Internal Server Error, Please retry after sometime";
                    break;

            }
            var exResult = JsonSerializer.Serialize(exModel);
            await context.Response.WriteAsync(exResult);
        }

    }
}