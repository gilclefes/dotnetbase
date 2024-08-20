using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class IdType : BaseModel
{
    public IdType()
    {
        Clients = new HashSet<Client>();
        ServiceProviders = new HashSet<ServiceProvider>();
        Partners = new HashSet<Partner>();
    }
    public required string Name { get; set; }

    public string? Code { get; set; }
    public Boolean Status { get; set; }

    public virtual ICollection<Client> Clients { get; set; }

    public virtual ICollection<ServiceProvider> ServiceProviders { get; set; }

    public virtual ICollection<Partner> Partners { get; set; }

}