using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class SubscriptionPlanChargeExemption : BaseModel
{


    public int ChargeId { get; set; }
    public int SubscriptionId { get; set; }
    public string? Description { get; set; }

    public Boolean Status { get; set; }

    public required virtual Charge Charge { get; set; }

    public required virtual SubscriptionPlan SubscriptionPlan { get; set; }


}