using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class ServiceProviderGeoLocation : BaseModel
{
    public int ServiceProviderId { get; set; }

    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public decimal Accuracy { get; set; }

    public decimal? AltitudeAccuracy { get; set; }

    public decimal? Altitude { get; set; }

    public decimal? Speed { get; set; }

    public decimal? Heading { get; set; }

    public required virtual ServiceProvider ServiceProvider { get; set; }
}