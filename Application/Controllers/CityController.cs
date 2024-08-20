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

    public class CityController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUriService uriService;
        private readonly IMapper _mapper;

        public CityController(DatabaseContext context, IUriService uriService, IMapper mapper)
        {
            _context = context;
            this.uriService = uriService;
            _mapper = mapper;
        }

        // GET: api/City
        [HttpGet]
        public async Task<ActionResult> GetCities([FromQuery] PaginationFilter filter)
        {
            if (_context.Cities == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Cities'  is null." });
            }

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.Cities
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).Select(x => new CityDto
               {
                   Id = x.Id,
                   Name = x.Name,
                   Code = x.Code,
                   Status = x.Status,
                   CountryId = x.CountryId,
                   CountryName = x.Country.Name
               }).AsNoTracking()
               .ToListAsync();
            var totalRecords = await _context.Cities.CountAsync();


            var pagedReponse = PaginationHelper.CreatePagedReponse<CityDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }


        // GET: api/City
        [HttpGet("countryId/{countryId}")]
        public async Task<ActionResult> GetCitiesByCountry([FromQuery] PaginationFilter filter, int countryId)
        {
            if (_context.Cities == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Cities'  is null." });
            }

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.Cities.Where(x => x.CountryId == countryId)
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).Select(x => new CityDto
               {
                   Id = x.Id,
                   Name = x.Name,
                   Code = x.Code,
                   Status = x.Status,
                   CountryId = x.CountryId,
                   CountryName = x.Country.Name
               }).AsNoTracking()
               .ToListAsync();
            var totalRecords = await _context.Cities.CountAsync();


            var pagedReponse = PaginationHelper.CreatePagedReponse<CityDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }


        // GET: api/City/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetCity(int id)
        {
            if (_context.Cities == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Cities'  is null." });
            }
            var city = await _context.Cities.FindAsync(id);

            if (city == null)
            {
                return NotFound(new { });
            }



            CityDto data = _mapper.Map<CityDto>(city);
            return Ok(new Wrappers.ApiResponse<CityDto>(data));
        }

        // PUT: api/City/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCity(int id, CityDto city)
        {
            if (id != city.Id)
            {
                return BadRequest(new { message = "Id mismatch" });
            }
            if (_context.Cities == null)
            {
                return Problem("Entity set 'DatabaseContext.Cities'  is null.");
            }

            if (_context.Countries == null)
            {
                return Problem("Entity set 'DatabaseContext.Countries'  is null.");
            }

            //code and name must be unique
            if (CityExists(city))
            {
                return BadRequest(new { message = "City name or code is already taken" });
            }



            Country? country = await _context.Countries.FindAsync(city.CountryId);

            if (country == null)
            {
                return NotFound(new { message = "Country not found" });
            }


            City data = new City
            {
                Name = city.Name,
                Country = country,
                Status = city.Status,
                UpdatedAt = DateTime.Now,
                CountryId = city.CountryId,
                Code = city.Code,
                Id = city.Id

            };


            // City data = _mapper.Map<City>(city);
            _context.Entry(data).State = EntityState.Modified;
            await _context.SaveChangesAsync();


            return NoContent();
        }

        // POST: api/City
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<City>> PostCity(CityDto city)
        {
            if (_context.Cities == null)
            {
                return Problem("Entity set 'DatabaseContext.Cities'  is null.");
            }

            if (_context.Countries == null)
            {
                return Problem("Entity set 'DatabaseContext.Countries'  is null.");
            }

            //code and name must be unique
            if (CityExists(city))
            {
                return BadRequest(new { message = "City name or code is already taken" });
            }

            Country? country = await _context.Countries.FindAsync(city.CountryId);

            if (country == null)
            {
                return NotFound(new { message = "Country not found" });
            }


            City data = new City
            {
                Name = city.Name,
                Country = country,
                Status = city.Status,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                CountryId = city.CountryId,
                Code = city.Code

            };

            _context.Countries.Attach(data.Country);
            _context.Cities.Add(data);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCity", new { id = data.Id }, _mapper.Map<CityDto>(data));
        }

        // DELETE: api/City/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCity(int id)
        {
            if (_context.Cities == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Cities'  is null." });
            }
            var city = await _context.Cities.FindAsync(id);
            if (city == null)
            {
                return NotFound(new { message = "City not found." });
            }

            _context.Cities.Remove(city);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CityExists(CityDto city)
        {
            return (_context.Cities?.Any(e => (e.Name == city.Name || e.Code == city.Code) && e.Id != city.Id)).GetValueOrDefault();
        }
    }
}
