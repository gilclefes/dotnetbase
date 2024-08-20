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
    //[Authorize]
    public class SubscriptionPlanPriceController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUriService uriService;
        private readonly IMapper _mapper;

        public SubscriptionPlanPriceController(DatabaseContext context, IUriService uriService, IMapper mapper)
        {
            _context = context;
            this.uriService = uriService;
            _mapper = mapper;
        }

        // GET: api/SubscriptionPlanPrice
        [HttpGet]
        public async Task<ActionResult> GetSubscriptionPlanPrices([FromQuery] PaginationFilter filter)
        {
            if (_context.SubscriptionPlanPrices == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.SubscriptionPlanPrices'  is null." });
            }

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.SubscriptionPlanPrices
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).Select(x => new SubscriptionPlanPriceDto
               {
                   Id = x.Id,
                   SubscriptionId = x.SubscriptionId,
                   PeriodId = x.PeriodId,
                   Amount = x.Amount,
                   SubscriptionPlanName = x.SubscriptionPlan.Name,
                   PeriodName = x.Period.Name,
                   CurrencyName = x.Currency.Name,
                   CurrencyId = x.CurrencyId,
                   Status = x.Status,
                   Description = x.Description,
                   IsFavorite = x.IsFavorite
               }).AsNoTracking()

               .ToListAsync();
            var totalRecords = await _context.OrderAssignments.CountAsync();

            var pagedReponse = PaginationHelper.CreatePagedReponse<SubscriptionPlanPriceDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        [HttpGet("Full")]
        public async Task<ActionResult> GetSubscriptionPlanPricesFull([FromQuery] PaginationFilter filter, string servicename)
        {
            if (_context.SubscriptionPlanPrices == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.SubscriptionPlanPrices'  is null." });
            }

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.SubscriptionPlanPrices.Include(x => x.Period)
            .Include(x => x.SubscriptionPlan).ThenInclude(x => x.SubscriptionPlanBenefits)
            .Where(x => x.Status && x.SubscriptionPlan.SubscriptionPlanServices.Any(x => x.Service.Name == servicename))
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).Select(x => new SubscriptionPlanPriceFullDto
               {
                   Id = x.Id,
                   SubscriptionId = x.SubscriptionId,
                   PeriodId = x.PeriodId,
                   Amount = x.Amount,
                   SubscriptionPlanName = x.SubscriptionPlan.Name,
                   PeriodName = x.Period.Name,
                   CurrencyName = x.Currency.Name,
                   CurrencyId = x.CurrencyId,
                   Status = x.Status,
                   Description = x.Description,
                   IsFavorite = x.IsFavorite,
                   SubscriptionPlanDescription = x.SubscriptionPlan.Description,
                   SubscriptionPlanTermAndConditions = x.SubscriptionPlan.TermAndConditions,
                   subscriptionPlanBenefitDtos = x.SubscriptionPlan.SubscriptionPlanBenefits.Where(x => x.Status).OrderBy(x => x.Rank).Select(x => new SubscriptionPlanBenefitDto
                   {
                       Id = x.Id,
                       SubscriptionId = x.SubscriptionId,
                       Status = x.Status,
                       Benefit = x.Benefit,
                       Rank = x.Rank,
                       SubscriptionPlanName = x.SubscriptionPlan.Name,

                   }).OrderBy(x => x.Rank).ToList()
               }).AsNoTracking()


               .ToListAsync();
            var totalRecords = await _context.OrderAssignments.CountAsync();

            var pagedReponse = PaginationHelper.CreatePagedReponse<SubscriptionPlanPriceFullDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }



        [HttpGet("Plan/{id}")]
        public async Task<ActionResult> GetSubscriptionPlanPricesByPlan(int id, [FromQuery] PaginationFilter filter)
        {
            if (_context.SubscriptionPlanPrices == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.SubscriptionPlanPrices'  is null." });
            }

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.SubscriptionPlanPrices.Where(x => x.SubscriptionId == id)
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).Select(x => new SubscriptionPlanPriceDto
               {
                   Id = x.Id,
                   SubscriptionId = x.SubscriptionId,
                   PeriodId = x.PeriodId,
                   Amount = x.Amount,
                   SubscriptionPlanName = x.SubscriptionPlan.Name,
                   PeriodName = x.Period.Name,
                   CurrencyName = x.Currency.Name,
                   CurrencyId = x.CurrencyId,
                   Status = x.Status,
                   Description = x.Description,
                   IsFavorite = x.IsFavorite
               }).AsNoTracking()

               .ToListAsync();
            var totalRecords = await _context.OrderAssignments.CountAsync();

            var pagedReponse = PaginationHelper.CreatePagedReponse<SubscriptionPlanPriceDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        // GET: api/SubscriptionPlanPrice/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SubscriptionPlanPrice>> GetSubscriptionPlanPrice(int id)
        {
            if (_context.SubscriptionPlanPrices == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.SubscriptionPlanPrices'  is null." });
            }
            var SubscriptionPlanPrice = await _context.SubscriptionPlanPrices.FindAsync(id);

            if (SubscriptionPlanPrice == null)
            {
                return NotFound(new { message = "SubscriptionPlanPrice not found." });
            }



            SubscriptionPlanPriceDto data = _mapper.Map<SubscriptionPlanPriceDto>(SubscriptionPlanPrice);
            return Ok(new Wrappers.ApiResponse<SubscriptionPlanPriceDto>(data));
        }

        // PUT: api/SubscriptionPlanPrice/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutSubscriptionPlanPrice(int id, SubscriptionPlanPriceDto SubscriptionPlanPrice)
        {
            if (id != SubscriptionPlanPrice.Id)
            {
                return BadRequest(new { message = "Invalid SubscriptionPlanPrice Id." });
            }


            //check if the name is unique
            if (SubscriptionPlanPriceExists(SubscriptionPlanPrice))
            {
                return BadRequest("SubscriptionPlanPrice already exists");
            }

            //find period with periodId
            Period? period = await _context.Periods.FindAsync(SubscriptionPlanPrice.PeriodId);
            //check for null and return not found
            if (period == null)
            {
                return NotFound(new { message = "Period does not exist." });
            }

            //find subscription with subscriptionId
            SubscriptionPlan? subscription = await _context.SubscriptionPlans.FindAsync(SubscriptionPlanPrice.SubscriptionId);
            //check for null and return not found
            if (subscription == null)
            {
                return NotFound(new { });
            }

            //find currency with currency id
            Currency? currency = await _context.Currencies.FindAsync(SubscriptionPlanPrice.CurrencyId);

            //check for null and return not found
            if (currency == null)
            {
                return NotFound(new { message = "Currency does not exist." });
            }

            SubscriptionPlanPrice data = new SubscriptionPlanPrice
            {
                SubscriptionId = SubscriptionPlanPrice.SubscriptionId,
                PeriodId = SubscriptionPlanPrice.PeriodId,
                Amount = SubscriptionPlanPrice.Amount,
                SubscriptionPlan = subscription,
                Period = period,
                UpdatedAt = DateTime.Now,

                Currency = currency,
                CurrencyId = currency.Id,
                Id = SubscriptionPlanPrice.Id,
                Description = SubscriptionPlanPrice.Description,
                Status = SubscriptionPlanPrice.Status,
                IsFavorite = SubscriptionPlanPrice.IsFavorite

            };

            _context.Entry(data).State = EntityState.Modified;




            await _context.SaveChangesAsync();


            return NoContent();
        }

        // POST: api/SubscriptionPlanPrice
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> PostSubscriptionPlanPrice(SubscriptionPlanPriceDto SubscriptionPlanPrice)
        {
            if (_context.SubscriptionPlanPrices == null)
            {
                return Problem("Entity set 'DatabaseContext.SubscriptionPlanPrices'  is null.");
            }

            //check if the name is unique
            if (SubscriptionPlanPriceExists(SubscriptionPlanPrice))
            {
                return BadRequest(new { message = "SubscriptionPlanPrice already exists." });
            }

            //find period with periodId
            Period? period = await _context.Periods.FindAsync(SubscriptionPlanPrice.PeriodId);
            //check for null and return not found
            if (period == null)
            {
                return NotFound(new { message = "Period does not exist." });
            }

            //find subscription with subscriptionId
            SubscriptionPlan? subscription = await _context.SubscriptionPlans.FindAsync(SubscriptionPlanPrice.SubscriptionId);
            //check for null and return not found
            if (subscription == null)
            {
                return NotFound(new { message = "Subscription does not exist." });
            }

            //find currency with currency id
            Currency? currency = await _context.Currencies.FindAsync(SubscriptionPlanPrice.CurrencyId);

            //check for null and return not found
            if (currency == null)
            {
                return NotFound(new { message = "Currency does not exist." });
            }

            SubscriptionPlanPrice data = new SubscriptionPlanPrice
            {
                SubscriptionId = SubscriptionPlanPrice.SubscriptionId,
                PeriodId = SubscriptionPlanPrice.PeriodId,
                Amount = SubscriptionPlanPrice.Amount,
                SubscriptionPlan = subscription,
                Period = period,
                UpdatedAt = DateTime.Now,
                CreatedAt = DateTime.Now,
                Currency = currency,
                CurrencyId = currency.Id,
                Description = SubscriptionPlanPrice.Description,
                Status = SubscriptionPlanPrice.Status,
                IsFavorite = SubscriptionPlanPrice.IsFavorite
            };

            _context.Currencies.Attach(data.Currency);
            _context.SubscriptionPlans.Attach(data.SubscriptionPlan);
            _context.Periods.Attach(data.Period);

            _context.SubscriptionPlanPrices.Add(data);


            await _context.SaveChangesAsync();
            SubscriptionPlanPrice.Id = data.Id;

            return CreatedAtAction("GetSubscriptionPlanPrice", new { id = data.Id }, SubscriptionPlanPrice);
        }

        // DELETE: api/SubscriptionPlanPrice/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteSubscriptionPlanPrice(int id)
        {
            if (_context.SubscriptionPlanPrices == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.SubscriptionPlanPrices'  is null." });
            }
            var SubscriptionPlanPrice = await _context.SubscriptionPlanPrices.FindAsync(id);
            if (SubscriptionPlanPrice == null)
            {
                return NotFound(new { message = "SubscriptionPlanPrice not found." });
            }

            _context.SubscriptionPlanPrices.Remove(SubscriptionPlanPrice);
            await _context.SaveChangesAsync();


            return NoContent();
        }

        private bool SubscriptionPlanPriceExists(SubscriptionPlanPriceDto subscriptionPlanPriceDto)
        {
            return (_context.SubscriptionPlanPrices?.Any(e => e.PeriodId == subscriptionPlanPriceDto.PeriodId && e.SubscriptionId == subscriptionPlanPriceDto.SubscriptionId && e.Id != subscriptionPlanPriceDto.Id)).GetValueOrDefault();
        }

    }
}
