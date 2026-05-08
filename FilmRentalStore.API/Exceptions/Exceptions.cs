using System.Net;
using System.Text.Json;

namespace FilmRentalStore.API.Exceptions
{
    public class Exceptions
    {
        public class NotFoundException : Exception
        {
            public NotFoundException(string message) : base(message) { }
            public NotFoundException(string entity, object key)
                : base($"{entity} with id '{key}' was not found.") { }
        }

        public class BadRequestException : Exception
        {
            public BadRequestException(string message) : base(message) { }
        }

        public class ConflictException : Exception
        {
            public ConflictException(string message) : base(message) { }
        }

        public class UnauthorizedException : Exception
        {
            public UnauthorizedException(string message) : base(message) { }
        }

        // ── Error Response Model ───────────────────────────────────────────────────

        public class ApiErrorResponse
        {
            public int StatusCode { get; set; }
            public string Message { get; set; } = null!;
            public string? Details { get; set; }
            public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        }

        // ── Global Exception Middleware ────────────────────────────────────────────

        public class GlobalExceptionMiddleware
        {
            private readonly RequestDelegate _next;
            private readonly ILogger<GlobalExceptionMiddleware> _logger;

            public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
                    await HandleExceptionAsync(context, ex);
                }
            }

            private static Task HandleExceptionAsync(HttpContext context, Exception exception)
            {
                var (statusCode, message) = exception switch
                {
                    NotFoundException => (HttpStatusCode.NotFound, exception.Message),
                    BadRequestException => (HttpStatusCode.BadRequest, exception.Message),
                    ConflictException => (HttpStatusCode.Conflict, exception.Message),
                    UnauthorizedException => (HttpStatusCode.Unauthorized, exception.Message),
                    FluentValidation.ValidationException ve
                                          => (HttpStatusCode.UnprocessableEntity, BuildValidationMessage(ve)),
                    _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
                };

                var response = new ApiErrorResponse
                {
                    StatusCode = (int)statusCode,
                    Message = message,
                    Details = exception is not NotFoundException
                              && exception is not BadRequestException
                              && exception is not ConflictException
                              && exception is not FluentValidation.ValidationException
                                 ? exception.Message : null
                };

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)statusCode;

                var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                return context.Response.WriteAsync(json);
            }

            private static string BuildValidationMessage(FluentValidation.ValidationException ve)
            {
                var errors = ve.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}");
                return string.Join(" | ", errors);
            }
        }

        // ── Extension Method ──────────────────────────────────────────────────────

        //public static class MiddlewareExtensions
        //{
        //    public static IApplicationBuilder UseGlobalExceptionMiddleware(this IApplicationBuilder app)
        //        => app.UseMiddleware<GlobalExceptionMiddleware>();
        //}

    }
}
