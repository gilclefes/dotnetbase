using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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
    public class CurrencyController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUriService uriService;
        private readonly IMapper _mapper;

        public CurrencyController(DatabaseContext context, IUriService uriService, IMapper mapper)
        {
            _context = context;
            this.uriService = uriService;
            _mapper = mapper;
        }

        // GET: api/Currency
        [HttpGet]
        public async Task<ActionResult> GetCurrencies([FromQuery] PaginationFilter filter)
        {
            if (_context.Currencies == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Currencies'  is null." });
            }


            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.Currencies
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).AsNoTracking()
               .ToListAsync();
            var totalRecords = await _context.Currencies.CountAsync();
            List<CurrencyDto> ilistDest = _mapper.Map<List<Currency>, List<CurrencyDto>>(pagedData);

            var pagedReponse = PaginationHelper.CreatePagedReponse<CurrencyDto>(ilistDest, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        // GET: api/Currency/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetCurrency(int id)
        {
            if (_context.Currencies == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Currencies'  is null." });
            }
            var currency = await _context.Currencies.FindAsync(id);

            if (currency == null)
            {
                return NotFound(new { message = "Currency not found" });
            }


            CurrencyDto data = _mapper.Map<CurrencyDto>(currency);
            return Ok(new Wrappers.ApiResponse<CurrencyDto>(data));
        }

        // PUT: api/Currency/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCurrency(int id, CurrencyDto currency)
        {
            if (id != currency.Id)
            {
                return BadRequest(new { message = "Invalid Currency Id" });
            }

            //check if currency exists
            if (CurrencyExists(currency))
            {
                return BadRequest(new { message = "Currency name or code already exists" });
            }

            //Currency data = _mapper.Map<Currency>(currency);

            Currency data = new Currency
            {
                Id = currency.Id,
                Name = currency.Name,
                Code = currency.Code,
                Status = currency.Status,
                UpdatedAt = DateTime.Now,
            };
            _context.Entry(data).State = EntityState.Modified;



            await _context.SaveChangesAsync();


            return NoContent();
        }

        // POST: api/Currency
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Currency>> PostCurrency(CurrencyDto currency)
        {
            if (_context.Currencies == null)
            {
                return Problem("Entity set 'DatabaseContext.Currencies'  is null.");
            }


            Currency data = _mapper.Map<Currency>(currency);
            _context.Currencies.Add(data);

            await _context.SaveChangesAsync();
            return CreatedAtAction("GetCurrency", new { id = data.Id }, _mapper.Map<CurrencyDto>(data));


        }

        // DELETE: api/Currency/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCurrency(int id)
        {
            if (_context.Currencies == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Currencies'  is null." });
            }
            var currency = await _context.Currencies.FindAsync(id);
            if (currency == null)
            {
                return NotFound(new { message = "Currency not found" });
            }

            _context.Currencies.Remove(currency);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CurrencyExists(CurrencyDto currencyDto)
        {
            return (_context.Currencies?.Any(e => (e.Name == currencyDto.Name || e.Code == currencyDto.Code) && e.Id != currencyDto.Id)).GetValueOrDefault();
        }

    }
}
