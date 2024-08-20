using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class Order : BaseModel
{

    public Order()
    {
        OrderAssigments = new HashSet<OrderAssignment>();
        OrderCharges = new HashSet<OrderCharge>();
        OrderPayments = new HashSet<OrderPayment>();
        OrderItems = new HashSet<OrderItem>();
        OrderMessages = new HashSet<OrderMessage>();
        OrderPromoCodes = new HashSet<OrderPromoCode>();
        OrderLocations = new HashSet<OrderLocation>();
        OrderDetergents = new HashSet<OrderDetergent>();
        OrderRatings = new HashSet<OrderRating>();
        OrderRefunds = new HashSet<OrderRefund>();
        OrderStatusUpdates = new HashSet<OrderStatusUpdate>();
        OrderRecons = new HashSet<OrderRecon>();
    }
    public required string RefNumber { get; set; }
    public required string ClientUserName { get; set; }

    public required DateTime OrderDate { get; set; }

    public required DateTime PickupDate { get; set; }
    public required DateTime DeliveryDate { get; set; }

    public required decimal GrossAmount { get; set; }
    public required decimal NetAmount { get; set; }

    public decimal TaxAmount { get; set; }
    public required decimal Discount { get; set; }

    public int OrderStatusId { get; set; }

    public virtual required OrderStatus OrderStatus { get; set; }

    public virtual ICollection<OrderAssignment> OrderAssigments { get; set; }
    public virtual ICollection<OrderCharge> OrderCharges { get; set; }
    public virtual ICollection<OrderItem> OrderItems { get; set; }
    public virtual ICollection<OrderMessage> OrderMessages { get; set; }
    public virtual ICollection<OrderPayment> OrderPayments { get; set; }
    public virtual ICollection<OrderStatusUpdate> OrderStatusUpdates { get; set; }
    public virtual ICollection<OrderPromoCode> OrderPromoCodes { get; set; }

    public virtual ICollection<OrderLocation> OrderLocations { get; set; }

    public virtual ICollection<OrderDetergent> OrderDetergents { get; set; }

    public virtual ICollection<OrderRating> OrderRatings { get; set; }

    public virtual ICollection<OrderRefund> OrderRefunds { get; set; }

    public virtual ICollection<OrderRecon> OrderRecons { get; set; }
}