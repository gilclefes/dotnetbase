using System.Text;
using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class Price : BaseModel
{

    public int ServiceId { get; set; }
    public int ItemId { get; set; }
    public int PeriodId { get; set; }
    public int UnitTypeId { get; set; }

    public decimal Amount { get; set; }

    public Boolean? Status { get; set; }

    public virtual required Service Service { get; set; }
    public virtual required LaundryItem Item { get; set; }
    public virtual required Period Period { get; set; }
    public virtual required UnitType UnitType { get; set; }

}