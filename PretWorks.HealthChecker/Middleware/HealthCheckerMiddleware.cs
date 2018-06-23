using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PretWorks.HealthChecker.Models;

namespace PretWorks.HealthChecker.Middleware
{
    public class HealthCheckerMiddleare
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<HealthCheckerMiddleare> _logger;
        private readonly HealthCheckerSettings _settings;

        public HealthCheckerMiddleare(RequestDelegate next, ILogger<HealthCheckerMiddleare> logger, HealthCheckerSettings settings)
        {
            _next = next;
            _logger = logger;
            _settings = settings;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (HttpMethods.IsGet(httpContext.Request.Method) && httpContext.Request.Path.StartsWithSegments(_settings.Path))
            {
                _logger.LogDebug("HealthChecker middleware triggered");

                var result = new HealthCheckerResult();

                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = (int)HttpStatusCode.OK;
                await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(result, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));

                return;
            }

            await _next.Invoke(httpContext);
        }
    }

    public static class HealthCheckerMiddlewareExtensions
    {
        public static IApplicationBuilder UseHealthChecker(this IApplicationBuilder builder,Action<HealthCheckerSettings> configureSettings)
        {
            var settings = new HealthCheckerSettings();

            configureSettings(settings);

            return builder.UseMiddleware<HealthCheckerMiddleare>(settings);
        }
    }
}