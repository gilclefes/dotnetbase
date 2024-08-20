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
    public class OrderChargeController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUriService uriService;
        private readonly IMapper _mapper;

        public OrderChargeController(DatabaseContext context, IUriService uriService, IMapper mapper)
        {
            _context = context;
            this.uriService = uriService;
            _mapper = mapper;
        }

        // GET: api/OrderCharge
        [HttpGet("order/{orderid}")]
        public async Task<ActionResult> GetOrderCharges(int orderid, [FromQuery] PaginationFilter filter)
        {
            if (_context.OrderCharges == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.OrderCharges'  is null." });
            }

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.OrderCharges.Where(x => x.OrderId == orderid)
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).AsNoTracking().Select(x => new OrderChargeDto
               {
                   Amount = x.Amount,

                   Id = x.Id,
                   OrderId = x.OrderId,
                   OrderRefNumber = x.Order.RefNumber,
                   ChargeDescription = x.ChargeDescription,
                   ChargeId = x.ChargeId,
                   ChargeName = x.Charge.Name,
                   DateAdded = x.DateAdded,

               })
               .ToListAsync();
            var totalRecords = await _context.OrderAssignments.CountAsync();

            var pagedReponse = PaginationHelper.CreatePagedReponse<OrderChargeDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        // GET: api/OrderCharge/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderCharge>> GetOrderCharge(int id)
        {
            if (_context.OrderCharges == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.OrderCharges'  is null." });
            }
            var OrderCharge = await _context.OrderCharges.FindAsync(id);

            if (OrderCharge == null)
            {
                return NotFound(new { message = "OrderCharge not found" });
            }



            OrderChargeDto data = new OrderChargeDto
            {
                Amount = OrderCharge.Amount,

                Id = OrderCharge.Id,
                OrderId = OrderCharge.OrderId,
                OrderRefNumber = OrderCharge.Order.RefNumber,
                ChargeDescription = OrderCharge.ChargeDescription,
                ChargeId = OrderCharge.ChargeId,
                ChargeName = OrderCharge.Charge.Name,
                DateAdded = OrderCharge.DateAdded,

            };
            return Ok(new Wrappers.ApiResponse<OrderChargeDto>(data));
        }

        // PUT: api/OrderCharge/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderCharge(int id, OrderChargeDto OrderCharge)
        {
            if (id != OrderCharge.Id)
            {
                return BadRequest(new { message = "Invalid OrderCharge Id" });
            }

            Charge? charge = await _context.Charges.FindAsync(OrderCharge.ChargeId);

            if (charge == null)
            {
                return NotFound(new { message = "Charge not found" });
            }

            Order? order = await _context.Orders.FindAsync(OrderCharge.OrderId);

            if (order == null)
            {
                return NotFound(new { message = "Order not found" });
            }


            OrderCharge data = new OrderCharge
            {
                Id = OrderCharge.Id,
                Amount = OrderCharge.Amount,
                ChargeDescription = OrderCharge.ChargeDescription,
                ChargeId = OrderCharge.ChargeId,
                Charge = charge,
                OrderId = OrderCharge.OrderId,
                Order = order,
                DateAdded = DateTime.Now,
                UpdatedAt = DateTime.Now,

            };
            _context.Entry(data).State = EntityState.Modified;




            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/OrderCharge
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostOrderCharge(OrderChargeDto OrderCharge)
        {
            if (_context.OrderCharges == null)
            {
                return Problem("Entity set 'DatabaseContext.OrderCharges'  is null.");
            }

            if (_context.Charges == null)
            {
                return Problem("Entity set 'DatabaseContext.Charges'  is null.");
            }

            if (_context.Orders == null)
            {
                return Problem("Entity set 'DatabaseContext.Orders'  is null.");
            }

            if (_context.OrderCharges.Any(x => x.OrderId == OrderCharge.OrderId && x.ChargeId == OrderCharge.ChargeId))
            {
                return BadRequest(new { message = "Charge already exist" });
            }

            Charge? charge = await _context.Charges.FindAsync(OrderCharge.ChargeId);

            if (charge == null)
            {
                return NotFound(new { message = "Charge not found" });
            }

            Order? order = await _context.Orders.FindAsync(OrderCharge.OrderId);

            if (order == null)
            {
                return NotFound(new { message = "Order not found" });
            }


            OrderCharge data = new OrderCharge
            {
                Amount = OrderCharge.Amount,
                ChargeDescription = OrderCharge.ChargeDescription,
                ChargeId = OrderCharge.ChargeId,
                Charge = charge,
                OrderId = OrderCharge.OrderId,
                Order = order,
                DateAdded = DateTime.Now,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,

            };

            _context.Charges.Attach(data.Charge);
            _context.Orders.Attach(data.Order);
            _context.OrderCharges.Add(data);



            await _context.SaveChangesAsync();


            return CreatedAtAction("GetOrderCharge", new { id = data.Id }, _mapper.Map<OrderChargeDto>(data));
        }

        // DELETE: api/OrderCharge/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderCharge(int id)
        {
            if (_context.OrderCharges == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.OrderCharges'  is null." });
            }
            var OrderCharge = await _context.OrderCharges.FindAsync(id);
            if (OrderCharge == null)
            {
                return NotFound(new { message = "OrderCharge not found" });
            }

            _context.OrderCharges.Remove(OrderCharge);
            await _context.SaveChangesAsync();

            return NoContent();
        }


    }
}
