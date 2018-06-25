using System;
using System.Collections.Generic;
using PretWorks.HealthChecker.Interfaces;

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

        /// <summary>
        /// Http status code if all tests are OK
        /// </summary>
        public int HttpStatusCodeOnOk { get; set; } = 200;

        /// <summary>
        /// Http status code if 1 or more tests end up as warning
        /// </summary>
        public int HttpStatusCodeOnWarning { get; set; } = 200;

        /// <summary>
        /// Http status code if 1 ore more tests end up as error
        /// </summary>
        public int HttpStatusCodeOnError { get; set; } = 503;

        public void AddHealthChecker<T>() where T : IHealthChecker
        {
            HealthCheckers.Add(typeof(T));
        }

        public List<Type> HealthCheckers { get; } = new List<Type>();
    }
}