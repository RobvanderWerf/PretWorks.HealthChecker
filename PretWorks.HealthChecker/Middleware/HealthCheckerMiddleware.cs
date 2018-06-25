using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PretWorks.HealthChecker.Interfaces;
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

        public async Task Invoke(HttpContext httpContext, IServiceProvider serviceProvider)
        {
            if (HttpMethods.IsGet(httpContext.Request.Method) && httpContext.Request.Path.StartsWithSegments(_settings.Path))
            {
                _logger.LogDebug("HealthChecker middleware triggered");

                var stopwatch = Stopwatch.StartNew();

                var results = new List<HealthCheckerResult>();

                foreach (var healthCheckerType in _settings.HealthCheckers)
                {
                    if (ActivatorUtilities.CreateInstance(serviceProvider, healthCheckerType) is IHealthChecker healthChecker)
                    {
                        var individualStopwatch = Stopwatch.StartNew();
                        var healthResult = new HealthCheckerResult();
                        try
                        {
                            healthResult = await healthChecker.RunAsync();
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e, "Healthchecker failed");
                            healthResult.Status = HealthCheckerStatus.Error;
                            healthResult.Message = $"{healthChecker.GetType()} failed to run to completion";
                        }

                        individualStopwatch.Stop();
                        healthResult.Time = individualStopwatch.ElapsedMilliseconds;
                        results.Add(healthResult);
                    }
                }

                stopwatch.Stop();

                var status = HealthCheckerStatus.Ok;
                httpContext.Response.StatusCode = _settings.HttpStatusCodeOnOk;

                if (results.Any(a => a.Status == HealthCheckerStatus.Error))
                {
                    status = HealthCheckerStatus.Error;
                    httpContext.Response.StatusCode = _settings.HttpStatusCodeOnError;
                }
                else if (results.Any(a => a.Status == HealthCheckerStatus.Warning))
                {
                    status = HealthCheckerStatus.Warning;
                    httpContext.Response.StatusCode = _settings.HttpStatusCodeOnWarning;
                }

                httpContext.Response.ContentType = "application/json";
                await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(
                    new
                    {
                        Totaltime = stopwatch.ElapsedMilliseconds,
                        HealthCheckers = results,
                        Status = status
                    },
                    new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    }
                ));

                return;
            }

            await _next.Invoke(httpContext);
        }
    }

    public static class HealthCheckerMiddlewareExtensions
    {
        public static IApplicationBuilder UseHealthChecker(this IApplicationBuilder builder, Action<HealthCheckerSettings> configureSettings)
        {
            var settings = new HealthCheckerSettings();

            configureSettings(settings);

            return builder.UseMiddleware<HealthCheckerMiddleare>(settings);
        }
    }
}