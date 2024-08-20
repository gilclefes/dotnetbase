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
    public class ServiceProviderGeoLocationController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUriService uriService;
        private readonly IMapper _mapper;

        public ServiceProviderGeoLocationController(DatabaseContext context, IUriService uriService, IMapper mapper)
        {
            _context = context;
            this.uriService = uriService;
            _mapper = mapper;
        }

        // GET: api/ServiceProviderGeoLocation
        [HttpGet]
        public async Task<ActionResult> GetServiceProviderGeoLocations([FromQuery] PaginationFilter filter)
        {
            if (_context.ServiceProviderGeoLocations == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.ServiceProviderGeoLocations'  is null." });
            }

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.ServiceProviderGeoLocations
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).AsNoTracking()
               .ToListAsync();
            var totalRecords = await _context.OrderAssignments.CountAsync();
            List<ServiceProviderGeoLocationDto> ilistDest = _mapper.Map<List<ServiceProviderGeoLocation>, List<ServiceProviderGeoLocationDto>>(pagedData);

            var pagedReponse = PaginationHelper.CreatePagedReponse<ServiceProviderGeoLocationDto>(ilistDest, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        // GET: api/ServiceProviderGeoLocation/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceProviderGeoLocation>> GetServiceProviderGeoLocation(int id)
        {
            if (_context.ServiceProviderGeoLocations == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.ServiceProviderGeoLocations'  is null." });
            }
            var ServiceProviderGeoLocation = await _context.ServiceProviderGeoLocations.FindAsync(id);

            if (ServiceProviderGeoLocation == null)
            {
                return NotFound(new { message = "ServiceProviderGeoLocation not found." });
            }



            ServiceProviderGeoLocationDto data = _mapper.Map<ServiceProviderGeoLocationDto>(ServiceProviderGeoLocation);
            return Ok(new Wrappers.ApiResponse<ServiceProviderGeoLocationDto>(data));
        }

        [HttpGet("byServiceProvider/{id}")]
        public async Task<ActionResult<ServiceProviderGeoLocation>> GetServiceProviderGeoLocationByProvider(int id)
        {
            if (_context.ServiceProviderGeoLocations == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.ServiceProviderGeoLocations'  is null." });
            }
            var ServiceProviderGeoLocation = await _context.ServiceProviderGeoLocations.FirstOrDefaultAsync(x => x.ServiceProviderId == id);

            if (ServiceProviderGeoLocation == null)
            {
                return NotFound(new { message = "ServiceProviderGeoLocation not found." });
            }



            ServiceProviderGeoLocationDto data = _mapper.Map<ServiceProviderGeoLocationDto>(ServiceProviderGeoLocation);
            return Ok(new Wrappers.ApiResponse<ServiceProviderGeoLocationDto>(data));
        }

        // PUT: api/ServiceProviderGeoLocation/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutServiceProviderGeoLocation(int id, ServiceProviderGeoLocationDto ServiceProviderGeoLocation)
        {
            if (id != ServiceProviderGeoLocation.Id)
            {
                return BadRequest();
            }


            ServiceProviderGeoLocation data = _mapper.Map<ServiceProviderGeoLocation>(ServiceProviderGeoLocation);
            _context.Entry(data).State = EntityState.Modified;



            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceProviderGeoLocationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ServiceProviderGeoLocation
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostServiceProviderGeoLocation(ServiceProviderGeoLocationDto ServiceProviderGeoLocation)
        {
            if (_context.ServiceProviderGeoLocations == null)
            {
                return Problem("Entity set 'DatabaseContext.ServiceProviderGeoLocations'  is null.");
            }

            ServiceProviderGeoLocation data = _mapper.Map<ServiceProviderGeoLocation>(ServiceProviderGeoLocation);
            _context.ServiceProviderGeoLocations.Add(data);


            await _context.SaveChangesAsync();


            return CreatedAtAction("GetServiceProviderGeoLocation", new { id = data.Id }, _mapper.Map<IdTypeDto>(data));
        }

        // DELETE: api/ServiceProviderGeoLocation/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServiceProviderGeoLocation(int id)
        {
            if (_context.ServiceProviderGeoLocations == null)
            {
                return NotFound();
            }
            var ServiceProviderGeoLocation = await _context.ServiceProviderGeoLocations.FindAsync(id);
            if (ServiceProviderGeoLocation == null)
            {
                return NotFound();
            }

            _context.ServiceProviderGeoLocations.Remove(ServiceProviderGeoLocation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ServiceProviderGeoLocationExists(int id)
        {
            return (_context.ServiceProviderGeoLocations?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
