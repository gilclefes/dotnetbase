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
    public class ServicePeriodController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUriService uriService;
        private readonly IMapper _mapper;

        public ServicePeriodController(DatabaseContext context, IUriService uriService, IMapper mapper)
        {
            _context = context;
            this.uriService = uriService;
            _mapper = mapper;
        }

        // GET: api/ServicePeriod
        [HttpGet]
        public async Task<ActionResult> GetServicePeriods([FromQuery] PaginationFilter filter)
        {
            if (_context.ServicePeriods == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.ServicePeriods'  is null." });
            }

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.ServicePeriods
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).Select(x => new ServicePeriodDto
               {
                   PeriodId = x.PeriodId,
                   PeriodName = x.Period.Name,
                   ServiceId = x.ServiceId,
                   ServiceName = x.Service.Name,


               }).AsNoTracking()
               .ToListAsync();
            var totalRecords = await _context.ServicePeriods.CountAsync();

            var pagedReponse = PaginationHelper.CreatePagedReponse<ServicePeriodDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }


        [HttpGet("service/{serviceid}")]
        public async Task<ActionResult> GetServicePeriodByService(int serviceid, [FromQuery] PaginationFilter filter)
        {
            if (_context.ServicePeriods == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.ServicePeriods'  is null." });
            }

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.ServicePeriods.Where(x => x.ServiceId == serviceid)
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).Select(x => new ServicePeriodDto
               {
                   PeriodId = x.PeriodId,
                   PeriodName = x.Period.Name,
                   ServiceId = x.ServiceId,
                   ServiceName = x.Service.Name,


               }).AsNoTracking()
               .ToListAsync();
            var totalRecords = await _context.ServicePeriods.CountAsync();

            var pagedReponse = PaginationHelper.CreatePagedReponse<ServicePeriodDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }


        // GET: api/ServicePeriod/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ServicePeriod>> GetServicePeriod(int id)
        {
            if (_context.ServicePeriods == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.ServicePeriods'  is null." });
            }
            var ServicePeriod = await _context.ServicePeriods.FindAsync(id);

            if (ServicePeriod == null)
            {
                return NotFound(new { message = "ServicePeriod not found." });
            }



            ServicePeriodDto data = _mapper.Map<ServicePeriodDto>(ServicePeriod);
            return Ok(new Wrappers.ApiResponse<ServicePeriodDto>(data));
        }

        // PUT: api/ServicePeriod/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutServicePeriod(int id, ServicePeriodDto ServicePeriod)
        {
            if (id != ServicePeriod.Id)
            {
                return BadRequest(new { message = "Invalid ServicePeriod Id." });
            }

            if (_context.ItemTypes == null)
            {
                return Problem("Entity set 'DatabaseContext.ItemTypes'  is null.");
            }

            //code and name must be unique
            if (ServicePeriodExists(ServicePeriod))
            {
                return BadRequest(new { message = "Charge already exist" });
            }


            Service? service = await _context.Services.FindAsync(ServicePeriod.ServiceId);

            if (service == null)
            {
                return NotFound(new { message = "Service not found." });
            }

            Period? period = await _context.Periods.FindAsync(ServicePeriod.PeriodId);

            if (period == null)
            {
                return NotFound(new { message = "Period not found." });
            }



            ServicePeriod data = new ServicePeriod
            {
                Id = ServicePeriod.Id,


                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                PeriodId = ServicePeriod.PeriodId,
                Period = period,
                ServiceId = ServicePeriod.ServiceId,
                Service = service,

            };


            _context.Entry(data).State = EntityState.Modified;
            await _context.SaveChangesAsync();


            return NoContent();
        }

        // POST: api/ServicePeriod
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostServicePeriod(ServicePeriodDto ServicePeriod)
        {
            if (_context.ServicePeriods == null)
            {
                return Problem("Entity set 'DatabaseContext.ServicePeriods'  is null.");
            }


            if (_context.ItemTypes == null)
            {
                return Problem("Entity set 'DatabaseContext.ItemTypes'  is null.");
            }



            //code and name must be unique
            if (ServicePeriodExists(ServicePeriod))
            {
                return BadRequest(new { message = "Charge already exist" });
            }


            Service? service = await _context.Services.FindAsync(ServicePeriod.ServiceId);

            if (service == null)
            {
                return NotFound(new { message = "Service not found." });
            }

            Period? period = await _context.Periods.FindAsync(ServicePeriod.PeriodId);

            if (period == null)
            {
                return NotFound(new { message = "Period not found." });
            }



            ServicePeriod data = new ServicePeriod
            {
                Id = ServicePeriod.Id,


                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                PeriodId = ServicePeriod.PeriodId,
                Period = period,
                ServiceId = ServicePeriod.ServiceId,
                Service = service,

            };


            _context.Periods.Attach(data.Period);
            _context.Services.Attach(data.Service);

            _context.ServicePeriods.Add(data);

            await _context.SaveChangesAsync();
            ServicePeriod.Id = data.Id;

            return CreatedAtAction("GetServicePeriod", new { id = data.Id }, ServicePeriod);
        }

        // DELETE: api/ServicePeriod/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServicePeriod(int id)
        {
            if (_context.ServicePeriods == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.ServicePeriods'  is null." });
            }
            var ServicePeriod = await _context.ServicePeriods.FindAsync(id);
            if (ServicePeriod == null)
            {
                return NotFound(new { message = "ServicePeriod not found." });
            }

            _context.ServicePeriods.Remove(ServicePeriod);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ServicePeriodExists(ServicePeriodDto ServicePeriodDto)
        {
            return (_context.ServicePeriods?.Any(e => e.PeriodId == ServicePeriodDto.PeriodId && e.ServiceId == ServicePeriodDto.ServiceId && e.Id != ServicePeriodDto.Id)).GetValueOrDefault();
        }

    }
}
