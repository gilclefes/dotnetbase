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
    public class SubscriptionPlanChargeController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUriService uriService;
        private readonly IMapper _mapper;

        public SubscriptionPlanChargeController(DatabaseContext context, IUriService uriService, IMapper mapper)
        {
            _context = context;
            this.uriService = uriService;
            _mapper = mapper;
        }

        // GET: api/SubscriptionPlanCharge
        [HttpGet]
        public async Task<ActionResult> GetSubscriptionPlanCharges([FromQuery] PaginationFilter filter)
        {
            if (_context.SubscriptionPlanCharges == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.SubscriptionPlanCharges'  is null." });
            }

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.SubscriptionPlanCharges
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).Select(x => new SubscriptionPlanChargeDto
               {
                   Id = x.Id,
                   SubscriptionId = x.SubscriptionId,
                   Description = x.Description,
                   Status = x.Status,
                   Amount = x.Amount,
                   ChargeId = x.ChargeId,
                   ChargeName = x.Charge.Name,
                   SubscriptionPlanName = x.SubscriptionPlan.Name,

               }).AsNoTracking()

               .ToListAsync();
            var totalRecords = await _context.OrderAssignments.CountAsync();

            var pagedReponse = PaginationHelper.CreatePagedReponse<SubscriptionPlanChargeDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }


        [HttpGet("Plan/{id}")]
        public async Task<ActionResult> GetSubscriptionPlanChargesByPaln(int id, [FromQuery] PaginationFilter filter)
        {
            if (_context.SubscriptionPlanCharges == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.SubscriptionPlanCharges'  is null." });
            }

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.SubscriptionPlanCharges.Where(x => x.SubscriptionId == id)
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).Select(x => new SubscriptionPlanChargeDto
               {
                   Id = x.Id,
                   SubscriptionId = x.SubscriptionId,
                   Description = x.Description,
                   Status = x.Status,
                   Amount = x.Amount,
                   ChargeId = x.ChargeId,
                   ChargeName = x.Charge.Name,
                   SubscriptionPlanName = x.SubscriptionPlan.Name,

               }).AsNoTracking()

               .ToListAsync();
            var totalRecords = await _context.OrderAssignments.CountAsync();

            var pagedReponse = PaginationHelper.CreatePagedReponse<SubscriptionPlanChargeDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }



        // GET: api/SubscriptionPlanCharge/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SubscriptionPlanCharge>> GetSubscriptionPlanCharge(int id)
        {
            if (_context.SubscriptionPlanCharges == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.SubscriptionPlanCharges'  is null." });
            }
            var SubscriptionPlanCharge = await _context.SubscriptionPlanCharges.FindAsync(id);

            if (SubscriptionPlanCharge == null)
            {
                return NotFound(new { message = "SubscriptionPlanCharge not found." });
            }



            SubscriptionPlanChargeDto data = _mapper.Map<SubscriptionPlanChargeDto>(SubscriptionPlanCharge);
            return Ok(new Wrappers.ApiResponse<SubscriptionPlanChargeDto>(data));
        }

        // PUT: api/SubscriptionPlanCharge/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutSubscriptionPlanCharge(int id, SubscriptionPlanChargeDto SubscriptionPlanCharge)
        {
            if (id != SubscriptionPlanCharge.Id)
            {
                return BadRequest(new { message = "Invalid SubscriptionPlanCharge Id." });
            }

            //find charge by chargeid
            Charge? charge = await _context.Charges.FindAsync(SubscriptionPlanCharge.ChargeId);

            //check for charge null and return not found
            if (charge == null)
            {
                return NotFound(new { message = "Charge not found." });
            }

            //find subscription by subscriptionid
            SubscriptionPlan? subscription = await _context.SubscriptionPlans.FindAsync(SubscriptionPlanCharge.SubscriptionId);

            //check for subscription null and return not found
            if (subscription == null)
            {
                return NotFound(new { message = "Subscription not found." });
            }

            SubscriptionPlanCharge data = new SubscriptionPlanCharge
            {
                Id = SubscriptionPlanCharge.Id,
                ChargeId = SubscriptionPlanCharge.ChargeId,
                SubscriptionId = SubscriptionPlanCharge.SubscriptionId,
                Amount = SubscriptionPlanCharge.Amount,
                Description = SubscriptionPlanCharge.Description,
                Status = SubscriptionPlanCharge.Status,
                Charge = charge,
                SubscriptionPlan = subscription,
                UpdatedAt = DateTime.Now,

            };

            _context.Entry(data).State = EntityState.Modified;


            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/SubscriptionPlanCharge
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> PostSubscriptionPlanCharge(SubscriptionPlanChargeDto SubscriptionPlanCharge)
        {
            if (_context.SubscriptionPlanCharges == null)
            {
                return Problem("Entity set 'DatabaseContext.SubscriptionPlanCharges'  is null.");
            }

            //find charge by chargeid
            Charge? charge = await _context.Charges.FindAsync(SubscriptionPlanCharge.ChargeId);

            //check for charge null and return not found
            if (charge == null)
            {
                return NotFound(new { message = "Charge not found." });
            }

            if (charge.AmountType == ChargeAmountType.PERCENTAGE && SubscriptionPlanCharge.Amount > 100 && SubscriptionPlanCharge.Amount < 0)
            {
                return BadRequest(new { message = "Amount can not be greater than 100 for percentage charge" });
            }

            //find subscription by subscriptionid
            SubscriptionPlan? subscription = await _context.SubscriptionPlans.FindAsync(SubscriptionPlanCharge.SubscriptionId);

            //check for subscription null and return not found
            if (subscription == null)
            {
                return NotFound(new { message = "Subscription not found." });
            }

            SubscriptionPlanCharge data = new SubscriptionPlanCharge
            {
                Id = SubscriptionPlanCharge.Id,
                ChargeId = SubscriptionPlanCharge.ChargeId,
                SubscriptionId = SubscriptionPlanCharge.SubscriptionId,
                Amount = SubscriptionPlanCharge.Amount,
                Description = SubscriptionPlanCharge.Description,
                Status = SubscriptionPlanCharge.Status,
                Charge = charge,
                SubscriptionPlan = subscription,
                UpdatedAt = DateTime.Now,
                CreatedAt = DateTime.Now
            };


            _context.Charges.Attach(data.Charge);
            _context.SubscriptionPlans.Attach(data.SubscriptionPlan);
            _context.SubscriptionPlanCharges.Add(data);


            await _context.SaveChangesAsync();

            SubscriptionPlanCharge.Id = data.Id;


            return CreatedAtAction("GetSubscriptionPlanCharge", new { id = data.Id }, SubscriptionPlanCharge);
        }

        // DELETE: api/SubscriptionPlanCharge/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteSubscriptionPlanCharge(int id)
        {
            if (_context.SubscriptionPlanCharges == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.SubscriptionPlanCharges'  is null." });
            }
            var SubscriptionPlanCharge = await _context.SubscriptionPlanCharges.FindAsync(id);
            if (SubscriptionPlanCharge == null)
            {
                return NotFound(new { message = "SubscriptionPlanCharge not found." });
            }

            _context.SubscriptionPlanCharges.Remove(SubscriptionPlanCharge);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SubscriptionPlanChargeExists(SubscriptionPlanChargeDto SubscriptionPlanCharge)
        {
            return (_context.SubscriptionPlanCharges?.Any(e => e.ChargeId == SubscriptionPlanCharge.ChargeId && e.SubscriptionId == SubscriptionPlanCharge.SubscriptionId && e.Id != SubscriptionPlanCharge.Id)).GetValueOrDefault();
        }

    }
}
