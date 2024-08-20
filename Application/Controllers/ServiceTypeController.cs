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
    public class ServiceTypeController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUriService uriService;
        private readonly IMapper _mapper;

        public ServiceTypeController(DatabaseContext context, IUriService uriService, IMapper mapper)
        {
            _context = context;
            this.uriService = uriService;
            _mapper = mapper;
        }

        // GET: api/ServiceType
        [HttpGet]
        public async Task<ActionResult> GetServiceTypes([FromQuery] PaginationFilter filter)
        {
            if (_context.ServiceTypes == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.ServiceTypes'  is null." });
            }
            // return await _context.ServiceTypes.ToListAsync();
            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.ServiceTypes
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).AsNoTracking()
               .ToListAsync();
            var totalRecords = await _context.ServiceTypes.CountAsync();
            List<ServiceTypeDto> ilistDest = _mapper.Map<List<ServiceType>, List<ServiceTypeDto>>(pagedData);

            var pagedReponse = PaginationHelper.CreatePagedReponse<ServiceTypeDto>(ilistDest, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        // GET: api/ServiceType/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceType>> GetServiceType(int id)
        {
            if (_context.ServiceTypes == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.ServiceTypes'  is null." });
            }
            var ServiceType = await _context.ServiceTypes.FindAsync(id);

            if (ServiceType == null)
            {
                return NotFound(new { message = "ServiceType not found." });
            }



            ServiceTypeDto data = _mapper.Map<ServiceTypeDto>(ServiceType);
            return Ok(new Wrappers.ApiResponse<ServiceTypeDto>(data));
        }

        // PUT: api/ServiceType/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutServiceType(int id, ServiceTypeDto ServiceType)
        {
            if (id != ServiceType.Id)
            {
                return BadRequest(new { message = "Invalid ServiceType Id." });
            }

            //check if service type exists
            if (ServiceTypeExists(ServiceType))
            {
                return BadRequest(new { message = "Service type already exists." });
            }

            ServiceType data = new ServiceType
            {
                Name = ServiceType.Name,
                Code = ServiceType.Code,
                Description = ServiceType.Description,

                UpdatedAt = DateTime.Now,
                Status = ServiceType.Status,
                Itemized = ServiceType.Itemized,
                Id = ServiceType.Id
            };
            _context.Entry(data).State = EntityState.Modified;




            await _context.SaveChangesAsync();


            return NoContent();
        }

        // POST: api/ServiceType
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostServiceType(ServiceTypeDto ServiceType)
        {
            if (_context.ServiceTypes == null)
            {
                return Problem("Entity set 'DatabaseContext.ServiceTypes'  is null.");
            }

            //check if service type exists
            if (ServiceTypeExists(ServiceType))
            {
                return BadRequest(new { message = "Service type already exists." });
            }

            ServiceType data = new ServiceType
            {
                Name = ServiceType.Name,
                Code = ServiceType.Code,
                Description = ServiceType.Description,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Status = ServiceType.Status,
                Itemized = ServiceType.Itemized,
            };
            _context.ServiceTypes.Add(data);
            ServiceType.Id = data.Id;

            await _context.SaveChangesAsync();


            return CreatedAtAction("GetServiceType", new { id = data.Id }, ServiceType);
        }

        // DELETE: api/ServiceType/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServiceType(int id)
        {
            if (_context.ServiceTypes == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.ServiceTypes'  is null." });
            }
            var ServiceType = await _context.ServiceTypes.FindAsync(id);
            if (ServiceType == null)
            {
                return NotFound(new { message = "ServiceType not found." });
            }

            var service = await _context.Services.FirstOrDefaultAsync(x => x.TypeId == id);
            if (service != null)
            {
                return BadRequest(new { message = "Service type is in use." });
            }

            _context.ServiceTypes.Remove(ServiceType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ServiceTypeExists(ServiceTypeDto serviceTypeDto)
        {
            return (_context.ServiceTypes?.Any(e => (e.Name == serviceTypeDto.Name || e.Code == serviceTypeDto.Code) && e.Id != serviceTypeDto.Id)).GetValueOrDefault();
        }

    }
}
