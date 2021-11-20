using ProductReviews.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ProductReviews.CustomExceptionMiddleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(ArgumentNullException ex)
            {
                _logger.LogError("A new Argument Null Exception has been thrown: " + ex);
                await HandleExceptionAsync(context, HttpStatusCode.BadRequest, ex.Message);
            }
            catch(ArgumentOutOfRangeException ex)
            {
                _logger.LogError("A new Argument Out Of Range Exception has been thrown: " + ex);
                await HandleExceptionAsync(context, HttpStatusCode.BadRequest, ex.Message);
            }
            catch(ValidationException ex)
            {
                _logger.LogError("A new Validation Exception has been thrown: " + ex);
                await HandleExceptionAsync(context, HttpStatusCode.BadRequest, ex.Message);
            }
            catch(ResourceNotFoundException ex)
            {
                _logger.LogError("A new Resource Not Found Exception has been thrown: " + ex);
                await HandleExceptionAsync(context, HttpStatusCode.NotFound, ex.Message);
            }
            catch(UnauthorizedAccessException ex)
            {
                _logger.LogError("A new Unauthorized Access Exception has been thrown: " + ex);
                await HandleExceptionAsync(context, HttpStatusCode.Unauthorized, ex.Message);
            }
            catch(Exception ex)
            {
                _logger.LogError("Unexpected ecxeption has occurred: " + ex);
                await HandleExceptionAsync(context, HttpStatusCode.InternalServerError, "Unable to process request. Please try again or contact support if this continues.");
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, HttpStatusCode httpStatusCode, string exceptionMessage)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)httpStatusCode;
            ErrorModel errorModel = new ErrorModel()
            {
                StatusCode = context.Response.StatusCode,
                ErrorMessage = exceptionMessage
            };
            await context.Response.WriteAsync(errorModel.ToString());
        }
    }
}
