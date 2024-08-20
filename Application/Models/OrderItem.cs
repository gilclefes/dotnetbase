using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class OrderItem : BaseModel
{
    public int OrderId { get; set; }
    public int ServiceId { get; set; }
    public int ItemId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal NetPrice { get; set; }

    public string? Comments { get; set; }
    public virtual required Order Order { get; set; }
    public virtual required Service Service { get; set; }
    public virtual required LaundryItem LaundryItem { get; set; }
}