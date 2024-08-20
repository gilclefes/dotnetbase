using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class Payment : BaseModel
{
    public Payment()
    {
        OrderPayments = new HashSet<OrderPayment>();
        ClientSubscriptionDetailPayments = new HashSet<ClientSubscriptionDetailPayment>();
    }
    public required string RefNumber { get; set; }
    public required string PaymentMethod { get; set; }
    public string? ExTransactionId { get; set; }
    public required DateTime PaymentStart { get; set; }
    public DateTime PaymentEnd { get; set; }
    public required decimal PaymentAmount { get; set; }

    public int CurrencyId { get; set; }

    public required string PaymentStatus { get; set; }
    public required string PaymentType { get; set; }
    public string? Name { get; set; }

    public string? Descripion { get; set; }

    public virtual required Currency Currency { get; set; }

    public virtual ICollection<OrderPayment> OrderPayments { get; set; }
    public virtual ICollection<ClientSubscriptionDetailPayment> ClientSubscriptionDetailPayments { get; set; }
}