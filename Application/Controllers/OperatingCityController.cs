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
    public class OperatingCityController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUriService uriService;
        private readonly IMapper _mapper;

        public OperatingCityController(DatabaseContext context, IUriService uriService, IMapper mapper)
        {
            _context = context;
            this.uriService = uriService;
            _mapper = mapper;
        }

        // GET: api/OperatingCity
        [HttpGet]
        public async Task<ActionResult> GetCurrencies([FromQuery] PaginationFilter filter)
        {
            if (_context.Currencies == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Currencies'  is null." });
            }


            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.OperatingCities
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).AsNoTracking()
               .ToListAsync();
            var totalRecords = await _context.OperatingCities.CountAsync();
            List<OperatingCityDto> ilistDest = _mapper.Map<List<OperatingCity>, List<OperatingCityDto>>(pagedData);

            var pagedReponse = PaginationHelper.CreatePagedReponse<OperatingCityDto>(ilistDest, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        // GET: api/OperatingCity/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetOperatingCity(int id)
        {
            if (_context.Currencies == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Currencies'  is null." });
            }
            var OperatingCity = await _context.Currencies.FindAsync(id);

            if (OperatingCity == null)
            {
                return NotFound(new { message = "OperatingCity not found" });
            }


            OperatingCityDto data = _mapper.Map<OperatingCityDto>(OperatingCity);
            return Ok(new Wrappers.ApiResponse<OperatingCityDto>(data));
        }

        // PUT: api/OperatingCity/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOperatingCity(int id, OperatingCityDto OperatingCity)
        {
            if (id != OperatingCity.Id)
            {
                return BadRequest(new { message = "Invalid OperatingCity Id" });
            }

            OperatingCity data = _mapper.Map<OperatingCity>(OperatingCity);
            _context.Entry(data).State = EntityState.Modified;



            await _context.SaveChangesAsync();


            return NoContent();
        }

        // POST: api/OperatingCity
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<OperatingCity>> PostOperatingCity(OperatingCityDto OperatingCity)
        {
            if (_context.Currencies == null)
            {
                return Problem("Entity set 'DatabaseContext.Currencies'  is null.");
            }


            OperatingCity data = _mapper.Map<OperatingCity>(OperatingCity);
            _context.OperatingCities.Add(data);

            await _context.SaveChangesAsync();
            return CreatedAtAction("GetOperatingCity", new { id = data.Id }, _mapper.Map<OperatingCityDto>(data));


        }

        // DELETE: api/OperatingCity/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOperatingCity(int id)
        {
            if (_context.Currencies == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Currencies'  is null." });
            }
            var OperatingCity = await _context.Currencies.FindAsync(id);
            if (OperatingCity == null)
            {
                return NotFound(new { message = "OperatingCity not found" });
            }

            _context.Currencies.Remove(OperatingCity);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OperatingCityExists(int id)
        {
            return (_context.Currencies?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
