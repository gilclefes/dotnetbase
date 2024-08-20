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
    public class ClientGeoLocationController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUriService uriService;
        private readonly IMapper _mapper;

        public ClientGeoLocationController(DatabaseContext context, IUriService uriService, IMapper mapper)
        {
            _context = context;
            this.uriService = uriService;
            _mapper = mapper;
        }

        // GET: api/ClientGeoLocation
        [HttpGet]
        public async Task<ActionResult> GetClientGeoLocations([FromQuery] PaginationFilter filter)
        {
            if (_context.ClientGeoLocations == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.ClientGeoLocations'  is null." });
            }
            // return await _context.ClientGeoLocations.ToListAsync();
            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.ClientGeoLocations
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).AsNoTracking()
               .ToListAsync();
            var totalRecords = await _context.OrderAssignments.CountAsync();
            List<ClientGeoLocationDto> ilistDest = _mapper.Map<List<ClientGeoLocation>, List<ClientGeoLocationDto>>(pagedData);

            var pagedReponse = PaginationHelper.CreatePagedReponse<ClientGeoLocationDto>(ilistDest, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        // GET: api/ClientGeoLocation/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ClientGeoLocation>> GetClientGeoLocation(int id)
        {
            if (_context.ClientGeoLocations == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.ClientGeoLocations'  is null." });
            }
            var ClientGeoLocation = await _context.ClientGeoLocations.FindAsync(id);

            if (ClientGeoLocation == null)
            {
                return NotFound(new { message = "ClientGeoLocation not found" });
            }



            ClientGeoLocationDto data = _mapper.Map<ClientGeoLocationDto>(ClientGeoLocation);
            return Ok(new Wrappers.ApiResponse<ClientGeoLocationDto>(data));
        }

        // PUT: api/ClientGeoLocation/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClientGeoLocation(int id, ClientGeoLocationDto ClientGeoLocation)
        {
            if (id != ClientGeoLocation.Id)
            {
                return BadRequest(new { message = "Invalid ClientGeoLocation Id" });
            }


            ClientGeoLocation data = _mapper.Map<ClientGeoLocation>(ClientGeoLocation);
            _context.Entry(data).State = EntityState.Modified;




            await _context.SaveChangesAsync();


            return NoContent();
        }

        // POST: api/ClientGeoLocation
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostClientGeoLocation(ClientGeoLocationDto ClientGeoLocation)
        {
            if (_context.ClientGeoLocations == null)
            {
                return Problem("Entity set 'DatabaseContext.ClientGeoLocations'  is null.");
            }

            ClientGeoLocation data = _mapper.Map<ClientGeoLocation>(ClientGeoLocation);
            _context.ClientGeoLocations.Add(data);


            await _context.SaveChangesAsync();


            return CreatedAtAction("GetClientGeoLocation", new { id = data.Id }, _mapper.Map<IdTypeDto>(data));
        }

        // DELETE: api/ClientGeoLocation/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClientGeoLocation(int id)
        {
            if (_context.ClientGeoLocations == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.ClientGeoLocations'  is null." });
            }
            var ClientGeoLocation = await _context.ClientGeoLocations.FindAsync(id);
            if (ClientGeoLocation == null)
            {
                return NotFound(new { message = "ClientGeoLocation not found" });
            }

            _context.ClientGeoLocations.Remove(ClientGeoLocation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClientGeoLocationExists(int id)
        {
            return (_context.ClientGeoLocations?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
