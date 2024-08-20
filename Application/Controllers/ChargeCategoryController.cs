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
    public class ChargeCategoryController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUriService uriService;
        private readonly IMapper _mapper;

        public ChargeCategoryController(DatabaseContext context, IUriService uriService, IMapper mapper)
        {
            _context = context;
            this.uriService = uriService;
            _mapper = mapper;
        }

        // GET: api/ChargeCategory
        [HttpGet]
        public async Task<ActionResult> GetChargeCategories([FromQuery] PaginationFilter filter)
        {
            if (_context.ChargeCategories == null)
            {
                return NotFound();
            }
            // return await _context.ChargeCategories.ToListAsync();

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.ChargeCategories
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).AsNoTracking()
               .ToListAsync();
            var totalRecords = await _context.ChargeCategories.CountAsync();
            List<ChargeCategoryDto> ilistDest = _mapper.Map<List<ChargeCategory>, List<ChargeCategoryDto>>(pagedData);

            var pagedReponse = PaginationHelper.CreatePagedReponse<ChargeCategoryDto>(ilistDest, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        // GET: api/ChargeCategory/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetChargeCategory(int id)
        {
            if (_context.ChargeCategories == null)
            {
                return NotFound();
            }
            var chargeCategory = await _context.ChargeCategories.FindAsync(id);

            if (chargeCategory == null)
            {
                return NotFound();
            }

            ChargeCategoryDto data = _mapper.Map<ChargeCategoryDto>(chargeCategory);
            return Ok(new Wrappers.ApiResponse<ChargeCategoryDto>(data));

        }

        // PUT: api/ChargeCategory/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutChargeCategory(int id, ChargeCategoryDto chargeCategory)
        {
            if (id != chargeCategory.Id)
            {
                return BadRequest(new { message = "Invalid ChargeCategory Id" });
            }

            //check if chargeCategory exists
            if (ChargeCategoryExists(chargeCategory))
            {
                return BadRequest("ChargeCategory already exists.");
            }

            ChargeCategory data = new ChargeCategory
            {
                Id = chargeCategory.Id,
                Name = chargeCategory.Name,
                Code = chargeCategory.Code,
                Status = chargeCategory.Status,
                Description = chargeCategory.Description,
                UpdatedAt = DateTime.Now
            };
            _context.Entry(data).State = EntityState.Modified;

            await _context.SaveChangesAsync();


            return NoContent();
        }

        //write a function that accepts image from angular application, save it to the server and get the path to save in an object


        // POST: api/ChargeCategory
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ChargeCategory>> PostChargeCategory(ChargeCategoryDto chargeCategory)
        {
            if (_context.ChargeCategories == null)
            {
                return Problem("Entity set 'DatabaseContext.ChargeCategories'  is null.");
            }

            //check if chargeCategory exists
            if (ChargeCategoryExists(chargeCategory))
            {
                return BadRequest(new { message = "ChargeCategory already exists." });
            }

            ChargeCategory data = new ChargeCategory
            {
                Name = chargeCategory.Name,
                Code = chargeCategory.Code,
                Status = chargeCategory.Status,
                Description = chargeCategory.Description,
                UpdatedAt = DateTime.Now,
                CreatedAt = DateTime.Now
            };

            _context.ChargeCategories.Add(data);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetChargeCategory", new { id = data.Id }, _mapper.Map<ChargeCategoryDto>(data));
        }

        // DELETE: api/ChargeCategory/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChargeCategory(int id)
        {
            if (_context.ChargeCategories == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.ChargeCategories'  is null." });
            }
            var chargeCategory = await _context.ChargeCategories.FindAsync(id);
            if (chargeCategory == null)
            {
                return NotFound(new { message = "ChargeCategory not found." });
            }

            _context.ChargeCategories.Remove(chargeCategory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ChargeCategoryExists(ChargeCategoryDto chargeCategoryDto)
        {
            return (_context.ChargeCategories?.Any(e => (e.Name == chargeCategoryDto.Name || e.Code == chargeCategoryDto.Code) && e.Id != chargeCategoryDto.Id)).GetValueOrDefault();
        }

    }
}
