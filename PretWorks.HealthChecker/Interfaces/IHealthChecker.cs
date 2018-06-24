using System.Threading.Tasks;
using PretWorks.HealthChecker.Models;

namespace PretWorks.HealthChecker.Interfaces
{
    public interface IHealthChecker
    {
        Task<HealthCheckerResult> RunAsync();
    }
}