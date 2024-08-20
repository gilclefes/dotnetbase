using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coravel.Invocable;
using dotnetbase.Application.Database;
using dotnetbase.Application.Models;
using dotnetbase.Application.Services;

namespace dotnetbase.Application.Tasks
{
    public class OrderRefundTask : IInvocable
    {

        private readonly DatabaseContext _context;

        private readonly YaboUtilsService _yaboUtilService;
        public OrderRefundTask(DatabaseContext context, YaboUtilsService yaboValidationService)
        {
            _context = context;

            _yaboUtilService = yaboValidationService;

        }
        public async Task Invoke()
        {

            //write a task that searches for order assignments with status NEW that has been pending for more than 30 minutes and update the status to rejected
            var orderRefunds = _context.OrderRefunds.Where(oa => oa.RefundStatus == RefundStatus.PENDING && oa.LastRetryDate.Date != DateTime.Now.Date).Take(30).ToList();
            var tasks = new List<Task>(orderRefunds.Count);
            foreach (var orderRefund in orderRefunds)
            {
                //tasks.Add(this._yaboUtilService.ProcessRefundTransaction(orderRefund.Id));
                await this._yaboUtilService.ProcessRefundTransaction(orderRefund.Id);

            }
            //  await Task.WhenAll(tasks);
            Console.WriteLine("Do something in the background.");
        }
    }
}