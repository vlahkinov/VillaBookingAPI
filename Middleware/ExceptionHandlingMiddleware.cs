using System.Net;
using System.Text.Json;
using VillaBookingAPI.Models;

namespace VillaBookingAPI.Middleware
{
    /// <summary>
    /// Middleware за глобална обработка на изключения.
    /// Хваща всички необработени грешки и връща стандартизиран ApiResponse.
    /// </summary>
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new ApiResponse
            {
                IsSuccess = false,
                StatusCode = HttpStatusCode.InternalServerError,
                Errors = new List<string>
                {
                    "An unexpected error occurred. Please try again later."
                }
            };

            // В Development режим добавяме детайли за грешката
            var env = context.RequestServices.GetService<IWebHostEnvironment>();
            if (env != null && env.IsDevelopment())
            {
                response.Errors.Add($"Details: {exception.Message}");
                if (exception.InnerException != null)
                {
                    response.Errors.Add($"Inner: {exception.InnerException.Message}");
                }
            }

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }
    }

    /// <summary>
    /// Extension method за регистриране на middleware-а в pipeline.
    /// </summary>
    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
