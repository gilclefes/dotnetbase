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
    public class ServiceDetergentController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUriService uriService;
        private readonly IMapper _mapper;

        public ServiceDetergentController(DatabaseContext context, IUriService uriService, IMapper mapper)
        {
            _context = context;
            this.uriService = uriService;
            _mapper = mapper;
        }

        // GET: api/ServiceDetergent
        [HttpGet]
        public async Task<ActionResult> GetServiceDetergents([FromQuery] PaginationFilter filter)
        {
            if (_context.ServiceDetergents == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.ServiceDetergents'  is null." });
            }

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.ServiceDetergents.Include(x => x.Service).Include(x => x.Detergent)
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).Select(x => new ServiceDetergentDto
               {
                   DetergentId = x.DetergentId,
                   DetergentName = x.Detergent.Name,
                   Price = x.Price,
                   ServiceId = x.ServiceId,
                   ServiceName = x.Service.Name,
                   Id = x.Id


               }).AsNoTracking()
               .ToListAsync();
            var totalRecords = await _context.ServiceDetergents.CountAsync();

            var pagedReponse = PaginationHelper.CreatePagedReponse<ServiceDetergentDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }


        [HttpGet("service/{serviceid}")]
        public async Task<ActionResult> GetServiceDetergentByService(int serviceid, [FromQuery] PaginationFilter filter)
        {
            if (_context.ServiceDetergents == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.ServiceDetergents'  is null." });
            }

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.ServiceDetergents.Include(x => x.Service).Include(x => x.Detergent).Where(x => x.ServiceId == serviceid)
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).Select(x => new ServiceDetergentDto
               {
                   DetergentId = x.DetergentId,
                   DetergentName = x.Detergent.Name,
                   Price = x.Price,
                   ServiceId = x.ServiceId,
                   ServiceName = x.Service.Name,
                   Id = x.Id


               }).AsNoTracking()
               .ToListAsync();
            var totalRecords = await _context.ServiceDetergents.CountAsync();

            var pagedReponse = PaginationHelper.CreatePagedReponse<ServiceDetergentDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }


        // GET: api/ServiceDetergent/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceDetergent>> GetServiceDetergent(int id)
        {
            if (_context.ServiceDetergents == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.ServiceDetergents'  is null." });
            }
            var ServiceDetergent = await _context.ServiceDetergents.FindAsync(id);

            if (ServiceDetergent == null)
            {
                return NotFound(new { message = "ServiceDetergent not found." });
            }



            ServiceDetergentDto data = _mapper.Map<ServiceDetergentDto>(ServiceDetergent);
            return Ok(new Wrappers.ApiResponse<ServiceDetergentDto>(data));
        }

        // PUT: api/ServiceDetergent/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutServiceDetergent(int id, ServiceDetergentDto ServiceDetergent)
        {
            if (id != ServiceDetergent.Id)
            {
                return BadRequest(new { message = "Invalid ServiceDetergent Id." });
            }

            if (_context.ItemTypes == null)
            {
                return Problem("Entity set 'DatabaseContext.ItemTypes'  is null.");
            }

            //code and name must be unique
            if (ServiceDetergentExists(ServiceDetergent))
            {
                return BadRequest(new { message = "Charge already exist" });
            }


            Service? service = await _context.Services.FindAsync(ServiceDetergent.ServiceId);

            if (service == null)
            {
                return NotFound(new { message = "Service not found." });
            }

            Detergent? detergent = await _context.Detergents.FindAsync(ServiceDetergent.DetergentId);

            if (detergent == null)
            {
                return NotFound(new { message = "Detergent not found." });
            }



            ServiceDetergent data = new ServiceDetergent
            {
                Id = ServiceDetergent.Id,


                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                DetergentId = ServiceDetergent.DetergentId,
                Detergent = detergent,
                Price = ServiceDetergent.Price,
                ServiceId = ServiceDetergent.ServiceId,
                Service = service,

            };


            _context.Entry(data).State = EntityState.Modified;
            await _context.SaveChangesAsync();


            return NoContent();
        }

        // POST: api/ServiceDetergent
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostServiceDetergent(ServiceDetergentDto ServiceDetergent)
        {
            if (_context.ServiceDetergents == null)
            {
                return Problem("Entity set 'DatabaseContext.ServiceDetergents'  is null.");
            }


            if (_context.ItemTypes == null)
            {
                return Problem("Entity set 'DatabaseContext.ItemTypes'  is null.");
            }



            //code and name must be unique
            if (ServiceDetergentExists(ServiceDetergent))
            {
                return BadRequest(new { message = "Charge already exist" });
            }


            Service? service = await _context.Services.FindAsync(ServiceDetergent.ServiceId);

            if (service == null)
            {
                return NotFound(new { message = "Service not found." });
            }

            Detergent? detergent = await _context.Detergents.FindAsync(ServiceDetergent.DetergentId);

            if (detergent == null)
            {
                return NotFound(new { message = "Detergent not found." });
            }



            ServiceDetergent data = new ServiceDetergent
            {
                Id = ServiceDetergent.Id,

                Price = ServiceDetergent.Price,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                ServiceId = ServiceDetergent.ServiceId,
                Service = service,
                Detergent = detergent

            };


            _context.Detergents.Attach(data.Detergent);
            _context.Services.Attach(data.Service);

            _context.ServiceDetergents.Add(data);

            await _context.SaveChangesAsync();
            ServiceDetergent.Id = data.Id;

            return CreatedAtAction("GetServiceDetergent", new { id = data.Id }, ServiceDetergent);
        }

        // DELETE: api/ServiceDetergent/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServiceDetergent(int id)
        {
            if (_context.ServiceDetergents == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.ServiceDetergents'  is null." });
            }
            var ServiceDetergent = await _context.ServiceDetergents.FindAsync(id);
            if (ServiceDetergent == null)
            {
                return NotFound(new { message = "ServiceDetergent not found." });
            }

            _context.ServiceDetergents.Remove(ServiceDetergent);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ServiceDetergentExists(ServiceDetergentDto ServiceDetergentDto)
        {
            return (_context.ServiceDetergents?.Any(e => e.DetergentId == ServiceDetergentDto.DetergentId && e.ServiceId == ServiceDetergentDto.ServiceId && e.Id != ServiceDetergentDto.Id)).GetValueOrDefault();
        }

    }
}
