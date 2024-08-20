using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class OrderRecon : BaseModel
{
    public int OrderId { get; set; }

    public required string PartnerUserName { get; set; }
    public required decimal TotalOrderAmount { get; set; }
    public required decimal YaboShare { get; set; }
    public required decimal PartnerShare { get; set; }
    public required DateTime OrdecCompletionDate { get; set; }
    public virtual required Order Order { get; set; }

}