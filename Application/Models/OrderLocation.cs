using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class OrderLocation : BaseModel
{
    public int OrderId { get; set; }

    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public decimal Accuracy { get; set; }

    public string? AddressLine { get; set; }
    public string? City { get; set; }
    public string? StateProvince { get; set; }
    public string? CountryRegion { get; set; }
    public string? PostalCode { get; set; }


    public virtual required Order Order { get; set; }
}