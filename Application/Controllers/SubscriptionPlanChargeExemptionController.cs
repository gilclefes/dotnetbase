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
    public class SubscriptionPlanChargeExemptionController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUriService uriService;
        private readonly IMapper _mapper;

        public SubscriptionPlanChargeExemptionController(DatabaseContext context, IUriService uriService, IMapper mapper)
        {
            _context = context;
            this.uriService = uriService;
            _mapper = mapper;
        }

        // GET: api/SubscriptionPlanChargeExemption
        [HttpGet]
        public async Task<ActionResult> GetSubscriptionPlanChargeExemptions([FromQuery] PaginationFilter filter)
        {
            if (_context.SubscriptionPlanChargeExemptions == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.SubscriptionPlanChargeExemptions'  is null." });
            }

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.SubscriptionPlanChargeExemptions
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).Select(x => new SubscriptionPlanChargeExemptionDto
               {
                   Id = x.Id,
                   SubscriptionId = x.SubscriptionId,
                   Description = x.Description,
                   Status = x.Status,
                   ChargeId = x.ChargeId,
                   ChargeName = x.Charge.Name,
                   SubscriptionPlanName = x.SubscriptionPlan.Name,

               }).AsNoTracking()

               .ToListAsync();
            var totalRecords = await _context.OrderAssignments.CountAsync();

            var pagedReponse = PaginationHelper.CreatePagedReponse<SubscriptionPlanChargeExemptionDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }


        [HttpGet("Plan/{id}")]
        public async Task<ActionResult> GetSubscriptionPlanChargeExemptionsByPaln(int id, [FromQuery] PaginationFilter filter)
        {
            if (_context.SubscriptionPlanChargeExemptions == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.SubscriptionPlanChargeExemptions'  is null." });
            }

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.SubscriptionPlanChargeExemptions.Where(x => x.SubscriptionId == id)
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).Select(x => new SubscriptionPlanChargeExemptionDto
               {
                   Id = x.Id,
                   SubscriptionId = x.SubscriptionId,
                   Description = x.Description,
                   Status = x.Status,
                   ChargeId = x.ChargeId,
                   ChargeName = x.Charge.Name,
                   SubscriptionPlanName = x.SubscriptionPlan.Name,

               }).AsNoTracking()

               .ToListAsync();
            var totalRecords = await _context.OrderAssignments.CountAsync();

            var pagedReponse = PaginationHelper.CreatePagedReponse<SubscriptionPlanChargeExemptionDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }



        // GET: api/SubscriptionPlanChargeExemption/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SubscriptionPlanChargeExemption>> GetSubscriptionPlanChargeExemption(int id)
        {
            if (_context.SubscriptionPlanChargeExemptions == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.SubscriptionPlanChargeExemptions'  is null." });
            }
            var SubscriptionPlanChargeExemption = await _context.SubscriptionPlanChargeExemptions.FindAsync(id);

            if (SubscriptionPlanChargeExemption == null)
            {
                return NotFound(new { message = "SubscriptionPlanChargeExemption not found." });
            }



            SubscriptionPlanChargeExemptionDto data = _mapper.Map<SubscriptionPlanChargeExemptionDto>(SubscriptionPlanChargeExemption);
            return Ok(new Wrappers.ApiResponse<SubscriptionPlanChargeExemptionDto>(data));
        }

        // PUT: api/SubscriptionPlanChargeExemption/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutSubscriptionPlanChargeExemption(int id, SubscriptionPlanChargeExemptionDto SubscriptionPlanChargeExemption)
        {
            if (id != SubscriptionPlanChargeExemption.Id)
            {
                return BadRequest(new { message = "Invalid SubscriptionPlanChargeExemption Id." });
            }

            //find charge by chargeid
            Charge? charge = await _context.Charges.FindAsync(SubscriptionPlanChargeExemption.ChargeId);

            //check for charge null and return not found
            if (charge == null)
            {
                return NotFound(new { message = "Charge not found." });
            }

            //find subscription by subscriptionid
            SubscriptionPlan? subscription = await _context.SubscriptionPlans.FindAsync(SubscriptionPlanChargeExemption.SubscriptionId);

            //check for subscription null and return not found
            if (subscription == null)
            {
                return NotFound(new { message = "Subscription not found." });
            }

            SubscriptionPlanChargeExemption data = new SubscriptionPlanChargeExemption
            {
                Id = SubscriptionPlanChargeExemption.Id,
                ChargeId = SubscriptionPlanChargeExemption.ChargeId,
                SubscriptionId = SubscriptionPlanChargeExemption.SubscriptionId,

                Description = SubscriptionPlanChargeExemption.Description,
                Status = SubscriptionPlanChargeExemption.Status,
                Charge = charge,
                SubscriptionPlan = subscription,
                UpdatedAt = DateTime.Now,

            };

            _context.Entry(data).State = EntityState.Modified;


            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/SubscriptionPlanChargeExemption
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> PostSubscriptionPlanChargeExemption(SubscriptionPlanChargeExemptionDto SubscriptionPlanChargeExemption)
        {
            if (_context.SubscriptionPlanChargeExemptions == null)
            {
                return Problem("Entity set 'DatabaseContext.SubscriptionPlanChargeExemptions'  is null.");
            }

            //find charge by chargeid
            Charge? charge = await _context.Charges.FindAsync(SubscriptionPlanChargeExemption.ChargeId);

            //check for charge null and return not found
            if (charge == null)
            {
                return NotFound(new { message = "Charge not found." });
            }


            //find subscription by subscriptionid
            SubscriptionPlan? subscription = await _context.SubscriptionPlans.FindAsync(SubscriptionPlanChargeExemption.SubscriptionId);

            //check for subscription null and return not found
            if (subscription == null)
            {
                return NotFound(new { message = "Subscription not found." });
            }

            SubscriptionPlanChargeExemption data = new SubscriptionPlanChargeExemption
            {
                Id = SubscriptionPlanChargeExemption.Id,
                ChargeId = SubscriptionPlanChargeExemption.ChargeId,
                SubscriptionId = SubscriptionPlanChargeExemption.SubscriptionId,
                Description = SubscriptionPlanChargeExemption.Description,
                Status = SubscriptionPlanChargeExemption.Status,
                Charge = charge,
                SubscriptionPlan = subscription,
                UpdatedAt = DateTime.Now,
                CreatedAt = DateTime.Now
            };


            _context.Charges.Attach(data.Charge);
            _context.SubscriptionPlans.Attach(data.SubscriptionPlan);
            _context.SubscriptionPlanChargeExemptions.Add(data);


            await _context.SaveChangesAsync();

            SubscriptionPlanChargeExemption.Id = data.Id;


            return CreatedAtAction("GetSubscriptionPlanChargeExemption", new { id = data.Id }, SubscriptionPlanChargeExemption);
        }

        // DELETE: api/SubscriptionPlanChargeExemption/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteSubscriptionPlanChargeExemption(int id)
        {
            if (_context.SubscriptionPlanChargeExemptions == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.SubscriptionPlanChargeExemptions'  is null." });
            }
            var SubscriptionPlanChargeExemption = await _context.SubscriptionPlanChargeExemptions.FindAsync(id);
            if (SubscriptionPlanChargeExemption == null)
            {
                return NotFound(new { message = "SubscriptionPlanChargeExemption not found." });
            }

            _context.SubscriptionPlanChargeExemptions.Remove(SubscriptionPlanChargeExemption);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SubscriptionPlanChargeExemptionExists(SubscriptionPlanChargeExemptionDto SubscriptionPlanChargeExemption)
        {
            return (_context.SubscriptionPlanChargeExemptions?.Any(e => e.ChargeId == SubscriptionPlanChargeExemption.ChargeId && e.SubscriptionId == SubscriptionPlanChargeExemption.SubscriptionId && e.Id != SubscriptionPlanChargeExemption.Id)).GetValueOrDefault();
        }

    }
}
