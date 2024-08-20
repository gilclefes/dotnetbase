using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class ClientSubscription : BaseModel
{

    public ClientSubscription()
    {
        ClientSubscriptionDetails = new HashSet<ClientSubscriptionDetail>();
    }
    public int ClientId { get; set; }
    public int SubscriptionId { get; set; }

    public int? CurrentSubscriptionDetailId { get; set; }

    public Boolean Status { get; set; }
    public DateTime FirstSubscriptionDate { get; set; }
    public DateTime LastRenewedDate { get; set; }
    public DateTime ExpiryDate { get; set; }

    public virtual required Client Client { get; set; }
    public virtual required SubscriptionPlan SubscriptionPlan { get; set; }

    public virtual ICollection<ClientSubscriptionDetail> ClientSubscriptionDetails { get; set; }

}