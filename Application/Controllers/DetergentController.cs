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
    public class DetergentController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUriService uriService;
        private readonly IMapper _mapper;

        public DetergentController(DatabaseContext context, IUriService uriService, IMapper mapper)
        {
            _context = context;
            this.uriService = uriService;
            _mapper = mapper;
        }

        // GET: api/Detergent
        [HttpGet]
        public async Task<ActionResult> GetDetergents([FromQuery] PaginationFilter filter)
        {
            if (_context.Detergents == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Detergents'  is null." });
            }
            // return await _context.Detergents.ToListAsync();
            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.Detergents
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).AsNoTracking()
               .ToListAsync();
            var totalRecords = await _context.Detergents.CountAsync();
            List<DetergentDto> ilistDest = _mapper.Map<List<Detergent>, List<DetergentDto>>(pagedData);

            var pagedReponse = PaginationHelper.CreatePagedReponse<DetergentDto>(ilistDest, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        // GET: api/Detergent/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Detergent>> GetDetergent(int id)
        {
            if (_context.Detergents == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Detergents'  is null." });
            }
            var Detergent = await _context.Detergents.FindAsync(id);

            if (Detergent == null)
            {
                return NotFound(new { message = "Detergent not found" });
            }



            DetergentDto data = _mapper.Map<DetergentDto>(Detergent);
            return Ok(new Wrappers.ApiResponse<DetergentDto>(data));
        }

        // PUT: api/Detergent/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDetergent(int id, DetergentDto Detergent)
        {
            if (id != Detergent.Id)
            {
                return BadRequest(new { message = "Invalid Detergent Id" });
            }

            //check if the name or code already exists
            if (DetergentExists(Detergent))
            {
                return BadRequest(new { message = "Detergent name or code is already taken" });
            }

            Detergent data = new Detergent
            {
                Id = Detergent.Id,
                Name = Detergent.Name,
                Code = Detergent.Code,
                Status = Detergent.Status,
                Description = Detergent.Description,
                UpdatedAt = DateTime.UtcNow
            };
            _context.Entry(data).State = EntityState.Modified;
            await _context.SaveChangesAsync();


            return NoContent();
        }

        // POST: api/Detergent
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostDetergent(DetergentDto Detergent)
        {
            if (_context.Detergents == null)
            {
                return Problem("Entity set 'DatabaseContext.Detergents'  is null.");
            }

            //check if the name or code already exists
            if (DetergentExists(Detergent))
            {
                return BadRequest(new { message = "Detergent name or code is already taken" });
            }

            Detergent data = new Detergent
            {
                Name = Detergent.Name,
                Code = Detergent.Code,
                Description = Detergent.Description,
                Status = Detergent.Status,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.Detergents.Add(data);


            await _context.SaveChangesAsync();
            Detergent.Id = data.Id;

            return CreatedAtAction("GetDetergent", new { id = data.Id }, Detergent);
        }

        // DELETE: api/Detergent/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDetergent(int id)
        {
            if (_context.Detergents == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Detergents'  is null." });
            }
            var Detergent = await _context.Detergents.FindAsync(id);
            if (Detergent == null)
            {
                return NotFound(new { message = "Detergent not found" });
            }

            _context.Detergents.Remove(Detergent);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DetergentExists(DetergentDto DetergentDto)
        {
            return (_context.Detergents?.Any(e => (e.Name == DetergentDto.Name || e.Code == DetergentDto.Code) && e.Id != DetergentDto.Id)).GetValueOrDefault();
        }

    }
}
