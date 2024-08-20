using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class Partner : BaseModel
{
  public Partner()
  {
    PartnerAddresses = new HashSet<PartnerAddress>();
    PartnerGeoLocations = new HashSet<PartnerGeoLocation>();
  }

  public string? ContactFirstName { get; set; }
  public string? ContactLastName { get; set; }

  public string? Code { get; set; }
  public required string CompanyName { get; set; }
  public required string Email { get; set; }
  public required string PhoneNumber { get; set; }
  public int IdTypeId { get; set; }
  public required string IdNumber { get; set; }
  public virtual required IdType IdType { get; set; }
  public int RegStatusId { get; set; }

  public decimal? Rating { get; set; } = 100m;

  public virtual required RegStatus RegStatus { get; set; }

  public string? Logo { get; set; }

  public virtual ICollection<PartnerAddress> PartnerAddresses { get; set; }
  public virtual ICollection<PartnerGeoLocation> PartnerGeoLocations { get; set; }
}