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
    public class OrderReconController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUriService uriService;
        private readonly IMapper _mapper;

        public OrderReconController(DatabaseContext context, IUriService uriService, IMapper mapper)
        {
            _context = context;
            this.uriService = uriService;
            _mapper = mapper;
        }

        // GET: api/OrderRecon
        [HttpGet("{username}")]
        public async Task<ActionResult> GetOrderReconByUsername(string username, [FromQuery] PaginationFilter filter)
        {
            if (_context.OrderRecons == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.OrderCharges'  is null." });
            }

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.OrderRecons.Where(x => x.PartnerUserName == username)
            .OrderByDescending(x => x.OrdecCompletionDate)
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).AsNoTracking().Select(x => new OrderReconDto
               {
                   Id = x.Id,
                   OrderId = x.OrderId,
                   OrderRefNumber = x.Order.RefNumber,
                   OrdecCompletionDate = x.OrdecCompletionDate,
                   PartnerUserName = x.PartnerUserName,
                   PartnerShare = x.PartnerShare,
                   TotalOrderAmount = x.TotalOrderAmount,
                   YaboShare = x.YaboShare,

               })
               .ToListAsync();
            var totalRecords = await _context.OrderAssignments.CountAsync();

            var pagedReponse = PaginationHelper.CreatePagedReponse<OrderReconDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }



        // GET: api/OrderRecon
        [HttpGet()]
        public async Task<ActionResult> GetOrderRecon([FromQuery] PaginationFilter filter)
        {
            if (_context.OrderRecons == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.OrderCharges'  is null." });
            }
            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.OrderRecons
            .OrderByDescending(x => x.OrdecCompletionDate)
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).AsNoTracking().Select(x => new OrderReconDto
               {
                   Id = x.Id,
                   OrderId = x.OrderId,
                   OrderRefNumber = x.Order.RefNumber,
                   OrdecCompletionDate = x.OrdecCompletionDate,
                   PartnerUserName = x.PartnerUserName,
                   PartnerShare = x.PartnerShare,
                   TotalOrderAmount = x.TotalOrderAmount,
                   YaboShare = x.YaboShare,

               })
               .ToListAsync();
            var totalRecords = await _context.OrderAssignments.CountAsync();

            var pagedReponse = PaginationHelper.CreatePagedReponse<OrderReconDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }


    }
}
