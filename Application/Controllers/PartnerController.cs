using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dotnetbase.Application.Database;
using dotnetbase.Application.Filter;
using dotnetbase.Application.Helpers;
using dotnetbase.Application.Models;
using dotnetbase.Application.Services;
using dotnetbase.Application.Services.Auth;
using dotnetbase.Application.ViewModels;

namespace dotnetbase.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartnerController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUriService uriService;
        private readonly IMapper _mapper;

        private readonly IWebHostEnvironment _env;

        private readonly IConfiguration _config;
        private readonly UsersService _usersService;

        private readonly CodeGenService _codeGenService;


        private readonly string _imagePath;

        public PartnerController(DatabaseContext context, CodeGenService codeGenService, IUriService uriService, IMapper mapper, IWebHostEnvironment env, IConfiguration configuration, UsersService usersService)
        {
            _context = context;
            this.uriService = uriService;
            _mapper = mapper;
            _env = env;
            _config = configuration;
            _usersService = usersService;
            _codeGenService = codeGenService;
            _imagePath = _config.GetValue<string>("IMAGE_STORAGE_PATH") ?? "";
        }

        // GET: api/Partner
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> GetPartners([FromQuery] PaginationFilter filter)
        {
            if (_context.Partners == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Partners'  is null." });
            }

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.Partners
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).AsNoTracking().Select(x => new PartnerDto
               {
                   Id = x.Id,
                   CompanyName = x.CompanyName,
                   Email = x.Email,
                   PhoneNumber = x.PhoneNumber,
                   IdTypeId = x.IdTypeId,
                   IdNumber = x.IdNumber,
                   Logo = x.Logo,
                   RegStatusId = x.RegStatusId,
                   IdTypeName = x.IdType.Name,
                   RegStatusName = x.RegStatus.Name,
                   Rating = x.Rating,
                   Code = x.Code,
               })
               .ToListAsync();
            var totalRecords = await _context.OrderAssignments.CountAsync();


            var pagedReponse = PaginationHelper.CreatePagedReponse<PartnerDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        // GET: api/Partner/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Partner>> GetPartner(int id)
        {
            if (_context.Partners == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Partners'  is null." });
            }
            var Partner = await _context.Partners.FindAsync(id);

            if (Partner == null)
            {
                return NotFound(new { message = "Partner not found" });
            }



            PartnerDto data = _mapper.Map<PartnerDto>(Partner);
            return Ok(new Wrappers.ApiResponse<PartnerDto>(data));
        }

        // PUT: api/Partner/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutPartner(int id, PartnerDto Partner)
        {
            if (id != Partner.Id)
            {
                return BadRequest(new { message = "Invalid Partner Id" });
            }


            Partner data = _mapper.Map<Partner>(Partner);
            _context.Entry(data).State = EntityState.Modified;




            await _context.SaveChangesAsync();


            return NoContent();
        }

        // POST: api/Partner
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostPartner(PartnerDto Partner)
        {
            if (_context.Partners == null)
            {
                return Problem("Entity set 'DatabaseContext.Partners'  is null.");
            }

            Partner data = _mapper.Map<Partner>(Partner);
            _context.Partners.Add(data);


            await _context.SaveChangesAsync();


            return CreatedAtAction("GetPartner", new { id = data.Id }, _mapper.Map<IdTypeDto>(data));
        }

        // DELETE: api/Partner/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeletePartner(int id)
        {
            if (_context.Partners == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Partners'  is null." });
            }
            var Partner = await _context.Partners.FindAsync(id);
            if (Partner == null)
            {
                return NotFound(new { message = "Partner not found" });
            }

            _context.Partners.Remove(Partner);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PartnerExists(PartnerDto partnerDto)

        {
            return (_context.Partners?.Any(e => e.Email == partnerDto.Email && e.Id != partnerDto.Id)).GetValueOrDefault();
        }

        [HttpGet("Email/{email}")]
        [Authorize]
        public async Task<ActionResult<PartnerDto>> GetPartnerFullByEmail(string email)
        {
            if (_context.Partners == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Partners'  is null." });
            }

            Partner? Partner = await _context.Partners.FirstOrDefaultAsync(x => x.Email == email);

            if (Partner == null)
            {
                return NotFound(new { message = "Partner not found" });
            }

            PartnerDto data = new PartnerDto
            {
                Id = Partner.Id,
                CompanyName = Partner.CompanyName,
                Email = Partner.Email,
                PhoneNumber = Partner.PhoneNumber,
                IdTypeId = Partner.IdTypeId,
                IdNumber = Partner.IdNumber,
                Logo = Partner.Logo,
                RegStatusId = Partner.RegStatusId,
                IdTypeName = Partner.IdType?.Name,
                RegStatusName = Partner.RegStatus?.Name,
                Code = Partner.Code,
                ContactFirstName = Partner.ContactFirstName,
                ContactLastName = Partner.ContactLastName,
                Rating = Partner.Rating

            };


            PartnerAddress? PartnerAddress = await _context.PartnerAddresses.FirstOrDefaultAsync(x => x.PartnerId == Partner.Id);
            PartnerAddressDto PartnerAddressDto = _mapper.Map<PartnerAddressDto>(PartnerAddress);

            PartnerGeoLocation? PartnerGeoLocation = await _context.PartnerGeoLocations.FirstOrDefaultAsync(x => x.PartnerId == Partner.Id);
            PartnerGeoLocationDto PartnerGeoLocationDto = _mapper.Map<PartnerGeoLocationDto>(PartnerGeoLocation);

            PartnerFullDto PartnerFullDto = new PartnerFullDto
            {
                partnerAddressDto = PartnerAddressDto,
                partnerDto = data,
                partnerGeoLocationDto = PartnerGeoLocationDto
            };


            return Ok(new Wrappers.ApiResponse<PartnerFullDto>(PartnerFullDto));
        }

        [HttpGet("ClientEmail/{email}")]
        [Authorize]
        public async Task<ActionResult> GetPartnersByClient(string email, [FromQuery] PaginationFilter filter)
        {
            if (_context.Partners == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Partners'  is null." });
            }


            var client = await _context.Clients.Include(x => x.ClientAddresses).FirstOrDefaultAsync(x => x.Email == email);
            if (client == null)
            {
                return NotFound(new { message = "Client not found" });
            }

            var clientCity = client.ClientAddresses.FirstOrDefault()?.City;

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.Partners.Include(x => x.PartnerAddresses).Where(x => x.PartnerAddresses.Any(y => y.City == clientCity))
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).AsNoTracking().Select(x => new PartnerDto
               {
                   Id = x.Id,
                   CompanyName = x.CompanyName,
                   Email = x.Email,
                   PhoneNumber = x.PhoneNumber,
                   ContactFirstName = x.ContactFirstName,
                   ContactLastName = x.ContactLastName,
                   Code = x.Code,
                   IdTypeId = x.IdTypeId,
                   IdNumber = x.IdNumber,
                   Logo = x.Logo,
                   RegStatusId = x.RegStatusId,
                   IdTypeName = x.IdType.Name,
                   RegStatusName = x.RegStatus.Name,
                   AddressLine = x.PartnerAddresses.Any() ? x.PartnerAddresses.FirstOrDefault().AddressLine : "",
                   City = x.PartnerAddresses.FirstOrDefault().City,
                   StateProvince = x.PartnerAddresses.FirstOrDefault().StateProvince,
                   CountryRegion = x.PartnerAddresses.FirstOrDefault().CountryRegion,
                   PostalCode = x.PartnerAddresses.FirstOrDefault().PostalCode,
                   Rating = x.Rating
               })

               .ToListAsync();
            var totalRecords = await _context.OrderAssignments.CountAsync();

            var pagedReponse = PaginationHelper.CreatePagedReponse<PartnerDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }


        [HttpGet("Full/{id}")]
        [Authorize]
        public async Task<ActionResult<PartnerDto>> GetPartnerFull(int id)
        {
            if (_context.Partners == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Partners'  is null." });
            }

            Partner? Partner = await _context.Partners.FindAsync(id);

            if (Partner == null)
            {
                return NotFound(new { message = "Partner not found" });
            }

            PartnerDto data = new PartnerDto
            {
                Id = Partner.Id,
                CompanyName = Partner.CompanyName,
                Email = Partner.Email,
                PhoneNumber = Partner.PhoneNumber,
                IdTypeId = Partner.IdTypeId,
                IdNumber = Partner.IdNumber,
                Logo = Partner.Logo,
                RegStatusId = Partner.RegStatusId,
                IdTypeName = Partner.IdType?.Name,
                RegStatusName = Partner.RegStatus?.Name,
                Code = Partner.Code,
                ContactFirstName = Partner.ContactFirstName,
                ContactLastName = Partner.ContactLastName,
                Rating = Partner.Rating
            };


            PartnerAddress? PartnerAddress = await _context.PartnerAddresses.FirstOrDefaultAsync(x => x.PartnerId == Partner.Id);
            PartnerAddressDto PartnerAddressDto = _mapper.Map<PartnerAddressDto>(PartnerAddress);

            PartnerGeoLocation? PartnerGeoLocation = await _context.PartnerGeoLocations.FirstOrDefaultAsync(x => x.PartnerId == Partner.Id);
            PartnerGeoLocationDto PartnerGeoLocationDto = _mapper.Map<PartnerGeoLocationDto>(PartnerGeoLocation);

            PartnerFullDto PartnerFullDto = new PartnerFullDto
            {
                partnerAddressDto = PartnerAddressDto,
                partnerDto = data,
                partnerGeoLocationDto = PartnerGeoLocationDto
            };


            return Ok(new Wrappers.ApiResponse<PartnerFullDto>(PartnerFullDto));
        }



        [HttpPut("Full/{id}")]
        [Consumes("multipart/form-data")]
        [Authorize]
        public async Task<IActionResult> PutPartnerFull(int id, [FromForm] PartnerFullDto Partner)
        {
            if (_context.Partners == null)
            {
                return Problem("Entity set 'DatabaseContext.Partners'  is null.");
            }

            if (Partner.partnerDto == null)
            {
                return NotFound(new { message = "Partner is null" });
            }

            if (Partner.partnerAddressDto == null)
            {
                return NotFound(new { message = "Service Provider Address not provided" });
            }

            if (Partner.partnerGeoLocationDto == null)
            {
                return NotFound(new { message = "Service Provider Geo Location not provided" });
            }


            if (id != Partner.partnerDto.Id)
            {
                return BadRequest(new { message = "Invalid Partner Id" });
            }

            //check if partner exist
            if (PartnerExists(Partner.partnerDto))
            {
                return BadRequest(new { message = "Partner with this email already exist" });
            }

            IdType? idType = await _context.IdTypes.FindAsync(Partner.partnerDto.IdTypeId);

            if (idType == null)
            {
                return NotFound(new { message = "Id Type not found" });
            }


            RegStatus? regStatus = await _context.RegStatuses.FindAsync(Partner.partnerDto.RegStatusId);
            if (regStatus == null)
            {
                return NotFound(new { message = "Reg Status not found" });
            }



            Partner data = new Partner
            {
                Id = Partner.partnerDto.Id,
                CompanyName = Partner.partnerDto.CompanyName,
                Email = Partner.partnerDto.Email,
                PhoneNumber = Partner.partnerDto.PhoneNumber,
                IdTypeId = Partner.partnerDto.IdTypeId,
                IdType = idType,
                IdNumber = Partner.partnerDto.IdNumber,
                Logo = Partner.partnerDto.Logo,
                RegStatusId = Partner.partnerDto.RegStatusId,
                RegStatus = regStatus,
                ContactFirstName = Partner.partnerDto.ContactFirstName,
                ContactLastName = Partner.partnerDto.ContactLastName,

                UpdatedAt = DateTime.Now,


            };

            //check if logo is not null, save it and use the file name as part of service provider object
            if (Partner.Image != null)
            {

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(Partner.Image.FileName);
                string filePath = Path.Combine(_env.ContentRootPath, _imagePath, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await Partner.Image.CopyToAsync(fileStream);
                }
                data.Logo = fileName;
            }

            _context.Entry(data).State = EntityState.Modified;

            PartnerAddress PartnerAddress = new PartnerAddress
            {
                Id = Partner.partnerAddressDto.Id,
                PartnerId = data.Id,
                Partner = data,
                City = Partner.partnerAddressDto.City,
                UpdatedAt = DateTime.Now,
                CountryRegion = Partner.partnerAddressDto.CountryRegion,
                PostalCode = Partner.partnerAddressDto.PostalCode,
                StateProvince = Partner.partnerAddressDto.StateProvince,
                Status = Partner.partnerAddressDto.Status,
                AddressLine = Partner.partnerAddressDto.AddressLine,

            };
            _context.Entry(PartnerAddress).State = EntityState.Modified;

            PartnerGeoLocation PartnerGeoLocation = new PartnerGeoLocation
            {
                Id = Partner.partnerGeoLocationDto.Id,
                PartnerId = data.Id,
                Partner = data,

                UpdatedAt = DateTime.Now,

                Latitude = Partner.partnerGeoLocationDto.Latitude,
                Longitude = Partner.partnerGeoLocationDto.Longitude,
                Accuracy = Partner.partnerGeoLocationDto.Accuracy,
                Altitude = Partner.partnerGeoLocationDto.Altitude,
                AltitudeAccuracy = Partner.partnerGeoLocationDto.AltitudeAccuracy,
                Heading = Partner.partnerGeoLocationDto.Heading,
                Speed = Partner.partnerGeoLocationDto.Speed,
            };

            _context.Entry(PartnerGeoLocation).State = EntityState.Modified;


            await _context.SaveChangesAsync();


            return NoContent();
        }

        [HttpPost("Full")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> PostPartnerFull([FromForm] PartnerFullDto Partner)
        {


            if (_context.Partners == null)
            {
                return Problem("Entity set 'DatabaseContext.Partners'  is null.");
            }

            if (Partner.partnerDto == null)
            {
                return NotFound(new { message = "Partner is null" });
            }

            if (Partner.partnerAddressDto == null)
            {
                return NotFound(new { message = "Service Provider Address not provided" });
            }

            if (Partner.partnerGeoLocationDto == null)
            {
                return NotFound(new { message = "Service Provider Geo Location not provided" });
            }

            //check if partner exist
            if (PartnerExists(Partner.partnerDto))
            {
                return BadRequest(new { message = "Partner with this email already exist" });
            }

            IdType? idType = await _context.IdTypes.FindAsync(Partner.partnerDto.IdTypeId);

            if (idType == null)
            {
                return NotFound(new { message = "Id Type not found" });
            }


            RegStatus? regStatus = await _context.RegStatuses.FindAsync(Partner.partnerDto.RegStatusId);
            if (regStatus == null)
            {
                return NotFound(new { message = "Reg Status not found" });
            }




            //check if serviceproviders email exist as a user else create as a user
            ApplicationUser? user = await _usersService.FindUserByEmailAsync(Partner.partnerDto.Email);
            if (user == null)
            {
                if (Partner.Password == null)
                {
                    return BadRequest(new
                    {
                        message = "Password is required"
                    });


                }
                //create user
                Register register = new Register
                {
                    Email = Partner.partnerDto.Email,
                    Password = Partner.Password,
                    Name = Partner.partnerDto.CompanyName

                };
                await _usersService.CreateUserAsync(register);
            }



            Models.Partner data = new Models.Partner
            {
                Id = Partner.partnerDto.Id,
                CompanyName = Partner.partnerDto.CompanyName,
                Email = Partner.partnerDto.Email,
                PhoneNumber = Partner.partnerDto.PhoneNumber,
                IdTypeId = Partner.partnerDto.IdTypeId,
                IdType = idType,
                IdNumber = Partner.partnerDto.IdNumber,
                Logo = Partner.partnerDto.Logo,
                RegStatusId = Partner.partnerDto.RegStatusId,
                RegStatus = regStatus,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                ContactFirstName = Partner.partnerDto.ContactFirstName,
                ContactLastName = Partner.partnerDto.ContactLastName,
                Code = _codeGenService.GenerateClientCode().Value,
                Rating = 100m
            };

            //check if logo is not null, save it and use the file name as part of service provider object
            if (Partner.Image != null)
            {

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(Partner.Image.FileName);
                string filePath = Path.Combine(_env.ContentRootPath, _imagePath, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await Partner.Image.CopyToAsync(fileStream);
                }
                data.Logo = fileName;
            }

            _context.IdTypes.Attach(data.IdType);
            _context.RegStatuses.Attach(data.RegStatus);
            _context.Partners.Add(data);
            await _context.SaveChangesAsync();
            Partner.partnerDto.Id = data.Id;

            PartnerAddress PartnerAddress = new PartnerAddress
            {
                PartnerId = data.Id,
                Partner = data,
                City = Partner.partnerAddressDto.City,

                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                CountryRegion = Partner.partnerAddressDto.CountryRegion,
                PostalCode = Partner.partnerAddressDto.PostalCode,
                StateProvince = Partner.partnerAddressDto.StateProvince,
                Status = Partner.partnerAddressDto.Status,
                AddressLine = Partner.partnerAddressDto.AddressLine,

            };
            _context.Partners.Attach(data);
            _context.PartnerAddresses.Add(PartnerAddress);

            PartnerGeoLocation PartnerGeoLocation = new PartnerGeoLocation
            {
                PartnerId = data.Id,
                Partner = data,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,

                Latitude = Partner.partnerGeoLocationDto.Latitude,
                Longitude = Partner.partnerGeoLocationDto.Longitude,
                Accuracy = Partner.partnerGeoLocationDto.Accuracy,
                Altitude = Partner.partnerGeoLocationDto.Altitude,
                AltitudeAccuracy = Partner.partnerGeoLocationDto.AltitudeAccuracy,
                Heading = Partner.partnerGeoLocationDto.Heading,
                Speed = Partner.partnerGeoLocationDto.Speed,
            };

            _context.PartnerGeoLocations.Add(PartnerGeoLocation);
            await _context.SaveChangesAsync();



            return CreatedAtAction("GetPartner", new { id = data.Id }, Partner.partnerDto);
        }

    }
}
