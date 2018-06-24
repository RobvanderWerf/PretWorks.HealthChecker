namespace PretWorks.HealthChecker.Models
{
    public class HealthCheckerResult
    {
        /// <summary>
        /// Overall status
        /// </summary>
        public HealthCheckerStatus Status { get; set; }

        /// <summary>
        /// Total time it took to run all tests in ms
        /// </summary>
        public long Time { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Message { get; set; }
    }
}