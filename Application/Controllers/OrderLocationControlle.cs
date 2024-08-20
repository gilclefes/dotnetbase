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
    public class OrderLocationController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUriService uriService;
        private readonly IMapper _mapper;

        public OrderLocationController(DatabaseContext context, IUriService uriService, IMapper mapper)
        {
            _context = context;
            this.uriService = uriService;
            _mapper = mapper;
        }

        // GET: api/OrderLocation
        [HttpGet("order/{orderid}")]
        public async Task<ActionResult> GetOrderLocations(int orderid, [FromQuery] PaginationFilter filter)
        {
            if (_context.OrderLocations == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.OrderLocations'  is null." });
            }

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.OrderLocations.Where(x => x.OrderId == orderid)
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).AsNoTracking().Select(x => new OrderLocationDto
               {


                   Id = x.Id,
                   OrderId = x.OrderId,
                   Accuracy = x.Accuracy,
                   AddressLine = x.AddressLine,

                   City = x.City,
                   CountryRegion = x.CountryRegion,
                   Latitude = x.Latitude,
                   Longitude = x.Longitude,
                   OrderName = x.Order.RefNumber,
                   PostalCode = x.PostalCode,
                   StateProvince = x.StateProvince,


               })
               .ToListAsync();
            var totalRecords = await _context.OrderAssignments.CountAsync();

            var pagedReponse = PaginationHelper.CreatePagedReponse<OrderLocationDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        // GET: api/OrderLocation/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderLocation>> GetOrderLocation(int id)
        {
            if (_context.OrderLocations == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.OrderLocations'  is null." });
            }
            OrderLocation? x = await _context.OrderLocations.FindAsync(id);

            if (x == null)
            {
                return NotFound(new { message = "OrderLocation not found" });
            }



            OrderLocationDto data = new OrderLocationDto
            {
                Id = x.Id,
                OrderId = x.OrderId,
                Accuracy = x.Accuracy,
                AddressLine = x.AddressLine,

                City = x.City,
                CountryRegion = x.CountryRegion,
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                OrderName = x.Order.RefNumber,
                PostalCode = x.PostalCode,
                StateProvince = x.StateProvince,

            };
            return Ok(new Wrappers.ApiResponse<OrderLocationDto>(data));
        }

        // PUT: api/OrderLocation/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderLocation(int id, OrderLocationDto OrderLocation)
        {
            if (id != OrderLocation.Id)
            {
                return BadRequest(new { message = "Invalid OrderLocation Id" });
            }


            Order? order = await _context.Orders.FindAsync(OrderLocation.OrderId);
            if (order == null)
            {
                return NotFound(new { message = "Order not found" });
            }

            OrderLocation data = new OrderLocation
            {
                Id = OrderLocation.Id,
                OrderId = OrderLocation.OrderId,
                Accuracy = OrderLocation.Accuracy,
                AddressLine = OrderLocation.AddressLine,

                City = OrderLocation.City,
                CountryRegion = OrderLocation.CountryRegion,
                Latitude = OrderLocation.Latitude,
                Longitude = OrderLocation.Longitude,
                Order = order,
                PostalCode = OrderLocation.PostalCode,
                StateProvince = OrderLocation.StateProvince,
                UpdatedAt = DateTime.Now,
                CreatedAt = DateTime.Now,
            };

            _context.Entry(data).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/OrderLocation
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostOrderLocation(OrderLocationDto OrderLocation)
        {
            if (_context.OrderLocations == null)
            {
                return Problem("Entity set 'DatabaseContext.OrderLocations'  is null.");
            }

            Order? order = await _context.Orders.FindAsync(OrderLocation.OrderId);
            if (order == null)
            {
                return NotFound(new { message = "Order not found" });
            }

            OrderLocation data = new OrderLocation
            {
                Id = OrderLocation.Id,
                OrderId = OrderLocation.OrderId,
                Accuracy = OrderLocation.Accuracy,
                AddressLine = OrderLocation.AddressLine,

                City = OrderLocation.City,
                CountryRegion = OrderLocation.CountryRegion,
                Latitude = OrderLocation.Latitude,
                Longitude = OrderLocation.Longitude,
                Order = order,
                PostalCode = OrderLocation.PostalCode,
                StateProvince = OrderLocation.StateProvince,
                UpdatedAt = DateTime.Now,
                CreatedAt = DateTime.Now,
            };
            _context.OrderLocations.Attach(data);
            _context.OrderLocations.Add(data);


            await _context.SaveChangesAsync();


            return CreatedAtAction("GetOrderLocation", new { id = data.Id }, _mapper.Map<OrderLocationDto>(data));
        }

        // DELETE: api/OrderLocation/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderLocation(int id)
        {
            if (_context.OrderLocations == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.OrderLocations'  is null." });
            }
            var OrderLocation = await _context.OrderLocations.FindAsync(id);
            if (OrderLocation == null)
            {
                return NotFound(new { message = "OrderLocation not found" });
            }

            _context.OrderLocations.Remove(OrderLocation);
            await _context.SaveChangesAsync();

            return NoContent();
        }


    }
}
