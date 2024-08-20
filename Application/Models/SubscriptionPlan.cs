using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class SubscriptionPlan : BaseModel
{
    public SubscriptionPlan()
    {
        SubscriptionPlanPrices = new HashSet<SubscriptionPlanPrice>();
        SubscriptionPlanCharges = new HashSet<SubscriptionPlanCharge>();
        SubscriptionPlanServices = new HashSet<SubscriptionPlanService>();
        ClientSubscriptions = new HashSet<ClientSubscription>();
        SubscriptionPlanBenefits = new HashSet<SubscriptionPlanBenefit>();
        SubscriptionPlanChargeExemptions = new HashSet<SubscriptionPlanChargeExemption>();
    }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? TermAndConditions { get; set; }
    public Boolean Status { get; set; }

    public int OrderPeriodId { get; set; }
    public int OrderFrequency { get; set; }

    public int MinOrder { get; set; }

    public decimal? MinOrderPenalty { get; set; }

    public required virtual Period Period { get; set; }

    public virtual ICollection<SubscriptionPlanPrice> SubscriptionPlanPrices { get; set; }
    public virtual ICollection<SubscriptionPlanBenefit> SubscriptionPlanBenefits { get; set; }
    public virtual ICollection<SubscriptionPlanCharge> SubscriptionPlanCharges { get; set; }

    public virtual ICollection<SubscriptionPlanChargeExemption> SubscriptionPlanChargeExemptions { get; set; }
    public virtual ICollection<SubscriptionPlanService> SubscriptionPlanServices { get; set; }

    public virtual ICollection<ClientSubscription> ClientSubscriptions { get; set; }
}