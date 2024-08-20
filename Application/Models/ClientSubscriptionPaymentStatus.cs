using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public static class ClientSubscriptionPaymentStatus
{
    // whether order if paid or based on subscription
    public const string PAID = nameof(PAID);
    public const string PAYMENTFAILED = nameof(PAYMENTFAILED);

    public const string ACTIVE = nameof(ACTIVE);
    public const string PENDING = nameof(PENDING);

    public const string CANCELLED = nameof(CANCELLED);

    public const string EXPIRED = nameof(EXPIRED);
}