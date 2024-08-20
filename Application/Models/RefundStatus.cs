using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public static class RefundStatus
{
    public const string PENDING = nameof(PENDING);
    public const string FAILED = nameof(FAILED);
    public const string COMPLETED = nameof(COMPLETED);
}