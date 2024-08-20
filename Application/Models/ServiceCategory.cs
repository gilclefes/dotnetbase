using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class ServiceCategory : BaseModel
{

    public ServiceCategory()
    {
        Services = new HashSet<Service>();
    }
    public required string Name { get; set; }

    public string? Code { get; set; }
    public Boolean? Status { get; set; }
    public string? Description { get; set; }
    public string? Logo { get; set; }

    public virtual ICollection<Service> Services { get; set; }
}