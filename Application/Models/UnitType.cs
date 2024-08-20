using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class UnitType : BaseModel
{
    public UnitType()
    {
        Prices = new HashSet<Price>();
    }
    public required string Name { get; set; }

    public string? Code { get; set; }
    public Boolean? Status { get; set; }
    public string? Description { get; set; }

    public virtual ICollection<Price> Prices { get; set; }
}