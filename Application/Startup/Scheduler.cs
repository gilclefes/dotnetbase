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
                   .Schedule<OrderAssignmentTask>()
                   .EveryThirtyMinutes().PreventOverlapping(nameof(OrderAssignmentTask));

                // scheduler
                scheduler
                  .Schedule<OrderRefundTask>()
                  .Hourly().PreventOverlapping(nameof(OrderRefundTask));

                // scheduler
                scheduler
                  .Schedule<SubscriptionUpdateTask>()
                  .Hourly().PreventOverlapping(nameof(SubscriptionUpdateTask));
            });
            return services;
        }
    }
}
