using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class OrderRefundDetail : BaseModel
{


    public int OrderRefundId { get; set; }

    public required DateTime RequestDate { get; set; }
    public string? RefundStatus { get; set; }
    public string? ResponseDetails { get; set; }
    public virtual required OrderRefund OrderRefund { get; set; }

}