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
    public class PriceController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUriService uriService;
        private readonly IMapper _mapper;

        public PriceController(DatabaseContext context, IUriService uriService, IMapper mapper)
        {
            _context = context;
            this.uriService = uriService;
            _mapper = mapper;
        }

        // GET: api/Price
        [HttpGet]
        public async Task<ActionResult> GetPrices([FromQuery] PaginationFilter filter)
        {
            if (_context.Prices == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Prices'  is null." });
            }

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.Prices
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).Select(x => new PriceDto
               {
                   Id = x.Id,
                   ServiceId = x.ServiceId,
                   ItemId = x.ItemId,
                   PeriodId = x.PeriodId,
                   UnitTypeId = x.UnitTypeId,
                   Amount = x.Amount,
                   Status = x.Status,
                   ItemName = x.Item.Name,
                   ServiceName = x.Service.Name,
                   PeriodName = x.Period.Name,
                   UnitTypeName = x.UnitType.Name,

               }).AsNoTracking()
               .ToListAsync();
            var totalRecords = await _context.Prices.CountAsync();


            var pagedReponse = PaginationHelper.CreatePagedReponse<PriceDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        [HttpGet("Active")]
        public async Task<ActionResult> GetActivePrices([FromQuery] PaginationFilter filter)
        {
            if (_context.Prices == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Prices'  is null." });
            }

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.Prices.Include(x => x.Item).ThenInclude(x => x.ItemType).Where(x => x.Status == true)
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).Select(x => new PriceDto
               {
                   Id = x.Id,
                   ServiceId = x.ServiceId,
                   ItemId = x.ItemId,
                   PeriodId = x.PeriodId,
                   UnitTypeId = x.UnitTypeId,
                   Amount = x.Amount,
                   Status = x.Status,
                   ItemName = x.Item.Name,
                   ServiceName = x.Service.Name,
                   PeriodName = x.Period.Name,
                   UnitTypeName = x.UnitType.Name,
                   ItemTypeName = x.Item.ItemType.Name

               }).AsNoTracking()
               .ToListAsync();
            var totalRecords = await _context.Prices.CountAsync();


            var pagedReponse = PaginationHelper.CreatePagedReponse<PriceDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }



        // GET: api/Price/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Price>> GetPrice(int id)
        {
            if (_context.Prices == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Prices'  is null." });
            }
            var Price = await _context.Prices.FindAsync(id);

            if (Price == null)
            {
                return NotFound(new { message = "Price not found" });
            }



            PriceDto data = _mapper.Map<PriceDto>(Price);
            return Ok(new Wrappers.ApiResponse<PriceDto>(data));
        }

        // PUT: api/Price/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutPrice(int id, PriceDto Price)
        {

            if (_context.Prices == null)
            {
                return Problem("Entity set 'DatabaseContext.Prices'  is null.");
            }

            if (id != Price.Id)
            {
                return BadRequest(new { message = "Invalid Price Id" });
            }


            //check if price already exists
            if (PriceExists(Price))
            {
                return BadRequest(new { message = "Price already exists" });
            }


            LaundryItem? laundryItem = await _context.LaundryItems.FindAsync(Price.ItemId);
            if (laundryItem == null)
            {
                return NotFound(new { message = "Laundry Item not found" });
            }

            UnitType? unitType = await _context.UnitTypes.FindAsync(Price.UnitTypeId);
            if (unitType == null)
            {
                return NotFound(new { message = "Unit Type not found" });
            }

            Period? period = await _context.Periods.FindAsync(Price.PeriodId);
            if (period == null)
            {
                return NotFound(new { message = "Period not found" });
            }

            Service? service = await _context.Services.FindAsync(Price.ServiceId);
            if (service == null)
            {
                return NotFound(new { message = "Service not found" });
            }


            Price data = new Price
            {
                ServiceId = Price.ServiceId,
                ItemId = Price.ItemId,
                PeriodId = Price.PeriodId,
                UnitTypeId = Price.UnitTypeId,
                Amount = Price.Amount,
                Status = Price.Status,
                Item = laundryItem,
                Service = service,
                Period = period,
                UnitType = unitType,
                UpdatedAt = DateTime.Now,
                Id = Price.Id

            };

            _context.Entry(data).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Price
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> PostPrice(PriceDto Price)
        {
            if (_context.Prices == null)
            {
                return Problem("Entity set 'DatabaseContext.Prices'  is null.");
            }

            //check if price already exists
            if (PriceExists(Price))
            {
                return BadRequest(new { message = "Price already exists" });
            }


            LaundryItem? laundryItem = await _context.LaundryItems.FindAsync(Price.ItemId);
            if (laundryItem == null)
            {
                return NotFound(new { message = "Laundry Item not found" });
            }

            UnitType? unitType = await _context.UnitTypes.FindAsync(Price.UnitTypeId);
            if (unitType == null)
            {
                return NotFound(new { message = "Unit Type not found" });
            }

            Period? period = await _context.Periods.FindAsync(Price.PeriodId);
            if (period == null)
            {
                return NotFound(new { message = "Period not found" });
            }

            Service? service = await _context.Services.FindAsync(Price.ServiceId);
            if (service == null)
            {
                return NotFound(new { message = "Service not found" });
            }


            Price data = new Price
            {
                ServiceId = Price.ServiceId,
                ItemId = Price.ItemId,
                PeriodId = Price.PeriodId,
                UnitTypeId = Price.UnitTypeId,
                Amount = Price.Amount,
                Status = Price.Status,
                Item = laundryItem,
                Service = service,
                Period = period,
                UnitType = unitType,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Services.Attach(data.Service);
            _context.LaundryItems.Attach(data.Item);
            _context.Periods.Attach(data.Period);
            _context.UnitTypes.Attach(data.UnitType);

            _context.Prices.Add(data);


            await _context.SaveChangesAsync();
            Price.Id = data.Id;


            return CreatedAtAction("GetPrice", new { id = data.Id }, Price);
        }

        // DELETE: api/Price/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrice(int id)
        {
            if (_context.Prices == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Prices'  is null." });
            }
            var Price = await _context.Prices.FindAsync(id);
            if (Price == null)
            {
                return NotFound(new { message = "Price not found" });
            }

            _context.Prices.Remove(Price);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PriceExists(PriceDto priceDto)
        {
            return (_context.Prices?.Any(e => e.UnitTypeId == priceDto.UnitTypeId && e.ItemId == priceDto.ItemId && e.ServiceId == priceDto.ServiceId && e.PeriodId == priceDto.PeriodId && (e.Id != priceDto.Id))).GetValueOrDefault();
        }
    }
}
