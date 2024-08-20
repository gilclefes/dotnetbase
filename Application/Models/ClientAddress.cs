using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class ClientAddress : BaseModel
{
    public int ClientId { get; set; }

    public string? AddressLine { get; set; }
    public string? City { get; set; }
    public string? StateProvince { get; set; }
    public string? CountryRegion { get; set; }
    public string? PostalCode { get; set; }

    public Boolean Status { get; set; }

    public required virtual Client Client { get; set; }


}