using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class Client : BaseModel
{
  public Client()
  {
    ClientAddresses = new HashSet<ClientAddress>();
    ClientGeoLocations = new HashSet<ClientGeoLocation>();
    ClientSubscriptions = new HashSet<ClientSubscription>();
    OrderRefunds = new HashSet<OrderRefund>();
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

  public virtual required RegStatus RegStatus { get; set; }

  public virtual ICollection<ClientAddress> ClientAddresses { get; set; }

  public virtual ICollection<ClientGeoLocation> ClientGeoLocations { get; set; }

  public virtual ICollection<ClientSubscription> ClientSubscriptions { get; set; }


  public virtual ICollection<OrderRefund> OrderRefunds { get; set; }

}