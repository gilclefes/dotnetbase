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
    public class ItemTypeController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUriService uriService;
        private readonly IMapper _mapper;

        public ItemTypeController(DatabaseContext context, IUriService uriService, IMapper mapper)
        {
            _context = context;
            this.uriService = uriService;
            _mapper = mapper;
        }

        // GET: api/ItemType
        [HttpGet]
        public async Task<ActionResult> GetItemTypes([FromQuery] PaginationFilter filter)
        {
            if (_context.ItemTypes == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.ItemTypes'  is null." });
            }

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.ItemTypes
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).AsNoTracking()
               .ToListAsync();
            var totalRecords = await _context.ItemTypes.CountAsync();
            List<ItemTypeDto> ilistDest = _mapper.Map<List<ItemType>, List<ItemTypeDto>>(pagedData);

            var pagedReponse = PaginationHelper.CreatePagedReponse<ItemTypeDto>(ilistDest, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        // GET: api/ItemType/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemType>> GetItemType(int id)
        {
            if (_context.ItemTypes == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.ItemTypes'  is null." });
            }
            var itemType = await _context.ItemTypes.FindAsync(id);

            if (itemType == null)
            {
                return NotFound(new { message = "ItemType not found" });
            }



            ItemTypeDto data = _mapper.Map<ItemTypeDto>(itemType);
            return Ok(new Wrappers.ApiResponse<ItemTypeDto>(data));
        }

        // PUT: api/ItemType/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutItemType(int id, ItemTypeDto itemType)
        {
            if (id != itemType.Id)
            {
                return BadRequest(new { message = "Invalid ItemType Id" });
            }

            //check if itemType exists
            if (ItemTypeExists(itemType))
            {
                return BadRequest(new { message = "ItemType already exists." });
            }


            ItemType data = new ItemType
            {
                Id = itemType.Id,
                Name = itemType.Name,
                Code = itemType.Code,
                Status = itemType.Status,
                Description = itemType.Description,
                UpdatedAt = DateTime.Now
            };
            _context.Entry(data).State = EntityState.Modified;




            await _context.SaveChangesAsync();


            return NoContent();
        }

        // POST: api/ItemType
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostItemType(ItemTypeDto itemType)
        {
            if (_context.ItemTypes == null)
            {
                return Problem("Entity set 'DatabaseContext.ItemTypes'  is null.");
            }

            //check if itemType exists
            if (ItemTypeExists(itemType))
            {
                return BadRequest(new { message = "ItemType already exists." });
            }

            ItemType data = new ItemType
            {
                Name = itemType.Name,
                Code = itemType.Code,
                Status = itemType.Status,
                Description = itemType.Description,
                UpdatedAt = DateTime.Now,
                CreatedAt = DateTime.Now
            };
            _context.ItemTypes.Add(data);


            await _context.SaveChangesAsync();

            itemType.Id = data.Id;
            return CreatedAtAction("GetItemType", new { id = data.Id }, itemType);
        }

        // DELETE: api/ItemType/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItemType(int id)
        {
            if (_context.ItemTypes == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.ItemTypes'  is null." });
            }
            var itemType = await _context.ItemTypes.FindAsync(id);
            if (itemType == null)
            {
                return NotFound(new { message = "ItemType not found" });
            }

            _context.ItemTypes.Remove(itemType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ItemTypeExists(ItemTypeDto itemType)
        {
            return (_context.ItemTypes?.Any(e => e.Name == itemType.Name && e.Id != itemType.Id)).GetValueOrDefault();
        }
    }
}
