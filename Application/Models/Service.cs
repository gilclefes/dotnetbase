using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class Service : BaseModel
{
  public Service()
  {
    ServicePeriods = new HashSet<ServicePeriod>();
    Prices = new HashSet<Price>();
    OrderItems = new HashSet<OrderItem>();
    SubscriptionPlanServices = new HashSet<SubscriptionPlanService>();
    ServiceDetergents = new HashSet<ServiceDetergent>();
    OrderDetergents = new HashSet<OrderDetergent>();
    ServiceCharges = new HashSet<ServiceCharge>();
  }
  public required string Name { get; set; }

  public string? Code { get; set; }
  public Boolean? Status { get; set; }
  public string? Description { get; set; }
  public string? HowTo { get; set; }
  public int CategoryId { get; set; }
  public int TypeId { get; set; }

  public bool? Pending { get; set; } = false;
  public string? ApprovedBy { get; set; }
  public DateTime? ApprovedDate { get; set; }

  public decimal MinOrderValue { get; set; }

  public virtual required ServiceCategory ServiceCategory { get; set; }
  public virtual required ServiceType ServiceType { get; set; }

  public virtual ICollection<ServicePeriod> ServicePeriods { get; set; }

  public virtual ICollection<ServiceCharge> ServiceCharges { get; set; }

  public virtual ICollection<ServiceDetergent> ServiceDetergents { get; set; }
  public virtual ICollection<Price> Prices { get; set; }

  public virtual ICollection<OrderItem> OrderItems { get; set; }

  public virtual ICollection<SubscriptionPlanService> SubscriptionPlanServices { get; set; }

  public virtual ICollection<OrderDetergent> OrderDetergents { get; set; }
}