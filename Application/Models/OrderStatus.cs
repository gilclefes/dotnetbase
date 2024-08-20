using System.ComponentModel.DataAnnotations;
using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class OrderStatus : BaseModel
{

    public OrderStatus()
    {
        Orders = new HashSet<Order>();
    }
    public required string Name { get; set; }

    public string? Code { get; set; }
    public Boolean? Status { get; set; }
    public string? Description { get; set; }

    public int Rank { get; set; }

    public virtual ICollection<Order> Orders { get; set; }
}