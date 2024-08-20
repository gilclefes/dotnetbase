using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Coravel.Events.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dotnetbase.Application.Database;
using dotnetbase.Application.Events;
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

    // to make order notifications as a service that generates a message on a status change
    public class OrderMessageController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUriService uriService;
        private readonly IMapper _mapper;

        private readonly YaboUtilsService _yaboUtilService;


        private readonly IDispatcher _dispatcher;

        public OrderMessageController(DatabaseContext context, IUriService uriService, IMapper mapper, YaboUtilsService yaboValidationService,
        IDispatcher dispatcher)
        {
            _context = context;
            this.uriService = uriService;
            _mapper = mapper;
            _dispatcher = dispatcher;
            _yaboUtilService = yaboValidationService;
        }

        // GET: api/OrderMessage
        [HttpGet]
        public async Task<ActionResult> GetOrderMessages([FromQuery] PaginationFilter filter)
        {
            if (_context.OrderMessages == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.OrderMessages'  is null." });
            }

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.OrderMessages
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).AsNoTracking().Select(x => new OrderMessageDto
               {
                   Message = x.Message,
                   Id = x.Id,
                   OrderId = x.OrderId,
                   MessageFrom = x.MessageFrom,
                   MessageStatus = x.MessageStatus,
                   MessageTo = x.MessageTo,
                   OrderName = x.Order.RefNumber,

               })
               .ToListAsync();
            var totalRecords = await _context.OrderAssignments.CountAsync();

            var pagedReponse = PaginationHelper.CreatePagedReponse<OrderMessageDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        [HttpGet("order/{orderid}")]
        public async Task<ActionResult> GetOrderMessagesByOrder(int orderid, [FromQuery] PaginationFilter filter)
        {
            if (_context.OrderMessages == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.OrderMessages'  is null." });
            }

            Order? order = await _context.Orders.FindAsync(orderid);
            if (order == null)
            {
                return NotFound(new { message = "Order not found" });
            }

            var owner = order.ClientUserName;

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.OrderMessages.Where(x => x.OrderId == orderid)
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).AsNoTracking().Select(x => new OrderMessageDto
               {
                   Message = x.Message,
                   Id = x.Id,
                   OrderId = x.OrderId,
                   MessageFrom = x.MessageFrom,
                   MessageStatus = x.MessageStatus,
                   MessageTo = x.MessageTo,
                   OrderName = x.Order.RefNumber,
                   MessageType = x.MessageFrom == owner ? "sender" : "receiver",

               })
               .ToListAsync();
            var totalRecords = await _context.OrderAssignments.CountAsync();

            var pagedReponse = PaginationHelper.CreatePagedReponse<OrderMessageDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        [HttpGet("messageto/{messageto}")]
        public async Task<ActionResult> GetOrderMessagesByMessageTo(string messageto, [FromQuery] PaginationFilter filter)
        {
            if (_context.OrderMessages == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.OrderMessages'  is null." });
            }

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.OrderMessages.Where(x => x.MessageTo == messageto)
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).AsNoTracking().Select(x => new OrderMessageDto
               {
                   Message = x.Message,
                   Id = x.Id,
                   OrderId = x.OrderId,
                   MessageFrom = x.MessageFrom,
                   MessageStatus = x.MessageStatus,
                   MessageTo = x.MessageTo,
                   OrderName = x.Order.RefNumber,
                   MessageType = x.MessageFrom == messageto ? "receiver" : "sender",

               })
               .ToListAsync();
            var totalRecords = await _context.OrderAssignments.CountAsync();

            var pagedReponse = PaginationHelper.CreatePagedReponse<OrderMessageDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        // GET: api/OrderMessage/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderMessage>> GetOrderMessage(int id)
        {
            if (_context.OrderMessages == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.OrderMessages'  is null." });
            }
            OrderMessage? x = await _context.OrderMessages.FindAsync(id);

            if (x == null)
            {
                return NotFound(new { message = "OrderMessage not found" });
            }



            OrderMessageDto data = new OrderMessageDto
            {
                Message = x.Message,
                Id = x.Id,
                OrderId = x.OrderId,
                MessageFrom = x.MessageFrom,
                MessageStatus = x.MessageStatus,
                MessageTo = x.MessageTo,
                OrderName = x.Order.RefNumber,

            };
            return Ok(new Wrappers.ApiResponse<OrderMessageDto>(data));
        }

        // PUT: api/OrderMessage/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderMessage(int id, OrderMessageDto OrderMessage)
        {
            if (id != OrderMessage.Id)
            {
                return BadRequest(new { message = "Invalid OrderMessage Id" });
            }


            Order? order = await _context.Orders.FindAsync(OrderMessage.OrderId);
            if (order == null)
            {
                return NotFound(new { message = "Order not found" });
            }

            OrderMessage data = new OrderMessage
            {
                Message = OrderMessage.Message,
                Id = OrderMessage.Id,
                OrderId = OrderMessage.OrderId,
                MessageFrom = OrderMessage.MessageFrom,
                MessageStatus = OrderMessage.MessageStatus,
                MessageTo = OrderMessage.MessageTo,
                Order = order,
                UpdatedAt = DateTime.Now,
                CreatedAt = DateTime.Now,
            };

            _context.Entry(data).State = EntityState.Modified;




            await _context.SaveChangesAsync();


            return NoContent();
        }

        // POST: api/OrderMessage
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostOrderMessage(OrderMessageDto OrderMessage)
        {
            if (_context.OrderMessages == null)
            {
                return Problem("Entity set 'DatabaseContext.OrderMessages'  is null.");
            }

            Order? order = await _context.Orders.FindAsync(OrderMessage.OrderId);
            if (order == null)
            {
                return NotFound(new { message = "Order not found" });
            }


            OrderMessage data = new OrderMessage
            {
                Message = OrderMessage.Message,
                Id = OrderMessage.Id,
                OrderId = OrderMessage.OrderId,
                MessageFrom = OrderMessage.MessageFrom,
                MessageStatus = OrderMessage.MessageStatus,
                MessageTo = OrderMessage.MessageTo,
                Order = order,
                UpdatedAt = DateTime.Now,
                CreatedAt = DateTime.Now,
            };

            _context.Orders.Attach(data.Order);
            _context.OrderMessages.Add(data);


            await _context.SaveChangesAsync();


            await this._yaboUtilService.SendNotification($"Message sent for the Order Number {order.RefNumber}: {OrderMessage.Message}", order.ClientUserName);
            var orderMessageEvent = new OrderMessageEvent(order.RefNumber, order.ClientUserName, OrderMessage.Message);
            await _dispatcher.Broadcast(orderMessageEvent);

            return CreatedAtAction("GetOrderMessage", new { id = data.Id }, OrderMessage);
        }

        // DELETE: api/OrderMessage/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderMessage(int id)
        {
            if (_context.OrderMessages == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.OrderMessages'  is null." });
            }
            var OrderMessage = await _context.OrderMessages.FindAsync(id);
            if (OrderMessage == null)
            {
                return NotFound(new { message = "OrderMessage not found" });
            }

            _context.OrderMessages.Remove(OrderMessage);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderMessageExists(int id)
        {
            return (_context.OrderMessages?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
