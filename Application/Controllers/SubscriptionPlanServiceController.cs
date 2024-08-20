using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dotnetbase.Application.Database;
using dotnetbase.Application.Filter;
using dotnetbase.Application.Helpers;
using dotnetbase.Application.Models;
using dotnetbase.Application.Services;
using dotnetbase.Application.ViewModels;

namespace dotnetbase.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SubscriptionPlanServiceController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUriService uriService;
        private readonly IMapper _mapper;

        public SubscriptionPlanServiceController(DatabaseContext context, IUriService uriService, IMapper mapper)
        {
            _context = context;
            this.uriService = uriService;
            _mapper = mapper;
        }

        // GET: api/SubscriptionPlanService
        [HttpGet]
        public async Task<ActionResult> GetSubscriptionPlanServices([FromQuery] PaginationFilter filter)
        {
            if (_context.SubscriptionPlanServices == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.SubscriptionPlanServices'  is null." });
            }

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.SubscriptionPlanServices
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).Select(x => new SubscriptionPlanServiceDto
               {
                   Description = x.Description,
                   Status = x.Status,
                   Id = x.Id,
                   ServiceId = x.ServiceId,
                   SubscriptionId = x.SubscriptionId,
                   ServiceName = x.Service.Name,
                   SubscriptionPlanName = x.SubscriptionPlan.Name,

               }).AsNoTracking()
               .ToListAsync();
            var totalRecords = await _context.OrderAssignments.CountAsync();


            var pagedReponse = PaginationHelper.CreatePagedReponse<SubscriptionPlanServiceDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }


        [HttpGet("Plan/{id}")]
        public async Task<ActionResult> GetSubscriptionPlanServicesByPlan(int id, [FromQuery] PaginationFilter filter)
        {
            if (_context.SubscriptionPlanServices == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.SubscriptionPlanServices'  is null." });
            }

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.SubscriptionPlanServices.Where(x => x.SubscriptionId == id)
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).Select(x => new SubscriptionPlanServiceDto
               {
                   Description = x.Description,
                   Status = x.Status,
                   Id = x.Id,
                   ServiceId = x.ServiceId,
                   SubscriptionId = x.SubscriptionId,
                   ServiceName = x.Service.Name,
                   SubscriptionPlanName = x.SubscriptionPlan.Name,

               }).AsNoTracking()
               .ToListAsync();
            var totalRecords = await _context.OrderAssignments.CountAsync();


            var pagedReponse = PaginationHelper.CreatePagedReponse<SubscriptionPlanServiceDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        // GET: api/SubscriptionPlanService/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SubscriptionPlanService>> GetSubscriptionPlanService(int id)
        {
            if (_context.SubscriptionPlanServices == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.SubscriptionPlanServices'  is null." });
            }
            var SubscriptionPlanService = await _context.SubscriptionPlanServices.FindAsync(id);

            if (SubscriptionPlanService == null)
            {
                return NotFound(new { message = "SubscriptionPlanService not found." });
            }



            SubscriptionPlanServiceDto data = _mapper.Map<SubscriptionPlanServiceDto>(SubscriptionPlanService);
            return Ok(new Wrappers.ApiResponse<SubscriptionPlanServiceDto>(data));
        }

        // PUT: api/SubscriptionPlanService/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSubscriptionPlanService(int id, SubscriptionPlanServiceDto SubscriptionPlanService)
        {
            if (id != SubscriptionPlanService.Id)
            {
                return BadRequest(new { message = "Invalid SubscriptionPlanService Id." });
            }

            //find service by serviceid
            Service? service = await _context.Services.FindAsync(SubscriptionPlanService.ServiceId);

            //check for null and return not found
            if (service == null)
            {
                return NotFound(new { message = "Service does not exist" });
            }

            //find subscription by subscriptionid
            SubscriptionPlan? subscription = await _context.SubscriptionPlans.FindAsync(SubscriptionPlanService.SubscriptionId);

            //check for null and return not found
            if (subscription == null)
            {
                return NotFound(new { message = "Subscription does not exist" });
            }

            SubscriptionPlanService data = new SubscriptionPlanService
            {
                SubscriptionId = SubscriptionPlanService.SubscriptionId,
                ServiceId = SubscriptionPlanService.ServiceId,
                Service = service,
                SubscriptionPlan = subscription,
                Description = SubscriptionPlanService.Description,
                Status = SubscriptionPlanService.Status,
                Id = SubscriptionPlanService.Id,
                UpdatedAt = DateTime.Now,
                CreatedAt = DateTime.Now,



            };



            _context.Entry(data).State = EntityState.Modified;
            await _context.SaveChangesAsync();


            return NoContent();
        }

        // POST: api/SubscriptionPlanService
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostSubscriptionPlanService(SubscriptionPlanServiceDto SubscriptionPlanService)
        {
            if (_context.SubscriptionPlanServices == null)
            {
                return Problem("Entity set 'DatabaseContext.SubscriptionPlanServices'  is null.");
            }

            //find service by serviceid
            Service? service = await _context.Services.FindAsync(SubscriptionPlanService.ServiceId);

            //check for null and return not found
            if (service == null)
            {
                return NotFound(new { message = "Service does not exist" });
            }

            //find subscription by subscriptionid
            SubscriptionPlan? subscription = await _context.SubscriptionPlans.FindAsync(SubscriptionPlanService.SubscriptionId);

            //check for null and return not found
            if (subscription == null)
            {
                return NotFound(new { message = "Subscription does not exist" });
            }

            SubscriptionPlanService data = new SubscriptionPlanService
            {
                SubscriptionId = SubscriptionPlanService.SubscriptionId,
                ServiceId = SubscriptionPlanService.ServiceId,
                Service = service,
                SubscriptionPlan = subscription,
                Description = SubscriptionPlanService.Description,
                Status = SubscriptionPlanService.Status,
                Id = SubscriptionPlanService.Id,
                UpdatedAt = DateTime.Now,
                CreatedAt = DateTime.Now


            };

            _context.Services.Attach(service);
            _context.SubscriptionPlans.Attach(subscription);
            _context.SubscriptionPlanServices.Add(data);


            await _context.SaveChangesAsync();

            SubscriptionPlanService.Id = data.Id;


            return CreatedAtAction("GetSubscriptionPlanService", new { id = data.Id }, SubscriptionPlanService);
        }

        // DELETE: api/SubscriptionPlanService/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubscriptionPlanService(int id)
        {
            if (_context.SubscriptionPlanServices == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.SubscriptionPlanServices'  is null." });
            }
            var SubscriptionPlanService = await _context.SubscriptionPlanServices.FindAsync(id);
            if (SubscriptionPlanService == null)
            {
                return NotFound(new { message = "SubscriptionPlanService not found." });
            }

            _context.SubscriptionPlanServices.Remove(SubscriptionPlanService);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SubscriptionPlanServiceExists(SubscriptionPlanService subscriptionPlanService)
        {
            return (_context.SubscriptionPlanServices?.Any(e => e.SubscriptionId == subscriptionPlanService.SubscriptionId && e.ServiceId == subscriptionPlanService.ServiceId && e.Id != subscriptionPlanService.Id)).GetValueOrDefault();
        }


    }
}
