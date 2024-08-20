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
    public class ServiceCategoryController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUriService uriService;
        private readonly IMapper _mapper;

        public ServiceCategoryController(DatabaseContext context, IUriService uriService, IMapper mapper)
        {
            _context = context;
            this.uriService = uriService;
            _mapper = mapper;
        }

        // GET: api/ServiceCategory
        [HttpGet]
        public async Task<ActionResult> GetServiceCategorys([FromQuery] PaginationFilter filter)
        {
            if (_context.ServiceCategories == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.ServiceCategorys'  is null." });
            }
            // return await _context.ServiceCategorys.ToListAsync();
            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.ServiceCategories
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).AsNoTracking()
               .ToListAsync();
            var totalRecords = await _context.OrderAssignments.CountAsync();
            List<ServiceCategoryDto> ilistDest = _mapper.Map<List<ServiceCategory>, List<ServiceCategoryDto>>(pagedData);

            var pagedReponse = PaginationHelper.CreatePagedReponse<ServiceCategoryDto>(ilistDest, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        // GET: api/ServiceCategory/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceCategory>> GetServiceCategory(int id)
        {
            if (_context.ServiceCategories == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.ServiceCategorys'  is null." });
            }
            var ServiceCategory = await _context.ServiceCategories.FindAsync(id);

            if (ServiceCategory == null)
            {
                return NotFound(new { message = "ServiceCategory not found." });
            }



            ServiceCategoryDto data = _mapper.Map<ServiceCategoryDto>(ServiceCategory);
            return Ok(new Wrappers.ApiResponse<ServiceCategoryDto>(data));
        }

        // PUT: api/ServiceCategory/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutServiceCategory(int id, ServiceCategoryDto ServiceCategory)
        {
            if (id != ServiceCategory.Id)
            {
                return BadRequest(new { message = "Invalid ServiceCategory Id." });
            }

            //check if service category exists
            if (ServiceCategoryExists(ServiceCategory))
            {
                return BadRequest(new { message = "Service category already exists." });
            }

            ServiceCategory data = new ServiceCategory
            {
                Id = ServiceCategory.Id,
                Name = ServiceCategory.Name,
                Code = ServiceCategory.Code,
                Description = ServiceCategory.Description,
                Status = ServiceCategory.Status,
                UpdatedAt = DateTime.Now,
            };

            _context.Entry(data).State = EntityState.Modified;
            await _context.SaveChangesAsync();


            return NoContent();
        }

        // POST: api/ServiceCategory
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostServiceCategory(ServiceCategoryDto ServiceCategory)
        {
            if (_context.ServiceCategories == null)
            {
                return Problem("Entity set 'DatabaseContext.ServiceCategorys'  is null.");
            }

            //check if service category exists
            if (ServiceCategoryExists(ServiceCategory))
            {
                return BadRequest(new { message = "Service category already exists." });
            }

            ServiceCategory data = new ServiceCategory
            {
                Name = ServiceCategory.Name,
                Code = ServiceCategory.Code,
                Description = ServiceCategory.Description,
                Status = ServiceCategory.Status,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

            _context.ServiceCategories.Add(data);
            await _context.SaveChangesAsync();
            ServiceCategory.Id = data.Id;


            return CreatedAtAction("GetServiceCategory", new { id = data.Id }, ServiceCategory);
        }

        // DELETE: api/ServiceCategory/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServiceCategory(int id)
        {
            if (_context.ServiceCategories == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.ServiceCategorys'  is null." });
            }
            var ServiceCategory = await _context.ServiceCategories.FindAsync(id);
            if (ServiceCategory == null)
            {
                return NotFound(new { message = "ServiceCategory not found." });
            }

            var service = await _context.Services.FirstOrDefaultAsync(x => x.TypeId == id);
            if (service != null)
            {
                return BadRequest(new { message = "Service Category is in use." });
            }

            _context.ServiceCategories.Remove(ServiceCategory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ServiceCategoryExists(ServiceCategoryDto ServiceCategoryDto)
        {
            return (_context.ServiceCategories?.Any(e => (e.Name == ServiceCategoryDto.Name || e.Code == ServiceCategoryDto.Code) && e.Id != ServiceCategoryDto.Id)).GetValueOrDefault();
        }

    }
}
