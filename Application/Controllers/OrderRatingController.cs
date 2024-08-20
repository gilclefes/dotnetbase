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
    public class OrderRatingController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUriService uriService;
        private readonly IMapper _mapper;

        public OrderRatingController(DatabaseContext context, IUriService uriService, IMapper mapper)
        {
            _context = context;
            this.uriService = uriService;
            _mapper = mapper;
        }

        // GET: api/OrderRating
        [HttpGet]
        public async Task<ActionResult> GetOrderRatings([FromQuery] PaginationFilter filter)
        {
            if (_context.OrderRatings == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.OrderRatings'  is null." });
            }

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.OrderRatings
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).AsNoTracking().Select(x => new OrderRatingDto
               {
                   Message = x.Message,
                   Id = x.Id,
                   OrderId = x.OrderId,
                   RatedEmail = x.RatedEmail,
                   RaterEmail = x.RaterEmail,
                   Rating = x.Rating,

               })
               .ToListAsync();
            var totalRecords = await _context.OrderAssignments.CountAsync();

            var pagedReponse = PaginationHelper.CreatePagedReponse<OrderRatingDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        [HttpGet("order/{orderid}")]
        public async Task<ActionResult> GetOrderRatingsByOrder(int orderid, [FromQuery] PaginationFilter filter)
        {
            if (_context.OrderRatings == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.OrderRatings'  is null." });
            }

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.OrderRatings.Where(x => x.OrderId == orderid)
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).AsNoTracking().Select(x => new OrderRatingDto
               {
                   Message = x.Message,
                   Id = x.Id,
                   OrderId = x.OrderId,
                   RatedEmail = x.RatedEmail,
                   RaterEmail = x.RaterEmail,
                   Rating = x.Rating,
                   OrderRefNumber = x.Order.RefNumber,

               })
               .ToListAsync();
            var totalRecords = await _context.OrderAssignments.CountAsync();

            var pagedReponse = PaginationHelper.CreatePagedReponse<OrderRatingDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        // GET: api/OrderRating/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderRating>> GetOrderRating(int id)
        {
            if (_context.OrderRatings == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.OrderRatings'  is null." });
            }
            OrderRating? x = await _context.OrderRatings.FindAsync(id);

            if (x == null)
            {
                return NotFound(new { message = "OrderRating not found" });
            }



            OrderRatingDto data = new OrderRatingDto
            {
                Message = x.Message,
                Id = x.Id,
                OrderId = x.OrderId,
                RatedEmail = x.RatedEmail,
                RaterEmail = x.RaterEmail,
                Rating = x.Rating,
                OrderRefNumber = x.Order.RefNumber,

            };
            return Ok(new Wrappers.ApiResponse<OrderRatingDto>(data));
        }

        // PUT: api/OrderRating/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderRating(int id, OrderRatingDto OrderRating)
        {
            if (id != OrderRating.Id)
            {
                return BadRequest(new { message = "Invalid OrderRating Id" });
            }


            Order? order = await _context.Orders.FindAsync(OrderRating.OrderId);
            if (order == null)
            {
                return NotFound(new { message = "Order not found" });
            }

            OrderRating data = new OrderRating
            {
                Message = OrderRating.Message,
                Id = OrderRating.Id,
                OrderId = OrderRating.OrderId,

                Order = order,
                UpdatedAt = DateTime.Now,
                CreatedAt = DateTime.Now,
                RatedEmail = OrderRating.RatedEmail,
                RaterEmail = OrderRating.RaterEmail,
                Rating = OrderRating.Rating,

            };

            _context.Entry(data).State = EntityState.Modified;




            await _context.SaveChangesAsync();


            return NoContent();
        }

        // POST: api/OrderRating
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostOrderRating(OrderRatingDto OrderRating)
        {
            if (_context.OrderRatings == null)
            {
                return Problem("Entity set 'DatabaseContext.OrderRatings'  is null.");
            }

            Order? order = await _context.Orders.Include(x => x.OrderAssigments).FirstOrDefaultAsync(x => x.Id == OrderRating.OrderId);
            if (order == null)
            {
                return NotFound(new { message = "Order not found" });
            }



            OrderRating data = new OrderRating
            {
                Message = OrderRating.Message,
                Id = OrderRating.Id,
                OrderId = OrderRating.OrderId,
                RatedEmail = OrderRating.RatedEmail,
                RaterEmail = OrderRating.RaterEmail,
                Rating = OrderRating.Rating,
                Order = order,
                UpdatedAt = DateTime.Now,
                CreatedAt = DateTime.Now,
            };

            _context.Orders.Attach(data.Order);
            _context.OrderRatings.Add(data);


            await _context.SaveChangesAsync();


            return CreatedAtAction("GetOrderRating", new { id = data.Id }, OrderRating);
        }

        // DELETE: api/OrderRating/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderRating(int id)
        {
            if (_context.OrderRatings == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.OrderRatings'  is null." });
            }
            var OrderRating = await _context.OrderRatings.FindAsync(id);
            if (OrderRating == null)
            {
                return NotFound(new { message = "OrderRating not found" });
            }

            _context.OrderRatings.Remove(OrderRating);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderRatingExists(int id)
        {
            return (_context.OrderRatings?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
