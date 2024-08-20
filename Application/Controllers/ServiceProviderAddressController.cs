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
    public class ServiceProviderAddressController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUriService uriService;
        private readonly IMapper _mapper;

        public ServiceProviderAddressController(DatabaseContext context, IUriService uriService, IMapper mapper)
        {
            _context = context;
            this.uriService = uriService;
            _mapper = mapper;
        }

        // GET: api/ServiceProviderAddress
        [HttpGet]
        public async Task<ActionResult> GetServiceProviderAddresss([FromQuery] PaginationFilter filter)
        {
            if (_context.ServiceProviderAddresses == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.ServiceProviderAddresss'  is null." });
            }
            // return await _context.ServiceProviderAddresss.ToListAsync();
            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.ServiceProviderAddresses
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).AsNoTracking()
               .ToListAsync();
            var totalRecords = await _context.OrderAssignments.CountAsync();
            List<ServiceProviderAddressDto> ilistDest = _mapper.Map<List<ServiceProviderAddress>, List<ServiceProviderAddressDto>>(pagedData);

            var pagedReponse = PaginationHelper.CreatePagedReponse<ServiceProviderAddressDto>(ilistDest, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        // GET: api/ServiceProviderAddress/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceProviderAddress>> GetServiceProviderAddress(int id)
        {
            if (_context.ServiceProviderAddresses == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.ServiceProviderAddresss'  is null." });
            }
            var ServiceProviderAddress = await _context.ServiceProviderAddresses.FindAsync(id);

            if (ServiceProviderAddress == null)
            {
                return NotFound(new { message = "ServiceProviderAddress not found." });
            }



            ServiceProviderAddressDto data = _mapper.Map<ServiceProviderAddressDto>(ServiceProviderAddress);
            return Ok(new Wrappers.ApiResponse<ServiceProviderAddressDto>(data));
        }

        [HttpGet("byServiceProvider/{id}")]
        public async Task<ActionResult<ServiceProviderAddress>> GetServiceProviderAddressByProvider(int id)
        {
            if (_context.ServiceProviderAddresses == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.ServiceProviderAddresss'  is null." });
            }
            var ServiceProviderAddress = await _context.ServiceProviderAddresses.FirstOrDefaultAsync(x => x.ServiceProviderId == id);

            if (ServiceProviderAddress == null)
            {
                return NotFound(new { message = "ServiceProviderAddress not found." });
            }



            ServiceProviderAddressDto data = _mapper.Map<ServiceProviderAddressDto>(ServiceProviderAddress);
            return Ok(new Wrappers.ApiResponse<ServiceProviderAddressDto>(data));
        }

        // PUT: api/ServiceProviderAddress/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutServiceProviderAddress(int id, ServiceProviderAddressDto ServiceProviderAddress)
        {
            if (id != ServiceProviderAddress.Id)
            {
                return BadRequest(new { message = "Invalid ServiceProviderAddress Id." });
            }


            ServiceProviderAddress data = _mapper.Map<ServiceProviderAddress>(ServiceProviderAddress);
            _context.Entry(data).State = EntityState.Modified;




            await _context.SaveChangesAsync();


            return NoContent();
        }

        // POST: api/ServiceProviderAddress
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostServiceProviderAddress(ServiceProviderAddressDto ServiceProviderAddress)
        {
            if (_context.ServiceProviderAddresses == null)
            {
                return Problem("Entity set 'DatabaseContext.ServiceProviderAddresss'  is null.");
            }

            ServiceProviderAddress data = _mapper.Map<ServiceProviderAddress>(ServiceProviderAddress);
            _context.ServiceProviderAddresses.Add(data);


            await _context.SaveChangesAsync();


            return CreatedAtAction("GetServiceProviderAddress", new { id = data.Id }, _mapper.Map<IdTypeDto>(data));
        }

        // DELETE: api/ServiceProviderAddress/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServiceProviderAddress(int id)
        {
            if (_context.ServiceProviderAddresses == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.ServiceProviderAddresss'  is null." });
            }
            var ServiceProviderAddress = await _context.ServiceProviderAddresses.FindAsync(id);
            if (ServiceProviderAddress == null)
            {
                return NotFound(new { message = "ServiceProviderAddress not found." });
            }

            _context.ServiceProviderAddresses.Remove(ServiceProviderAddress);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ServiceProviderAddressExists(int id)
        {
            return (_context.ServiceProviderAddresses?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
