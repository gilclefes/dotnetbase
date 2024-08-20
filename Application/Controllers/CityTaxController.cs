using AutoMapper;
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
    public class CityTaxController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUriService uriService;
        private readonly IMapper _mapper;

        public CityTaxController(DatabaseContext context, IUriService uriService, IMapper mapper)
        {
            _context = context;
            this.uriService = uriService;
            _mapper = mapper;
        }

        // GET: api/CityTax
        [HttpGet]
        public async Task<ActionResult> GetCityTaxes([FromQuery] PaginationFilter filter)
        {
            if (_context.Currencies == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Currencies'  is null." });
            }


            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.CityTaxes
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).AsNoTracking()
               .ToListAsync();
            var totalRecords = await _context.OperatingCities.CountAsync();
            List<CityTaxDto> ilistDest = _mapper.Map<List<CityTax>, List<CityTaxDto>>(pagedData);

            var pagedReponse = PaginationHelper.CreatePagedReponse<CityTaxDto>(ilistDest, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        // GET: api/CityTax/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetCityTax(int id)
        {
            if (_context.Currencies == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Currencies'  is null." });
            }
            var CityTax = await _context.CityTaxes.FindAsync(id);

            if (CityTax == null)
            {
                return NotFound(new { message = "CityTax not found" });
            }

            // return CityTax;
            CityTaxDto data = _mapper.Map<CityTaxDto>(CityTax);
            return Ok(new Wrappers.ApiResponse<CityTaxDto>(data));
        }

        // GET: api/CityTax/5
        [HttpGet("client/{email}")]
        public async Task<ActionResult> GetClientCityTax(string email)
        {
            if (_context.Currencies == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Currencies'  is null." });
            }

            Client? client = await _context.Clients.Include(x => x.ClientAddresses).FirstOrDefaultAsync(x => x.Email == email);
            if (client == null)
            {
                return NotFound(new { message = "Client not found" });
            }

            string cityname = client.ClientAddresses?.FirstOrDefault()?.City ?? string.Empty;
            CityTax? cityTax = await _context.CityTaxes.Include(x => x.City).FirstOrDefaultAsync(x => x.City.Name == cityname);


            if (cityTax == null)
            {
                cityTax = new CityTax
                {
                    TaxPercentage = 0,
                    CityId = 0,
                    TaxName = "No Tax",
                    Status = true
                    ,
                    City = new City
                    {
                        Name = "No City",
                        Code = "000",
                        CountryId = 0
                    }
                };
            }

            // return CityTax;
            CityTaxDto data = _mapper.Map<CityTaxDto>(cityTax);
            return Ok(new Wrappers.ApiResponse<CityTaxDto>(data));
        }




        // PUT: api/CityTax/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCityTax(int id, CityTaxDto CityTax)
        {
            if (id != CityTax.Id)
            {
                return BadRequest(new { message = "Invalid CityTax Id" });
            }

            if (CityTaxExists(CityTax))
            {
                return BadRequest(new { message = "Tax already setup for the selected city" });
            }

            City? city = await _context.Cities.FindAsync(CityTax.CityId);

            if (city == null)
            {
                return NotFound(new { message = "City not found" });
            }


            CityTax data = new CityTax
            {
                City = city,
                TaxName = CityTax.TaxName,
                TaxPercentage = CityTax.TaxPercentage,
                CityId = city.Id,
                Id = CityTax.Id,
                Status = CityTax.Status,
                UpdatedAt = DateTime.Now

            };
            _context.Entry(data).State = EntityState.Modified;


            await _context.SaveChangesAsync();


            return NoContent();
        }

        // POST: api/CityTax
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CityTax>> PostCityTax(CityTaxDto CityTax)
        {
            if (_context.Currencies == null)
            {
                return Problem("Entity set 'DatabaseContext.Currencies'  is null.");
            }


            if (CityTaxExists(CityTax))
            {
                return BadRequest(new { message = "Tax already setup for the selected city" });
            }

            City? city = await _context.Cities.FindAsync(CityTax.CityId);

            if (city == null)
            {
                return NotFound(new { message = "City not found" });
            }


            CityTax data = new CityTax
            {
                City = city,
                TaxName = CityTax.TaxName,
                TaxPercentage = CityTax.TaxPercentage,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                CityId = city.Id,
                Status = CityTax.Status
            };

            _context.Cities.Attach(data.City);
            _context.CityTaxes.Add(data);

            await _context.SaveChangesAsync();
            return CreatedAtAction("GetCityTax", new { id = data.Id }, _mapper.Map<CityTaxDto>(data));


        }




        private bool CityTaxExists(CityTaxDto city)
        {
            return (_context.CityTaxes?.Any(e => (e.CityId == city.CityId) && e.Id != city.Id)).GetValueOrDefault();
        }
    }
}
