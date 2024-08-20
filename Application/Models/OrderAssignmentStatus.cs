using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public static class OrderAssignmentStatus
{
    public const string NEW = nameof(NEW);
    public const string ACCEPTED = nameof(ACCEPTED);
    public const string REJECTED = nameof(REJECTED);
}