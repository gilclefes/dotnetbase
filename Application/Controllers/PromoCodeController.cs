
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
    public class PromoCodeController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUriService uriService;
        private readonly IMapper _mapper;

        public PromoCodeController(DatabaseContext context, IUriService uriService, IMapper mapper)
        {
            _context = context;
            this.uriService = uriService;
            _mapper = mapper;
        }
        // GET: api/PromoCode
        [HttpGet]
        public async Task<ActionResult> GetPromoCodes([FromQuery] PaginationFilter filter)
        {
            if (_context.PromoCodes == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.PromoCodes'  is null." });
            }


            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.PromoCodes.Where(x => x.Status == true && x.PromoEndDate > DateTime.Now)
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).AsNoTracking().Select(x => new PromoCodeDto
               {
                   Id = x.Id,
                   CodeName = x.CodeName,
                   CodeValue = x.CodeValue,
                   Discount = x.Discount,
                   MaxOrderValue = x.MaxOrderValue,
                   MinOrderValue = x.MinOrderValue,
                   PromoEndDate = x.PromoEndDate,
                   PromoStartDate = x.PromoStartDate,
                   UsageCount = x.UsageCount,
                   UsageLimit = x.UsageLimit,
                   Status = x.Status,
                   Description = x.Description
               })
               .ToListAsync();
            var totalRecords = await _context.PromoCodes.CountAsync();

            var pagedReponse = PaginationHelper.CreatePagedReponse<PromoCodeDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);

        }

        // GET: api/PromoCode/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetPromoCode(int id)
        {
            if (_context.PromoCodes == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.PromoCodes'  is null." });
            }
            var PromoCode = await _context.PromoCodes.FindAsync(id);

            if (PromoCode == null)
            {
                return NotFound(new { message = "PromoCode not found." });
            }


            PromoCodeDto data = _mapper.Map<PromoCodeDto>(PromoCode);
            return Ok(new Wrappers.ApiResponse<PromoCodeDto>(data));
        }

        // PUT: api/PromoCode/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutPromoCode(int id, PromoCodeDto PromoCode)
        {
            if (id != PromoCode.Id)
            {
                return BadRequest();
            }

            if (PromoCodeExists(PromoCode))
            {
                return BadRequest(new { message = "PromoCode already exists." });
            }

            PromoCode data = new PromoCode
            {
                Id = PromoCode.Id,
                CodeName = PromoCode.CodeName,
                CodeValue = PromoCode.CodeValue,
                Discount = PromoCode.Discount,

                MaxOrderValue = PromoCode.MaxOrderValue,
                MinOrderValue = PromoCode.MinOrderValue,
                PromoEndDate = PromoCode.PromoEndDate,
                PromoStartDate = PromoCode.PromoStartDate,
                UsageCount = PromoCode.UsageCount,
                UsageLimit = PromoCode.UsageLimit,
                Status = PromoCode.Status,
                UpdatedAt = DateTime.Now,
                Description = PromoCode.Description
            };
            _context.Entry(data).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/PromoCode
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<PromoCode>> PostPromoCode(PromoCodeDto PromoCode)
        {
            if (_context.PromoCodes == null)
            {
                return Problem("Entity set 'DatabaseContext.PromoCodes'  is null.");
            }

            //check if PromoCode exists
            if (PromoCodeExists(PromoCode))
            {
                return BadRequest(new { message = "PromoCode already exists." });
            }

            PromoCode data = _mapper.Map<PromoCode>(PromoCode);
            _context.PromoCodes.Add(data);

            await _context.SaveChangesAsync();


            return CreatedAtAction("GetPromoCode", new { id = data.Id }, _mapper.Map<PromoCodeDto>(data));
        }

        // DELETE: api/PromoCode/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeletePromoCode(int id)
        {
            if (_context.PromoCodes == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.PromoCodes'  is null." });
            }
            var PromoCode = await _context.PromoCodes.FindAsync(id);
            if (PromoCode == null)
            {
                return NotFound(new { message = "PromoCode not found." });
            }

            _context.PromoCodes.Remove(PromoCode);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PromoCodeExists(PromoCodeDto PromoCode)
        {
            return (_context.PromoCodes?.Any(e => (e.CodeName == PromoCode.CodeName || e.CodeValue == PromoCode.CodeValue) && e.Id != PromoCode.Id)).GetValueOrDefault();
        }
    }
}
