using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NuGet.Protocol;
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
    public class ServiceProviderController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUriService uriService;
        private readonly IMapper _mapper;

        private readonly UsersService _usersService;

        private readonly CodeGenService _codeGenService;

        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;
        private readonly string _imagePath;

        public ServiceProviderController(DatabaseContext context, CodeGenService codeGenService, IUriService uriService, IMapper mapper, IWebHostEnvironment env,
           IConfiguration configuration, UsersService usersService)
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

        // GET: api/ServiceProvider
        [HttpGet]
        public async Task<ActionResult> GetServiceProviders([FromQuery] PaginationFilter filter)
        {
            if (_context.ServiceProviders == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.ServiceProviders'  is null." });
            }

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.ServiceProviders
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).AsNoTracking().Select(x => new ServiceProviderDto
               {
                   Id = x.Id,
                   FirstName = x.FirstName,
                   LastName = x.LastName,
                   Email = x.Email,
                   PhoneNumber = x.PhoneNumber,
                   IdTypeId = x.IdTypeId,
                   IdNumber = x.IdNumber,
                   Logo = x.Logo,
                   RegStatusId = x.RegStatusId,
                   IdTypeName = x.IdType.Name,
                   RegStatusName = x.RegStatus.Name,
                   Code = x.Code,
                   Rating = x.Rating
               })
               .ToListAsync();
            var totalRecords = await _context.OrderAssignments.CountAsync();

            var pagedReponse = PaginationHelper.CreatePagedReponse<ServiceProviderDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        [HttpGet("ClientEmail/{email}")]
        public async Task<ActionResult> GetServiceProvidersByClient(string email, [FromQuery] PaginationFilter filter)
        {
            if (_context.ServiceProviders == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.ServiceProviders'  is null." });
            }


            var client = await _context.Clients.Include(x => x.ClientAddresses).FirstOrDefaultAsync(x => x.Email == email);
            if (client == null)
            {
                return NotFound(new { message = "Client not found" });
            }

            var clientCity = client.ClientAddresses.FirstOrDefault()?.City;

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.ServiceProviders.Include(x => x.ServiceProviderAddresses).Where(x => x.ServiceProviderAddresses.Any(y => y.City == clientCity))
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).AsNoTracking().Select(x => new ServiceProviderDto
               {
                   Id = x.Id,
                   FirstName = x.FirstName,
                   LastName = x.LastName,
                   Code = x.Code,
                   Email = x.Email,
                   PhoneNumber = x.PhoneNumber,
                   IdTypeId = x.IdTypeId,
                   IdNumber = x.IdNumber,
                   Logo = x.Logo,
                   RegStatusId = x.RegStatusId,
                   IdTypeName = x.IdType.Name,
                   RegStatusName = x.RegStatus.Name,
                   Rating = x.Rating,
                   AddressLine = x.ServiceProviderAddresses.Any() ? x.ServiceProviderAddresses.FirstOrDefault().AddressLine : "",
                   City = x.ServiceProviderAddresses.FirstOrDefault().City,
                   StateProvince = x.ServiceProviderAddresses.FirstOrDefault().StateProvince,
                   CountryRegion = x.ServiceProviderAddresses.FirstOrDefault().CountryRegion,
                   PostalCode = x.ServiceProviderAddresses.FirstOrDefault().PostalCode,
               })
               .ToListAsync();
            var totalRecords = await _context.OrderAssignments.CountAsync();

            var pagedReponse = PaginationHelper.CreatePagedReponse<ServiceProviderDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        // GET: api/ServiceProvider/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Models.ServiceProvider>> GetServiceProvider(int id)
        {
            if (_context.ServiceProviders == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.ServiceProviders'  is null." });
            }
            var ServiceProvider = await _context.ServiceProviders.FindAsync(id);

            if (ServiceProvider == null)
            {
                return NotFound(new { message = "ServiceProvider not found." });
            }



            ServiceProviderDto data = _mapper.Map<ServiceProviderDto>(ServiceProvider);
            return Ok(new Wrappers.ApiResponse<ServiceProviderDto>(data));
        }

        [HttpGet("Email/{email}")]
        public async Task<ActionResult<dotnetbase.Application.Models.ServiceProvider>> GetServiceProviderFullByEmail(string email)
        {
            if (_context.ServiceProviders == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.ServiceProviders'  is null." });
            }
            var ServiceProvider = await _context.ServiceProviders.FirstOrDefaultAsync(x => x.Email == email);

            if (ServiceProvider == null)
            {
                return NotFound(new { message = "ServiceProvider not found." });
            }


            ServiceProviderDto data = new ServiceProviderDto
            {
                Id = ServiceProvider.Id,
                FirstName = ServiceProvider.FirstName,
                LastName = ServiceProvider.LastName,
                Email = ServiceProvider.Email,
                PhoneNumber = ServiceProvider.PhoneNumber,
                IdTypeId = ServiceProvider.IdTypeId,
                IdNumber = ServiceProvider.IdNumber,
                Logo = ServiceProvider.Logo,
                RegStatusId = ServiceProvider.RegStatusId,
                IdTypeName = ServiceProvider.IdType?.Name,
                RegStatusName = ServiceProvider.RegStatus?.Name,
                Code = ServiceProvider.Code,
                Rating = ServiceProvider.Rating
            };

            ServiceProviderAddress? serviceProviderAddress = await _context.ServiceProviderAddresses.FirstOrDefaultAsync(x => x.ServiceProviderId == ServiceProvider.Id);
            ServiceProviderAddressDto serviceProviderAddressDto = _mapper.Map<ServiceProviderAddressDto>(serviceProviderAddress);

            ServiceProviderGeoLocation? serviceProviderGeoLocation = await _context.ServiceProviderGeoLocations.FirstOrDefaultAsync(x => x.ServiceProviderId == ServiceProvider.Id);
            ServiceProviderGeoLocationDto serviceProviderGeoLocationDto = _mapper.Map<ServiceProviderGeoLocationDto>(serviceProviderGeoLocation);

            ServiceProviderFullDto serviceProviderFullDto = new ServiceProviderFullDto
            {
                ServiceProviderAddressDto = serviceProviderAddressDto,
                ServiceProviderDto = data,
                ServiceProviderGeoLocationDto = serviceProviderGeoLocationDto
            };


            return Ok(new Wrappers.ApiResponse<ServiceProviderFullDto>(serviceProviderFullDto));
        }

        [HttpGet("Full/{id}")]
        public async Task<ActionResult<ServiceProviderFullDto>> GetServiceProviderFull(int id)
        {
            if (_context.ServiceProviders == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.ServiceProviders'  is null." });
            }

            Models.ServiceProvider? serviceProvider = await _context.ServiceProviders.FindAsync(id);

            if (serviceProvider == null)
            {
                return NotFound(new { message = "ServiceProvider not found." });
            }

            ServiceProviderDto data = new ServiceProviderDto
            {
                Id = serviceProvider.Id,
                FirstName = serviceProvider.FirstName,
                LastName = serviceProvider.LastName,
                Email = serviceProvider.Email,
                PhoneNumber = serviceProvider.PhoneNumber,
                IdTypeId = serviceProvider.IdTypeId,
                IdNumber = serviceProvider.IdNumber,
                Logo = serviceProvider.Logo,
                RegStatusId = serviceProvider.RegStatusId,
                IdTypeName = serviceProvider.IdType?.Name,
                RegStatusName = serviceProvider.RegStatus?.Name,
                Code = serviceProvider.Code,
                Rating = serviceProvider.Rating

            };

            ServiceProviderAddress? serviceProviderAddress = await _context.ServiceProviderAddresses.FirstOrDefaultAsync(x => x.ServiceProviderId == serviceProvider.Id);
            ServiceProviderAddressDto serviceProviderAddressDto;
            if (serviceProviderAddress == null)
            {
                serviceProviderAddressDto = new ServiceProviderAddressDto();
            }
            else
            {
                serviceProviderAddressDto = new ServiceProviderAddressDto
                {
                    Id = serviceProviderAddress.Id,
                    ServiceProviderId = serviceProviderAddress.ServiceProviderId,
                    City = serviceProviderAddress.City,
                    CountryRegion = serviceProviderAddress.CountryRegion,
                    PostalCode = serviceProviderAddress.PostalCode,
                    StateProvince = serviceProviderAddress.StateProvince,
                    Status = serviceProviderAddress.Status,
                    AddressLine = serviceProviderAddress.AddressLine,
                };
            }


            ServiceProviderGeoLocation? serviceProviderGeoLocation = await _context.ServiceProviderGeoLocations.FirstOrDefaultAsync(x => x.ServiceProviderId == serviceProvider.Id);
            ServiceProviderGeoLocationDto serviceProviderGeoLocationDto;

            if (serviceProviderGeoLocation == null)
            {
                serviceProviderGeoLocationDto = new ServiceProviderGeoLocationDto();
            }
            else
            {
                serviceProviderGeoLocationDto = new ServiceProviderGeoLocationDto
                {
                    Id = serviceProviderGeoLocation.Id,
                    ServiceProviderId = serviceProviderGeoLocation.ServiceProviderId,
                    Latitude = serviceProviderGeoLocation.Latitude,
                    Longitude = serviceProviderGeoLocation.Longitude,
                    Accuracy = serviceProviderGeoLocation.Accuracy,
                    Altitude = serviceProviderGeoLocation.Altitude,
                    AltitudeAccuracy = serviceProviderGeoLocation.AltitudeAccuracy,
                    Heading = serviceProviderGeoLocation.Heading,
                    Speed = serviceProviderGeoLocation.Speed,
                };
            }
            serviceProviderGeoLocationDto = _mapper.Map<ServiceProviderGeoLocationDto>(serviceProviderGeoLocation);

            ServiceProviderFullDto serviceProviderFullDto = new ServiceProviderFullDto
            {
                ServiceProviderAddressDto = serviceProviderAddressDto,
                ServiceProviderDto = data,
                ServiceProviderGeoLocationDto = serviceProviderGeoLocationDto
            };


            return Ok(new Wrappers.ApiResponse<ServiceProviderFullDto>(serviceProviderFullDto));
        }

        // PUT: api/ServiceProvider/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutServiceProvider(int id, ServiceProviderDto ServiceProvider)
        {
            if (id != ServiceProvider.Id)
            {
                return BadRequest(new { message = "Invalid ServiceProvider Id." });
            }

            if (ServiceProviderExists(ServiceProvider))
            {
                return BadRequest(new { message = "Email already exist." });
            }


            Models.ServiceProvider data = _mapper.Map<Models.ServiceProvider>(ServiceProvider);
            _context.Entry(data).State = EntityState.Modified;

            await _context.SaveChangesAsync();


            return NoContent();
        }


        [HttpPut("Full/{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> PutServiceProviderFull(int id, [FromForm] ServiceProviderFullDto serviceProvider)
        {
            if (_context.ServiceProviders == null)
            {
                return Problem("Entity set 'DatabaseContext.ServiceProviders'  is null.");
            }

            if (serviceProvider.ServiceProviderDto == null)
            {
                return NotFound(new { message = "ServiceProviderDto is null." });
            }

            if (serviceProvider.ServiceProviderAddressDto == null)
            {
                return NotFound(new { message = "Service Provider Address not provided." });
            }

            if (serviceProvider.ServiceProviderGeoLocationDto == null)
            {
                return NotFound(new { message = "Service Provider Geo Location not provided." });
            }


            if (id != serviceProvider.ServiceProviderDto.Id)
            {
                return BadRequest(new { message = "Invalid ServiceProvider Id." });
            }

            if (ServiceProviderExists(serviceProvider.ServiceProviderDto))
            {
                return BadRequest(new { message = "Email already exist." });
            }

            IdType? idType = await _context.IdTypes.FindAsync(serviceProvider.ServiceProviderDto.IdTypeId);

            if (idType == null)
            {
                return NotFound(new { message = "Id Type not found." });
            }


            RegStatus? regStatus = await _context.RegStatuses.FindAsync(serviceProvider.ServiceProviderDto.RegStatusId);
            if (regStatus == null)
            {
                return NotFound(new { message = "Reg Status not found." });
            }



            Models.ServiceProvider data = new Models.ServiceProvider
            {
                Id = serviceProvider.ServiceProviderDto.Id,
                FirstName = serviceProvider.ServiceProviderDto.FirstName,
                LastName = serviceProvider.ServiceProviderDto.LastName,
                Email = serviceProvider.ServiceProviderDto.Email,
                PhoneNumber = serviceProvider.ServiceProviderDto.PhoneNumber,
                IdTypeId = serviceProvider.ServiceProviderDto.IdTypeId,
                IdType = idType,
                IdNumber = serviceProvider.ServiceProviderDto.IdNumber,
                Logo = serviceProvider.ServiceProviderDto.Logo,
                RegStatusId = serviceProvider.ServiceProviderDto.RegStatusId,
                RegStatus = regStatus,


                UpdatedAt = DateTime.Now,



            };

            //check if logo is not null, save it and use the file name as part of service provider object
            if (serviceProvider.Image != null)
            {

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(serviceProvider.Image.FileName);
                string filePath = Path.Combine(_env.ContentRootPath, _imagePath, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await serviceProvider.Image.CopyToAsync(fileStream);
                }
                data.Logo = fileName;
            }

            _context.Entry(data).State = EntityState.Modified;

            ServiceProviderAddress serviceProviderAddress = new ServiceProviderAddress
            {
                Id = serviceProvider.ServiceProviderAddressDto.Id,
                ServiceProviderId = data.Id,
                ServiceProvider = data,
                City = serviceProvider.ServiceProviderAddressDto.City,
                UpdatedAt = DateTime.Now,
                CountryRegion = serviceProvider.ServiceProviderAddressDto.CountryRegion,
                PostalCode = serviceProvider.ServiceProviderAddressDto.PostalCode,
                StateProvince = serviceProvider.ServiceProviderAddressDto.StateProvince,
                Status = serviceProvider.ServiceProviderAddressDto.Status,
                AddressLine = serviceProvider.ServiceProviderAddressDto.AddressLine,

            };
            _context.Entry(serviceProviderAddress).State = EntityState.Modified;

            ServiceProviderGeoLocation serviceProviderGeoLocation = new ServiceProviderGeoLocation
            {
                Id = serviceProvider.ServiceProviderGeoLocationDto.Id,
                ServiceProviderId = data.Id,
                ServiceProvider = data,

                UpdatedAt = DateTime.Now,

                Latitude = serviceProvider.ServiceProviderGeoLocationDto.Latitude,
                Longitude = serviceProvider.ServiceProviderGeoLocationDto.Longitude,
                Accuracy = serviceProvider.ServiceProviderGeoLocationDto.Accuracy,
                Altitude = serviceProvider.ServiceProviderGeoLocationDto.Altitude,
                AltitudeAccuracy = serviceProvider.ServiceProviderGeoLocationDto.AltitudeAccuracy,
                Heading = serviceProvider.ServiceProviderGeoLocationDto.Heading,
                Speed = serviceProvider.ServiceProviderGeoLocationDto.Speed,
            };

            _context.Entry(serviceProviderGeoLocation).State = EntityState.Modified;


            await _context.SaveChangesAsync();


            return NoContent();
        }



        // POST: api/ServiceProvider
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostServiceProvider(ServiceProviderDto ServiceProvider)
        {
            if (_context.ServiceProviders == null)
            {
                return Problem("Entity set 'DatabaseContext.ServiceProviders'  is null.");
            }


            IdType? idType = await _context.IdTypes.FindAsync(ServiceProvider.IdTypeId);

            if (idType == null)
            {
                return NotFound(new { message = "Id Type not found." });
            }

            if (ServiceProviderExists(ServiceProvider))
            {
                return BadRequest(new { message = "Email already exist." });
            }


            RegStatus? regStatus = await _context.RegStatuses.FindAsync(ServiceProvider.RegStatusId);
            if (regStatus == null)
            {
                return NotFound(new { message = "Reg Status not found." });
            }

            Models.ServiceProvider data = new Models.ServiceProvider
            {
                Id = ServiceProvider.Id,
                FirstName = ServiceProvider.FirstName,
                LastName = ServiceProvider.LastName,
                Email = ServiceProvider.Email,
                PhoneNumber = ServiceProvider.PhoneNumber,
                IdTypeId = ServiceProvider.IdTypeId,
                IdType = idType,
                IdNumber = ServiceProvider.IdNumber,
                Logo = ServiceProvider.Logo,
                RegStatusId = ServiceProvider.RegStatusId,
                RegStatus = regStatus,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Rating = 100

            };

            _context.IdTypes.Attach(data.IdType);
            _context.RegStatuses.Attach(data.RegStatus);
            _context.ServiceProviders.Add(data);


            await _context.SaveChangesAsync();


            return CreatedAtAction("GetServiceProvider", new
            {
                id = data.Id
            }, _mapper.Map<IdTypeDto>(data));
        }

        [HttpPost("Full")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> PostServiceProviderFull([FromForm] ServiceProviderFullDto serviceProvider)
        {
            //Console.WriteLine("PostServiceProviderFull" + serviceProvider.ToJson());
            if (_context.ServiceProviders == null)
            {
                return Problem("Entity set 'DatabaseContext.ServiceProviders'  is null.");
            }

            if (serviceProvider.ServiceProviderDto == null)
            {
                return NotFound(new { message = "ServiceProviderDto is null." });
            }

            if (serviceProvider.ServiceProviderAddressDto == null)
            {
                return NotFound(new { message = "Service Provider Address not provided." });
            }

            if (serviceProvider.ServiceProviderGeoLocationDto == null)
            {
                return NotFound(new { message = "Service Provider Geo Location not provided." });
            }

            //check if email already exist
            if (ServiceProviderExists(serviceProvider.ServiceProviderDto))
            {
                return BadRequest(new { message = "Email already exist." });
            }

            IdType? idType = await _context.IdTypes.FindAsync(serviceProvider.ServiceProviderDto.IdTypeId);

            if (idType == null)
            {
                return NotFound(new { message = "Id Type not found." });
            }


            RegStatus? regStatus = await _context.RegStatuses.FindAsync(serviceProvider.ServiceProviderDto.RegStatusId);
            if (regStatus == null)
            {
                return NotFound(new { message = "Reg Status not found." });
            }



            //check if serviceproviders email exist as a user else create as a user
            ApplicationUser? user = await _usersService.FindUserByEmailAsync(serviceProvider.ServiceProviderDto.Email);
            if (user == null)
            {
                if (serviceProvider.Password == null)
                {
                    return BadRequest(new
                    {
                        message = "Password is required"
                    });


                }
                //create user
                Register register = new Register
                {
                    Email = serviceProvider.ServiceProviderDto.Email,
                    Password = serviceProvider.Password,
                    Name = serviceProvider.ServiceProviderDto.FirstName + " " + serviceProvider.ServiceProviderDto.LastName,

                };
                await _usersService.CreateUserAsync(register);
            }



            Models.ServiceProvider data = new Models.ServiceProvider
            {
                Id = serviceProvider.ServiceProviderDto.Id,
                FirstName = serviceProvider.ServiceProviderDto.FirstName,
                LastName = serviceProvider.ServiceProviderDto.LastName,
                Email = serviceProvider.ServiceProviderDto.Email,
                PhoneNumber = serviceProvider.ServiceProviderDto.PhoneNumber,
                IdTypeId = serviceProvider.ServiceProviderDto.IdTypeId,
                IdType = idType,
                IdNumber = serviceProvider.ServiceProviderDto.IdNumber,
                Logo = serviceProvider.ServiceProviderDto.Logo,
                RegStatusId = serviceProvider.ServiceProviderDto.RegStatusId,
                RegStatus = regStatus,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Code = _codeGenService.GenerateClientCode().Value,
                Rating = 100

            };

            //check if logo is not null, save it and use the file name as part of service provider object
            if (serviceProvider.Image != null)
            {

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(serviceProvider.Image.FileName);
                string filePath = Path.Combine(_env.ContentRootPath, _imagePath, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await serviceProvider.Image.CopyToAsync(fileStream);
                }
                data.Logo = fileName;
            }

            _context.IdTypes.Attach(data.IdType);
            _context.RegStatuses.Attach(data.RegStatus);
            _context.ServiceProviders.Add(data);
            await _context.SaveChangesAsync();
            serviceProvider.ServiceProviderDto.Id = data.Id;

            ServiceProviderAddress serviceProviderAddress = new ServiceProviderAddress
            {
                ServiceProviderId = data.Id,
                ServiceProvider = data,
                City = serviceProvider.ServiceProviderAddressDto.City,

                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                CountryRegion = serviceProvider.ServiceProviderAddressDto.CountryRegion,
                PostalCode = serviceProvider.ServiceProviderAddressDto.PostalCode,
                StateProvince = serviceProvider.ServiceProviderAddressDto.StateProvince,
                Status = serviceProvider.ServiceProviderAddressDto.Status,
                AddressLine = serviceProvider.ServiceProviderAddressDto.AddressLine,

            };

            _context.ServiceProviders.Attach(data);
            _context.ServiceProviderAddresses.Add(serviceProviderAddress);

            ServiceProviderGeoLocation serviceProviderGeoLocation = new ServiceProviderGeoLocation
            {
                ServiceProviderId = data.Id,
                ServiceProvider = data,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,

                Latitude = serviceProvider.ServiceProviderGeoLocationDto.Latitude,
                Longitude = serviceProvider.ServiceProviderGeoLocationDto.Longitude,
                Accuracy = serviceProvider.ServiceProviderGeoLocationDto.Accuracy,
                Altitude = serviceProvider.ServiceProviderGeoLocationDto.Altitude,
                AltitudeAccuracy = serviceProvider.ServiceProviderGeoLocationDto.AltitudeAccuracy,
                Heading = serviceProvider.ServiceProviderGeoLocationDto.Heading,
                Speed = serviceProvider.ServiceProviderGeoLocationDto.Speed,
            };

            _context.ServiceProviderGeoLocations.Add(serviceProviderGeoLocation);
            await _context.SaveChangesAsync();



            return CreatedAtAction("GetServiceProvider", new { id = data.Id }, serviceProvider.ServiceProviderDto);
        }

        // DELETE: api/ServiceProvider/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServiceProvider(int id)
        {
            if (_context.ServiceProviders == null)
            {
                return NotFound();
            }
            var ServiceProvider = await _context.ServiceProviders.FindAsync(id);
            if (ServiceProvider == null)
            {
                return NotFound();
            }

            _context.ServiceProviders.Remove(ServiceProvider);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ServiceProviderExists(ServiceProviderDto serviceProviderDto)
        {
            return (_context.ServiceProviders?.Any(e => e.Email == serviceProviderDto.Email && e.Id != serviceProviderDto.Id)).GetValueOrDefault();
        }

    }
}
