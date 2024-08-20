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
    public class FaqController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUriService uriService;
        private readonly IMapper _mapper;

        public FaqController(DatabaseContext context, IUriService uriService, IMapper mapper)
        {
            _context = context;
            this.uriService = uriService;
            _mapper = mapper;
        }

        // GET: api/Faq
        [HttpGet]
        public async Task<ActionResult> GetFaqs([FromQuery] PaginationFilter filter)
        {
            if (_context.Faqs == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Faqs'  is null." });
            }

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.Faqs
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).AsNoTracking()
               .ToListAsync();
            var totalRecords = await _context.Faqs.CountAsync();
            List<FaqDto> ilistDest = _mapper.Map<List<Faq>, List<FaqDto>>(pagedData);

            var pagedReponse = PaginationHelper.CreatePagedReponse<FaqDto>(ilistDest, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        [HttpGet("Active")]
        public async Task<ActionResult> GetFaqsActive([FromQuery] PaginationFilter filter)
        {
            if (_context.Faqs == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Faqs'  is null." });
            }

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.Faqs.Where(x => x.Status).OrderBy(x => x.Rank)
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).AsNoTracking()
               .ToListAsync();
            var totalRecords = await _context.Faqs.CountAsync();
            List<FaqDto> ilistDest = _mapper.Map<List<Faq>, List<FaqDto>>(pagedData);

            var pagedReponse = PaginationHelper.CreatePagedReponse<FaqDto>(ilistDest, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        // GET: api/Faq/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Faq>> GetFaq(int id)
        {
            if (_context.Faqs == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Faqs'  is null." });
            }
            var Faq = await _context.Faqs.FindAsync(id);

            if (Faq == null)
            {
                return NotFound(new { message = "Faq not found" });
            }



            FaqDto data = _mapper.Map<FaqDto>(Faq);
            return Ok(new Wrappers.ApiResponse<FaqDto>(data));
        }

        // PUT: api/Faq/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFaq(int id, FaqDto Faq)
        {
            if (id != Faq.Id)
            {
                return BadRequest(new { message = "Invalid Faq Id" });
            }

            //check if service type exists
            if (FaqExists(Faq))
            {
                return BadRequest("Service type already exists");
            }

            Faq data = new Faq
            {
                Answer = Faq.Answer,
                Question = Faq.Question,
                Rank = Faq.Rank,
                UpdatedAt = DateTime.Now,
                Status = Faq.Status,

                Id = Faq.Id
            };
            _context.Entry(data).State = EntityState.Modified;




            await _context.SaveChangesAsync();


            return NoContent();
        }

        // POST: api/Faq
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostFaq(FaqDto Faq)
        {
            if (_context.Faqs == null)
            {
                return Problem("Entity set 'DatabaseContext.Faqs'  is null.");
            }

            //check if service type exists
            if (FaqExists(Faq))
            {
                return BadRequest(new { message = "Service type already exists" });
            }

            Faq data = new Faq
            {
                Answer = Faq.Answer,
                Question = Faq.Question,
                Rank = Faq.Rank,
                UpdatedAt = DateTime.Now,
                Status = Faq.Status,


                CreatedAt = DateTime.Now,

            };
            _context.Faqs.Add(data);
            Faq.Id = data.Id;

            await _context.SaveChangesAsync();


            return CreatedAtAction("GetFaq", new { id = data.Id }, Faq);
        }

        // DELETE: api/Faq/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFaq(int id)
        {
            if (_context.Faqs == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Faqs'  is null." });
            }
            var Faq = await _context.Faqs.FindAsync(id);
            if (Faq == null)
            {
                return NotFound(new { message = "Faq not found" });
            }

            _context.Faqs.Remove(Faq);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FaqExists(FaqDto FaqDto)
        {
            return (_context.Faqs?.Any(e => (e.Question == FaqDto.Question) && e.Id != FaqDto.Id)).GetValueOrDefault();
        }

    }
}
