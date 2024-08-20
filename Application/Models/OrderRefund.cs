using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class OrderRefund : BaseModel
{
    public OrderRefund()
    {
        OrderRefundDetails = new HashSet<OrderRefundDetail>();
    }
    public int OrderId { get; set; }
    public int ClientId { get; set; }
    public required string ExtransacitonId { get; set; }
    public required decimal RefundAmount { get; set; }
    public required int RefundRetry { get; set; }
    public required DateTime LastRetryDate { get; set; }
    public string? RefundStatus { get; set; }

    public string? RefundExTransactionId { get; set; }
    public string? RefundReason { get; set; }

    public virtual required Order Order { get; set; }
    public virtual required Client Client { get; set; }

    public virtual ICollection<OrderRefundDetail> OrderRefundDetails { get; set; }
}