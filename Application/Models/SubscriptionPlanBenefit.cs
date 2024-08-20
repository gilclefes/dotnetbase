using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class SubscriptionPlanBenefit : BaseModel
{


    public int SubscriptionId { get; set; }
    public required string Benefit { get; set; }
    public int Rank { get; set; }
    public Boolean Status { get; set; }

    public required virtual SubscriptionPlan SubscriptionPlan { get; set; }



}