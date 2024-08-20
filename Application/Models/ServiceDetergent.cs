using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class ServiceDetergent : BaseModel
{
    public int ServiceId { get; set; }
    public int DetergentId { get; set; }

    public decimal Price { get; set; }
    public virtual required Service Service { get; set; }
    public virtual required Detergent Detergent { get; set; }
}