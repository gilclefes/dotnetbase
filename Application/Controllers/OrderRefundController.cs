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
    public class OrderRefundController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUriService uriService;
        private readonly IMapper _mapper;

        public OrderRefundController(DatabaseContext context, IUriService uriService, IMapper mapper)
        {
            _context = context;
            this.uriService = uriService;
            _mapper = mapper;
        }

        // GET: api/OrderRating
        [HttpGet]
        public async Task<ActionResult> GetOrderRefunds([FromQuery] PaginationFilter filter)
        {
            if (_context.OrderRatings == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.OrderRatings'  is null." });
            }

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.OrderRefunds.Include(x => x.Client).Include(x => x.Order)
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).AsNoTracking().Select(x => new OrderRefundDto
               {
                   Id = x.Id,

                   RefundStatus = x.RefundStatus,
                   RefundAmount = x.RefundAmount,
                   RefundReason = x.RefundReason,
                   ClientuserName = x.Client.Email,
                   LastRetryDate = x.LastRetryDate,
                   OrderRefNumber = x.Order.RefNumber,
                   CreatedAt = x.CreatedAt,
                   UpdatedAt = x.UpdatedAt,
                   ExtransacitonId = x.ExtransacitonId,
                   RefundRetry = x.RefundRetry,
                   RefundExTransactionId = x.RefundExTransactionId,


               })
               .ToListAsync();
            var totalRecords = await _context.OrderRefunds.CountAsync();

            var pagedReponse = PaginationHelper.CreatePagedReponse<OrderRefundDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        [HttpGet("{email}")]
        public async Task<ActionResult> GetOrderRatingsByOrder(string email, [FromQuery] PaginationFilter filter)
        {
            if (_context.OrderRatings == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.OrderRatings'  is null." });
            }

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.OrderRefunds.Include(x => x.Client).Include(x => x.Order).Where(x => x.Client.Email == email)
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                .Take(validFilter.PageSize).AsNoTracking().Select(x => new OrderRefundDto
                {
                    Id = x.Id,

                    RefundStatus = x.RefundStatus,
                    RefundAmount = x.RefundAmount,
                    RefundReason = x.RefundReason,
                    ClientuserName = x.Client.Email,
                    LastRetryDate = x.LastRetryDate,
                    OrderRefNumber = x.Order.RefNumber,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    ExtransacitonId = x.ExtransacitonId,
                    RefundRetry = x.RefundRetry,
                    RefundExTransactionId = x.RefundExTransactionId,


                })
               .ToListAsync();
            var totalRecords = await _context.OrderRefunds.CountAsync();

            var pagedReponse = PaginationHelper.CreatePagedReponse<OrderRefundDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }


    }
}
