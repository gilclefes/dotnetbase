using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class OperatingCity : BaseModel
{
    public int CityId { get; set; }
    public Boolean Status { get; set; }
    public virtual required City City { get; set; }
}