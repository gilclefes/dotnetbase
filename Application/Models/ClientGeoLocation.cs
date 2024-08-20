using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class ClientGeoLocation : BaseModel
{
    public int ClientId { get; set; }

    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public decimal Accuracy { get; set; }

    public decimal? AltitudeAccuracy { get; set; }

    public decimal? Altitude { get; set; }

    public decimal? Speed { get; set; }

    public decimal? Heading { get; set; }

    public required virtual Client Client { get; set; }
}