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
    public class OrderStatusController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUriService uriService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        // private const string NEW_ORDERSTATUS_ID = "NEW_ORDERSTATUS_ID";
        //private const string PAID_ORDERSTATUS_ID = "PAID_ORDERSTATUS_ID";

        public OrderStatusController(DatabaseContext context, IConfiguration configuration, IUriService uriService, IMapper mapper)
        {
            _context = context;
            this.uriService = uriService;
            _mapper = mapper;
            _configuration = configuration;
        }

        [HttpGet("UserAllowed")]
        public async Task<ActionResult> GetOrderStatusUserAllowed([FromQuery] PaginationFilter filter)
        {

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.OrderStatus
                .Where(x => x.Description != null && x.Description.Contains("User Allowed"))
                .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                .Take(validFilter.PageSize)
                .AsNoTracking()
                .ToListAsync();
            var totalRecords = await _context.OrderStatus.CountAsync();
            List<OrderStatusDto> ilistDest = _mapper.Map<List<OrderStatus>, List<OrderStatusDto>>(pagedData);

            var pagedReponse = PaginationHelper.CreatePagedReponse<OrderStatusDto>(ilistDest, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        // GET: api/OrderStatus
        [HttpGet]
        public async Task<ActionResult> GetOrderStatuss([FromQuery] PaginationFilter filter)
        {
            if (_context.OrderStatus == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.OrderStatuss'  is null." });
            }
            // return await _context.OrderStatuss.ToListAsync();
            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.OrderStatus
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).AsNoTracking()
               .ToListAsync();
            var totalRecords = await _context.OrderStatus.CountAsync();
            List<OrderStatusDto> ilistDest = _mapper.Map<List<OrderStatus>, List<OrderStatusDto>>(pagedData);

            var pagedReponse = PaginationHelper.CreatePagedReponse<OrderStatusDto>(ilistDest, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        // GET: api/OrderStatus/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderStatus>> GetOrderStatus(int id)
        {
            if (_context.OrderStatus == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.OrderStatuss'  is null." });
            }
            var OrderStatus = await _context.OrderStatus.FindAsync(id);

            if (OrderStatus == null)
            {
                return NotFound(new { message = "OrderStatus not found" });
            }



            OrderStatusDto data = _mapper.Map<OrderStatusDto>(OrderStatus);
            return Ok(new Wrappers.ApiResponse<OrderStatusDto>(data));
        }

        // PUT: api/OrderStatus/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderStatus(int id, OrderStatusDto OrderStatus)
        {
            if (id != OrderStatus.Id)
            {
                return BadRequest(new { message = "Invalid OrderStatus Id" });
            }

            //check if the name or code already exists
            if (OrderStatusExists(OrderStatus))
            {
                return BadRequest(new { message = "OrderStatus name or code is already taken" });
            }

            OrderStatus data = new OrderStatus
            {
                Id = OrderStatus.Id,
                Name = OrderStatus.Name,
                Code = OrderStatus.Code,
                Rank = OrderStatus.Rank,
                Description = OrderStatus.Description,
                UpdatedAt = DateTime.UtcNow
            };
            _context.Entry(data).State = EntityState.Modified;
            await _context.SaveChangesAsync();


            return NoContent();
        }

        // POST: api/OrderStatus
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostOrderStatus(OrderStatusDto OrderStatus)
        {
            if (_context.OrderStatus == null)
            {
                return Problem("Entity set 'DatabaseContext.OrderStatuss'  is null.");
            }

            //check if the name or code already exists
            if (OrderStatusExists(OrderStatus))
            {
                return BadRequest(new { message = "OrderStatus name or code is already taken" });
            }

            OrderStatus data = new OrderStatus
            {
                Name = OrderStatus.Name,
                Code = OrderStatus.Code,
                Description = OrderStatus.Description,
                Rank = OrderStatus.Rank,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.OrderStatus.Add(data);


            await _context.SaveChangesAsync();
            OrderStatus.Id = data.Id;

            return CreatedAtAction("GetOrderStatus", new { id = data.Id }, OrderStatus);
        }

        // DELETE: api/OrderStatus/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderStatus(int id)
        {
            if (_context.OrderStatus == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.OrderStatuss'  is null." });
            }
            var OrderStatus = await _context.OrderStatus.FindAsync(id);
            if (OrderStatus == null)
            {
                return NotFound(new { message = "OrderStatus not found" });
            }

            _context.OrderStatus.Remove(OrderStatus);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderStatusExists(OrderStatusDto orderStatusDto)
        {
            return (_context.OrderStatus?.Any(e => (e.Name == orderStatusDto.Name || e.Code == orderStatusDto.Code) && e.Id != orderStatusDto.Id)).GetValueOrDefault();
        }

    }
}
