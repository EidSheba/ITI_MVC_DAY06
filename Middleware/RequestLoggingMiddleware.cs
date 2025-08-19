using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CourseManagement.Web.Middleware
{
    /// <summary>
    /// Middleware for logging incoming requests
    /// وسيط لتسجيل الطلبات الواردة
    /// </summary>
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Log request information / تسجيل معلومات الطلب
            var stopwatch = Stopwatch.StartNew();
            var requestId = Guid.NewGuid().ToString();

            _logger.LogInformation($"[{requestId}] Request starting: {context.Request.Method} {context.Request.Path}");

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation(
                    $"[{requestId}] Request finished in {stopwatch.ElapsedMilliseconds}ms - " +
                    $"Status: {context.Response.StatusCode}");
            }
        }
    }

    /// <summary>
    /// Extension method to add the middleware into pipeline
    /// دالة امتداد لإضافة الـ Middleware في الـ pipeline
    /// </summary>
    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}
