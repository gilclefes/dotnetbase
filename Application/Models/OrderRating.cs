using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class OrderRating : BaseModel
{
    public int OrderId { get; set; }
    public required string RaterEmail { get; set; }
    public required string RatedEmail { get; set; }
    public int Rating { get; set; }
    public string? Message { get; set; }

    public virtual required Order Order { get; set; }
}