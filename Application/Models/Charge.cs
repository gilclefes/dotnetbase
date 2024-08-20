using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class Charge : BaseModel
{

    public Charge()
    {
        OrderCharges = new HashSet<OrderCharge>();
        SubscriptionPlanCharges = new HashSet<SubscriptionPlanCharge>();
        ServiceCharges = new HashSet<ServiceCharge>();
        SubscriptionPlanChargeExemptions = new HashSet<SubscriptionPlanChargeExemption>();
    }

    public required string Name { get; set; }

    public required string Code { get; set; }
    public Boolean? Status { get; set; }
    public string? Description { get; set; }

    public Boolean IsYaboCharge { get; set; } = false;

    public required string AmountType { get; set; }//percentage or fixed

    public required int CategoryId { get; set; }

    public virtual required ChargeCategory ChargeCategory { get; set; }

    public virtual ICollection<OrderCharge> OrderCharges { get; set; }

    public virtual ICollection<ServiceCharge> ServiceCharges { get; set; }

    public virtual ICollection<SubscriptionPlanCharge> SubscriptionPlanCharges { get; set; }
    public virtual ICollection<SubscriptionPlanChargeExemption> SubscriptionPlanChargeExemptions { get; set; }
}