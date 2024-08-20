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
    public class OrderAssignmentTask : IInvocable
    {

        private readonly DatabaseContext _context;
        private readonly YaboUtilsService _yaboUtilService;
        private readonly IConfiguration _configuration;
        private const string PAID_ORDERSTATUS_ID = "PAID_ORDERSTATUS_ID";

        public OrderAssignmentTask(DatabaseContext context, YaboUtilsService yaboValidationService, IConfiguration configuration)
        {
            _context = context;

            _yaboUtilService = yaboValidationService;
            _configuration = configuration;

        }
        public async Task Invoke()
        {

            int paidOrderStatusId = _configuration.GetValue<int>(PAID_ORDERSTATUS_ID);
            //write a task that searches for order assignments with status NEW that has been pending for more than 30 minutes and update the status to rejected
            var orderAssignments = _context.OrderAssignments.Where(oa => oa.AssignedStatus == OrderAssignmentStatus.NEW && oa.CreatedAt.AddMinutes(30) < DateTime.Now && oa.Order.OrderStatusId == paidOrderStatusId).ToList();
            var tasks = new List<Task>(orderAssignments.Count);
            foreach (var orderAssignment in orderAssignments)
            {
                tasks.Add(this._yaboUtilService.UpdateOrderAssignmentStatus(orderAssignment.OrderId, OrderAssignmentStatus.REJECTED));
            }
            await Task.WhenAll(tasks);
            Console.WriteLine("Do something in the background.");
        }
    }
}