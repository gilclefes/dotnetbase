using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class OrderItemController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUriService uriService;
        private readonly IMapper _mapper;

        public OrderItemController(DatabaseContext context, IUriService uriService, IMapper mapper)
        {
            _context = context;
            this.uriService = uriService;
            _mapper = mapper;
        }

        // GET: api/OrderItem
        [HttpGet]
        public async Task<ActionResult> GetOrderItems([FromQuery] PaginationFilter filter)
        {
            if (_context.OrderItems == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.OrderItems'  is null" });
            }

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.OrderItems
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).AsNoTracking().Select(x => new OrderItemDto
               {
                   Comments = x.Comments,
                   Id = x.Id,
                   OrderId = x.OrderId,
                   Quantity = x.Quantity,
                   UnitPrice = x.UnitPrice,
                   Discount = x.Discount,
                   ItemId = x.ItemId,
                   LaundryItemName = x.LaundryItem.Name,
                   NetPrice = x.NetPrice,
                   OrderName = x.Order.RefNumber,
                   ServiceId = x.ServiceId,
                   ServiceName = x.Service.Name,

               })
               .ToListAsync();
            var totalRecords = await _context.OrderAssignments.CountAsync();

            var pagedReponse = PaginationHelper.CreatePagedReponse<OrderItemDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        [HttpGet("order/{orderid}")]
        public async Task<ActionResult> GetOrderItems(int orderid, [FromQuery] PaginationFilter filter)
        {
            if (_context.OrderItems == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.OrderItems'  is null." });
            }

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.OrderItems.Where(x => x.OrderId == orderid)
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).AsNoTracking().Select(x => new OrderItemDto
               {
                   Comments = x.Comments,
                   Id = x.Id,
                   OrderId = x.OrderId,
                   Quantity = x.Quantity,
                   UnitPrice = x.UnitPrice,
                   Discount = x.Discount,
                   ItemId = x.ItemId,
                   LaundryItemName = x.LaundryItem.Name,
                   NetPrice = x.NetPrice,
                   OrderName = x.Order.RefNumber,
                   ServiceId = x.ServiceId,
                   ServiceName = x.Service.Name,

               })
               .ToListAsync();
            var totalRecords = await _context.OrderAssignments.CountAsync();

            var pagedReponse = PaginationHelper.CreatePagedReponse<OrderItemDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }


        // GET: api/OrderItem/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderItem>> GetOrderItem(int id)
        {
            if (_context.OrderItems == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.OrderItems'  is null." });
            }
            OrderItem? x = await _context.OrderItems.FindAsync(id);

            if (x == null)
            {
                return NotFound(new { message = "OrderItem not found" });
            }



            OrderItemDto data = new OrderItemDto
            {
                Comments = x.Comments,
                Id = x.Id,
                OrderId = x.OrderId,
                Quantity = x.Quantity,
                UnitPrice = x.UnitPrice,
                Discount = x.Discount,
                ItemId = x.ItemId,
                LaundryItemName = x.LaundryItem.Name,
                NetPrice = x.NetPrice,
                OrderName = x.Order.RefNumber,
                ServiceId = x.ServiceId,
                ServiceName = x.Service.Name,

            };
            return Ok(new Wrappers.ApiResponse<OrderItemDto>(data));
        }

        // PUT: api/OrderItem/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderItem(int id, OrderItemDto OrderItem)
        {
            if (id != OrderItem.Id)
            {
                return BadRequest(new { message = "Invalid OrderItem Id" });
            }


            Order? order = await _context.Orders.FindAsync(OrderItem.OrderId);
            if (order == null)
            {
                return NotFound(new { message = "Order not found" });
            }

            Service? service = await _context.Services.FindAsync(OrderItem.ServiceId);
            if (service == null)
            {
                return NotFound(new { message = "Service not found" });
            }

            LaundryItem? laundryItem = await _context.LaundryItems.FindAsync(OrderItem.ItemId);
            if (laundryItem == null)
            {
                return NotFound(new { message = "LaundryItem not found" });
            }


            OrderItem data = new OrderItem
            {
                Comments = OrderItem.Comments,
                Id = OrderItem.Id,
                OrderId = OrderItem.OrderId,
                Quantity = OrderItem.Quantity,
                UnitPrice = OrderItem.UnitPrice,
                Discount = OrderItem.Discount,
                ItemId = OrderItem.ItemId,
                LaundryItem = laundryItem,
                NetPrice = OrderItem.NetPrice,
                Order = order,
                ServiceId = OrderItem.ServiceId,
                Service = service,
                UpdatedAt = DateTime.Now,
                CreatedAt = DateTime.Now,
            };

            _context.Entry(data).State = EntityState.Modified;




            await _context.SaveChangesAsync();


            return NoContent();
        }

        // POST: api/OrderItem
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostOrderItem(OrderItemDto OrderItem)
        {
            if (_context.OrderItems == null)
            {
                return Problem("Entity set 'DatabaseContext.OrderItems'  is null.");
            }

            Order? order = await _context.Orders.FindAsync(OrderItem.OrderId);
            if (order == null)
            {
                return NotFound(new { message = "Order not found" });
            }

            Service? service = await _context.Services.FindAsync(OrderItem.ServiceId);
            if (service == null)
            {
                return NotFound(new { message = "Service not found" });
            }

            LaundryItem? laundryItem = await _context.LaundryItems.FindAsync(OrderItem.ItemId);
            if (laundryItem == null)
            {
                return NotFound(new { message = "LaundryItem not found" });
            }


            OrderItem data = new OrderItem
            {
                Comments = OrderItem.Comments,
                Id = OrderItem.Id,
                OrderId = OrderItem.OrderId,
                Quantity = OrderItem.Quantity,
                UnitPrice = OrderItem.UnitPrice,
                Discount = OrderItem.Discount,
                ItemId = OrderItem.ItemId,
                LaundryItem = laundryItem,
                NetPrice = OrderItem.NetPrice,
                Order = order,
                ServiceId = OrderItem.ServiceId,
                Service = service,
                UpdatedAt = DateTime.Now,
                CreatedAt = DateTime.Now,
            };
            _context.Orders.Attach(data.Order);
            _context.Services.Attach(data.Service);
            _context.LaundryItems.Attach(data.LaundryItem);
            _context.OrderItems.Add(data);


            await _context.SaveChangesAsync();


            return CreatedAtAction("GetOrderItem", new { id = data.Id }, _mapper.Map<IdTypeDto>(data));
        }

        // DELETE: api/OrderItem/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderItem(int id)
        {
            if (_context.OrderItems == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.OrderItems'  is null." });
            }
            var OrderItem = await _context.OrderItems.FindAsync(id);
            if (OrderItem == null)
            {
                return NotFound(new { message = "OrderItem not found" });
            }

            _context.OrderItems.Remove(OrderItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderItemExists(int id)
        {
            return (_context.OrderItems?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
