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
    public class ServiceController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUriService uriService;
        private readonly IMapper _mapper;

        public ServiceController(DatabaseContext context, IUriService uriService, IMapper mapper)
        {
            _context = context;
            this.uriService = uriService;
            _mapper = mapper;
        }

        // GET: api/Service
        [HttpGet]
        public async Task<ActionResult> GetServices([FromQuery] PaginationFilter filter)
        {
            if (_context.Services == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Services'  is null." });
            }

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.Services
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).Select(x => new ServiceDto
               {
                   Id = x.Id,
                   Name = x.Name,
                   Code = x.Code,
                   Status = x.Status,
                   CategoryId = x.CategoryId,
                   MinOrderValue = x.MinOrderValue,
                   TypeId = x.TypeId,
                   Description = x.Description,
                   ServiceCategoryName = x.ServiceCategory.Name,
                   ServiceTypeName = x.ServiceType.Name,
                   HowTo = x.HowTo
               }).AsNoTracking()
               .ToListAsync();
            var totalRecords = await _context.Services.CountAsync();


            var pagedReponse = PaginationHelper.CreatePagedReponse<ServiceDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        // GET: api/Service
        [HttpGet("Active")]
        public async Task<ActionResult> GetServicesActive([FromQuery] PaginationFilter filter)
        {
            if (_context.Services == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Services'  is null." });
            }

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.Services.Where(x => x.Status == true)
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).Select(x => new ServiceDto
               {
                   Id = x.Id,
                   Name = x.Name,
                   Code = x.Code,
                   Status = x.Status,
                   CategoryId = x.CategoryId,
                   MinOrderValue = x.MinOrderValue,
                   TypeId = x.TypeId,
                   Description = x.Description,
                   ServiceCategoryName = x.ServiceCategory.Name,
                   ServiceTypeName = x.ServiceType.Name,
                   HowTo = x.HowTo
               }).AsNoTracking()
               .ToListAsync();
            var totalRecords = await _context.Services.CountAsync();


            var pagedReponse = PaginationHelper.CreatePagedReponse<ServiceDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        // GET: api/Service/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Service>> GetService(int id)
        {
            if (_context.Services == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Services'  is null." });
            }
            Service? Service = await _context.Services.FindAsync(id);

            if (Service == null)
            {
                return NotFound(new { message = "Service not found." });
            }



            ServiceDto data = new ServiceDto
            {
                Id = Service.Id,
                Code = Service.Code,
                Name = Service.Name,
                Status = Service.Status,
                CategoryId = Service.CategoryId,
                ServiceCategoryName = Service.ServiceCategory.Name,
                TypeId = Service.TypeId,
                ServiceTypeName = Service.ServiceType.Name,
                Description = Service.Description,
                MinOrderValue = Service.MinOrderValue,
                HowTo = Service.HowTo
            };
            return Ok(new Wrappers.ApiResponse<ServiceDto>(data));
        }

        // PUT: api/Service/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutService(int id, ServiceDto Service)
        {
            if (id != Service.Id)
            {
                return BadRequest(new { message = "Invalid Service Id." });
            }

            ServiceCategory? serviceCategory = await _context.ServiceCategories.FindAsync(Service.CategoryId);
            if (serviceCategory == null)
            {
                return BadRequest(new { message = "Service category not found" });
            }

            ServiceType? serviceType = await _context.ServiceTypes.FindAsync(Service.TypeId);
            if (serviceType == null)
            {
                return BadRequest(new { message = "Service category not found" });
            }



            Service data = new Service
            {
                Id = Service.Id,
                Code = Service.Code,
                Name = Service.Name,

                UpdatedAt = DateTime.Now,
                Status = Service.Status,
                CategoryId = Service.CategoryId,
                ServiceCategory = serviceCategory,
                TypeId = Service.TypeId,
                ServiceType = serviceType,
                Description = Service.Description,
                MinOrderValue = Service.MinOrderValue,
                HowTo = Service.HowTo
            };

            _context.Entry(data).State = EntityState.Modified;


            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Service
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostService(ServiceDto Service)
        {
            if (_context.Services == null)
            {
                return Problem("Entity set 'DatabaseContext.Services'  is null.");
            }

            //check if Service exist
            if (ServiceExists(Service))
            {
                return BadRequest(new { message = "Service name or code is already taken" });
            }


            ServiceCategory? serviceCategory = await _context.ServiceCategories.FindAsync(Service.CategoryId);
            if (serviceCategory == null)
            {
                return BadRequest(new { message = "Service category not found" });
            }

            ServiceType? serviceType = await _context.ServiceTypes.FindAsync(Service.TypeId);
            if (serviceType == null)
            {
                return BadRequest(new { message = "Service category not found" });
            }



            Service data = new Service
            {
                Id = Service.Id,
                Code = Service.Code,
                Name = Service.Name,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Status = Service.Status,
                CategoryId = Service.CategoryId,
                ServiceCategory = serviceCategory,
                TypeId = Service.TypeId,
                ServiceType = serviceType,
                Description = Service.Description,
                MinOrderValue = Service.MinOrderValue,
                HowTo = Service.HowTo
            };

            _context.ServiceCategories.Attach(data.ServiceCategory);
            _context.ServiceTypes.Attach(data.ServiceType);
            _context.Services.Add(data);
            await _context.SaveChangesAsync();
            Service.Id = data.Id;

            return CreatedAtAction("GetService", new { id = data.Id }, Service);
        }

        // DELETE: api/Service/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            if (_context.Services == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Services'  is null." });
            }
            var Service = await _context.Services.FindAsync(id);
            if (Service == null)
            {
                return NotFound(new { message = "Service not found." });
            }

            _context.Services.Remove(Service);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ServiceExists(ServiceDto ServiceDto)
        {
            return (_context.Services?.Any(e => (e.Code == ServiceDto.Code || e.Name == ServiceDto.Name) && e.Id != ServiceDto.Id)).GetValueOrDefault();
        }

    }
}
