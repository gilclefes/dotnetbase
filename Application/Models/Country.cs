using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class Country : BaseModel
{

    public Country()
    {
        Cities = new HashSet<City>();
    }
    public required string Name { get; set; }

    public required string Code { get; set; }
    public Boolean? Status { get; set; }

    public virtual ICollection<City> Cities { get; set; }
}