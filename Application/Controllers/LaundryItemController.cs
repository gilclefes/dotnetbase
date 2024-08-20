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
    public class LaundryItemController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUriService uriService;
        private readonly IMapper _mapper;

        public LaundryItemController(DatabaseContext context, IUriService uriService, IMapper mapper)
        {
            _context = context;
            this.uriService = uriService;
            _mapper = mapper;
        }

        // GET: api/LaundryItem
        [HttpGet]
        public async Task<ActionResult> GetLaundryItems([FromQuery] PaginationFilter filter)
        {
            if (_context.LaundryItems == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.LaundryItems'  is null." });
            }

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.LaundryItems
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).Select(x => new LaundryItemDto
               {
                   Id = x.Id,
                   Name = x.Name,
                   Code = x.Code,
                   Status = x.Status,
                   ItemTypeId = x.ItemTypeId,
                   ItemTypeName = x.ItemType.Name,
                   Description = x.Description,

               }).AsNoTracking()
               .ToListAsync();
            var totalRecords = await _context.LaundryItems.CountAsync();

            var pagedReponse = PaginationHelper.CreatePagedReponse<LaundryItemDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        // GET: api/LaundryItem/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LaundryItem>> GetLaundryItem(int id)
        {
            if (_context.LaundryItems == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.LaundryItems'  is null." });
            }
            var LaundryItem = await _context.LaundryItems.FindAsync(id);

            if (LaundryItem == null)
            {
                return NotFound(new { message = "LaundryItem not found" });
            }



            LaundryItemDto data = _mapper.Map<LaundryItemDto>(LaundryItem);
            return Ok(new Wrappers.ApiResponse<LaundryItemDto>(data));
        }

        // PUT: api/LaundryItem/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLaundryItem(int id, LaundryItemDto LaundryItem)
        {
            if (id != LaundryItem.Id)
            {
                return BadRequest(new { message = "Invalid LaundryItem Id" });
            }

            if (_context.ItemTypes == null)
            {
                return Problem("Entity set 'DatabaseContext.ItemTypes'  is null.");
            }

            //code and name must be unique
            if (LaundryItemExists(LaundryItem))
            {
                return BadRequest(new { message = "Charge already exist" });
            }


            ItemType? itemType = await _context.ItemTypes.FindAsync(LaundryItem.ItemTypeId);

            if (itemType == null)
            {
                return NotFound(new { message = "Item type not found" });
            }


            LaundryItem data = new LaundryItem
            {
                Id = LaundryItem.Id,
                Name = LaundryItem.Name,
                ItemTypeId = LaundryItem.ItemTypeId,
                ItemType = itemType,
                Code = LaundryItem.Code,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Description = LaundryItem.Description,
                Status = LaundryItem.Status,
            };


            _context.Entry(data).State = EntityState.Modified;
            await _context.SaveChangesAsync();


            return NoContent();
        }

        // POST: api/LaundryItem
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostLaundryItem(LaundryItemDto LaundryItem)
        {
            if (_context.LaundryItems == null)
            {
                return Problem("Entity set 'DatabaseContext.LaundryItems'  is null.");
            }


            if (_context.ItemTypes == null)
            {
                return Problem("Entity set 'DatabaseContext.ItemTypes'  is null.");
            }



            //code and name must be unique
            if (LaundryItemExists(LaundryItem))
            {
                return BadRequest(new { message = "Charge already exist" });
            }



            ItemType? itemType = await _context.ItemTypes.FindAsync(LaundryItem.ItemTypeId);

            if (itemType == null)
            {
                return NotFound(new { message = "Item type not found" });
            }


            LaundryItem data = new LaundryItem
            {
                Id = LaundryItem.Id,
                Name = LaundryItem.Name,
                ItemTypeId = LaundryItem.ItemTypeId,
                ItemType = itemType,
                Code = LaundryItem.Code,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Description = LaundryItem.Description,
                Status = LaundryItem.Status,
            };

            _context.ItemTypes.Attach(data.ItemType);

            _context.LaundryItems.Add(data);

            await _context.SaveChangesAsync();
            LaundryItem.Id = data.Id;

            return CreatedAtAction("GetLaundryItem", new { id = data.Id }, LaundryItem);
        }

        // DELETE: api/LaundryItem/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLaundryItem(int id)
        {
            if (_context.LaundryItems == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.LaundryItems'  is null." });
            }
            var LaundryItem = await _context.LaundryItems.FindAsync(id);
            if (LaundryItem == null)
            {
                return NotFound(new { message = "LaundryItem not found" });
            }

            _context.LaundryItems.Remove(LaundryItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LaundryItemExists(LaundryItemDto laundryItemDto)
        {
            return (_context.LaundryItems?.Any(e => (e.Code == laundryItemDto.Code || e.Name == laundryItemDto.Name) && e.Id != laundryItemDto.Id)).GetValueOrDefault();
        }

    }
}
