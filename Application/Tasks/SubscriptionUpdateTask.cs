using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coravel.Invocable;
using Microsoft.EntityFrameworkCore;
using dotnetbase.Application.Database;
using dotnetbase.Application.Models;
using dotnetbase.Application.Services;

namespace dotnetbase.Application.Tasks
{
    public class SubscriptionUpdateTask : IInvocable
    {

        private readonly DatabaseContext _context;

        private readonly YaboUtilsService _yaboUtilService;
        public SubscriptionUpdateTask(DatabaseContext context, YaboUtilsService yaboValidationService)
        {
            _context = context;

            _yaboUtilService = yaboValidationService;

        }
        public async Task Invoke()
        {

            //write a task that searches for order assignments with status NEW that has been pending for more than 30 minutes and update the status to rejected
            var subscriptions = await _context.ClientSubscriptions.Where(x => x.ExpiryDate < DateTime.Now && x.Status).Take(50).ToListAsync();


            foreach (var subscription in subscriptions)
            {
                subscription.Status = false;
                subscription.UpdatedAt = DateTime.Now;
                _context.Entry(subscription).State = EntityState.Modified;
                await _context.SaveChangesAsync();

            }
            //  await Task.WhenAll(tasks);
            Console.WriteLine("Do something in the background.");
        }
    }
}