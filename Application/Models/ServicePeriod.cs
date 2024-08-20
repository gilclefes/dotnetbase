using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class ServicePeriod : BaseModel
{
    public int ServiceId { get; set; }
    public int PeriodId { get; set; }

    public virtual required Service Service { get; set; }
    public virtual required Period Period { get; set; }
}