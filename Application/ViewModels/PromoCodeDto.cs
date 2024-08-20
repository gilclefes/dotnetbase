using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.ViewModels
{
  public class PromoCodeDto
  {
    public int Id { get; set; }
    public required string CodeName { get; set; }

    public required string CodeValue { get; set; }
    public required bool Status { get; set; }

    public required int UsageLimit { get; set; }
    public required int UsageCount { get; set; }
    public required Decimal Discount { get; set; }

    public string? Description { get; set; }

    public required Decimal MinOrderValue { get; set; }
    public required Decimal MaxOrderValue { get; set; }

    public required DateTime PromoStartDate { get; set; }
    public required DateTime PromoEndDate { get; set; }
  }
}