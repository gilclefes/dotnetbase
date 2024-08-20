using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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
    public class OrderAssignmentController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUriService uriService;
        private readonly IMapper _mapper;

        public OrderAssignmentController(DatabaseContext context, IUriService uriService, IMapper mapper)
        {
            _context = context;
            this.uriService = uriService;
            _mapper = mapper;
        }

        // GET: api/OrderAssignment
        [HttpGet("order/{orderid}")]
        public async Task<ActionResult> GetOrderAssignments(int orderid, [FromQuery] PaginationFilter filter)
        {
            if (_context.OrderAssignments == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.OrderAssignments'  is null." });
            }

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.OrderAssignments.Where(x => x.OrderId == orderid)
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).AsNoTracking().Select(x => new OrderAssignmentDto
               {
                   AssingedUserName = x.AssingedUserName,
                   DateAssigned = x.DateAssigned,
                   OrderAssignmentType = x.OrderAssignmentType,
                   OrderId = x.OrderId,
                   Id = x.Id,
                   Comments = x.Comments,
                   OrderRefNumber = x.Order.RefNumber,

               })
               .ToListAsync();
            var totalRecords = await _context.OrderAssignments.CountAsync();

            var pagedReponse = PaginationHelper.CreatePagedReponse<OrderAssignmentDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        // GET: api/OrderAssignment/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderAssignment>> GetOrderAssignment(int id)
        {
            if (_context.OrderAssignments == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.OrderAssignments'  is null." });
            }
            var OrderAssignment = await _context.OrderAssignments.FindAsync(id);

            if (OrderAssignment == null)
            {
                return NotFound(new { message = "OrderAssignment not found" });
            }



            OrderAssignmentDto data = new OrderAssignmentDto
            {
                AssingedUserName = OrderAssignment.AssingedUserName,
                DateAssigned = OrderAssignment.DateAssigned,
                OrderAssignmentType = OrderAssignment.OrderAssignmentType,
                OrderId = OrderAssignment.OrderId,
                Id = OrderAssignment.Id,
                Comments = OrderAssignment.Comments,
                OrderRefNumber = OrderAssignment.Order.RefNumber,

            };
            return Ok(new Wrappers.ApiResponse<OrderAssignmentDto>(data));
        }

        // PUT: api/OrderAssignment/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderAssignment(int id, OrderAssignmentDto OrderAssignment)
        {
            if (id != OrderAssignment.Id)
            {
                return BadRequest(new { message = "Invalid OrderAssignment Id" });
            }

            if (_context.OrderAssignments == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.OrderAssignments'  is null." });
            }

            Order? order = await _context.Orders.FindAsync(OrderAssignment.OrderId);
            if (order == null)
            {
                return NotFound(new { message = "Order not found" });
            }


            OrderAssignment data = new OrderAssignment
            {

                AssingedUserName = OrderAssignment.AssingedUserName,
                DateAssigned = OrderAssignment.DateAssigned,
                OrderAssignmentType = OrderAssignment.OrderAssignmentType,
                OrderId = OrderAssignment.OrderId,
                Id = OrderAssignment.Id,
                Comments = OrderAssignment.Comments,
                Order = order,
                UpdatedAt = DateTime.Now
            };
            _context.Entry(data).State = EntityState.Modified;




            await _context.SaveChangesAsync();


            return NoContent();
        }

        // POST: api/OrderAssignment
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostOrderAssignment(OrderAssignmentDto OrderAssignment)
        {
            if (_context.OrderAssignments == null)
            {
                return Problem("Entity set 'DatabaseContext.OrderAssignments'  is null.");
            }

            Order? order = await _context.Orders.FindAsync(OrderAssignment.OrderId);
            if (order == null)
            {
                return NotFound(new { message = "Order not found" });
            }



            OrderAssignment data = new OrderAssignment
            {
                AssingedUserName = OrderAssignment.AssingedUserName,
                DateAssigned = OrderAssignment.DateAssigned,
                OrderAssignmentType = OrderAssignment.OrderAssignmentType,
                OrderId = OrderAssignment.OrderId,
                Id = OrderAssignment.Id,
                Comments = OrderAssignment.Comments,
                Order = order,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Orders.Attach(data.Order);
            _context.OrderAssignments.Add(data);


            await _context.SaveChangesAsync();


            return CreatedAtAction("GetOrderAssignment", new { id = data.Id }, _mapper.Map<IdTypeDto>(data));
        }

        // DELETE: api/OrderAssignment/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderAssignment(int id)
        {
            if (_context.OrderAssignments == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.OrderAssignments'  is null." });
            }
            var OrderAssignment = await _context.OrderAssignments.FindAsync(id);
            if (OrderAssignment == null)
            {
                return NotFound(new { message = "OrderAssignment not found" });
            }

            _context.OrderAssignments.Remove(OrderAssignment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderAssignmentExists(int id)
        {
            return (_context.OrderAssignments?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

