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
    public class PartnerAddressController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUriService uriService;
        private readonly IMapper _mapper;

        public PartnerAddressController(DatabaseContext context, IUriService uriService, IMapper mapper)
        {
            _context = context;
            this.uriService = uriService;
            _mapper = mapper;
        }

        // GET: api/PartnerAddress
        [HttpGet]
        public async Task<ActionResult> GetPartnerAddresss([FromQuery] PaginationFilter filter)
        {
            if (_context.PartnerAddresses == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.PartnerAddresss'  is null." });
            }
            // return await _context.PartnerAddresss.ToListAsync();
            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.PartnerAddresses
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).AsNoTracking()
               .ToListAsync();
            var totalRecords = await _context.OrderAssignments.CountAsync();
            List<PartnerAddressDto> ilistDest = _mapper.Map<List<PartnerAddress>, List<PartnerAddressDto>>(pagedData);

            var pagedReponse = PaginationHelper.CreatePagedReponse<PartnerAddressDto>(ilistDest, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        // GET: api/PartnerAddress/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PartnerAddress>> GetPartnerAddress(int id)
        {
            if (_context.PartnerAddresses == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.PartnerAddresss'  is null." });
            }
            var PartnerAddress = await _context.PartnerAddresses.FindAsync(id);

            if (PartnerAddress == null)
            {
                return NotFound(new { message = "PartnerAddress not found." });
            }



            PartnerAddressDto data = _mapper.Map<PartnerAddressDto>(PartnerAddress);
            return Ok(new Wrappers.ApiResponse<PartnerAddressDto>(data));
        }

        // PUT: api/PartnerAddress/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPartnerAddress(int id, PartnerAddressDto PartnerAddress)
        {
            if (id != PartnerAddress.Id)
            {
                return BadRequest(new { message = "Invalid PartnerAddress Id." });
            }


            PartnerAddress data = _mapper.Map<PartnerAddress>(PartnerAddress);
            _context.Entry(data).State = EntityState.Modified;




            await _context.SaveChangesAsync();


            return NoContent();
        }

        // POST: api/PartnerAddress
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostPartnerAddress(PartnerAddressDto PartnerAddress)
        {
            if (_context.PartnerAddresses == null)
            {
                return Problem("Entity set 'DatabaseContext.PartnerAddresss'  is null.");
            }

            PartnerAddress data = _mapper.Map<PartnerAddress>(PartnerAddress);
            _context.PartnerAddresses.Add(data);


            await _context.SaveChangesAsync();


            return CreatedAtAction("GetPartnerAddress", new { id = data.Id }, _mapper.Map<IdTypeDto>(data));
        }

        // DELETE: api/PartnerAddress/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePartnerAddress(int id)
        {
            if (_context.PartnerAddresses == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.PartnerAddresss'  is null." });
            }
            var PartnerAddress = await _context.PartnerAddresses.FindAsync(id);
            if (PartnerAddress == null)
            {
                return NotFound(new { message = "PartnerAddress not found." });
            }

            _context.PartnerAddresses.Remove(PartnerAddress);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PartnerAddressExists(int id)
        {
            return (_context.PartnerAddresses?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
