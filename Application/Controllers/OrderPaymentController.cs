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
    public class OrderPaymentController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUriService uriService;
        private readonly IMapper _mapper;

        public OrderPaymentController(DatabaseContext context, IUriService uriService, IMapper mapper)
        {
            _context = context;
            this.uriService = uriService;
            _mapper = mapper;
        }

        // GET: api/OrderPayment
        [HttpGet]
        public async Task<ActionResult> GetOrderPayments([FromQuery] PaginationFilter filter)
        {
            if (_context.OrderPayments == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.OrderPayments'  is null." });
            }

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.OrderPayments
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).AsNoTracking().Select(x => new OrderPaymentDto
               {
                   OrderRefNumber = x.Order.RefNumber,
                   Id = x.Id,
                   OrderId = x.OrderId,
                   PaymentId = x.PaymentId,
                   PaymentRefNumber = x.Payment.RefNumber,
               })
               .ToListAsync();
            var totalRecords = await _context.OrderAssignments.CountAsync();

            var pagedReponse = PaginationHelper.CreatePagedReponse<OrderPaymentDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        [HttpGet("order/{orderid}")]
        public async Task<ActionResult> GetOrderPaymentsByOrder(int ordorid, [FromQuery] PaginationFilter filter)
        {
            if (_context.OrderPayments == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.OrderPayments'  is null." });
            }

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.OrderPayments.Where(x => x.OrderId == ordorid)
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).AsNoTracking().Select(x => new OrderPaymentDto
               {
                   OrderRefNumber = x.Order.RefNumber,
                   Id = x.Id,
                   OrderId = x.OrderId,
                   PaymentId = x.PaymentId,
                   PaymentRefNumber = x.Payment.RefNumber,
               })
               .ToListAsync();
            var totalRecords = await _context.OrderAssignments.CountAsync();

            var pagedReponse = PaginationHelper.CreatePagedReponse<OrderPaymentDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        // GET: api/OrderPayment/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderPayment>> GetOrderPayment(int id)
        {
            if (_context.OrderPayments == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.OrderPayments'  is null." });
            }
            OrderPayment? x = await _context.OrderPayments.FindAsync(id);

            if (x == null)
            {
                return NotFound(new { message = "OrderPayment not found" });
            }



            OrderPaymentDto data = new OrderPaymentDto
            {
                OrderRefNumber = x.Order.RefNumber,
                Id = x.Id,
                OrderId = x.OrderId,
                PaymentId = x.PaymentId,
                PaymentRefNumber = x.Payment.RefNumber,
            };
            return Ok(new Wrappers.ApiResponse<OrderPaymentDto>(data));
        }

        // PUT: api/OrderPayment/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderPayment(int id, OrderPaymentDto OrderPayment)
        {
            if (id != OrderPayment.Id)
            {
                return BadRequest(new { message = "Invalid OrderPayment Id" });
            }


            OrderPayment data = _mapper.Map<OrderPayment>(OrderPayment);
            _context.Entry(data).State = EntityState.Modified;



            await _context.SaveChangesAsync();


            return NoContent();
        }

        // POST: api/OrderPayment
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostOrderPayment(OrderPaymentDto OrderPayment)
        {
            if (_context.OrderPayments == null)
            {
                return Problem("Entity set 'DatabaseContext.OrderPayments'  is null.");
            }

            OrderPayment data = _mapper.Map<OrderPayment>(OrderPayment);
            _context.OrderPayments.Add(data);


            await _context.SaveChangesAsync();


            return CreatedAtAction("GetOrderPayment", new { id = data.Id }, _mapper.Map<IdTypeDto>(data));
        }

        // DELETE: api/OrderPayment/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderPayment(int id)
        {
            if (_context.OrderPayments == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.OrderPayments'  is null." });
            }
            var OrderPayment = await _context.OrderPayments.FindAsync(id);
            if (OrderPayment == null)
            {
                return NotFound(new { message = "OrderPayment not found" });
            }

            _context.OrderPayments.Remove(OrderPayment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderPaymentExists(int id)
        {
            return (_context.OrderPayments?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
