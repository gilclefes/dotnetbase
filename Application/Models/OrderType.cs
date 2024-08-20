using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public static class OrderType
{
    // whether order if paid or based on subscription
    public const string PAID = nameof(PAID);
    public const string SUB = nameof(SUB);
}