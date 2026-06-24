using System.Diagnostics;

namespace IdentityService.Middleware
{
    public class HttpLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<HttpLoggingMiddleware> _logger;

        public HttpLoggingMiddleware(RequestDelegate next, ILogger<HttpLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var originalBodyStream = context.Response.Body;

            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();

                // Always copy buffered response back to original stream first
                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
                context.Response.Body = originalBodyStream;

                // Log after the response is safely written
                var clientAddress = context.Connection.RemoteIpAddress?.ToString() ?? "-";
                var port          = context.Connection.RemotePort;
                var httpVersion   = context.Request.Protocol;
                var statusCode    = context.Response.StatusCode;
                var statusText    = GetStatusText(statusCode);
                var elapsedMs     = stopwatch.Elapsed.TotalMilliseconds;

                var logMessage = $"{clientAddress}:{port} - \"{context.Request.Method} {context.Request.Path} {httpVersion}\" {statusCode} {statusText} ({elapsedMs:F2}ms)";
                _logger.LogInformation(logMessage);

                try
                {
                    var logFilePath       = Path.Combine(AppContext.BaseDirectory, "server.log");
                    var timestamp         = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss,fff");
                    var fileLogMessage    = $"{timestamp} INFO {logMessage}";
                    await File.AppendAllTextAsync(logFilePath, fileLogMessage + Environment.NewLine);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error writing to log file");
                }
            }
        }

        private static string GetStatusText(int statusCode) => statusCode switch
        {
            200 => "OK",
            201 => "Created",
            400 => "Bad Request",
            401 => "Unauthorized",
            404 => "Not Found",
            500 => "Internal Server Error",
            _   => "Unknown"
        };
    }
}
