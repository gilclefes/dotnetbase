using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class ClientSubscriptionDetailPayment : BaseModel
{

    public int ClientSubscriptionDetailId { get; set; }

    public int PaymentId { get; set; }

    public virtual required ClientSubscriptionDetail ClientSubscriptionDetail { get; set; }

    public virtual required Payment Payment { get; set; }

}