using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class OrderCharge : BaseModel
{
    public int OrderId { get; set; }
    public int ChargeId { get; set; }
    public DateTime DateAdded { get; set; }


    public string? ChargeDescription { get; set; }
    public required decimal Amount { get; set; }
    public virtual required Order Order { get; set; }
    public virtual required Charge Charge { get; set; }
}