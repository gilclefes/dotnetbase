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
    public class ClientAddressController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUriService uriService;
        private readonly IMapper _mapper;

        public ClientAddressController(DatabaseContext context, IUriService uriService, IMapper mapper)
        {
            _context = context;
            this.uriService = uriService;
            _mapper = mapper;
        }

        // GET: api/ClientAddress
        [HttpGet]
        public async Task<ActionResult> GetClientAddress([FromQuery] PaginationFilter filter)
        {
            if (_context.ClientAddress == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.ClientAddress'  is null." });
            }


            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.ClientAddress
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).AsNoTracking()
               .ToListAsync();
            var totalRecords = await _context.ClientAddress.CountAsync();
            List<ClientAddressDto> ilistDest = _mapper.Map<List<ClientAddress>, List<ClientAddressDto>>(pagedData);

            var pagedReponse = PaginationHelper.CreatePagedReponse<ClientAddressDto>(ilistDest, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        // GET: api/ClientAddress/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetClientAddress(int id)
        {
            if (_context.ClientAddress == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.ClientAddress'  is null." });
            }
            var clientAddress = await _context.ClientAddress.FindAsync(id);

            if (clientAddress == null)
            {
                return NotFound(new { message = "ClientAddress not found" });
            }



            ClientAddressDto data = _mapper.Map<ClientAddressDto>(clientAddress);
            return Ok(new Wrappers.ApiResponse<ClientAddressDto>(data));
        }

        // PUT: api/ClientAddress/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClientAddress(int id, ClientAddressDto clientAddress)
        {
            if (id != clientAddress.Id)
            {
                return BadRequest(new { message = "Invalid ClientAddress Id" });
            }


            ClientAddress data = _mapper.Map<ClientAddress>(clientAddress);
            _context.Entry(data).State = EntityState.Modified;


            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/ClientAddress
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostClientAddress(ClientAddressDto clientAddress)
        {
            if (_context.ClientAddress == null)
            {
                return Problem("Entity set 'DatabaseContext.ClientAddress'  is null.");
            }

            ClientAddress data = _mapper.Map<ClientAddress>(clientAddress);
            _context.ClientAddress.Add(data);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetClientAddress", new { id = data.Id }, _mapper.Map<ClientAddressDto>(data));
        }

        // DELETE: api/ClientAddress/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClientAddress(int id)
        {
            if (_context.ClientAddress == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.ClientAddress'  is null." });
            }
            var clientAddress = await _context.ClientAddress.FindAsync(id);
            if (clientAddress == null)
            {
                return NotFound(new { message = "ClientAddress not found" });
            }

            _context.ClientAddress.Remove(clientAddress);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClientAddressExists(int id)
        {
            return (_context.ClientAddress?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
