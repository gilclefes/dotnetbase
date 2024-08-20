using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using dotnetbase.Application.Database;
using dotnetbase.Application.Filter;
using dotnetbase.Application.Helpers;
using dotnetbase.Application.Models;
using dotnetbase.Application.Models.AuthorizeNet;
using dotnetbase.Application.Services;
using dotnetbase.Application.ViewModels;

namespace dotnetbase.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ClientSubscriptionController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUriService uriService;
        private readonly IMapper _mapper;

        private readonly YaboUtilsService _yaboValidationService;

        private readonly AuthorizePaymentService _authorizePaymentService;
        // private const int CURRENCYID = 1;
        private const string OkReponseCode = "Ok";

        public ClientSubscriptionController(DatabaseContext context, IUriService uriService,
        IMapper mapper, YaboUtilsService yaboValidationService, AuthorizePaymentService authorizePaymentService)
        {
            _context = context;
            this.uriService = uriService;
            _mapper = mapper;
            this._yaboValidationService = yaboValidationService;
            _authorizePaymentService = authorizePaymentService;
        }

        // GET: api/ClientSubscription
        [HttpGet("clientId")]
        public async Task<ActionResult> GetClientSubscriptions(int clientId, [FromQuery] PaginationFilter filter)
        {
            if (_context.ClientSubscriptions == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.ClientSubscriptions'  is null." });
            }
            // return await _context.ClientSubscriptions.ToListAsync();
            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.ClientSubscriptions.Include(x => x.ClientSubscriptionDetails).Where(x => x.ClientId == clientId)
            .OrderByDescending(x => x.ExpiryDate)
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).AsNoTracking().Select(x => new ClientSubscriptionDto
               {
                   ClientId = x.ClientId,
                   ExpiryDate = x.ExpiryDate,
                   FirstSubscriptionDate = x.FirstSubscriptionDate,
                   Id = x.Id,
                   LastRenewedDate = x.LastRenewedDate,
                   Status = x.Status,
                   SubscriptionId = x.SubscriptionId,
                   ClientName = x.Client.FirstName + " " + x.Client.LastName,
                   SubscriptionPlanName = x.SubscriptionPlan.Name,
                   SubscriptionPlanDetailId = x.CurrentSubscriptionDetailId,
                   SubscriptionPlanPriceId = x.ClientSubscriptionDetails.Where(y => y.Id == x.CurrentSubscriptionDetailId).FirstOrDefault().SubscriptionPlanPriceId

               })
               .ToListAsync();
            var totalRecords = await _context.OrderAssignments.CountAsync();

            var pagedReponse = PaginationHelper.CreatePagedReponse<ClientSubscriptionDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }


        [HttpGet("Email/{email}")]
        public async Task<ActionResult> GetClientActiveSubscriptionByEmail(string email)
        {
            if (_context.ClientSubscriptions == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.ClientSubscriptions'  is null." });
            }

            var clientSubscription = await _context.ClientSubscriptions.Include(x => x.Client).Include(x => x.SubscriptionPlan).Where(x => x.Client.Email == email && x.ExpiryDate > DateTime.Now && x.Status == true).FirstOrDefaultAsync();

            if (clientSubscription == null)
            {
                return NotFound(new { message = "Client has no active subscription" });
            }

            var clientSubscriptionDetail = await _context.ClientSubscriptionDetails.Include(x => x.SubscriptionPlanPrice).ThenInclude(x => x.Period).Where(x => x.Id == clientSubscription.CurrentSubscriptionDetailId).FirstOrDefaultAsync();

            if (clientSubscriptionDetail == null)
            {
                return NotFound(new { message = "Client has no active subscription" });
            }

            var subscriptionPlan = await _context.SubscriptionPlans.Include(x => x.SubscriptionPlanServices).ThenInclude(x => x.Service).Include(x => x.SubscriptionPlanCharges).ThenInclude(x => x.Charge).Where(x => x.Id == clientSubscription.SubscriptionId).FirstOrDefaultAsync();

            if (subscriptionPlan == null)
            {
                return NotFound(new { message = "Subscription Plan not found" });
            }

            ClientActiveSubscriptionDetailDto clientActiveSubscriptionDetailDto = new ClientActiveSubscriptionDetailDto
            {
                Charges = string.Join(",", subscriptionPlan.SubscriptionPlanCharges.Select(x => x.Charge.Name + ":" + x.Amount.ToString())),
                Description = subscriptionPlan.Description,
                ExpireDate = clientSubscription.ExpiryDate,
                Name = subscriptionPlan.Name,
                Services = string.Join(",", subscriptionPlan.SubscriptionPlanServices.Select(x => x.Service.Name)),
                PricePeriodName = clientSubscriptionDetail.SubscriptionPlanPrice.Period.Name,
                SubscriptionPrice = clientSubscriptionDetail.SubscriptionPlanPrice.Amount,
                TermAndConditions = subscriptionPlan.TermAndConditions,
                SubscritpionPlanId = subscriptionPlan.Id

            };


            return Ok(new Wrappers.ApiResponse<ClientActiveSubscriptionDetailDto>(clientActiveSubscriptionDetailDto));
        }

        // GET: api/ClientSubscription
        [HttpGet("detail")]
        public async Task<ActionResult> GetClientSubscriptionDetail(int clientSubscriptionId, [FromQuery] PaginationFilter filter)
        {
            if (_context.ClientSubscriptionDetails == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.ClientSubscriptionDetails'  is null." });
            }
            // return await _context.ClientSubscriptions.ToListAsync();
            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.ClientSubscriptionDetails.Include(x => x.ClientSubscription).Where(x => x.ClientSubscriptionId == clientSubscriptionId)
            .OrderByDescending(x => x.ExpiryDate)
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).AsNoTracking().Select(x => new ClientSubscriptionDetailDto
               {
                   ClientSubscriptionId = x.ClientSubscriptionId,
                   DateSubscibed = x.DateSubscibed,
                   ExpiryDate = x.ExpiryDate,
                   Id = x.Id,
                   SubscriptionPlanPriceId = x.SubscriptionPlanPriceId,
                   TotalAmount = x.TotalAmount,
                   AmountPaid = x.AmountPaid,
                   Discount = x.Discount,
                   ClientSubscriptionSubscriptionPlanName = x.ClientSubscription != null && x.ClientSubscription.SubscriptionPlan != null ? x.ClientSubscription.SubscriptionPlan.Name : null
               })
               .ToListAsync();
            var totalRecords = await _context.OrderAssignments.CountAsync();

            var pagedReponse = PaginationHelper.CreatePagedReponse<ClientSubscriptionDetailDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }
        // GET: api/ClientSubscription/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ClientSubscription>> GetClientSubscription(int id)
        {
            if (_context.ClientSubscriptions == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.ClientSubscriptions'  is null." });
            }
            var ClientSubscription = await _context.ClientSubscriptions.Include(x => x.Client).FirstOrDefaultAsync(x => x.Id == id);

            if (ClientSubscription == null)
            {
                return NotFound(new { message = "ClientSubscription not found" });
            }



            ClientSubscriptionDto data = new ClientSubscriptionDto
            {
                ClientId = ClientSubscription.ClientId,
                ExpiryDate = ClientSubscription.ExpiryDate,
                FirstSubscriptionDate = ClientSubscription.FirstSubscriptionDate,
                Id = ClientSubscription.Id,
                LastRenewedDate = ClientSubscription.LastRenewedDate,
                Status = ClientSubscription.Status,
                SubscriptionId = ClientSubscription.SubscriptionId,
                ClientName = ClientSubscription.Client.FirstName + " " + ClientSubscription.Client.LastName,
                SubscriptionPlanName = ClientSubscription.SubscriptionPlan.Name
            };
            return Ok(new Wrappers.ApiResponse<ClientSubscriptionDto>(data));
        }



        // POST: api/ClientSubscription
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostClientSubscription(int ClientId, int SubscriptionId, int SubscriptionPlanPriceId)
        {
            if (_context.ClientSubscriptions == null)
            {
                return Problem("Entity set 'DatabaseContext.ClientSubscriptions'  is null.");
            }

            //check if client has an active subscription base on the expiry date
            if (ClientHasActiveSubscription(ClientId))
            {
                return BadRequest(new { message = "Client already has an active subscription" });
            }

            SubscriptionPlanPrice? subscriptionPlanPrice = await _context.SubscriptionPlanPrices.FindAsync(SubscriptionPlanPriceId);

            if (subscriptionPlanPrice == null)
            {
                return NotFound(new { message = "SubscriptionPlanPrice not found for =" + SubscriptionPlanPriceId });
            }

            if (subscriptionPlanPrice.SubscriptionId != SubscriptionId)
            {
                return BadRequest(new { message = "SubscriptionPlanPrice does not belong to the selected subscription" });
            }


            Period? period = await _context.Periods.FindAsync(subscriptionPlanPrice.PeriodId);
            if (period == null)
            {
                return NotFound(new { message = "Period not found" });
            }


            SubscriptionPlan? subscriptionPlan = await _context.SubscriptionPlans.FindAsync(SubscriptionId);

            if (subscriptionPlan == null)
            {
                return NotFound(new { message = "SubscriptionPlan not found" });
            }



            Client? client = await _context.Clients.Include(x => x.ClientAddresses).FirstOrDefaultAsync(x => x.Id == ClientId);
            if (client == null)
            {
                return NotFound(new { message = "Client not found" });
            }

            // search with the names

            decimal taxPercentage = 0;

            string cityname = client.ClientAddresses?.FirstOrDefault()?.City ?? string.Empty;

            CityTax? cityTax = await _context.CityTaxes.Include(x => x.City).FirstOrDefaultAsync(x => x.City.Name == cityname);

            if (cityTax != null)
            {
                taxPercentage = cityTax.TaxPercentage;
            }

            var result = await this._yaboValidationService.CustomerHasPaymentProfile(client.Id);
            if (!result)
            {
                return BadRequest(new { message = "Client has no payment profile" });
            }

            ClientSubscription? clientSubscription = await _context.ClientSubscriptions.Where(x => x.ClientId == ClientId && x.SubscriptionId == SubscriptionId).FirstOrDefaultAsync();
            decimal taxAmount = subscriptionPlanPrice.Amount * taxPercentage / 100;
            if (clientSubscription != null)
            {
                ClientSubscriptionDetail clientSubscriptionDetail = new ClientSubscriptionDetail
                {
                    ClientSubscriptionId = clientSubscription.Id,
                    DateSubscibed = DateTime.Now,
                    ExpiryDate = DateTime.Now.AddDays(period.NoOfDays),
                    SubscriptionPlanPriceId = subscriptionPlanPrice.Id,
                    TotalAmount = subscriptionPlanPrice.Amount + taxAmount,
                    AmountPaid = 0,
                    Discount = 0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    Status = ClientSubscriptionPaymentStatus.PENDING,
                    TaxAmount = taxAmount
                };

                _context.SubscriptionPlanPrices.Attach(subscriptionPlanPrice);
                _context.ClientSubscriptions.Attach(clientSubscription);
                _context.ClientSubscriptionDetails.Add(clientSubscriptionDetail);
                await _context.SaveChangesAsync();

                //save client subscript d etails
                clientSubscription.ExpiryDate = clientSubscription.ExpiryDate > DateTime.Now ? clientSubscription.ExpiryDate.AddDays(period.NoOfDays) : DateTime.Now.AddDays(period.NoOfDays);
                clientSubscription.LastRenewedDate = DateTime.Now;
                clientSubscription.Status = false;
                clientSubscription.UpdatedAt = DateTime.Now;
                clientSubscription.CurrentSubscriptionDetailId = clientSubscriptionDetail.Id;
                _context.Entry(clientSubscription).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                var paymentResponse = await this._authorizePaymentService.CreateSubscriptionPayment(new SubscriptionPaymentDto
                {
                    clientId = ClientId,
                    ClientSubscriptionDetailId = clientSubscriptionDetail.Id,
                    PlanName = subscriptionPlan.Name,
                    TotalAmount = subscriptionPlanPrice.Amount + taxAmount
                });

                if (paymentResponse.resultCode == OkReponseCode)
                {
                    clientSubscriptionDetail.Status = ClientSubscriptionPaymentStatus.PAID;
                    clientSubscriptionDetail.UpdatedAt = DateTime.Now;
                    clientSubscription.Status = true;
                    clientSubscription.UpdatedAt = DateTime.Now;
                    clientSubscriptionDetail.AmountPaid = subscriptionPlanPrice.Amount + taxAmount;
                }
                else
                {
                    clientSubscription.Status = false;
                    clientSubscriptionDetail.Status = ClientSubscriptionPaymentStatus.PAYMENTFAILED;
                    clientSubscription.UpdatedAt = DateTime.Now;
                    clientSubscriptionDetail.UpdatedAt = DateTime.Now;
                }

                _context.Entry(clientSubscriptionDetail).State = EntityState.Modified;
                _context.Entry(clientSubscription).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new Wrappers.ApiResponse<ClientSubscriptionDto>(_mapper.Map<ClientSubscriptionDto>(clientSubscription)));

            }


            else
            {
                clientSubscription = new ClientSubscription
                {
                    ClientId = ClientId,
                    ExpiryDate = DateTime.Now.AddDays(period.NoOfDays),
                    FirstSubscriptionDate = DateTime.Now,
                    LastRenewedDate = DateTime.Now,
                    Status = false,
                    SubscriptionId = SubscriptionId,
                    Client = client,
                    SubscriptionPlan = subscriptionPlan,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };
                _context.Clients.Attach(client);
                _context.SubscriptionPlans.Attach(subscriptionPlan);
                _context.ClientSubscriptions.Add(clientSubscription);
                await _context.SaveChangesAsync();


                ClientSubscriptionDetail clientSubscriptionDetail = new ClientSubscriptionDetail
                {
                    ClientSubscriptionId = clientSubscription.Id,
                    DateSubscibed = DateTime.Now,
                    ExpiryDate = DateTime.Now.AddDays(period.NoOfDays),
                    SubscriptionPlanPriceId = subscriptionPlanPrice.Id,
                    TotalAmount = subscriptionPlanPrice.Amount + taxAmount,
                    AmountPaid = 0,
                    Discount = 0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    Status = ClientSubscriptionPaymentStatus.PENDING
                };

                _context.SubscriptionPlanPrices.Attach(subscriptionPlanPrice);
                _context.ClientSubscriptions.Attach(clientSubscription);
                _context.ClientSubscriptionDetails.Add(clientSubscriptionDetail);
                await _context.SaveChangesAsync();

                //map current subscription detail id to client subscription
                clientSubscription.CurrentSubscriptionDetailId = clientSubscriptionDetail.Id;
                await _context.SaveChangesAsync();

                var paymentResponse = await this._authorizePaymentService.CreateSubscriptionPayment(new SubscriptionPaymentDto
                {
                    clientId = ClientId,
                    ClientSubscriptionDetailId = clientSubscriptionDetail.Id,
                    PlanName = subscriptionPlan.Name,
                    TotalAmount = subscriptionPlanPrice.Amount + taxAmount
                });

                if (paymentResponse.resultCode == OkReponseCode)
                {
                    clientSubscriptionDetail.Status = ClientSubscriptionPaymentStatus.PAID;
                    clientSubscriptionDetail.UpdatedAt = DateTime.Now;
                    clientSubscription.Status = true;
                    clientSubscription.UpdatedAt = DateTime.Now;
                    clientSubscriptionDetail.AmountPaid = subscriptionPlanPrice.Amount;
                }
                else
                {
                    clientSubscription.Status = false;
                    clientSubscriptionDetail.Status = ClientSubscriptionPaymentStatus.PAYMENTFAILED;
                    clientSubscription.UpdatedAt = DateTime.Now;
                    clientSubscriptionDetail.UpdatedAt = DateTime.Now;
                }

                _context.Entry(clientSubscriptionDetail).State = EntityState.Modified;
                _context.Entry(clientSubscription).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new Wrappers.ApiResponse<ClientSubscriptionDto>(_mapper.Map<ClientSubscriptionDto>(clientSubscription)));

            }




        }

        [HttpPost("PaySubscription/{id}")]
        public async Task<ActionResult> PayClientSubscription(int id)
        {

            if (_context.ClientSubscriptions == null)
            {
                return Problem("Entity set 'DatabaseContext.ClientSubscriptions'  is null.");
            }

            ClientSubscription? clientSubscription = await _context.ClientSubscriptions.Include(x => x.SubscriptionPlan).FirstOrDefaultAsync(x => x.Id == id);
            if (clientSubscription == null)
            {
                return NotFound(new
                {
                    message = "Client Subscription not found",
                    errorCode = "Nok"
                });

            }

            if (clientSubscription.Status && clientSubscription.ExpiryDate > DateTime.Now)
            {
                return BadRequest(new
                {
                    message = "Client Subscription already paid",
                    errorCode = "Nok"
                });
            }

            ClientSubscriptionDetail? clientSubscriptionDetail = await _context.ClientSubscriptionDetails.FindAsync(clientSubscription.CurrentSubscriptionDetailId);

            if (clientSubscriptionDetail == null)
            {
                return NotFound(new
                {
                    message = "Client Subscription Detail not found",
                    errorCode = "Nok"
                });
            }
            if (string.IsNullOrEmpty(clientSubscriptionDetail.Status) || clientSubscriptionDetail.Status != ClientSubscriptionPaymentStatus.PENDING || clientSubscriptionDetail.Status != ClientSubscriptionPaymentStatus.PAYMENTFAILED)
            {
                // Code logic goes here
            }
            else
            {
                return BadRequest(new
                {
                    message = "You can only pay for pending subscription",
                    errorCode = "Nok"
                });
            }

            Period? period = await _context.Periods.FindAsync(clientSubscription.SubscriptionPlan.OrderPeriodId);

            if (period == null)
            {
                return NotFound(new
                {
                    message = "Period not found",
                    errorCode = "Nok"
                });
            }


            var paymentResponse = await this._authorizePaymentService.CreateSubscriptionPayment(new SubscriptionPaymentDto
            {
                clientId = clientSubscription.ClientId,
                ClientSubscriptionDetailId = clientSubscriptionDetail.Id,
                PlanName = clientSubscription.SubscriptionPlan.Name,
                TotalAmount = clientSubscriptionDetail.TotalAmount + clientSubscriptionDetail.TaxAmount
            });

            if (paymentResponse.resultCode == OkReponseCode)
            {
                clientSubscriptionDetail.Status = ClientSubscriptionPaymentStatus.PAID;
                clientSubscriptionDetail.UpdatedAt = DateTime.Now;
                clientSubscription.Status = true;
                clientSubscription.UpdatedAt = DateTime.Now;
                clientSubscriptionDetail.DateSubscibed = DateTime.Now;
                clientSubscriptionDetail.ExpiryDate = DateTime.Now.AddDays(period.NoOfDays);
                clientSubscription.LastRenewedDate = DateTime.Now;
                clientSubscription.ExpiryDate = DateTime.Now.AddDays(period.NoOfDays);

                _context.Entry(clientSubscriptionDetail).State = EntityState.Modified;
                _context.Entry(clientSubscription).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Payment Successful",
                    errorCode = "Ok"
                });
            }
            else
            {
                clientSubscription.Status = false;
                clientSubscriptionDetail.Status = ClientSubscriptionPaymentStatus.PAYMENTFAILED;
                clientSubscription.UpdatedAt = DateTime.Now;
                clientSubscriptionDetail.UpdatedAt = DateTime.Now;

                _context.Entry(clientSubscriptionDetail).State = EntityState.Modified;
                _context.Entry(clientSubscription).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Payment Failed",
                    errorCode = "Nok"
                });
            }

        }


        //check if client has an active subscription base on the expiry date
        private bool ClientHasActiveSubscription(int clientId)
        {
            return (_context.ClientSubscriptions?.Any(e => e.ClientId == clientId && e.ExpiryDate > DateTime.Now && e.Status)).GetValueOrDefault();
        }






        [HttpPost("testpayment")]
        public async Task<ActionResult> TestPayment()
        {
            SubscriptionPaymentDto subscriptionPaymentDto = new SubscriptionPaymentDto
            {
                clientId = 1,
                ClientSubscriptionDetailId = 1,
                PlanName = "Test Plan",
                TotalAmount = 100
            };

            AuthorizeGenericResponse response = await _authorizePaymentService.CreateSubscriptionPayment(subscriptionPaymentDto);
            return Ok(response);
        }



    }
}
