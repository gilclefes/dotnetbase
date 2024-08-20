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
    public class IdTypeController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUriService uriService;
        private readonly IMapper _mapper;

        public IdTypeController(DatabaseContext context, IUriService uriService, IMapper mapper)
        {
            _context = context;
            this.uriService = uriService;
            _mapper = mapper;
        }
        // GET: api/IdType
        [HttpGet]
        public async Task<ActionResult> GetIdTypes([FromQuery] PaginationFilter filter)
        {
            if (_context.IdTypes == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.IdTypes'  is null." });
            }


            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.IdTypes
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).AsNoTracking()
               .ToListAsync();
            var totalRecords = await _context.IdTypes.CountAsync();
            List<IdTypeDto> ilistDest = _mapper.Map<List<IdType>, List<IdTypeDto>>(pagedData);

            var pagedReponse = PaginationHelper.CreatePagedReponse<IdTypeDto>(ilistDest, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);

        }

        // GET: api/IdType/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetIdType(int id)
        {
            if (_context.IdTypes == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.IdTypes'  is null." });
            }
            var idType = await _context.IdTypes.FindAsync(id);

            if (idType == null)
            {
                return NotFound(new { message = "IdType not found" });
            }


            IdTypeDto data = _mapper.Map<IdTypeDto>(idType);
            return Ok(new Wrappers.ApiResponse<IdTypeDto>(data));
        }

        // PUT: api/IdType/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutIdType(int id, IdTypeDto idType)
        {
            if (id != idType.Id)
            {
                return BadRequest(new { message = "Invalid IdType Id" });
            }

            if (IdTypeExists(idType))
            {
                return BadRequest(new { message = "IdType already exists" });
            }

            IdType data = new IdType
            {
                Id = idType.Id,
                Name = idType.Name,
                Code = idType.Code,
                Status = idType.Status,
                UpdatedAt = DateTime.Now
            };
            _context.Entry(data).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/IdType
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<IdType>> PostIdType(IdTypeDto idType)
        {
            if (_context.IdTypes == null)
            {
                return Problem("Entity set 'DatabaseContext.IdTypes'  is null.");
            }

            //check if idtype exists
            if (IdTypeExists(idType))
            {
                return BadRequest(new { message = "IdType already exists" });
            }

            IdType data = _mapper.Map<IdType>(idType);
            _context.IdTypes.Add(data);

            await _context.SaveChangesAsync();


            return CreatedAtAction("GetIdType", new { id = data.Id }, _mapper.Map<IdTypeDto>(data));
        }

        // DELETE: api/IdType/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIdType(int id)
        {
            if (_context.IdTypes == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.IdTypes'  is null." });
            }
            var idType = await _context.IdTypes.FindAsync(id);
            if (idType == null)
            {
                return NotFound(new { message = "IdType not found" });
            }

            _context.IdTypes.Remove(idType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool IdTypeExists(IdTypeDto idType)
        {
            return (_context.IdTypes?.Any(e => (e.Name == idType.Name) && e.Id != idType.Id)).GetValueOrDefault();
        }
    }
}
