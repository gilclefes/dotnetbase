using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Spark.Library.Database;

namespace dotnetbase.Application.Models
{
    public class PromoCode : BaseModel
    {
        public required string CodeName { get; set; }
        public required bool Status { get; set; }

        public required int UsageLimit { get; set; }
        public required int UsageCount { get; set; }
        public required Decimal Discount { get; set; }

        public required Decimal MinOrderValue { get; set; }
        public required Decimal MaxOrderValue { get; set; }

        public required string CodeValue { get; set; }

        public required DateTime PromoStartDate { get; set; }
        public required DateTime PromoEndDate { get; set; }

        public string? Description { get; set; }

        public virtual ICollection<OrderPromoCode> OrderPromoCodes { get; set; }

        public PromoCode()
        {
            OrderPromoCodes = new HashSet<OrderPromoCode>();
        }


        //promo_code_name | promo_code_value | promo_code_expiration | promo_code_usage_limit | promo_code_usage_count | promo_code_created_at | promo_code_updated_at | promo_code_deleted_at
    }
}