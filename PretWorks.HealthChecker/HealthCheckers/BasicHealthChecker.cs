using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PretWorks.HealthChecker.Interfaces;
using PretWorks.HealthChecker.Models;

namespace PretWorks.HealthChecker.HealthCheckers
{
    public class BasicHealthChecker : IHealthChecker
    {
        private ILogger<BasicHealthChecker> _logger;

        public BasicHealthChecker(ILogger<BasicHealthChecker> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Most basic healthchecker
        /// </summary>
        /// <returns></returns>
        Task<HealthCheckerResult> IHealthChecker.RunAsync()
        {
            var result = new HealthCheckerResult
            {
                Status = HealthCheckerStatus.Ok,
                Message = "I'm running fine"
            };

            return Task.FromResult(result);
        }
    }
}