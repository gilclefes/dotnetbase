
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
    public class ClientController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUriService uriService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        private readonly IConfiguration _config;

        private readonly string _imagePath;

        private readonly CodeGenService _codeGenService;

        public ClientController(DatabaseContext context, CodeGenService codeGenService, IUriService uriService, IMapper mapper, IWebHostEnvironment env, IConfiguration configuration)
        {
            _context = context;
            this.uriService = uriService;
            _mapper = mapper;
            _env = env;
            _config = configuration;
            _codeGenService = codeGenService;

            _imagePath = _config.GetValue<string>("IMAGE_STORAGE_PATH") ?? "";
        }

        // GET: api/Client
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> GetClients([FromQuery] PaginationFilter filter)
        {
            if (_context.Clients == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Clients'  is null." });
            }


            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.Clients
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).AsNoTracking().Select(x => new ClientDto
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
                   Code = x.Code
               })
               .ToListAsync();
            var totalRecords = await _context.Clients.CountAsync();


            var pagedReponse = PaginationHelper.CreatePagedReponse<ClientDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        // GET: api/Client/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult> GetClient(int id)
        {
            if (_context.Clients == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Clients'  is null." });
            }
            var client = await _context.Clients.FindAsync(id);

            if (client == null)
            {
                return NotFound(new { message = "Client not found" });
            }

            ClientDto data = _mapper.Map<ClientDto>(client);
            return Ok(new Wrappers.ApiResponse<ClientDto>(data));
        }

        // PUT: api/Client/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutClient(int id, ClientDto client)
        {
            if (id != client.Id)
            {
                return BadRequest(new { message = "Invalid Client Id" });
            }



            Client data = _mapper.Map<Client>(client);
            _context.Entry(data).State = EntityState.Modified;



            await _context.SaveChangesAsync();


            return NoContent();
        }

        // POST: api/Client
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Client>> PostClient(ClientDto client)
        {
            if (_context.Clients == null)
            {
                return Problem("Entity set 'DatabaseContext.Clients'  is null.");
            }

            Client data = _mapper.Map<Client>(client);
            _context.Clients.Add(data);

            await _context.SaveChangesAsync();


            return CreatedAtAction("GetClient", new { id = data.Id }, _mapper.Map<ClientDto>(data));
        }

        // DELETE: api/Client/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteClient(int id)
        {
            if (_context.Clients == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Clients'  is null." });
            }
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound(new { message = "Client not found" });
            }

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClientExists(ClientDto clientDto)

        {
            return (_context.Clients?.Any(e => e.Email == clientDto.Email && e.Id != clientDto.Id)).GetValueOrDefault();
        }


        [HttpGet("Email/{email}")]
        [Authorize]
        public async Task<ActionResult<ClientDto>> GetClientFullByEmail(string email)
        {
            if (_context.Clients == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Clients'  is null." });
            }

            Client? client = await _context.Clients.FirstOrDefaultAsync(x => x.Email == email);

            if (client == null)
            {
                return NotFound(new { message = "Client not found" });
            }

            ClientDto data = new ClientDto
            {
                Id = client.Id,
                FirstName = client.FirstName,
                LastName = client.LastName,
                Email = client.Email,
                PhoneNumber = client.PhoneNumber,
                IdTypeId = client.IdTypeId,
                IdNumber = client.IdNumber,
                Logo = client.Logo,
                RegStatusId = client.RegStatusId,
                IdTypeName = client.IdType?.Name,
                RegStatusName = client.RegStatus?.Name,
                Code = client.Code
            };


            ClientAddress? ClientAddress = await _context.ClientAddress.FirstOrDefaultAsync(x => x.ClientId == client.Id);
            ClientAddressDto ClientAddressDto = _mapper.Map<ClientAddressDto>(ClientAddress);

            ClientGeoLocation? ClientGeoLocation = await _context.ClientGeoLocations.FirstOrDefaultAsync(x => x.ClientId == client.Id);
            ClientGeoLocationDto ClientGeoLocationDto = _mapper.Map<ClientGeoLocationDto>(ClientGeoLocation);

            ClientFullDto ClientFullDto = new ClientFullDto
            {
                clientAddressDto = ClientAddressDto,
                clientDto = data,
                clientGeoLocationDto = ClientGeoLocationDto
            };


            return Ok(new Wrappers.ApiResponse<ClientFullDto>(ClientFullDto));
        }

        [HttpGet("Full/{id}")]
        [Authorize]
        public async Task<ActionResult<ClientDto>> GetClientFull(int id)
        {
            if (_context.Clients == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Clients'  is null." });
            }

            Client? client = await _context.Clients.FindAsync(id);

            if (client == null)
            {
                return NotFound(new { message = "Client not found" });
            }

            ClientDto data = new ClientDto
            {
                Id = client.Id,
                FirstName = client.FirstName,
                LastName = client.LastName,
                Email = client.Email,
                PhoneNumber = client.PhoneNumber,
                IdTypeId = client.IdTypeId,
                IdNumber = client.IdNumber,
                Logo = client.Logo,
                RegStatusId = client.RegStatusId,
                IdTypeName = client.IdType?.Name,
                RegStatusName = client.RegStatus?.Name,
                Code = client.Code
            };


            ClientAddress? ClientAddress = await _context.ClientAddress.FirstOrDefaultAsync(x => x.ClientId == client.Id);
            ClientAddressDto ClientAddressDto = _mapper.Map<ClientAddressDto>(ClientAddress);

            ClientGeoLocation? ClientGeoLocation = await _context.ClientGeoLocations.FirstOrDefaultAsync(x => x.ClientId == client.Id);
            ClientGeoLocationDto ClientGeoLocationDto = _mapper.Map<ClientGeoLocationDto>(ClientGeoLocation);

            ClientFullDto ClientFullDto = new ClientFullDto
            {
                clientAddressDto = ClientAddressDto,
                clientDto = data,
                clientGeoLocationDto = ClientGeoLocationDto
            };


            return Ok(new Wrappers.ApiResponse<ClientFullDto>(ClientFullDto));
        }



        [HttpPut("Full/{id}")]
        [Authorize]
        public async Task<IActionResult> PutClientFull(int id, [FromForm] ClientFullDto Client)
        {
            if (_context.Clients == null)
            {
                return Problem("Entity set 'DatabaseContext.Clients'  is null.");
            }

            if (Client.clientDto == null)
            {
                return NotFound(new { message = "ClientDto is null" });
            }

            if (Client.clientAddressDto == null)
            {
                return NotFound(new { message = "Service Provider Address not provided" });
            }

            if (Client.clientGeoLocationDto == null)
            {
                return NotFound(new { message = "Service Provider Geo Location not provided" });
            }


            if (id != Client.clientDto.Id)
            {
                return BadRequest(new { message = "Invalid Client Id" });
            }

            IdType? idType = await _context.IdTypes.FindAsync(Client.clientDto.IdTypeId);

            if (idType == null)
            {
                return NotFound(new { message = "Id Type not found" });
            }


            RegStatus? regStatus = await _context.RegStatuses.FindAsync(Client.clientDto.RegStatusId);
            if (regStatus == null)
            {
                return NotFound(new { message = "Reg Status not found" });
            }


            //check if client exist
            if (this.ClientExists(Client.clientDto))
            {
                return BadRequest(new { message = "Client already exist" });
            }

            Models.Client data = new Models.Client
            {
                Id = Client.clientDto.Id,
                FirstName = Client.clientDto.FirstName,
                LastName = Client.clientDto.LastName,
                Email = Client.clientDto.Email,
                PhoneNumber = Client.clientDto.PhoneNumber,
                IdTypeId = Client.clientDto.IdTypeId,
                IdType = idType,
                IdNumber = Client.clientDto.IdNumber,
                Logo = Client.clientDto.Logo,
                RegStatusId = Client.clientDto.RegStatusId,
                RegStatus = regStatus,


                UpdatedAt = DateTime.Now,

            };

            //check if logo is not null, save it and use the file name as part of service provider object
            if (Client.image != null)
            {

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(Client.image.FileName);
                string filePath = Path.Combine(_env.ContentRootPath, _imagePath, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await Client.image.CopyToAsync(fileStream);
                }
                data.Logo = fileName;
            }

            _context.Entry(data).State = EntityState.Modified;

            ClientAddress ClientAddress = new ClientAddress
            {
                Id = Client.clientAddressDto.Id,
                ClientId = data.Id,
                Client = data,
                City = Client.clientAddressDto.City,
                UpdatedAt = DateTime.Now,
                CountryRegion = Client.clientAddressDto.CountryRegion,
                PostalCode = Client.clientAddressDto.PostalCode,
                StateProvince = Client.clientAddressDto.StateProvince,
                Status = Client.clientAddressDto.Status,
                AddressLine = Client.clientAddressDto.AddressLine,

            };
            _context.Entry(ClientAddress).State = EntityState.Modified;

            ClientGeoLocation ClientGeoLocation = new ClientGeoLocation
            {
                Id = Client.clientGeoLocationDto.Id,
                ClientId = data.Id,
                Client = data,

                UpdatedAt = DateTime.Now,

                Latitude = Client.clientGeoLocationDto.Latitude,
                Longitude = Client.clientGeoLocationDto.Longitude,
                Accuracy = Client.clientGeoLocationDto.Accuracy,
                Altitude = Client.clientGeoLocationDto.Altitude,
                AltitudeAccuracy = Client.clientGeoLocationDto.AltitudeAccuracy,
                Heading = Client.clientGeoLocationDto.Heading,
                Speed = Client.clientGeoLocationDto.Speed,
            };

            _context.Entry(ClientGeoLocation).State = EntityState.Modified;


            await _context.SaveChangesAsync();


            return NoContent();
        }

        [HttpPost("Full")]
        [Authorize]
        public async Task<ActionResult> PostClientFull([FromForm] ClientFullDto Client)
        {
            if (_context.Clients == null)
            {
                return Problem("Entity set 'DatabaseContext.Clients'  is null.");
            }

            if (Client.clientDto == null)
            {
                return NotFound(new { message = "ClientDto is null" });
            }

            if (Client.clientAddressDto == null)
            {
                return NotFound(new { message = "Client Address not provided" });
            }

            if (Client.clientGeoLocationDto == null)
            {
                return NotFound(new { message = "Client Geo Location not provided" });
            }

            IdType? idType = await _context.IdTypes.FindAsync(Client.clientDto.IdTypeId);

            if (idType == null)
            {
                return NotFound(new { message = "Id Type not found" });
            }


            RegStatus? regStatus = await _context.RegStatuses.FindAsync(Client.clientDto.RegStatusId);
            if (regStatus == null)
            {
                return NotFound(new { message = "Reg Status not found" });
            }

            //check if client exist
            if (this.ClientExists(Client.clientDto))
            {
                return BadRequest(new { message = "Client already exist" });
            }




            Client data = new Client
            {
                Id = Client.clientDto.Id,
                FirstName = Client.clientDto.FirstName,
                LastName = Client.clientDto.LastName,
                Email = Client.clientDto.Email,
                PhoneNumber = Client.clientDto.PhoneNumber,
                IdTypeId = Client.clientDto.IdTypeId,
                IdType = idType,
                IdNumber = Client.clientDto.IdNumber,
                Logo = Client.clientDto.Logo,
                RegStatusId = Client.clientDto.RegStatusId,
                RegStatus = regStatus,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Code = _codeGenService.GenerateClientCode().Value

            };

            //check if logo is not null, save it and use the file name as part of service provider object
            if (Client.image != null)
            {

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(Client.image.FileName);
                string filePath = Path.Combine(_env.ContentRootPath, _imagePath, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await Client.image.CopyToAsync(fileStream);
                }
                data.Logo = fileName;
            }

            _context.IdTypes.Attach(data.IdType);
            _context.RegStatuses.Attach(data.RegStatus);
            _context.Clients.Add(data);
            await _context.SaveChangesAsync();
            Client.clientDto.Id = data.Id;

            ClientAddress ClientAddress = new ClientAddress
            {
                ClientId = data.Id,
                Client = data,
                City = Client.clientAddressDto.City,

                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                CountryRegion = Client.clientAddressDto.CountryRegion,
                PostalCode = Client.clientAddressDto.PostalCode,
                StateProvince = Client.clientAddressDto.StateProvince,
                Status = Client.clientAddressDto.Status,
                AddressLine = Client.clientAddressDto.AddressLine,

            };
            _context.Clients.Attach(data);
            _context.ClientAddress.Add(ClientAddress);

            ClientGeoLocation ClientGeoLocation = new ClientGeoLocation
            {
                ClientId = data.Id,
                Client = data,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,

                Latitude = Client.clientGeoLocationDto.Latitude,
                Longitude = Client.clientGeoLocationDto.Longitude,
                Accuracy = Client.clientGeoLocationDto.Accuracy,
                Altitude = Client.clientGeoLocationDto.Altitude,
                AltitudeAccuracy = Client.clientGeoLocationDto.AltitudeAccuracy,
                Heading = Client.clientGeoLocationDto.Heading,
                Speed = Client.clientGeoLocationDto.Speed,
            };

            _context.ClientGeoLocations.Add(ClientGeoLocation);
            await _context.SaveChangesAsync();



            return CreatedAtAction("GetClient", new { id = data.Id }, Client.clientDto);
        }

        [HttpGet("ClosePartners/{email}")]
        [Authorize]
        public async Task<ActionResult<ClientDto>> GetClosePartners(string email)
        {

            if (_context.Clients == null)
            {
                return Problem("Entity set 'DatabaseContext.Clients'  is null.");
            }

            var client = await _context.Clients.Include(x => x.ClientGeoLocations).FirstOrDefaultAsync(x => x.Email == email);

            if (client == null)
            {
                return NotFound(new { message = "Client with this email not found" });
            }
            var clientCity = client.ClientAddresses.FirstOrDefault()?.City;


            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(1, 10);
            var partners = await _context.Partners.Include(x => x.PartnerGeoLocations).Include(x => x.PartnerAddresses).Where(x => x.PartnerAddresses.Any(y => y.City == clientCity)).ToListAsync();
            var pagedData = partners
           .OrderBy(p => HaversineDistance(client.ClientGeoLocations.FirstOrDefault()?.Latitude ?? 0, client.ClientGeoLocations.FirstOrDefault()?.Longitude ?? 0, p.PartnerGeoLocations.FirstOrDefault()?.Latitude ?? 0, p.PartnerGeoLocations.FirstOrDefault()?.Longitude ?? 0))
           .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
           .Take(validFilter.PageSize).Select(x => new PartnerFullDto
           {
               partnerDto = new PartnerDto
               {
                   CompanyName = x.CompanyName,
                   Email = x.Email,
                   IdNumber = x.IdNumber,
                   PhoneNumber = x.PhoneNumber,
                   ContactFirstName = x.ContactFirstName,
                   ContactLastName = x.ContactLastName,
                   Id = x.Id,
                   IdTypeId = x.IdTypeId,
                   RegStatusId = x.RegStatusId,
                   Logo = x.Logo,
                   Code = x.Code


               },
               partnerAddressDto = new PartnerAddressDto
               {
                   AddressLine = x.PartnerAddresses.ToArray()[0].AddressLine,
                   City = x.PartnerAddresses.ToArray()[0].City,
                   CountryRegion = x.PartnerAddresses.ToArray()[0].CountryRegion,
                   Id = x.PartnerAddresses.ToArray()[0].Id,
                   StateProvince = x.PartnerAddresses.ToArray()[0].StateProvince,
                   PostalCode = x.PartnerAddresses.ToArray()[0].PostalCode,
               },
               partnerGeoLocationDto = new PartnerGeoLocationDto
               {
                   Id = x.PartnerGeoLocations.ToArray()[0].Id,
                   Latitude = x.PartnerGeoLocations.ToArray()[0].Latitude,
                   Longitude = x.PartnerGeoLocations.ToArray()[0].Longitude,
               }
           }).ToList();

            var totalRecords = await _context.Partners.CountAsync();


            var pagedReponse = PaginationHelper.CreatePagedReponse<PartnerFullDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        //TODO: Operating cities should be done, and every customer, partner or service provider should have a list of cities they operate in one.


        private double HaversineDistance(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
        {
            var R = 6371; // Radius of the earth in km
            var dLat = Deg2Rad((double)(lat2 - lat1));
            var dLon = Deg2Rad((double)(lon2 - lon1));
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(Deg2Rad((double)lat1)) * Math.Cos(Deg2Rad((double)lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var d = R * c; // Distance in km
            return d;
        }

        private double Deg2Rad(double deg)
        {
            return deg * (Math.PI / 180);
        }


    }
}
