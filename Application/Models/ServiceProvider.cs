using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class ServiceProvider : BaseModel
{
    public ServiceProvider()
    {
        ServiceProviderAddresses = new HashSet<ServiceProviderAddress>();
        ServiceProviderGeoLocations = new HashSet<ServiceProviderGeoLocation>();
    }
    public string? Code { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string PhoneNumber { get; set; }
    public int IdTypeId { get; set; }
    public required string IdNumber { get; set; }

    public string? Logo { get; set; }
    public virtual required IdType IdType { get; set; }

    public int RegStatusId { get; set; }

    public decimal? Rating { get; set; } = 100m;

    public virtual required RegStatus RegStatus { get; set; }

    public virtual ICollection<ServiceProviderAddress> ServiceProviderAddresses { get; set; }
    public virtual ICollection<ServiceProviderGeoLocation> ServiceProviderGeoLocations { get; set; }
}