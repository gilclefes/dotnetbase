using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.ViewModels
{
  public class ClientSubscriptionDetailDto
  {
    public int Id { get; set; }
    public int ClientSubscriptionId { get; set; }
    public int? SubscriptionPlanPriceId { get; set; }

    // subscriptionId, datesubscribed, expirydate,TotalAmount, amountpaid
    public DateTime DateSubscibed { get; set; }
    public DateTime ExpiryDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal Discount { get; set; }

    public decimal TaxAmount { get; set; }

    public int subscritpionPlanId { get; set; }

    public string? ClientSubscriptionSubscriptionPlanName { get; set; }

  }
}