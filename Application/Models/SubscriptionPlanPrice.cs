using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class SubscriptionPlanPrice : BaseModel
{

    public SubscriptionPlanPrice()
    {
        ClientSubscriptionDetails = new HashSet<ClientSubscriptionDetail>();
    }
    public int SubscriptionId { get; set; }
    public int PeriodId { get; set; }

    public decimal Amount { get; set; }
    public int CurrencyId { get; set; }

    public string? Description { get; set; }

    public Boolean Status { get; set; }

    public Boolean? IsFavorite { get; set; } = false;

    public required virtual SubscriptionPlan SubscriptionPlan { get; set; }
    public required virtual Period Period { get; set; }
    public required virtual Currency Currency { get; set; }

    public virtual ICollection<ClientSubscriptionDetail> ClientSubscriptionDetails { get; set; }

}