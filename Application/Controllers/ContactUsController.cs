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
    public class ContactUsController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUriService uriService;
        private readonly IMapper _mapper;

        public ContactUsController(DatabaseContext context, IUriService uriService, IMapper mapper)
        {
            _context = context;
            this.uriService = uriService;
            _mapper = mapper;
        }

        // GET: api/ContactUs
        [HttpGet]
        public async Task<ActionResult> GetContactUss([FromQuery] PaginationFilter filter)
        {
            if (_context.ContactUs == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.ContactUs'  is null." });
            }

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.ContactUs
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).Select(x => new ContactUsDto
               {
                   Id = x.Id,
                   Name = x.Name,
                   Message = x.Message,
                   Email = x.Email,
                   Read = x.Read,
                   ReadBy = x.ReadBy,
                   ReadAt = x.ReadAt,
                   CreatedAt = x.CreatedAt,

               }).OrderByDescending(x => x.Id).AsNoTracking()
               .ToListAsync();
            var totalRecords = await _context.ContactUs.CountAsync();

            var pagedReponse = PaginationHelper.CreatePagedReponse<ContactUsDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        // GET: api/ContactUs/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetContactUs(int id)
        {
            if (_context.ContactUs == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.ContactUs'  is null." });
            }
            var ContactUs = await _context.ContactUs.FindAsync(id);

            if (ContactUs == null)
            {
                return NotFound(new { message = "ContactUs not found" });
            }

            ContactUsDto data = _mapper.Map<ContactUsDto>(ContactUs);
            return Ok(new Wrappers.ApiResponse<ContactUsDto>(data));

        }

        // PUT: api/ContactUs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContactUs(int id, ContactUsDto ContactUs)
        {
            if (id != ContactUs.Id)
            {
                return BadRequest(new { message = "Invalid ContactUs Id" });
            }


            ContactUs data = new ContactUs
            {
                Id = ContactUs.Id,
                Name = ContactUs.Name,
                Message = ContactUs.Message,
                Email = ContactUs.Email,
                Read = ContactUs.Read,
                ReadBy = ContactUs.ReadBy,
                ReadAt = ContactUs.ReadAt,
                UpdatedAt = DateTime.Now,

            };
            _context.Entry(data).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/ContactUs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostContactUs(ContactUsDto ContactUs)
        {
            if (_context.ContactUs == null)
            {
                return Problem("Entity set 'DatabaseContext.ContactUss'  is null.");
            }


            ContactUs data = new ContactUs
            {
                Id = ContactUs.Id,
                Name = ContactUs.Name,
                Message = ContactUs.Message,
                Email = ContactUs.Email,
                Read = ContactUs.Read,
                ReadBy = ContactUs.ReadBy,
                ReadAt = ContactUs.ReadAt,
                UpdatedAt = DateTime.Now,
                CreatedAt = DateTime.Now
            };


            _context.ContactUs.Add(data);
            await _context.SaveChangesAsync();
            ContactUs.Id = data.Id;
            return CreatedAtAction("GetContactUs", new { id = data.Id }, ContactUs);
        }

        // DELETE: api/ContactUs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContactUs(int id)
        {
            if (_context.ContactUs == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.ContactUs'  is null." });
            }
            var ContactUs = await _context.ContactUs.FindAsync(id);
            if (ContactUs == null)
            {
                return NotFound(new { message = "ContactUs not found" });
            }

            _context.ContactUs.Remove(ContactUs);
            await _context.SaveChangesAsync();

            return NoContent();
        }



    }
}
