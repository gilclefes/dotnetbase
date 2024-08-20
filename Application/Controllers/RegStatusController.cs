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
    public class RegStatusController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUriService uriService;
        private readonly IMapper _mapper;

        public RegStatusController(DatabaseContext context, IUriService uriService, IMapper mapper)
        {
            _context = context;
            this.uriService = uriService;
            _mapper = mapper;
        }

        // GET: api/RegStatus
        [HttpGet]
        public async Task<ActionResult> GetRegStatuss([FromQuery] PaginationFilter filter)
        {
            if (_context.RegStatuses == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.RegStatuss'  is null." });
            }

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.RegStatuses
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).AsNoTracking()
               .ToListAsync();
            var totalRecords = await _context.RegStatuses.CountAsync();
            List<RegStatusDto> ilistDest = _mapper.Map<List<RegStatus>, List<RegStatusDto>>(pagedData);

            var pagedReponse = PaginationHelper.CreatePagedReponse<RegStatusDto>(ilistDest, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        // GET: api/RegStatus/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RegStatus>> GetRegStatus(int id)
        {
            if (_context.RegStatuses == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.RegStatuss'  is null." });
            }
            var RegStatus = await _context.RegStatuses.FindAsync(id);

            if (RegStatus == null)
            {
                return NotFound(new { message = "RegStatus not found." });
            }



            RegStatusDto data = _mapper.Map<RegStatusDto>(RegStatus);
            return Ok(new Wrappers.ApiResponse<RegStatusDto>(data));
        }

        // PUT: api/RegStatus/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRegStatus(int id, RegStatusDto RegStatus)
        {
            if (id != RegStatus.Id)
            {
                return BadRequest(new { message = "Invalid RegStatus Id." });
            }


            RegStatus data = new RegStatus
            {
                Id = RegStatus.Id,
                Code = RegStatus.Code,
                Name = RegStatus.Name,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Status = RegStatus.Status,
            };

            _context.Entry(data).State = EntityState.Modified;


            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/RegStatus
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostRegStatus(RegStatusDto RegStatus)
        {
            if (_context.RegStatuses == null)
            {
                return Problem("Entity set 'DatabaseContext.RegStatuss'  is null.");
            }

            //check if RegStatus exist
            if (RegStatusExists(RegStatus))
            {
                return BadRequest(new { message = "RegStatus name or code is already taken" });
            }

            RegStatus data = new RegStatus
            {
                Code = RegStatus.Code,
                Name = RegStatus.Name,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Status = RegStatus.Status,
            };

            _context.RegStatuses.Add(data);
            await _context.SaveChangesAsync();
            RegStatus.Id = data.Id;

            return CreatedAtAction("GetRegStatus", new { id = data.Id }, RegStatus);
        }

        // DELETE: api/RegStatus/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRegStatus(int id)
        {
            if (_context.RegStatuses == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.RegStatuss'  is null." });
            }
            var RegStatus = await _context.RegStatuses.FindAsync(id);
            if (RegStatus == null)
            {
                return NotFound();
            }

            _context.RegStatuses.Remove(RegStatus);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RegStatusExists(RegStatusDto regStatusDto)
        {
            return (_context.RegStatuses?.Any(e => (e.Code == regStatusDto.Code || e.Name == regStatusDto.Name) && e.Id != regStatusDto.Id)).GetValueOrDefault();
        }

    }
}
