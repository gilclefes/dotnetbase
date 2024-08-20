using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class Detergent : BaseModel
{

    public Detergent()
    {
        ServiceDetergents = new HashSet<ServiceDetergent>();
        OrderDetergents = new HashSet<OrderDetergent>();
    }

    public required string Name { get; set; }

    public string? Code { get; set; }
    public Boolean Status { get; set; }
    public string? Description { get; set; }

    public virtual ICollection<ServiceDetergent> ServiceDetergents { get; set; }
    public virtual ICollection<OrderDetergent> OrderDetergents { get; set; }

}