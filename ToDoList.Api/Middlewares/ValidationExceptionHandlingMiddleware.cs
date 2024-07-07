using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ToDoList.Middlewares
{
    public class ValidationExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ValidationExceptionHandlingMiddleware(RequestDelegate next) 
        {
            _next = next;
        }
        public async Task InvokeAsync (HttpContext httpcontext)
        {
            try
            {
                await _next(httpcontext);
            }
            catch (ValidationException ex)
            {
                await HandleValidationExceptionAsync(httpcontext, ex);

            }
            catch (Exception ex)
            {
                await HandleNotFoundExceptionAsync(httpcontext, ex);
            }
        }
        private static Task HandleValidationExceptionAsync(HttpContext httpcontext, ValidationException exception)
        {
            string[] errors = exception.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}").ToArray();
            ProblemDetails problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Type = "ValidationFailure",
                Title = "Validation error",
                Detail = string.Join(", ", errors),
                Instance = httpcontext.Request.Path
            };
            httpcontext.Response.ContentType = "application/json";
            httpcontext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return httpcontext.Response.WriteAsync(JsonConvert.SerializeObject(new { problemDetails }));
        }
        private static Task HandleNotFoundExceptionAsync(HttpContext httpcontext, Exception exception)
        {
            ProblemDetails problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Type = "NotFound",
                Title = "Resource not found",
                Detail = exception.Message,
                Instance = httpcontext.Request.Path
            };
            httpcontext.Response.ContentType = "application/json";
            httpcontext.Response.StatusCode = StatusCodes.Status404NotFound;
            return httpcontext.Response.WriteAsync(JsonConvert.SerializeObject(new { problemDetails }));
        }
    }
}
