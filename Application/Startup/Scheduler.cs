using dotnetbase.Application.Tasks;
using Coravel;

namespace dotnetbase.Application.Startup
{
    public static class Scheduler
    {
        public static IServiceProvider RegisterScheduledJobs(this IServiceProvider services)
        {
            services.UseScheduler(scheduler =>
            {
                // example scheduled job
                scheduler
                   .Schedule<ExampleTask>()
                   .EveryThirtyMinutes().PreventOverlapping(nameof(ExampleTask));

                // scheduler

            });
            return services;
        }
    }
}
