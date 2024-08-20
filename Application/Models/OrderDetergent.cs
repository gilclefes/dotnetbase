using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class OrderDetergent : BaseModel
{
    public int OrderId { get; set; }
    public int ServiceId { get; set; }
    public int DetergentId { get; set; }
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    public string? Comments { get; set; }
    public virtual required Order Order { get; set; }
    public virtual Service? Service { get; set; }
    public virtual Detergent? Detergent { get; set; }


}