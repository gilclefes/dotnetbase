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

    public class UnitTypeController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUriService uriService;
        private readonly IMapper _mapper;

        public UnitTypeController(DatabaseContext context, IUriService uriService, IMapper mapper)
        {
            _context = context;
            this.uriService = uriService;
            _mapper = mapper;
        }

        // GET: api/UnitType
        [HttpGet]
        public async Task<ActionResult> GetUnitTypes([FromQuery] PaginationFilter filter)
        {
            if (_context.UnitTypes == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.UnitTypes'  is null." });
            }

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.UnitTypes
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).AsNoTracking()
               .ToListAsync();
            var totalRecords = await _context.OrderAssignments.CountAsync();
            List<UnitTypeDto> ilistDest = _mapper.Map<List<UnitType>, List<UnitTypeDto>>(pagedData);

            var pagedReponse = PaginationHelper.CreatePagedReponse<UnitTypeDto>(ilistDest, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        // GET: api/UnitType/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UnitType>> GetUnitType(int id)
        {
            if (_context.UnitTypes == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.UnitTypes'  is null." });
            }
            var UnitType = await _context.UnitTypes.FindAsync(id);

            if (UnitType == null)
            {
                return NotFound(new { message = "UnitType not found." });
            }



            UnitTypeDto data = _mapper.Map<UnitTypeDto>(UnitType);
            return Ok(new Wrappers.ApiResponse<UnitTypeDto>(data));
        }

        // PUT: api/UnitType/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutUnitType(int id, UnitTypeDto UnitType)
        {
            if (id != UnitType.Id)
            {
                return BadRequest(new { message = "Invalid UnitType Id." });
            }


            UnitType data = new UnitType
            {
                Id = UnitType.Id,
                Name = UnitType.Name,
                Code = UnitType.Code,
                Description = UnitType.Description,
                Status = UnitType.Status,

                UpdatedAt = DateTime.Now,
            };

            _context.Entry(data).State = EntityState.Modified;




            await _context.SaveChangesAsync();


            return NoContent();
        }

        // POST: api/UnitType
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> PostUnitType(UnitTypeDto UnitType)
        {
            if (_context.UnitTypes == null)
            {
                return Problem("Entity set 'DatabaseContext.UnitTypes'  is null.");
            }

            UnitType data = new UnitType
            {
                Name = UnitType.Name,
                Code = UnitType.Code,
                Description = UnitType.Description,
                Status = UnitType.Status,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

            _context.UnitTypes.Add(data);


            await _context.SaveChangesAsync();
            UnitType.Id = data.Id;


            return CreatedAtAction("GetUnitType", new { id = data.Id }, UnitType);
        }

        // DELETE: api/UnitType/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUnitType(int id)
        {
            if (_context.UnitTypes == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.UnitTypes'  is null." });
            }
            var UnitType = await _context.UnitTypes.FindAsync(id);
            if (UnitType == null)
            {
                return NotFound(new { message = "UnitType not found." });
            }

            _context.UnitTypes.Remove(UnitType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UnitTypeExists(UnitTypeDto unitTypeDto)
        {
            return (_context.UnitTypes?.Any(e => (e.Name == unitTypeDto.Name || e.Code == unitTypeDto.Code) && e.Id != unitTypeDto.Id)).GetValueOrDefault();
        }

    }
}
