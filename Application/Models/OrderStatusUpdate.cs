using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class OrderStatusUpdate : BaseModel
{
    public int OrderId { get; set; }
    public required string Status { get; set; }
    public DateTime StatusDate { get; set; }
    public string? UpdatedBy { get; set; }
    public string? Description { get; set; }
    public virtual required Order Order { get; set; }
}