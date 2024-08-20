using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class OrderMessage : BaseModel
{
    public int OrderId { get; set; }
    public required string MessageFrom { get; set; }
    public required string MessageTo { get; set; }

    public required string Message { get; set; }

    public required string MessageStatus { get; set; }
    public virtual required Order Order { get; set; }
}