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
    public class PeriodController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUriService uriService;
        private readonly IMapper _mapper;

        public PeriodController(DatabaseContext context, IUriService uriService, IMapper mapper)
        {
            _context = context;
            this.uriService = uriService;
            _mapper = mapper;
        }

        // GET: api/Period
        [HttpGet]
        public async Task<ActionResult> GetPeriods([FromQuery] PaginationFilter filter)
        {
            if (_context.Periods == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Periods'  is null." });
            }
            // return await _context.Periods.ToListAsync();
            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.Periods
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).AsNoTracking()
               .ToListAsync();
            var totalRecords = await _context.Periods.CountAsync();
            List<PeriodDto> ilistDest = _mapper.Map<List<Period>, List<PeriodDto>>(pagedData);

            var pagedReponse = PaginationHelper.CreatePagedReponse<PeriodDto>(ilistDest, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        // GET: api/Period/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Period>> GetPeriod(int id)
        {
            if (_context.Periods == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Periods'  is null." });
            }
            var Period = await _context.Periods.FindAsync(id);

            if (Period == null)
            {
                return NotFound(new { message = "Period not found." });
            }



            PeriodDto data = _mapper.Map<PeriodDto>(Period);
            return Ok(new Wrappers.ApiResponse<PeriodDto>(data));
        }

        // PUT: api/Period/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPeriod(int id, PeriodDto Period)
        {
            if (id != Period.Id)
            {
                return BadRequest(new { message = "Invalid Period Id." });
            }

            //check if period exist
            if (PeriodExists(Period))
            {
                return BadRequest(new { message = "Period name or code is already taken" });
            }

            Period data = _mapper.Map<Period>(Period);
            _context.Entry(data).State = EntityState.Modified;




            await _context.SaveChangesAsync();


            return NoContent();
        }

        // POST: api/Period
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostPeriod(PeriodDto Period)
        {
            if (_context.Periods == null)
            {
                return Problem("Entity set 'DatabaseContext.Periods'  is null.");
            }

            //check if period exist
            if (PeriodExists(Period))
            {
                return BadRequest(new { message = "Period name or code is already taken" });
            }

            Period data = _mapper.Map<Period>(Period);
            _context.Periods.Add(data);
            Period.Id = data.Id;


            await _context.SaveChangesAsync();


            return CreatedAtAction("GetPeriod", new { id = data.Id }, Period);
        }

        // DELETE: api/Period/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePeriod(int id)
        {
            if (_context.Periods == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Periods'  is null." });
            }
            var Period = await _context.Periods.FindAsync(id);
            if (Period == null)
            {
                return NotFound(new { message = "Period not found." });
            }

            _context.Periods.Remove(Period);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PeriodExists(PeriodDto period)
        {
            return (_context.Periods?.Any(e => (e.Name == period.Name || e.Code == period.Code) && e.Id != period.Id)).GetValueOrDefault();
        }

    }
}
