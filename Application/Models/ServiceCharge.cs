using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class ServiceCharge : BaseModel
{
    public int ServiceId { get; set; }
    public int ChargeId { get; set; }

    public decimal Price { get; set; }

    public virtual required Service Service { get; set; }
    public virtual required Charge Charge { get; set; }
}