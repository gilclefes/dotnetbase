using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class SubscriptionPlanCharge : BaseModel
{


    public int ChargeId { get; set; }
    public int SubscriptionId { get; set; }
    public required decimal Amount { get; set; }
    public string? Description { get; set; }

    public Boolean Status { get; set; }

    public required virtual Charge Charge { get; set; }

    public required virtual SubscriptionPlan SubscriptionPlan { get; set; }


}