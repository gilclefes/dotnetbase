using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core.Features;
using Microsoft.EntityFrameworkCore;
using dotnetbase.Application.Database;
using dotnetbase.Application.Filter;
using dotnetbase.Application.Helpers;
using dotnetbase.Application.Models;
using dotnetbase.Application.Services;
using dotnetbase.Application.ViewModels;
using dotnetbase.Application.Wrappers;

namespace dotnetbase.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize]
    public class ChargeController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUriService uriService;
        private readonly IMapper _mapper;


        public ChargeController(DatabaseContext context, IUriService uriService, IMapper mapper)
        {
            _context = context;
            this.uriService = uriService;
            _mapper = mapper;
        }

        // GET: api/Charge
        [HttpGet]
        public async Task<ActionResult> GetCharges([FromQuery] PaginationFilter filter)
        {
            if (_context.Charges == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Charges'  is null." });
            }

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.Charges
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).Select(x => new ChargeDto
               {
                   Id = x.Id,
                   Name = x.Name,
                   Code = x.Code,
                   Status = x.Status,
                   CategoryId = x.CategoryId,
                   ChargeCategoryName = x.ChargeCategory.Name,
                   Description = x.Description,
                   AmountType = x.AmountType,
                   IsYaboCharge = x.IsYaboCharge

               }).AsNoTracking()
               .ToListAsync();
            var totalRecords = await _context.Charges.CountAsync();

            var pagedReponse = PaginationHelper.CreatePagedReponse<ChargeDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        // GET: api/Charge/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetCharge(int id)
        {
            if (_context.Charges == null)
            {
                return NotFound();
            }
            var charge = await _context.Charges.FindAsync(id);

            if (charge == null)
            {
                return NotFound();
            }

            ChargeDto data = _mapper.Map<ChargeDto>(charge);
            return Ok(new Wrappers.ApiResponse<ChargeDto>(data));

        }

        // PUT: api/Charge/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCharge(int id, ChargeDto charge)
        {
            if (id != charge.Id)
            {
                return BadRequest(new { message = "Invalid Charge Id" });
            }

            if (_context.ChargeCategories == null)
            {
                return Problem("Entity set 'DatabaseContext.ChargeCategories'  is null.");
            }

            //code and name must be unique
            if (ChargeExists(charge))
            {
                return BadRequest(new { message = "Charge already exist" });
            }


            ChargeCategory? chargeCategory = await _context.ChargeCategories.FindAsync(charge.CategoryId);

            if (chargeCategory == null)
            {
                return NotFound(new { message = "ChargeCategory not found" });
            }

            if (charge.AmountType != ChargeAmountType.FIXED)
            {
                charge.AmountType = ChargeAmountType.PERCENTAGE;
            }


            Charge data = new Charge
            {
                Id = charge.Id,
                Name = charge.Name,
                CategoryId = charge.CategoryId,
                ChargeCategory = chargeCategory,
                Code = charge.Code,
                Description = charge.Description,
                Status = charge.Status,
                UpdatedAt = DateTime.Now,
                AmountType = charge.AmountType,
                IsYaboCharge = charge.IsYaboCharge

            };
            _context.Entry(data).State = EntityState.Modified;



            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Charge
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostCharge(ChargeDto charge)
        {
            if (_context.Charges == null)
            {
                return Problem("Entity set 'DatabaseContext.Charges'  is null.");
            }


            if (_context.ChargeCategories == null)
            {
                return Problem("Entity set 'DatabaseContext.ChargeCategories'  is null.");
            }

            //code and name must be unique
            if (ChargeExists(charge))
            {
                return BadRequest(new { message = "Charge already exist" });
            }

            if (charge.AmountType != ChargeAmountType.FIXED)
            {
                charge.AmountType = ChargeAmountType.PERCENTAGE;
            }


            ChargeCategory? chargeCategory = await _context.ChargeCategories.FindAsync(charge.CategoryId);

            if (chargeCategory == null)
            {
                return NotFound(new { message = "ChargeCategory not found" });
            }


            Charge data = new Charge
            {
                Id = charge.Id,
                Name = charge.Name,
                CategoryId = charge.CategoryId,
                ChargeCategory = chargeCategory,
                Code = charge.Code,
                Description = charge.Description,
                Status = charge.Status,
                UpdatedAt = DateTime.Now,
                CreatedAt = DateTime.Now,
                AmountType = charge.AmountType,
                IsYaboCharge = charge.IsYaboCharge
            };

            _context.ChargeCategories.Attach(data.ChargeCategory);
            _context.Charges.Add(data);
            await _context.SaveChangesAsync();
            charge.Id = data.Id;

            return CreatedAtAction("GetCharge", new { id = data.Id }, charge);
        }

        // DELETE: api/Charge/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCharge(int id)
        {
            if (_context.Charges == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Charges'  is null." });
            }
            var charge = await _context.Charges.FindAsync(id);
            if (charge == null)
            {
                return NotFound(new { message = "Charge not found." });
            }

            _context.Charges.Remove(charge);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ChargeExists(ChargeDto charge)
        {
            return (_context.Charges?.Any(e => (e.Name == charge.Name || e.Code == charge.Code) && e.Id != charge.Id)).GetValueOrDefault();
        }

    }
}
