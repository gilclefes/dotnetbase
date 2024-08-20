using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class SubscriptionPlanService : BaseModel
{
    public int SubscriptionId { get; set; }
    public int ServiceId { get; set; }

    public string? Description { get; set; }

    public Boolean Status { get; set; }

    public virtual required SubscriptionPlan SubscriptionPlan { get; set; }
    public virtual required Service Service { get; set; }
}