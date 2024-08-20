using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class LaundryItem : BaseModel
{
    public LaundryItem()
    {
        Prices = new HashSet<Price>();
        OrderItems = new HashSet<OrderItem>();
    }
    public required string Name { get; set; }

    public string? Code { get; set; }
    public Boolean? Status { get; set; }
    public string? Description { get; set; }

    public int ItemTypeId { get; set; }
    public virtual required ItemType ItemType { get; set; }
    public virtual ICollection<Price> Prices { get; set; }
    public virtual ICollection<OrderItem> OrderItems { get; set; }
}