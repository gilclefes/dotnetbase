using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class OrderRefundDetailDto
{
    public int OrderRefundId { get; set; }

    public required DateTime RequestDate { get; set; }
    public string? RefundStatus { get; set; }
    public string? ResponseDetails { get; set; }


}