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
    public class CountryController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUriService uriService;
        private readonly IMapper _mapper;

        public CountryController(DatabaseContext context, IUriService uriService, IMapper mapper)
        {
            _context = context;
            this.uriService = uriService;
            _mapper = mapper;
        }

        // GET: api/Country
        [HttpGet]
        public async Task<ActionResult> GetCountries([FromQuery] PaginationFilter filter)
        {
            if (_context.Countries == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Countries'  is null." });
            }
            //return await _context.Countries.ToListAsync();
            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.Countries
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).AsNoTracking()
               .ToListAsync();
            var totalRecords = await _context.Countries.CountAsync();
            List<CountryDto> ilistDest = _mapper.Map<List<Country>, List<CountryDto>>(pagedData);

            var pagedReponse = PaginationHelper.CreatePagedReponse<CountryDto>(ilistDest, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        // GET: api/Country/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetCountry(int id)
        {
            if (_context.Countries == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Countries'  is null." });
            }
            var country = await _context.Countries.FindAsync(id);

            if (country == null)
            {
                return NotFound(new { message = "Country not found" });
            }



            CountryDto data = _mapper.Map<CountryDto>(country);
            return Ok(new Wrappers.ApiResponse<CountryDto>(data));
        }

        // PUT: api/Country/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCountry(int id, CountryDto country)
        {
            if (id != country.Id)
            {
                return BadRequest(new { message = "Invalid Country Id" });
            }

            //check if country name or code already exists
            if (CountryExists(country))
            {
                return BadRequest(new { message = "Country name or code already exists" });
            }

            Country data = _mapper.Map<Country>(country);
            _context.Entry(data).State = EntityState.Modified;


            await _context.SaveChangesAsync();


            return NoContent();
        }

        // POST: api/Country
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Country>> PostCountry(CountryDto country)
        {
            if (_context.Countries == null)
            {
                return Problem("Entity set 'DatabaseContext.Countries'  is null.");
            }

            //check if country name or code already exists
            if (CountryExists(country))
            {
                return BadRequest(new { message = "Country name or code already exists" });
            }

            Country data = _mapper.Map<Country>(country);
            _context.Countries.Add(data);

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCountry", new { id = data.Id }, _mapper.Map<CountryDto>(data));

        }

        // DELETE: api/Country/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            if (_context.Countries == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Countries'  is null." });
            }
            var country = await _context.Countries.FindAsync(id);
            if (country == null)
            {
                return NotFound(new { message = "Country not found" });
            }

            _context.Countries.Remove(country);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CountryExists(CountryDto countryDto)
        {
            return (_context.Countries?.Any(e => (e.Name == countryDto.Name || e.Code == countryDto.Code) && e.Id != countryDto.Id)).GetValueOrDefault();
        }

    }
}
