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
    public class SubscriptionPlanController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUriService uriService;
        private readonly IMapper _mapper;

        public SubscriptionPlanController(DatabaseContext context, IUriService uriService, IMapper mapper)
        {
            _context = context;
            this.uriService = uriService;
            _mapper = mapper;
        }

        // GET: api/SubscriptionPlan
        [HttpGet]
        public async Task<ActionResult> GetSubscriptionPlans([FromQuery] PaginationFilter filter)
        {
            if (_context.SubscriptionPlans == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.SubscriptionPlans'  is null." });
            }

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.SubscriptionPlans
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).Select(x => new SubscriptionPlanDto
               {
                   Id = x.Id,
                   Name = x.Name,
                   Description = x.Description,

                   PeriodName = x.Period.Name,

                   Status = x.Status,
                   MinOrder = x.MinOrder,
                   MinOrderPenalty = x.MinOrderPenalty,
                   OrderFrequency = x.OrderFrequency,
                   OrderPeriodId = x.OrderPeriodId,
                   TermAndConditions = x.TermAndConditions,



               }).AsNoTracking()
               .ToListAsync();
            var totalRecords = await _context.OrderAssignments.CountAsync();

            var pagedReponse = PaginationHelper.CreatePagedReponse<SubscriptionPlanDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        [HttpGet("Active")]
        public async Task<ActionResult> GetActiveSubscriptionPlans([FromQuery] PaginationFilter filter)
        {
            if (_context.SubscriptionPlans == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.SubscriptionPlans'  is null." });
            }

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.SubscriptionPlans.Where(x => x.Status == true)
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).Select(x => new SubscriptionPlanDto
               {
                   Id = x.Id,
                   Name = x.Name,
                   Description = x.Description,

                   PeriodName = x.Period.Name,

                   Status = x.Status,
                   MinOrder = x.MinOrder,
                   MinOrderPenalty = x.MinOrderPenalty,
                   OrderFrequency = x.OrderFrequency,
                   OrderPeriodId = x.OrderPeriodId,
                   TermAndConditions = x.TermAndConditions,



               }).AsNoTracking()
               .ToListAsync();
            var totalRecords = await _context.OrderAssignments.CountAsync();

            var pagedReponse = PaginationHelper.CreatePagedReponse<SubscriptionPlanDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        // GET: api/SubscriptionPlan/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SubscriptionPlan>> GetSubscriptionPlan(int id)
        {
            if (_context.SubscriptionPlans == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.SubscriptionPlans'  is null." });
            }
            var SubscriptionPlan = await _context.SubscriptionPlans.FindAsync(id);

            if (SubscriptionPlan == null)
            {
                return NotFound(new { message = "SubscriptionPlan not found." });
            }



            SubscriptionPlanDto data = _mapper.Map<SubscriptionPlanDto>(SubscriptionPlan);
            data.PeriodName = SubscriptionPlan.Period.Name;
            return Ok(new Wrappers.ApiResponse<SubscriptionPlanDto>(data));
        }

        [HttpGet("Full/{id}")]
        public async Task<ActionResult<SubscriptionFullDto>> GetSubscriptionPlanFull(int id)
        {
            if (_context.SubscriptionPlans == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.SubscriptionPlans'  is null." });
            }
            SubscriptionPlanDto? subscriptionPlanDto = await _context.SubscriptionPlans.Where(x => x.Id == id).Select(x => new SubscriptionPlanDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,

                PeriodName = x.Period.Name,

                Status = x.Status,
                MinOrder = x.MinOrder,
                MinOrderPenalty = x.MinOrderPenalty,
                OrderFrequency = x.OrderFrequency,
                OrderPeriodId = x.OrderPeriodId,
                TermAndConditions = x.TermAndConditions
            }
                ).FirstOrDefaultAsync();

            if (subscriptionPlanDto == null)
            {
                return NotFound(new { message = "SubscriptionPlan not found." });
            }


            List<SubscriptionPlanChargeDto> subscriptionPlanChargeDto = await _context.SubscriptionPlanCharges.Include(x => x.Charge).Where(x => x.SubscriptionId == id).Select(x => new SubscriptionPlanChargeDto
            {
                Amount = x.Amount,
                ChargeId = x.ChargeId,
                ChargeName = x.Charge.Name,
                ChargeAmountType = x.Charge.AmountType,
                Id = x.Id,
                SubscriptionId = x.SubscriptionId,

                Description = x.Description,
                Status = x.Status,
                SubscriptionPlanName = x.SubscriptionPlan.Name,


            }).ToListAsync();

            List<SubscriptionPlanChargeExemptionDto> subscriptionPlanChargeExemptionDto = await _context.SubscriptionPlanChargeExemptions.Include(x => x.Charge).Where(x => x.SubscriptionId == id).Select(x => new SubscriptionPlanChargeExemptionDto
            {

                ChargeId = x.ChargeId,
                ChargeName = x.Charge.Name,
                ChargeAmountType = x.Charge.AmountType,
                Id = x.Id,
                SubscriptionId = x.SubscriptionId,

                Description = x.Description,
                Status = x.Status,
                SubscriptionPlanName = x.SubscriptionPlan.Name,


            }).ToListAsync();

            List<SubscriptionPlanServiceDto> subscriptionPlanServiceDto = await _context.SubscriptionPlanServices.Where(x => x.SubscriptionId == id).Select(x => new SubscriptionPlanServiceDto
            {

                ServiceId = x.ServiceId,
                ServiceName = x.Service.Name,
                Id = x.Id,
                SubscriptionId = x.SubscriptionId,

                Description = x.Description,
                Status = x.Status,
                SubscriptionPlanName = x.SubscriptionPlan.Name,


            }).ToListAsync();

            List<SubscriptionPlanPriceDto> subscriptionPlanPriceDto = await _context.SubscriptionPlanPrices.Where(x => x.SubscriptionId == id).Select(x => new SubscriptionPlanPriceDto
            {

                Id = x.Id,
                SubscriptionId = x.SubscriptionId,

                Description = x.Description,
                Status = x.Status,
                SubscriptionPlanName = x.SubscriptionPlan.Name,
                Amount = x.Amount,

                CurrencyId = x.CurrencyId,
                CurrencyName = x.Currency.Name,
                PeriodId = x.PeriodId,
                PeriodName = x.Period.Name,
            }).ToListAsync();

            List<SubscriptionPlanBenefitDto> subscriptionPlanBenefitDto = await _context.SubscriptionPlanBenefits.Where(x => x.SubscriptionId == id).OrderBy(x => x.Rank).Select(x => new SubscriptionPlanBenefitDto
            {

                Id = x.Id,
                SubscriptionId = x.SubscriptionId,


                Status = x.Status,
                SubscriptionPlanName = x.SubscriptionPlan.Name,
                Benefit = x.Benefit,
                Rank = x.Rank,



            }).ToListAsync();

            SubscriptionFullDto data = new SubscriptionFullDto
            {
                subscriptionPlanDto = subscriptionPlanDto,
                subscriptionPlanChargeDto = subscriptionPlanChargeDto,
                subscriptionPlanServiceDto = subscriptionPlanServiceDto,
                subscriptionPlanPriceDto = subscriptionPlanPriceDto,
                subscriptionPlanBenefitDto = subscriptionPlanBenefitDto,
                subscriptionPlanChargeExemptionDto = subscriptionPlanChargeExemptionDto
            }
            ;

            return Ok(new Wrappers.ApiResponse<SubscriptionFullDto>(data));
        }

        // PUT: api/SubscriptionPlan/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutSubscriptionPlan(int id, SubscriptionPlanDto SubscriptionPlan)
        {
            if (id != SubscriptionPlan.Id)
            {
                return BadRequest(new { message = "Invalid SubscriptionPlan Id." });
            }

            //check if the name is unique
            if (SubscriptionPlanExists(SubscriptionPlan))
            {
                return BadRequest(new { message = "SubscriptionPlan name already exists." });
            }

            if (_context.SubscriptionPlans == null)
            {
                return Problem("Entity set 'DatabaseContext.SubscriptionPlans'  is null.");
            }

            //find period using subscriptionPlan.periodid
            Period? period = await _context.Periods.FindAsync(SubscriptionPlan.OrderPeriodId);

            if (period == null)
            {
                return BadRequest(new { message = "Period does not exist." });
            }

            SubscriptionPlan data = new SubscriptionPlan
            {
                Id = SubscriptionPlan.Id,
                Name = SubscriptionPlan.Name,
                Description = SubscriptionPlan.Description,
                Status = SubscriptionPlan.Status,
                MinOrder = SubscriptionPlan.MinOrder,
                MinOrderPenalty = SubscriptionPlan.MinOrderPenalty,
                OrderFrequency = SubscriptionPlan.OrderFrequency,
                OrderPeriodId = SubscriptionPlan.OrderPeriodId,
                TermAndConditions = SubscriptionPlan.TermAndConditions,
                Period = period,
                UpdatedAt = DateTime.Now,
            };
            _context.Entry(data).State = EntityState.Modified;

            await _context.SaveChangesAsync();


            return NoContent();
        }

        // POST: api/SubscriptionPlan
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> PostSubscriptionPlan(SubscriptionPlanDto SubscriptionPlan)
        {
            if (_context.SubscriptionPlans == null)
            {
                return Problem("Entity set 'DatabaseContext.SubscriptionPlans'  is null.");
            }

            if (SubscriptionPlanExists(SubscriptionPlan))
            {
                return BadRequest(new { message = "SubscriptionPlan name already exists." });
            }

            if (_context.SubscriptionPlans == null)
            {
                return Problem("Entity set 'DatabaseContext.SubscriptionPlans'  is null.");
            }

            //find period using subscriptionPlan.periodid
            Period? period = await _context.Periods.FindAsync(SubscriptionPlan.OrderPeriodId);

            if (period == null)
            {
                return BadRequest(new { message = "Period does not exist." });
            }

            SubscriptionPlan data = new SubscriptionPlan
            {
                Name = SubscriptionPlan.Name,
                Description = SubscriptionPlan.Description,
                Status = SubscriptionPlan.Status,
                MinOrder = SubscriptionPlan.MinOrder,
                MinOrderPenalty = SubscriptionPlan.MinOrderPenalty,
                OrderFrequency = SubscriptionPlan.OrderFrequency,
                OrderPeriodId = SubscriptionPlan.OrderPeriodId,
                TermAndConditions = SubscriptionPlan.TermAndConditions,
                Period = period,
                UpdatedAt = DateTime.Now,
                CreatedAt = DateTime.Now
            };

            _context.Periods.Attach(data.Period);
            _context.SubscriptionPlans.Add(data);

            await _context.SaveChangesAsync();
            SubscriptionPlan.Id = data.Id;

            return CreatedAtAction("GetSubscriptionPlan", new { id = data.Id }, SubscriptionPlan);
        }

        // DELETE: api/SubscriptionPlan/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteSubscriptionPlan(int id)
        {
            if (_context.SubscriptionPlans == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.SubscriptionPlans'  is null." });
            }
            var SubscriptionPlan = await _context.SubscriptionPlans.FindAsync(id);
            if (SubscriptionPlan == null)
            {
                return NotFound(new { message = "SubscriptionPlan not found." });
            }

            _context.SubscriptionPlans.Remove(SubscriptionPlan);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SubscriptionPlanExists(SubscriptionPlanDto subscriptionPlanDto)
        {
            return (_context.SubscriptionPlans?.Any(e => e.Name == subscriptionPlanDto.Name && e.Id != subscriptionPlanDto.Id)).GetValueOrDefault();
        }

    }
}
