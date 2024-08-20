using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class City : BaseModel
{

    public required string Name { get; set; }

    public required string Code { get; set; }
    public Boolean? Status { get; set; }

    public required int CountryId { get; set; }

    public virtual Country Country { get; set; }

}