using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public static class PaymentStatus
{
    public const string PAID = nameof(PAID);
    public const string PENDING = nameof(PENDING);
    public const string PARTIAL = nameof(PARTIAL);
}