using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class ServiceType : BaseModel
{
    // whether itemized or not
    public ServiceType()
    {
        Services = new HashSet<Service>();
    }
    public required string Name { get; set; }

    public required string Code { get; set; }

    public Boolean Itemized { get; set; }
    public Boolean? Status { get; set; }
    public string? Description { get; set; }

    public virtual ICollection<Service> Services { get; set; }
}