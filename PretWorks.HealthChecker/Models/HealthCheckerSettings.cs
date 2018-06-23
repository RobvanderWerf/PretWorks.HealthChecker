namespace PretWorks.HealthChecker.Models
{
    public class HealthCheckerSettings
    {
        /// <summary>
        /// Path for the healthchecker to listen on
        /// </summary>
        public string Path { get; set; } = "/status";

        /// <summary>
        /// Timeout each healthcheck after x seconds
        /// </summary>
        public int Timeout { get; set; } = 10;
    }
}