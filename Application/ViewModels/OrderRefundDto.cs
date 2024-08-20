using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class OrderRefundDto
{


    public int Id { get; set; }
    public required string OrderRefNumber { get; set; }
    public required string ClientuserName { get; set; }
    public required string ExtransacitonId { get; set; }
    public required decimal RefundAmount { get; set; }
    public required int RefundRetry { get; set; }
    public required DateTime LastRetryDate { get; set; }
    public string? RefundStatus { get; set; }

    public string? RefundExTransactionId { get; set; }
    public string? RefundReason { get; set; }

    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }



}