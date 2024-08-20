using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class Currency : BaseModel
{
    public Currency()
    {
        SubscriptionPlanPrices = new HashSet<SubscriptionPlanPrice>();
        Payments = new HashSet<Payment>();
    }
    public required string Name { get; set; }

    public required string Code { get; set; }
    public Boolean Status { get; set; }

    public string? Symbol { get; set; }

    public virtual ICollection<SubscriptionPlanPrice> SubscriptionPlanPrices { get; set; }
    public virtual ICollection<Payment> Payments { get; set; }
}