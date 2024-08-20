using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class CityTax : BaseModel
{
    public required string TaxName { get; set; }
    public required decimal TaxPercentage { get; set; }
    public int CityId { get; set; }
    public Boolean Status { get; set; }
    public virtual required City City { get; set; }
}