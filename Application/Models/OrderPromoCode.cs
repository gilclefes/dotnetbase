using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Spark.Library.Database;

namespace dotnetbase.Application.Models
{
    public class OrderPromoCode : BaseModel
    {
        public int OrderId { get; set; }

        public int PromoId { get; set; }

        public virtual required Order Order { get; set; }

        public virtual required PromoCode PromoCode { get; set; }

        public required Decimal TotalAmount { get; set; }

        public string? Comments { get; set; }
    }
}