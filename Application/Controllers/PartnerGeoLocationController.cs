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

    public class PartnerGeoLocationController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUriService uriService;
        private readonly IMapper _mapper;

        public PartnerGeoLocationController(DatabaseContext context, IUriService uriService, IMapper mapper)
        {
            _context = context;
            this.uriService = uriService;
            _mapper = mapper;
        }

        // GET: api/PartnerGeoLocation
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> GetPartnerGeoLocations([FromQuery] PaginationFilter filter)
        {
            if (_context.PartnerGeoLocations == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.PartnerGeoLocations'  is null." });
            }
            // return await _context.PartnerGeoLocations.ToListAsync();
            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.PartnerGeoLocations
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).AsNoTracking()
               .ToListAsync();
            var totalRecords = await _context.PartnerGeoLocations.CountAsync();
            List<PartnerGeoLocationDto> ilistDest = _mapper.Map<List<PartnerGeoLocation>, List<PartnerGeoLocationDto>>(pagedData);

            var pagedReponse = PaginationHelper.CreatePagedReponse<PartnerGeoLocationDto>(ilistDest, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        // GET: api/PartnerGeoLocation/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PartnerGeoLocation>> GetPartnerGeoLocation(int id)
        {
            if (_context.PartnerGeoLocations == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.PartnerGeoLocations'  is null." });
            }
            var PartnerGeoLocation = await _context.PartnerGeoLocations.FindAsync(id);

            if (PartnerGeoLocation == null)
            {
                return NotFound(new { message = "PartnerGeoLocation not found." });
            }



            PartnerGeoLocationDto data = _mapper.Map<PartnerGeoLocationDto>(PartnerGeoLocation);
            return Ok(new Wrappers.ApiResponse<PartnerGeoLocationDto>(data));
        }

        // PUT: api/PartnerGeoLocation/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPartnerGeoLocation(int id, PartnerGeoLocationDto PartnerGeoLocation)
        {
            if (id != PartnerGeoLocation.Id)
            {
                return BadRequest(new { message = "Invalid PartnerGeoLocation Id." });
            }


            PartnerGeoLocation data = _mapper.Map<PartnerGeoLocation>(PartnerGeoLocation);
            _context.Entry(data).State = EntityState.Modified;



            await _context.SaveChangesAsync();


            return NoContent();
        }

        // POST: api/PartnerGeoLocation
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostPartnerGeoLocation(PartnerGeoLocationDto PartnerGeoLocation)
        {
            if (_context.PartnerGeoLocations == null)
            {
                return Problem("Entity set 'DatabaseContext.PartnerGeoLocations'  is null.");
            }

            PartnerGeoLocation data = _mapper.Map<PartnerGeoLocation>(PartnerGeoLocation);
            _context.PartnerGeoLocations.Add(data);


            await _context.SaveChangesAsync();


            return CreatedAtAction("GetPartnerGeoLocation", new { id = data.Id }, _mapper.Map<IdTypeDto>(data));
        }

        // DELETE: api/PartnerGeoLocation/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePartnerGeoLocation(int id)
        {
            if (_context.PartnerGeoLocations == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.PartnerGeoLocations'  is null." });
            }
            var PartnerGeoLocation = await _context.PartnerGeoLocations.FindAsync(id);
            if (PartnerGeoLocation == null)
            {
                return NotFound(new { message = "PartnerGeoLocation not found." });
            }

            _context.PartnerGeoLocations.Remove(PartnerGeoLocation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PartnerGeoLocationExists(int id)
        {
            return (_context.PartnerGeoLocations?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
