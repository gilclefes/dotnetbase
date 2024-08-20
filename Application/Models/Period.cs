using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class Period : BaseModel
{

    public Period()
    {
        ServicePeriods = new HashSet<ServicePeriod>();
        Prices = new HashSet<Price>();
        SubscriptionPlans = new HashSet<SubscriptionPlan>();
        SubscriptionPlanPrices = new HashSet<SubscriptionPlanPrice>();
    }
    public required string Name { get; set; }

    public string? Code { get; set; }
    public Boolean? Status { get; set; }
    public string? Description { get; set; }
    public int NoOfDays { get; set; }

    public virtual ICollection<ServicePeriod> ServicePeriods { get; set; }
    public virtual ICollection<Price> Prices { get; set; }
    public virtual ICollection<SubscriptionPlanPrice> SubscriptionPlanPrices { get; set; }
    public virtual ICollection<SubscriptionPlan> SubscriptionPlans { get; set; }
}