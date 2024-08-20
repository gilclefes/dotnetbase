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
    public class SubscriptionPlanBenefitController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUriService uriService;
        private readonly IMapper _mapper;

        public SubscriptionPlanBenefitController(DatabaseContext context, IUriService uriService, IMapper mapper)
        {
            _context = context;
            this.uriService = uriService;
            _mapper = mapper;
        }

        // GET: api/SubscriptionPlanBenefit
        [HttpGet]
        public async Task<ActionResult> GetSubscriptionPlanBenefits([FromQuery] PaginationFilter filter)
        {
            if (_context.SubscriptionPlanBenefits == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.SubscriptionPlanBenefits'  is null." });
            }

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.SubscriptionPlanBenefits
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).Select(x => new SubscriptionPlanBenefitDto
               {
                   Id = x.Id,
                   SubscriptionId = x.SubscriptionId,
                   Status = x.Status,
                   Benefit = x.Benefit,
                   Rank = x.Rank,
                   SubscriptionPlanName = x.SubscriptionPlan.Name,

               }).OrderBy(x => x.Rank).AsNoTracking()

               .ToListAsync();
            var totalRecords = await _context.OrderAssignments.CountAsync();

            var pagedReponse = PaginationHelper.CreatePagedReponse<SubscriptionPlanBenefitDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }


        [HttpGet("Plan/{id}")]
        public async Task<ActionResult> GetSubscriptionPlanBenefitsByPlan(int id, [FromQuery] PaginationFilter filter)
        {
            if (_context.SubscriptionPlanBenefits == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.SubscriptionPlanBenefits'  is null." });
            }

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.SubscriptionPlanBenefits.Where(x => x.SubscriptionId == id)
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).Select(x => new SubscriptionPlanBenefitDto
               {
                   Id = x.Id,
                   SubscriptionId = x.SubscriptionId,
                   Status = x.Status,
                   Benefit = x.Benefit,
                   Rank = x.Rank,
                   SubscriptionPlanName = x.SubscriptionPlan.Name,

               }).OrderBy(x => x.Rank).AsNoTracking()

               .ToListAsync();
            var totalRecords = await _context.OrderAssignments.CountAsync();

            var pagedReponse = PaginationHelper.CreatePagedReponse<SubscriptionPlanBenefitDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }



        // GET: api/SubscriptionPlanBenefit/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SubscriptionPlanBenefit>> GetSubscriptionPlanBenefit(int id)
        {
            if (_context.SubscriptionPlanBenefits == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.SubscriptionPlanBenefits'  is null." });
            }
            var SubscriptionPlanBenefit = await _context.SubscriptionPlanBenefits.FindAsync(id);

            if (SubscriptionPlanBenefit == null)
            {
                return NotFound(new { message = "SubscriptionPlanBenefit not found." });
            }



            SubscriptionPlanBenefitDto data = _mapper.Map<SubscriptionPlanBenefitDto>(SubscriptionPlanBenefit);
            return Ok(new Wrappers.ApiResponse<SubscriptionPlanBenefitDto>(data));
        }

        // PUT: api/SubscriptionPlanBenefit/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutSubscriptionPlanBenefit(int id, SubscriptionPlanBenefitDto SubscriptionPlanBenefit)
        {
            if (id != SubscriptionPlanBenefit.Id)
            {
                return BadRequest(new { message = "Invalid SubscriptionPlanBenefit Id." });
            }

            //cherck if subscription plan benefit exists
            if (SubscriptionPlanBenefitExists(SubscriptionPlanBenefit))
            {
                return BadRequest(new { message = "Subscription Plan Benefit already exists" });
            }



            //find subscription by subscriptionid
            SubscriptionPlan? subscription = await _context.SubscriptionPlans.FindAsync(SubscriptionPlanBenefit.SubscriptionId);

            //check for subscription null and return not found
            if (subscription == null)
            {
                return NotFound(new { message = "Subscription not found." });
            }

            SubscriptionPlanBenefit data = new SubscriptionPlanBenefit
            {
                Id = SubscriptionPlanBenefit.Id,

                SubscriptionId = SubscriptionPlanBenefit.SubscriptionId,

                Status = SubscriptionPlanBenefit.Status,
                Benefit = SubscriptionPlanBenefit.Benefit,
                Rank = SubscriptionPlanBenefit.Rank,
                SubscriptionPlan = subscription,
                UpdatedAt = DateTime.Now,

            };

            _context.Entry(data).State = EntityState.Modified;


            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/SubscriptionPlanBenefit
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> PostSubscriptionPlanBenefit(SubscriptionPlanBenefitDto SubscriptionPlanBenefit)
        {
            if (_context.SubscriptionPlanBenefits == null)
            {
                return Problem("Entity set 'DatabaseContext.SubscriptionPlanBenefits'  is null.");
            }

            //cherck if subscription plan benefit exists
            if (SubscriptionPlanBenefitExists(SubscriptionPlanBenefit))
            {
                return BadRequest(new { message = "Subscription Plan Benefit already exists" });
            }

            //find subscription by subscriptionid
            SubscriptionPlan? subscription = await _context.SubscriptionPlans.FindAsync(SubscriptionPlanBenefit.SubscriptionId);

            //check for subscription null and return not found
            if (subscription == null)
            {
                return NotFound(new { message = "Subscription not found." });
            }

            SubscriptionPlanBenefit data = new SubscriptionPlanBenefit
            {
                Id = SubscriptionPlanBenefit.Id,

                SubscriptionId = SubscriptionPlanBenefit.SubscriptionId,

                Status = SubscriptionPlanBenefit.Status,
                Benefit = SubscriptionPlanBenefit.Benefit,
                Rank = SubscriptionPlanBenefit.Rank,
                SubscriptionPlan = subscription,
                UpdatedAt = DateTime.Now,
                CreatedAt = DateTime.Now
            };



            _context.SubscriptionPlans.Attach(data.SubscriptionPlan);
            _context.SubscriptionPlanBenefits.Add(data);


            await _context.SaveChangesAsync();

            SubscriptionPlanBenefit.Id = data.Id;


            return CreatedAtAction("GetSubscriptionPlanBenefit", new { id = data.Id }, SubscriptionPlanBenefit);
        }

        // DELETE: api/SubscriptionPlanBenefit/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubscriptionPlanBenefit(int id)
        {
            if (_context.SubscriptionPlanBenefits == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.SubscriptionPlanBenefits'  is null." });
            }
            var SubscriptionPlanBenefit = await _context.SubscriptionPlanBenefits.FindAsync(id);
            if (SubscriptionPlanBenefit == null)
            {
                return NotFound(new { message = "SubscriptionPlanBenefit not found." });
            }

            _context.SubscriptionPlanBenefits.Remove(SubscriptionPlanBenefit);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SubscriptionPlanBenefitExists(SubscriptionPlanBenefitDto SubscriptionPlanBenefit)
        {
            return (_context.SubscriptionPlanBenefits?.Any(e => e.Benefit == SubscriptionPlanBenefit.Benefit && e.SubscriptionId == SubscriptionPlanBenefit.SubscriptionId && e.Id != SubscriptionPlanBenefit.Id)).GetValueOrDefault();
        }

    }
}
