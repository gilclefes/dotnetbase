using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.ViewModels
{
    public class ClientSubscriptionDetailPaymentDto
    {
        public int Id { get; set; }
        public int ClientSubscriptionDetailId { get; set; }

        public int PaymentId { get; set; }


    }
}