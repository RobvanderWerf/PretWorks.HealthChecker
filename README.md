## Welcome to PretWorks.HealthChecker

Inspired by [NotDeadYet](https://github.com/NotDeadYetContributors/NotDeadYet) but rebuild from the ground up for asp.net core as a .net standard 2.0 package

## Basic installation

Getting the package:

    Install-Package PretWorks.HealthChecker

In startup.cs:

    app.UseHealthChecker(settings =>
            {
                settings.AddHealthChecker<BasicHealthChecker>();
            });
            
Run your site and go to:

    http://yoursite.com/status
