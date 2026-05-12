using FilmRentalStore.API.Exceptions;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Text.Json;

namespace FilmRentalStore.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            catch (SecurityTokenException ex)
            {
                _logger.LogWarning("JWT validation failed: {Message}", ex.Message);

                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    success = false,
                    message = "Unauthorized"
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
            catch (ApiException ex)
            {
                _logger.LogWarning("Handled API exception ({StatusCode}): {Message}", ex.StatusCode, ex.Message);

                context.Response.StatusCode = ex.StatusCode;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    success = false,
                    message = ex.Message
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception while processing request.");

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    success = false,
                    message = "Something went wrong."
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }
}
