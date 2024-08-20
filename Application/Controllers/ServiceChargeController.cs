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
    public class ServiceChargeController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUriService uriService;
        private readonly IMapper _mapper;

        public ServiceChargeController(DatabaseContext context, IUriService uriService, IMapper mapper)
        {
            _context = context;
            this.uriService = uriService;
            _mapper = mapper;
        }

        // GET: api/ServiceCharge
        [HttpGet]
        public async Task<ActionResult> GetServiceCharges([FromQuery] PaginationFilter filter)
        {
            if (_context.ServiceCharges == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.ServiceCharges'  is null." });
            }

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.ServiceCharges
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).Select(x => new ServiceChargeDto
               {
                   ChargeId = x.ChargeId,
                   ChargeName = x.Charge.Name,
                   ServiceId = x.ServiceId,
                   ServiceName = x.Service.Name,
                   Price = x.Price,
                   Id = x.Id


               }).AsNoTracking()
               .ToListAsync();
            var totalRecords = await _context.ServiceCharges.CountAsync();

            var pagedReponse = PaginationHelper.CreatePagedReponse<ServiceChargeDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }


        [HttpGet("service/{serviceid}")]
        public async Task<ActionResult> GetServiceChargeByService(int serviceid, [FromQuery] PaginationFilter filter)
        {
            if (_context.ServiceCharges == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.ServiceCharges'  is null." });
            }

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.ServiceCharges.Where(x => x.ServiceId == serviceid)
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).Select(x => new ServiceChargeDto
               {
                   ChargeId = x.ChargeId,
                   ChargeName = x.Charge.Name,
                   ServiceId = x.ServiceId,
                   ServiceName = x.Service.Name,
                   Price = x.Price,
                   Id = x.Id

               }).AsNoTracking()
               .ToListAsync();
            var totalRecords = await _context.ServiceCharges.CountAsync();

            var pagedReponse = PaginationHelper.CreatePagedReponse<ServiceChargeDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        [HttpGet("serviceIds")]
        public async Task<ActionResult> GetServiceChargeByServiceIds([FromQuery] int[] serviceIds)
        {
            if (_context.ServiceCharges == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.ServiceCharges'  is null." });
            }

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(1, 500);
            var pagedData = await _context.ServiceCharges.Include(x => x.Charge).Where(x => serviceIds.Contains(x.ServiceId))
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).Select(x => new ServiceChargeDto
               {
                   ChargeId = x.ChargeId,
                   ChargeName = x.Charge.Name,
                   ServiceId = x.ServiceId,
                   ServiceName = x.Service.Name,
                   Price = x.Price,
                   Id = x.Id,
                   ChargeAmountType = x.Charge.AmountType

               }).AsNoTracking()
               .ToListAsync();
            var totalRecords = await _context.ServiceCharges.CountAsync();

            var pagedReponse = PaginationHelper.CreatePagedReponse<ServiceChargeDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }



        // GET: api/ServiceCharge/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceCharge>> GetServiceCharge(int id)
        {
            if (_context.ServiceCharges == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.ServiceCharges'  is null." });
            }
            var ServiceCharge = await _context.ServiceCharges.FindAsync(id);

            if (ServiceCharge == null)
            {
                return NotFound(new { message = "ServiceCharge not found." });
            }



            ServiceChargeDto data = _mapper.Map<ServiceChargeDto>(ServiceCharge);
            return Ok(new Wrappers.ApiResponse<ServiceChargeDto>(data));
        }

        // PUT: api/ServiceCharge/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutServiceCharge(int id, ServiceChargeDto ServiceCharge)
        {
            if (id != ServiceCharge.Id)
            {
                return BadRequest(new { message = "Invalid ServiceCharge Id." });
            }

            if (_context.ItemTypes == null)
            {
                return Problem("Entity set 'DatabaseContext.ItemTypes'  is null.");
            }

            //code and name must be unique
            if (ServiceChargeExists(ServiceCharge))
            {
                return BadRequest(new { message = "Charge already exist" });
            }


            Service? service = await _context.Services.FindAsync(ServiceCharge.ServiceId);

            if (service == null)
            {
                return NotFound(new { message = "Service not found." });
            }

            Charge? Charge = await _context.Charges.FindAsync(ServiceCharge.ChargeId);

            if (Charge == null)
            {
                return NotFound(new { message = "Charge not found." });
            }

            if (Charge.AmountType == ChargeAmountType.PERCENTAGE && ServiceCharge.Price > 100 && ServiceCharge.Price < 0)
            {
                return BadRequest(new { message = "Price cannot be greater than 100 for percentage charge" });
            }


            ServiceCharge data = new ServiceCharge
            {
                Id = ServiceCharge.Id,


                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                ChargeId = ServiceCharge.ChargeId,
                Charge = Charge,
                ServiceId = ServiceCharge.ServiceId,
                Service = service,
                Price = ServiceCharge.Price,

            };


            _context.Entry(data).State = EntityState.Modified;
            await _context.SaveChangesAsync();


            return NoContent();
        }

        // POST: api/ServiceCharge
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostServiceCharge(ServiceChargeDto ServiceCharge)
        {
            if (_context.ServiceCharges == null)
            {
                return Problem("Entity set 'DatabaseContext.ServiceCharges'  is null.");
            }


            if (_context.ItemTypes == null)
            {
                return Problem("Entity set 'DatabaseContext.ItemTypes'  is null.");
            }



            //code and name must be unique
            if (ServiceChargeExists(ServiceCharge))
            {
                return BadRequest(new { message = "Charge already exist" });
            }


            Service? service = await _context.Services.FindAsync(ServiceCharge.ServiceId);

            if (service == null)
            {
                return NotFound(new { message = "Service not found." });
            }

            Charge? Charge = await _context.Charges.FindAsync(ServiceCharge.ChargeId);

            if (Charge == null)
            {
                return NotFound(new { message = "Charge not found." });
            }



            ServiceCharge data = new ServiceCharge
            {
                Id = ServiceCharge.Id,


                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                ChargeId = ServiceCharge.ChargeId,
                Charge = Charge,
                ServiceId = ServiceCharge.ServiceId,
                Service = service,
                Price = ServiceCharge.Price,

            };


            _context.Charges.Attach(data.Charge);
            _context.Services.Attach(data.Service);

            _context.ServiceCharges.Add(data);

            await _context.SaveChangesAsync();
            ServiceCharge.Id = data.Id;

            return CreatedAtAction("GetServiceCharge", new { id = data.Id }, ServiceCharge);
        }

        // DELETE: api/ServiceCharge/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServiceCharge(int id)
        {
            if (_context.ServiceCharges == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.ServiceCharges'  is null." });
            }
            var ServiceCharge = await _context.ServiceCharges.FindAsync(id);
            if (ServiceCharge == null)
            {
                return NotFound(new { message = "ServiceCharge not found." });
            }

            _context.ServiceCharges.Remove(ServiceCharge);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ServiceChargeExists(ServiceChargeDto ServiceChargeDto)
        {
            return (_context.ServiceCharges?.Any(e => e.ChargeId == ServiceChargeDto.ChargeId && e.ServiceId == ServiceChargeDto.ServiceId && e.Id != ServiceChargeDto.Id)).GetValueOrDefault();
        }

    }
}
