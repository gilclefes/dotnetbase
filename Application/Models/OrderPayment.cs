using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class OrderPayment : BaseModel
{
    public int OrderId { get; set; }

    public int PaymentId { get; set; }

    public virtual required Order Order { get; set; }

    public virtual required Payment Payment { get; set; }

}