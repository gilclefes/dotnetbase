using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class ClientSubscriptionDetail : BaseModel
{

    public ClientSubscriptionDetail()
    {
        ClientSubscriptionDetailPayments = new HashSet<ClientSubscriptionDetailPayment>();
    }
    public int ClientSubscriptionId { get; set; }
    public int? SubscriptionPlanPriceId { get; set; }

    // subscriptionId, datesubscribed, expirydate,TotalAmount, amountpaid
    public DateTime DateSubscibed { get; set; }
    public DateTime ExpiryDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal Discount { get; set; }

    public decimal TaxAmount { get; set; }

    public string? Status { get; set; }

    public virtual ClientSubscription? ClientSubscription { get; set; }
    public virtual SubscriptionPlanPrice? SubscriptionPlanPrice { get; set; }

    public virtual ICollection<ClientSubscriptionDetailPayment> ClientSubscriptionDetailPayments { get; set; }

}