using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class RegStatus : BaseModel
{

    public RegStatus()
    {
        Clients = new HashSet<Client>();
        Partners = new HashSet<Partner>();
        ServiceProviders = new HashSet<ServiceProvider>();
    }
    public required string Name { get; set; }
    public string? Code { get; set; }
    public Boolean Status { get; set; }

    public virtual ICollection<Client> Clients { get; set; }
    public virtual ICollection<Partner> Partners { get; set; }
    public virtual ICollection<ServiceProvider> ServiceProviders { get; set; }
}