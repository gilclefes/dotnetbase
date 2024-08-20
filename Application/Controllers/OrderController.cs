using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Coravel.Events.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Index.HPRtree;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Finance.Implementations;
using Org.BouncyCastle.Asn1.Icao;
using dotnetbase.Application.Database;
using dotnetbase.Application.Events;
using dotnetbase.Application.Filter;
using dotnetbase.Application.Helpers;
using dotnetbase.Application.Models;
using dotnetbase.Application.Models.AuthorizeNet;
using dotnetbase.Application.Services;
using dotnetbase.Application.ViewModels;
using dotnetbase.Application.Wrappers;

namespace dotnetbase.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IUriService uriService;
        private readonly IMapper _mapper;

        private readonly IConfiguration _configuration;


        private readonly YaboUtilsService _yaboUtilService;
        private readonly CodeGenService _codeGenService;

        private readonly AuthorizePaymentService _authorizePaymentService;

        private readonly IDispatcher _dispatcher;


        private const string OkReponseCode = "Ok";
        private const string NEW_ORDERSTATUS_ID = "NEW_ORDERSTATUS_ID";
        private const string PAID_ORDERSTATUS_ID = "PAID_ORDERSTATUS_ID";
        private const string CANCEL_ORDERSTATUS_ID = "CANCEL_ORDERSTATUS_ID";

        private const string COMPLETED_ORDERSTATUS_ID = "SERVICECOMPLETE_ORDERSTATUS_ID";
        private const string CONFIRMED_ORDERSTATUS_ID = "CUSTOMERCONFIRMED_ORDERSTATUS_ID";

        public OrderController(DatabaseContext context, CodeGenService codeGenService, IUriService uriService, IMapper mapper, IConfiguration configuration, YaboUtilsService yaboValidationService,
        IDispatcher dispatcher, IHttpClientFactory clientFactory, AuthorizePaymentService authorizePaymentService)
        {
            _context = context;
            this.uriService = uriService;
            _mapper = mapper;
            _configuration = configuration;
            _yaboUtilService = yaboValidationService;
            _codeGenService = codeGenService;
            _dispatcher = dispatcher;

            _authorizePaymentService = authorizePaymentService;
        }

        // GET: api/Order
        [HttpGet]
        public async Task<ActionResult> GetOrders([FromQuery] PaginationFilter filter)
        {
            if (_context.Orders == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Orders'  is null." });
            }

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.Orders.OrderByDescending(x => x.CreatedAt)
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).AsNoTracking().Select(x => new OrderDto
               {
                   ClientUserName = x.ClientUserName,
                   OrderDate = x.OrderDate,
                   DeliveryDate = x.DeliveryDate,
                   Discount = x.Discount,
                   GrossAmount = x.GrossAmount,
                   NetAmount = x.NetAmount,
                   TaxAmount = x.TaxAmount,
                   RefNumber = x.RefNumber,
                   PickupDate = x.PickupDate,

                   Id = x.Id,
                   OrderStatusId = x.OrderStatusId,
                   OrderStatusName = x.OrderStatus.Name,
               })
               .ToListAsync();
            var totalRecords = await _context.OrderAssignments.CountAsync();


            var pagedReponse = PaginationHelper.CreatePagedReponse<OrderDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        [HttpGet("User/{email}")]
        public async Task<ActionResult> GetOrdersByUserEmail(string email, [FromQuery] PaginationFilter filter)
        {
            if (_context.Orders == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Orders'  is null." });
            }

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.Orders.Include(x => x.OrderStatus).Where(x => x.ClientUserName == email)
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).OrderByDescending(x => x.CreatedAt).AsNoTracking().Select(x => new OrderDto
               {
                   ClientUserName = x.ClientUserName,
                   OrderDate = x.OrderDate,
                   DeliveryDate = x.DeliveryDate,
                   Discount = x.Discount,
                   GrossAmount = x.GrossAmount,
                   NetAmount = x.NetAmount,
                   TaxAmount = x.TaxAmount,
                   RefNumber = x.RefNumber,
                   PickupDate = x.PickupDate,
                   Id = x.Id,
                   OrderStatusId = x.OrderStatusId,
                   OrderStatusName = x.OrderStatus.Name,
               })
               .ToListAsync();
            var totalRecords = await _context.OrderAssignments.CountAsync();


            var pagedReponse = PaginationHelper.CreatePagedReponse<OrderDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        [HttpGet("Search/{orderno}")]
        public async Task<ActionResult> GetOrdersBySearch(string orderno, [FromQuery] PaginationFilter filter)
        {
            if (_context.Orders == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Orders'  is null." });
            }

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.Orders.Include(x => x.OrderStatus).Where(x => x.RefNumber.Contains(orderno))
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).OrderByDescending(x => x.CreatedAt).AsNoTracking().Select(x => new OrderDto
               {
                   ClientUserName = x.ClientUserName,
                   OrderDate = x.OrderDate,
                   DeliveryDate = x.DeliveryDate,
                   Discount = x.Discount,
                   GrossAmount = x.GrossAmount,
                   NetAmount = x.NetAmount,
                   TaxAmount = x.TaxAmount,
                   RefNumber = x.RefNumber,
                   PickupDate = x.PickupDate,
                   Id = x.Id,
                   OrderStatusId = x.OrderStatusId,
                   OrderStatusName = x.OrderStatus.Name,
               })
               .ToListAsync();
            var totalRecords = await _context.OrderAssignments.CountAsync();


            var pagedReponse = PaginationHelper.CreatePagedReponse<OrderDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }

        [HttpGet("Assigned/{email}")]
        public async Task<ActionResult> GetOrdersByAssignedEmail(string email, [FromQuery] PaginationFilter filter)
        {
            if (_context.Orders == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Orders'  is null." });
            }

            int cancelledOrderStatusId = _configuration.GetValue<int>(CANCEL_ORDERSTATUS_ID);

            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.Orders.Include(x => x.OrderStatus).Where(x => x.OrderStatusId != cancelledOrderStatusId && x.OrderAssigments.Any(y => y.AssingedUserName == email && (y.AssignedStatus == OrderAssignmentStatus.NEW || y.AssignedStatus == OrderAssignmentStatus.ACCEPTED || y.AssignedStatus == null)))
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).OrderByDescending(x => x.CreatedAt).AsNoTracking().Select(x => new OrderDto
               {
                   ClientUserName = x.ClientUserName,
                   OrderDate = x.OrderDate,
                   DeliveryDate = x.DeliveryDate,
                   Discount = x.Discount,
                   GrossAmount = x.GrossAmount,
                   NetAmount = x.NetAmount,
                   TaxAmount = x.TaxAmount,
                   RefNumber = x.RefNumber,
                   PickupDate = x.PickupDate,
                   Id = x.Id,
                   OrderStatusId = x.OrderStatusId,
                   OrderStatusName = x.OrderStatus.Name,
               })
               .ToListAsync();
            var totalRecords = await _context.OrderAssignments.CountAsync();


            var pagedReponse = PaginationHelper.CreatePagedReponse<OrderDto>(pagedData, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }



        // GET: api/Order/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            if (_context.Orders == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Orders'  is null." });
            }
            Order? x = await _context.Orders.FindAsync(id);

            if (x == null)
            {
                return NotFound(new { message = "Order not found" });
            }



            OrderDto data = new OrderDto
            {
                ClientUserName = x.ClientUserName,
                OrderDate = x.OrderDate,
                DeliveryDate = x.DeliveryDate,
                Discount = x.Discount,
                GrossAmount = x.GrossAmount,
                NetAmount = x.NetAmount,
                TaxAmount = x.TaxAmount,
                RefNumber = x.RefNumber,
                PickupDate = x.PickupDate,
                Id = x.Id,
                OrderStatusId = x.OrderStatusId,
                OrderStatusName = x.OrderStatus.Name,
            };
            return Ok(new Wrappers.ApiResponse<OrderDto>(data));
        }


        [HttpGet("orderstats/{startDate}/{endDate}")]
        public async Task<ActionResult<OrderStatsDto>> GetOrderStats(DateTime startDate, DateTime endDate)
        {

            int completedOrderStatusId = _configuration.GetValue<int>(COMPLETED_ORDERSTATUS_ID);
            int confirmedOrderStatusId = _configuration.GetValue<int>(CONFIRMED_ORDERSTATUS_ID);
            int cancelledOrderStatusId = _configuration.GetValue<int>(CANCEL_ORDERSTATUS_ID);

            int totalOrder = await _context.Orders.CountAsync(x => x.OrderDate >= startDate && x.OrderDate <= endDate);
            decimal totalSales = await _context.Orders.Where(x => x.OrderDate >= startDate && x.OrderDate <= endDate && (x.OrderStatusId == completedOrderStatusId || x.OrderStatusId == confirmedOrderStatusId)).SumAsync(x => x.NetAmount);
            int TotalCancelledOrders = await _context.Orders.CountAsync(x => x.OrderDate >= startDate && x.OrderDate <= endDate && x.OrderStatusId == cancelledOrderStatusId);
            int TotalCompletedOrders = await _context.Orders.CountAsync(x => x.OrderDate >= startDate && x.OrderDate <= endDate && (x.OrderStatusId == completedOrderStatusId || x.OrderStatusId == confirmedOrderStatusId));

            int TotalClients = await _context.Clients.CountAsync(x => x.CreatedAt >= startDate && x.CreatedAt <= endDate);
            int TotalSubscriptions = await _context.ClientSubscriptions.CountAsync(x => (x.CreatedAt >= startDate && x.CreatedAt <= endDate) || (x.LastRenewedDate >= startDate && x.LastRenewedDate <= endDate));

            OrderStatsDto data = new OrderStatsDto
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalOrders = totalOrder,
                TotalSales = totalSales,
                TotalCancelledOrders = TotalCancelledOrders,
                TotalCompletedOrders = TotalCompletedOrders,
                TotalClients = TotalClients,
                TotalSubscriptions = TotalSubscriptions
            };

            return Ok(new Wrappers.ApiResponse<OrderStatsDto>(data));

        }

        [HttpGet("orderdetail/{id}")]
        public async Task<ActionResult<OrderDetailDto>> GetOrderDetail(int id)
        {
            if (_context.Orders == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Orders'  is null." });
            }
            Order? order = await _context.Orders.Include(x => x.OrderMessages).Include(x => x.OrderRatings)
            .Include(x => x.OrderLocations).Include(x => x.OrderStatus).Include(x => x.OrderStatusUpdates).Include(x => x.OrderAssigments)
            .Include(x => x.OrderCharges).ThenInclude(x => x.Charge)
            .Include(x => x.OrderPayments).ThenInclude(x => x.Payment)
            .Include(x => x.OrderDetergents).ThenInclude(x => x.Detergent)
            .Include(x => x.OrderItems).ThenInclude(x => x.LaundryItem).FirstOrDefaultAsync(x => x.Id == id);

            if (order == null)
            {
                return NotFound(new { message = "Order not found" });
            }



            OrderDetailDto data = new OrderDetailDto
            {
                OrderDto = new OrderDto
                {
                    ClientUserName = order.ClientUserName,
                    OrderDate = order.OrderDate,
                    DeliveryDate = order.DeliveryDate,
                    Discount = order.Discount,
                    GrossAmount = order.GrossAmount,
                    NetAmount = order.NetAmount,
                    TaxAmount = order.TaxAmount,
                    RefNumber = order.RefNumber,
                    PickupDate = order.PickupDate,
                    Id = order.Id,
                    OrderStatusId = order.OrderStatusId,
                    OrderStatusName = order.OrderStatus.Name,
                },
                OrderItems = order.OrderItems.Select(x => new OrderItemDto
                {
                    Comments = x.Comments,
                    Discount = x.Discount,
                    ItemId = x.ItemId,
                    Quantity = x.Quantity,
                    ServiceId = x.ServiceId,
                    UnitPrice = x.UnitPrice,
                    NetPrice = x.NetPrice,
                    Id = x.Id,
                    LaundryItemName = x.LaundryItem.Name,
                    OrderName = order.RefNumber,
                    OrderId = x.OrderId,

                }).ToList(),
                OrderCharges = order.OrderCharges.Select(x => new OrderChargeDto
                {
                    Amount = x.Amount,
                    ChargeDescription = x.ChargeDescription,
                    ChargeId = x.ChargeId,
                    Id = x.Id,
                    OrderId = x.OrderId,
                    ChargeName = x.Charge.Name,
                    OrderRefNumber = order.RefNumber,
                    DateAdded = x.DateAdded,
                }).ToList(),
                OrderLocation = order.OrderLocations.Select(x => new OrderLocationDto
                {
                    Accuracy = x.Accuracy,
                    AddressLine = x.AddressLine,
                    City = x.City,
                    CountryRegion = x.CountryRegion,
                    Latitude = x.Latitude,
                    Longitude = x.Longitude,
                    OrderId = x.OrderId,
                    PostalCode = x.PostalCode,
                    StateProvince = x.StateProvince,
                    Id = x.Id,

                }).FirstOrDefault(),
                OrderMessages = order.OrderMessages.Select(x => new OrderMessageDto
                {
                    Id = x.Id,
                    Message = x.Message,
                    OrderId = x.OrderId,
                    MessageFrom = x.MessageFrom,
                    MessageStatus = x.MessageStatus,
                    MessageTo = x.MessageTo,
                    OrderName = order.RefNumber,
                    MessageType = x.MessageFrom == order.ClientUserName ? "sender" : "receiver",
                }).ToList(),
                OrderDetergents = order.OrderDetergents.Select(x => new OrderDetergentDto
                {
                    DetergentId = x.DetergentId,
                    Id = x.Id,
                    OrderId = x.OrderId,
                    Quantity = x.Quantity,
                    DetergentName = x.Detergent?.Name,
                }).ToList(),
                OrderPayment = order.OrderPayments.Select(x => new OrderPaymentDto
                {
                    Id = x.Id,
                    OrderId = x.OrderId,
                    PaymentId = x.PaymentId,
                    PaymentMethod = x.Payment.PaymentMethod,
                    OrderRefNumber = order.RefNumber,
                    AmountPaid = x.Payment.PaymentAmount,
                    ExTransactionId = x.Payment.ExTransactionId,
                    PaymentDate = x.Payment.PaymentEnd,
                    PaymentRefNumber = x.Payment.RefNumber,
                }).FirstOrDefault(),
                OrderStatuses = order.OrderStatusUpdates.Select(x => new OrderStatusUpdateDto
                {
                    Status = x.Status,
                    Description = x.Description,
                    OrderId = x.OrderId,
                    StatusDate = x.StatusDate,
                    Id = x.Id,
                }).ToList(),

                OrderRatings = order.OrderRatings.Select(x => new OrderRatingDto
                {
                    Id = x.Id,
                    Message = x.Message,
                    OrderId = x.OrderId,
                    RatedEmail = x.RatedEmail,
                    RaterEmail = x.RaterEmail,
                    Rating = x.Rating,
                    OrderRefNumber = order.RefNumber,
                }).ToList(),

                orderAssignments = order.OrderAssigments.Select(x => new OrderAssignmentDto
                {
                    AssingedUserName = x.AssingedUserName,
                    DateAssigned = x.DateAssigned,
                    Id = x.Id,
                    OrderAssignmentType = x.OrderAssignmentType,
                    OrderId = x.OrderId,
                    OrderRefNumber = order.RefNumber,
                    AssignedStatus = x.AssignedStatus,
                    DateStatusChanged = x.DateStatusChanged,
                }).ToList(),

            };
            return Ok(new Wrappers.ApiResponse<OrderDetailDto>(data));
        }


        [HttpPost("UpdateOrderAssignmentStatus")]
        public async Task<ActionResult> UpdateOrderAssignmentStatus(int orderId, string statusName)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'DatabaseContext.Orders'  is null.");
            }

            var response = await this._yaboUtilService.UpdateOrderAssignmentStatus(orderId, statusName);
            if (response.Succeeded)
            {
                return Ok(new { message = "Order Assignment Status updated successfully" });
            }
            else
            {
                return BadRequest(new { message = response.Message });
            }

        }

        [HttpPost("OrderStatusUpdate")]
        public async Task<ActionResult> UpdateOrderStatus(OrderStatusUpdateDto orderStatusUpdateDto)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'DatabaseContext.Orders'  is null.");
            }

            int paidOrderStatusId = _configuration.GetValue<int>(PAID_ORDERSTATUS_ID);
            int newOrderStatusId = _configuration.GetValue<int>(NEW_ORDERSTATUS_ID);

            int completedOrderStatusId = _configuration.GetValue<int>(COMPLETED_ORDERSTATUS_ID);

            if (orderStatusUpdateDto.OrderStatusId == paidOrderStatusId || orderStatusUpdateDto.OrderStatusId == newOrderStatusId)
            {
                return BadRequest(new { message = "You can not update order status to New or Paid" });
            }

            Order? order = await _context.Orders.Include(x => x.OrderAssigments).Include(x => x.OrderItems).Include(x => x.OrderCharges).ThenInclude(y => y.Charge).FirstOrDefaultAsync(x => x.Id == orderStatusUpdateDto.OrderId);
            if (order == null)
            {
                return NotFound(new { message = "Order not found" });
            }

            OrderStatus? orderStatus = await _context.OrderStatus.FindAsync(orderStatusUpdateDto.OrderStatusId);
            if (orderStatus == null)
            {
                return NotFound(new { message = "OrderStatus not found" });
            }



            order.OrderStatusId = orderStatusUpdateDto.OrderStatusId;
            order.UpdatedAt = DateTime.Now;
            _context.Entry(order).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            OrderStatusUpdate orderStatusUpdate = new OrderStatusUpdate
            {
                OrderId = orderStatusUpdateDto.OrderId,
                Status = orderStatus.Name,
                StatusDate = DateTime.Now,
                UpdatedBy = orderStatusUpdateDto.UpdatedBy,
                Description = orderStatusUpdateDto.Description,
                UpdatedAt = DateTime.Now,
                CreatedAt = DateTime.Now,
                Order = order
            };
            _context.Orders.Attach(orderStatusUpdate.Order);
            _context.OrderStatusUpdates.Add(orderStatusUpdate);
            await _context.SaveChangesAsync();

            OrderRecon? orderRecon = await _context.OrderRecons.FirstOrDefaultAsync(x => x.OrderId == order.Id);
            if (orderRecon == null)
            {
                if (orderStatusUpdateDto.OrderStatusId == completedOrderStatusId)
                {
                    decimal totalcharges = 0;
                    decimal totalAmount = 0;
                    foreach (var item in order.OrderItems)
                    {
                        totalAmount += item.Quantity * item.UnitPrice;
                    }


                    foreach (var item in order.OrderCharges)
                    {
                        if (!item.Charge.IsYaboCharge)
                        {
                            if (item.Charge.AmountType == ChargeAmountType.FIXED)
                            {
                                totalcharges += item.Amount;
                            }
                            else
                            {
                                totalcharges += (item.Amount / 100) * totalAmount;
                            }
                        }

                    }

                    decimal partnerAmount = 0.9m * (totalAmount + totalcharges);
                    decimal yaboAmount = order.NetAmount - partnerAmount;

                    OrderRecon orderRecon1 = new OrderRecon
                    {
                        OrderId = order.Id,
                        OrdecCompletionDate = DateTime.Now,
                        Order = order,
                        PartnerShare = partnerAmount,
                        YaboShare = yaboAmount,
                        TotalOrderAmount = order.NetAmount,
                        PartnerUserName = order.OrderAssigments.LastOrDefault(x => x.AssignedStatus == OrderAssignmentStatus.ACCEPTED).AssingedUserName,
                        UpdatedAt = DateTime.Now,
                        CreatedAt = DateTime.Now,
                    };

                    _context.Orders.Attach(orderRecon1.Order);
                    _context.OrderRecons.Add(orderRecon1);
                    await _context.SaveChangesAsync();
                }
            }



            await this._yaboUtilService.SendNotification($"Order {order.RefNumber} status updated to {orderStatus.Name}", order.ClientUserName);

            return Ok(new { message = "Order status updated successfully" });
        }

        [HttpPost("Full")]
        public async Task<ActionResult> PostOrderFull(OrderFullDto Order)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'DatabaseContext.Orders'  is null.");
            }



            //get client details by email
            Client? client = await _context.Clients.Include(x => x.ClientAddresses).FirstOrDefaultAsync(x => x.Email == Order.ClientEmailAddress);

            if (client == null)
            {
                return NotFound(new { message = "User is not registered as a client" });
            }

            //check if any items was added
            if (Order.OrderItems.Count == 0)
            {
                return BadRequest(new { message = "No items added to order" });
            }


            decimal totalCharges = 0;

            var result = await this._yaboUtilService.CustomerHasPaymentProfile(client.Id);
            if (!result)
            {
                return BadRequest(new { message = "Client does not have a payment profile" });
            }

            if (Order.AssignedCleaner != null)
            {
                //check if cleaner exist
                var cleaner = await _context.ServiceProviders.FirstOrDefaultAsync(x => x.Email == Order.AssignedCleaner);
                if (cleaner == null)
                {
                    return BadRequest(new { message = "Yabo Cleaner does not exist" });
                }
            }


            decimal taxPercentage = 0;

            string cityname = client.ClientAddresses?.FirstOrDefault()?.City ?? string.Empty;

            CityTax? cityTax = await _context.CityTaxes.Include(x => x.City).FirstOrDefaultAsync(x => x.City.Name == cityname);
            if (cityTax != null)
            {
                taxPercentage = cityTax.TaxPercentage;
            }

            //query the subscritpionplancharges list based on the subscriptionplanid and get me the sum of the amount


            ServiceDto?[] subscribedServices = new ServiceDto?[] { };
            List<SubscriptionPlanCharge> subscriptionPlanCharges = new List<SubscriptionPlanCharge>();
            List<SubscriptionPlanChargeExemption> subscriptionPlanChargeExemptions = new List<SubscriptionPlanChargeExemption>();
            List<ServiceCharge> serviceCharges = new List<ServiceCharge>();
            //get client subscription if any

            ClientSubscription? clientSubscription = await _context.ClientSubscriptions
            .FirstOrDefaultAsync(x => x.ClientId == client.Id && x.Status && x.ExpiryDate >= DateTime.Now);

            if (clientSubscription != null)
            {
                subscribedServices = await _context.SubscriptionPlanServices.Include(cs => cs.Service).Where(x => x.SubscriptionId == clientSubscription.SubscriptionId)
                  .Select(x => new ServiceDto
                  {
                      Name = x.Service.Name,
                      CategoryId = x.Service.CategoryId,
                      Code = x.Service.Code,
                      Description = x.Service.Description,
                      Id = x.Service.Id,
                      TypeId = x.Service.TypeId,
                      ServiceCategoryName = x.Service.ServiceCategory.Name,
                      ServiceTypeName = x.Service.ServiceType.Name,
                  }).ToArrayAsync();



            }



            int NoOfDays = 0;
            decimal TotalAmount = 0;
            decimal minimumOrderValue = 0;
            decimal discount = 0;

            foreach (var item in Order.OrderItems)
            {

                Price? price = await _context.Prices.Include(x => x.Service).FirstOrDefaultAsync(x => x.ItemId == item.ItemId && x.ServiceId == item.ServiceId && x.PeriodId == item.PeriodId && x.UnitTypeId == item.UnitTypeId);
                if (price == null)
                {
                    return NotFound(new { message = "Item's Price no setup, check with Support Team" });
                }

                if (price.Service.MinOrderValue > minimumOrderValue)
                {
                    minimumOrderValue = price.Service.MinOrderValue;
                }



                if (Array.Exists(subscribedServices, x => x?.Id == item.ServiceId))
                {
                    TotalAmount += 0;
                }
                else
                {
                    TotalAmount += item.Quantity * price.Amount;
                }

                //changing how subscription wors
                //TotalAmount += item.Quantity * price.Amount;

                Period? period = await _context.Periods.FirstOrDefaultAsync(x => x.Id == item.PeriodId);
                if (period != null)
                {
                    //check if period noofdays is greate than nofodays in subscription and pick one
                    if (period.NoOfDays > NoOfDays)
                    {
                        NoOfDays = period.NoOfDays;
                    }
                }
                else
                {
                    return NotFound(new { message = "Delivery period not found" });
                }

                serviceCharges.AddRange(await _context.ServiceCharges.Include(x => x.Charge).Where(spc => spc.ServiceId == item.ServiceId).ToListAsync());



            }

            if (clientSubscription != null)
            {

                subscriptionPlanCharges = await _context.SubscriptionPlanCharges.Include(x => x.Charge).Where(spc => spc.SubscriptionId == clientSubscription.SubscriptionId).
                ToListAsync();

                subscriptionPlanChargeExemptions = await _context.SubscriptionPlanChargeExemptions.Include(x => x.Charge).Where(spc => spc.SubscriptionId == clientSubscription.SubscriptionId).
               ToListAsync();

                totalCharges = 0;
                foreach (var item in subscriptionPlanCharges)
                {
                    if (!subscriptionPlanChargeExemptions.Any(x => x.ChargeId == item.ChargeId))
                    {
                        if (item.Charge.AmountType == ChargeAmountType.FIXED)
                        {
                            totalCharges += item.Amount;
                        }
                        else
                        {
                            totalCharges += (item.Amount / 100) * TotalAmount;
                        }
                    }


                    // Get the sum of the amount
                }

                foreach (var item in serviceCharges)
                {

                    if (!subscriptionPlanChargeExemptions.Any(x => x.ChargeId == item.ChargeId))
                    {
                        if (item.Charge.AmountType == ChargeAmountType.FIXED)
                        {
                            totalCharges += item.Price;
                        }
                        else
                        {
                            totalCharges += (item.Price / 100) * TotalAmount;
                        }
                    }
                    // Get the sum of the amount
                }
            }
            else
            {
                totalCharges = 0;
                foreach (var item in serviceCharges)
                {
                    if (item.Charge.AmountType == ChargeAmountType.FIXED)
                    {
                        totalCharges += item.Price;
                    }
                    else
                    {
                        totalCharges += (item.Price / 100) * TotalAmount;
                    }
                    // Get the sum of the amount
                }
            }



            if (clientSubscription == null && TotalAmount < minimumOrderValue)
            {
                return BadRequest(new { message = $"Minimum order value is {minimumOrderValue}" });
            }

            //check detergents and add them to the total amount

            foreach (var item in Order.OrderDetergents ?? new List<OrderDetergentDto>())
            {
                ServiceDetergent? detergent = await _context.ServiceDetergents.FirstOrDefaultAsync(x => x.ServiceId == item.ServiceId && x.DetergentId == item.DetergentId);
                if (detergent == null)
                {
                    return NotFound(new { message = "Detergent not found" });
                }

                TotalAmount += item.Quantity * detergent.Price;
            }

            //work on detergents and order location from user

            //get order details, with order items, order messages, order status, these should be tabs
            //keep initial order status id in app settings and use it here

            int orderStatusId = _configuration.GetValue<int>(NEW_ORDERSTATUS_ID);

            OrderStatus? orderStatus = await _context.OrderStatus.FindAsync(orderStatusId);
            if (orderStatus == null)
            {
                return NotFound(new { message = "New OrderStatus not found" });
            }


            int paidOrderStatusId = _configuration.GetValue<int>(PAID_ORDERSTATUS_ID);

            OrderStatus? paidOrderStatus = await _context.OrderStatus.FindAsync(paidOrderStatusId);
            if (paidOrderStatus == null)
            {
                return NotFound(new { message = "Paid OrderStatus not found" });
            }

            var orderPickupDate = DateTime.Now > Order.PickupDate ? DateTime.Now : Order.PickupDate;

            bool promoApplied = false;
            // OrderPromoCode? orderPromoCode = null;
            if (!string.IsNullOrEmpty(Order.PromoCode))
            {
                var promoResult = await this.VerifyPromoCode(Order.PromoCode, client.Email);

                if (promoResult.Succeeded)
                {
                    var promoData = (PromoCodeDto)promoResult.Data;
                    if (promoData != null && TotalAmount >= promoData.MinOrderValue && TotalAmount <= promoData.MaxOrderValue)
                    {
                        //apply discount to total amount
                        discount = (promoData.Discount / 100) * TotalAmount;
                        TotalAmount = TotalAmount - discount;
                        promoApplied = true;
                    }
                }
            }

            string orderAssignedUsername = null;

            decimal taxAmount = taxPercentage / 100 * (TotalAmount + totalCharges);

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // Save the order

                    Order data = new Order
                    {
                        ClientUserName = client.Email,
                        OrderDate = DateTime.Now,
                        PickupDate = orderPickupDate,
                        DeliveryDate = orderPickupDate.AddDays(NoOfDays),
                        Discount = discount,
                        GrossAmount = TotalAmount + totalCharges,
                        NetAmount = TotalAmount + totalCharges - discount,
                        RefNumber = this._codeGenService.GenerateOrderCode().Value,
                        TaxAmount = taxAmount,
                        OrderStatusId = orderStatus.Id,
                        OrderStatus = orderStatus,
                        UpdatedAt = DateTime.Now,
                        CreatedAt = DateTime.Now,

                    };

                    _context.OrderStatus.Attach(data.OrderStatus);
                    _context.Orders.Add(data);

                    await _context.SaveChangesAsync();



                    //update order status
                    OrderStatusUpdate orderStatusUpdate = new OrderStatusUpdate
                    {
                        OrderId = data.Id,
                        Status = "New",
                        StatusDate = DateTime.Now,

                        UpdatedAt = DateTime.Now,
                        CreatedAt = DateTime.Now,
                        Order = data
                    };
                    _context.Orders.Attach(orderStatusUpdate.Order);
                    _context.OrderStatusUpdates.Add(orderStatusUpdate);

                    // Get the ID of the order
                    var orderId = data.Id;

                    // Save the order items
                    foreach (var item in Order.OrderItems)
                    {
                        LaundryItem? laundryItem = await _context.LaundryItems.FindAsync(item.ItemId);
                        if (laundryItem == null)
                        {
                            return NotFound(new { message = "LaundryItem not found" });
                        }

                        Service? service = await _context.Services.FindAsync(item.ServiceId);
                        if (service == null)
                        {
                            return NotFound(new { message = "Service not found" });
                        }

                        OrderItem orderItem = new OrderItem
                        {
                            Comments = item.Comments,
                            Discount = item.Discount,
                            ItemId = item.ItemId,
                            Quantity = item.Quantity,
                            ServiceId = item.ServiceId,
                            UnitPrice = item.UnitPrice,
                            NetPrice = item.NetPrice,
                            OrderId = orderId,
                            UpdatedAt = DateTime.Now,
                            CreatedAt = DateTime.Now,
                            LaundryItem = laundryItem,
                            Order = data,
                            Service = service

                        };

                        _context.Orders.Attach(orderItem.Order);
                        _context.Services.Attach(orderItem.Service);
                        _context.LaundryItems.Attach(orderItem.LaundryItem);
                        _context.OrderItems.Add(orderItem);
                    }


                    if (Order.OrderLocation != null)
                    {
                        // Save the order location
                        OrderLocation orderLocation = new OrderLocation
                        {
                            Accuracy = Order.OrderLocation.Accuracy,
                            AddressLine = Order.OrderLocation.AddressLine,
                            City = Order.OrderLocation.City,
                            CountryRegion = Order.OrderLocation.CountryRegion,
                            Latitude = Order.OrderLocation.Latitude,
                            Longitude = Order.OrderLocation.Longitude,
                            OrderId = orderId,
                            PostalCode = Order.OrderLocation.PostalCode,
                            StateProvince = Order.OrderLocation.StateProvince,
                            UpdatedAt = DateTime.Now,
                            CreatedAt = DateTime.Now,
                            Order = data
                        };
                        _context.Orders.Attach(orderLocation.Order);
                        _context.OrderLocations.Add(orderLocation);
                    }
                    else
                    {
                        //find client profile based on the client email and user the client location and address to create the order location
                        Client? dbclient = await _context.Clients.Include(x => x.ClientAddresses).Include(x => x.ClientGeoLocations).FirstOrDefaultAsync(x => x.Email == Order.ClientEmailAddress);

                        if (dbclient == null)
                        {
                            return NotFound(new { message = "Client not found" });
                        }

                        OrderLocation orderLocation = new OrderLocation
                        {
                            Accuracy = dbclient.ClientGeoLocations.Any() ? dbclient.ClientGeoLocations.FirstOrDefault().Accuracy : 0,
                            AddressLine = dbclient.ClientAddresses.FirstOrDefault()?.AddressLine,
                            City = dbclient.ClientAddresses.FirstOrDefault()?.City,
                            CountryRegion = dbclient.ClientAddresses.FirstOrDefault()?.CountryRegion,
                            Latitude = dbclient.ClientGeoLocations.FirstOrDefault().Latitude,
                            Longitude = dbclient.ClientGeoLocations.FirstOrDefault().Longitude,
                            OrderId = orderId,
                            PostalCode = dbclient.ClientAddresses.FirstOrDefault()?.PostalCode,
                            StateProvince = dbclient.ClientAddresses.FirstOrDefault()?.StateProvince,
                            UpdatedAt = DateTime.Now,
                            CreatedAt = DateTime.Now,
                            Order = data
                        };
                        _context.Orders.Attach(orderLocation.Order);
                        _context.OrderLocations.Add(orderLocation);
                    }


                    if (Order.OrderDetergents != null)
                    {

                        foreach (var item in Order.OrderDetergents)
                        {
                            ServiceDetergent? detergent = await _context.ServiceDetergents.Include(x => x.Detergent).Include(x => x.Service).FirstOrDefaultAsync(x => x.ServiceId == item.ServiceId && x.DetergentId == item.DetergentId);
                            if (detergent == null)
                            {
                                return NotFound(new { message = "Detergent not found" });
                            }

                            OrderDetergent orderDetergent = new OrderDetergent
                            {
                                DetergentId = item.DetergentId,
                                OrderId = orderId,
                                Quantity = item.Quantity,
                                UpdatedAt = DateTime.Now,
                                CreatedAt = DateTime.Now,
                                Order = data,
                                Detergent = detergent.Detergent,
                                Service = detergent.Service,
                                ServiceId = detergent.ServiceId,
                                Price = detergent.Price
                            };
                            _context.Orders.Attach(orderDetergent.Order);
                            _context.Detergents.Attach(orderDetergent.Detergent);
                            _context.Services.Attach(orderDetergent.Service);
                            _context.OrderDetergents.Add(orderDetergent);
                        }


                    }

                    if (clientSubscription == null)
                    {

                        // Save the order charges
                        foreach (var charge in serviceCharges)
                        {

                            Charge? newCharge = await _context.Charges.FindAsync(charge.ChargeId);
                            if (newCharge == null)
                            {
                                return NotFound(new { message = "Service not found" });
                            }

                            OrderCharge orderCharge = new OrderCharge
                            {
                                Amount = charge.Price,
                                ChargeDescription = "Subscription Charge",
                                ChargeId = charge.ChargeId,
                                OrderId = orderId,
                                UpdatedAt = DateTime.Now,
                                CreatedAt = DateTime.Now,
                                Charge = newCharge,
                                Order = data,
                                DateAdded = DateTime.Now,

                            };
                            _context.Charges.Attach(orderCharge.Charge);
                            _context.Orders.Attach(orderCharge.Order);
                            _context.OrderCharges.Add(orderCharge);
                        }
                    }
                    else
                    {
                        // Save the order charges
                        foreach (var charge in subscriptionPlanCharges)
                        {

                            Charge? newCharge = await _context.Charges.FindAsync(charge.ChargeId);
                            if (newCharge == null)
                            {
                                return NotFound(new { message = "Service not found" });
                            }

                            OrderCharge orderCharge = new OrderCharge
                            {
                                Amount = charge.Amount,
                                ChargeDescription = "Service Charge",
                                ChargeId = charge.ChargeId,
                                OrderId = orderId,
                                UpdatedAt = DateTime.Now,
                                CreatedAt = DateTime.Now,
                                Charge = charge.Charge,
                                Order = data,

                            };
                            _context.Charges.Attach(orderCharge.Charge);
                            _context.Orders.Attach(orderCharge.Order);
                            _context.OrderCharges.Add(orderCharge);
                        }
                    }
                    if (Order.AssignedCleaner != null)
                    {
                        OrderAssignment orderAssignment = new OrderAssignment
                        {
                            AssingedUserName = Order.AssignedCleaner,
                            DateAssigned = DateTime.Now,
                            OrderAssignmentType = OrderAssignmentType.PROVIDER,
                            OrderId = orderId,
                            UpdatedAt = DateTime.Now,
                            CreatedAt = DateTime.Now,
                            Order = data,
                            AssignedStatus = OrderAssignmentStatus.NEW
                        };
                        _context.Orders.Attach(orderAssignment.Order);
                        _context.OrderAssignments.Add(orderAssignment);

                        orderAssignedUsername = Order.AssignedCleaner;
                    }
                    else
                    {
                        var serviceProvier = await _yaboUtilService.GetClientCloseProvider(Order.ClientEmailAddress);
                        if (serviceProvier != null)
                        {
                            OrderAssignment orderAssignment = new OrderAssignment
                            {
                                AssingedUserName = serviceProvier.Email,
                                DateAssigned = DateTime.Now,
                                OrderAssignmentType = OrderAssignmentType.PROVIDER,
                                OrderId = orderId,
                                UpdatedAt = DateTime.Now,
                                CreatedAt = DateTime.Now,
                                Order = data,
                                AssignedStatus = OrderAssignmentStatus.NEW
                            };
                            _context.Orders.Attach(orderAssignment.Order);
                            _context.OrderAssignments.Add(orderAssignment);

                            orderAssignedUsername = serviceProvier.Email;
                        }


                    }


                    if (promoApplied)
                    {
                        var promoCodeDb = await _context.PromoCodes.FirstOrDefaultAsync(x => x.CodeValue == Order.PromoCode);
                        if (promoCodeDb != null)
                        {
                            promoCodeDb.UsageCount += 1;
                            _context.Entry(promoCodeDb).State = EntityState.Modified;


                            OrderPromoCode orderPromoCodeDb = new OrderPromoCode
                            {
                                OrderId = orderId,

                                UpdatedAt = DateTime.Now,
                                CreatedAt = DateTime.Now,
                                Order = data,
                                PromoCode = promoCodeDb,
                                TotalAmount = discount,
                                PromoId = promoCodeDb.Id,
                            };

                            _context.Orders.Attach(orderPromoCodeDb.Order);
                            _context.PromoCodes.Attach(orderPromoCodeDb.PromoCode);
                            _context.OrderPromoCodes.Add(orderPromoCodeDb);
                        }
                    }



                    await _context.SaveChangesAsync();



                    // Commit the transaction
                    await transaction.CommitAsync();



                    //making order payment and updating order status.

                    var paymentResponse = await this._authorizePaymentService.CreateOrderPayment(data.Id, client.Id);
                    if (paymentResponse.resultCode == OkReponseCode)
                    {
                        data.OrderStatusId = paidOrderStatus.Id;
                        data.OrderStatus = paidOrderStatus;
                        data.UpdatedAt = DateTime.Now;
                        _context.Entry(data).State = EntityState.Modified;
                        await _context.SaveChangesAsync();


                        //update order status
                        OrderStatusUpdate orderStatusUpdatePaid = new OrderStatusUpdate
                        {
                            OrderId = data.Id,
                            Status = "Paid",
                            StatusDate = DateTime.Now,

                            UpdatedAt = DateTime.Now,
                            CreatedAt = DateTime.Now,
                            Order = data
                        };
                        _context.Orders.Attach(orderStatusUpdatePaid.Order);
                        _context.OrderStatusUpdates.Add(orderStatusUpdatePaid);

                        await _context.SaveChangesAsync();


                        if (orderAssignedUsername != null)
                        {
                            await this._yaboUtilService.SendNotification($"Order {data.RefNumber} has been assigned to you, You have 30 minutes to Accept", orderAssignedUsername);
                            var orderAssingedEvent = new OrderAssignedEvent(data.RefNumber, orderAssignedUsername);
                            await _dispatcher.Broadcast(orderAssingedEvent);

                        }

                    }
                    else
                    {
                        await this._yaboUtilService.SendNotification($" Payment for Order {data.RefNumber} failed with the message {paymentResponse.message ?? "Payment Failed"}", data.ClientUserName);

                    }

                    //make  payment.

                    return CreatedAtAction("GetOrder", new { id = data.Id }, _mapper.Map<OrderDto>(data));
                }
                catch (Exception)
                {
                    // If anything goes wrong, roll back the transaction


                    await transaction.RollbackAsync();
                    throw;
                }
            }

        }

        //TODO: when the order is paid, update the assined username.

        [HttpPost("PayOder/{id}")]
        public async Task<ActionResult> PayForOrder(int id)
        {
            if (_context.Orders == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Orders'  is null." });
            }

            Order? order = await _context.Orders.Include(x => x.OrderItems).Include(x => x.OrderAssigments).FirstOrDefaultAsync(x => x.Id == id);
            if (order == null)
            {
                return BadRequest(new
                {
                    message = "Order Id provided does not exist."

                });
            }

            if (order.OrderStatusId != _configuration.GetValue<int>(NEW_ORDERSTATUS_ID))
            {
                return BadRequest(new
                {
                    message = "You can only pay for Order in New State"
                });
            }

            int paidOrderStatusId = _configuration.GetValue<int>(PAID_ORDERSTATUS_ID);

            OrderStatus? paidOrderStatus = await _context.OrderStatus.FindAsync(paidOrderStatusId);
            if (paidOrderStatus == null)
            {
                return NotFound(new { message = "Paid OrderStatus not found" });
            }

            //checkl if order payment does not exist



            Client? client = await _context.Clients.FirstOrDefaultAsync(x => x.Email == order.ClientUserName);
            if (client == null)
            {
                return BadRequest(new
                {
                    message = "Client not found"
                });
            }

            int noOfDays = order.DeliveryDate.Subtract(order.PickupDate).Days;

            var orderPickupDate = DateTime.Now > order.PickupDate ? DateTime.Now : order.PickupDate;

            OrderPayment? orderPayment = await _context.OrderPayments.FirstOrDefaultAsync(x => x.OrderId == order.Id);
            if (orderPayment != null)
            {
                order.OrderStatusId = paidOrderStatus.Id;
                order.OrderStatus = paidOrderStatus;
                order.OrderDate = DateTime.Now;
                order.DeliveryDate = orderPickupDate.AddDays(noOfDays);
                order.UpdatedAt = DateTime.Now;
                _context.Entry(order).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                //update order status
                OrderStatusUpdate orderStatusUpdatePaid = new OrderStatusUpdate
                {
                    OrderId = order.Id,
                    Status = "Paid",
                    StatusDate = DateTime.Now,

                    UpdatedAt = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    Order = order
                };
                _context.Orders.Attach(orderStatusUpdatePaid.Order);
                _context.OrderStatusUpdates.Add(orderStatusUpdatePaid);

                await _context.SaveChangesAsync();
                return Ok(new
                {
                    message = "Order already paid for"
                });
            }


            var paymentResponse = await this._authorizePaymentService.CreateOrderPayment(order.Id, client.Id);
            if (paymentResponse.resultCode == OkReponseCode)
            {
                order.OrderStatusId = paidOrderStatus.Id;
                order.OrderStatus = paidOrderStatus;
                order.OrderDate = DateTime.Now;
                order.DeliveryDate = orderPickupDate.AddDays(noOfDays);
                order.UpdatedAt = DateTime.Now;
                _context.Entry(order).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                //update order status
                OrderStatusUpdate orderStatusUpdatePaid = new OrderStatusUpdate
                {
                    OrderId = order.Id,
                    Status = "Paid",
                    StatusDate = DateTime.Now,

                    UpdatedAt = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    Order = order
                };
                _context.Orders.Attach(orderStatusUpdatePaid.Order);
                _context.OrderStatusUpdates.Add(orderStatusUpdatePaid);

                await _context.SaveChangesAsync();

                //update the assigned username
                string? assignedUsername = order.OrderAssigments.FirstOrDefault()?.AssingedUserName;
                if (assignedUsername != null)
                {
                    await this._yaboUtilService.SendNotification($"Order {order.RefNumber} has been paid for", assignedUsername);

                }

                return Ok(new
                {
                    message = paymentResponse.message
                });


            }
            else
            {
                await this._yaboUtilService.SendNotification($" Payment for Order {order.RefNumber} failed with the message {paymentResponse.message}", order.ClientUserName);

                return BadRequest(new
                {
                    message = paymentResponse.message
                });
            }


        }


        //write a controller that cancels and order using the order Id and setting the status to cancelled
        [HttpPost("Cancel/{id}")]
        public async Task<ActionResult> CancelOrder(int id)
        {
            if (_context.Orders == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Orders'  is null." });
            }

            Order? order = await _context.Orders.Include(x => x.OrderPayments).ThenInclude(y => y.Payment).FirstOrDefaultAsync(x => x.Id == id);
            if (order == null)
            {
                return NotFound(new { message = "Order not found" });
            }

            int cancelOrderStatusId = _configuration.GetValue<int>(CANCEL_ORDERSTATUS_ID);


            int newOrderStatusId = _configuration.GetValue<int>(NEW_ORDERSTATUS_ID);
            int paidOrderStatusId = _configuration.GetValue<int>(PAID_ORDERSTATUS_ID);

            //check if order status is 1 or 3

            if (order.OrderStatusId != newOrderStatusId && order.OrderStatusId != paidOrderStatusId)
            {
                return BadRequest(new { message = "You can only cancel order in New or Paid State" });
            }

            OrderStatus? orderStatus = await _context.OrderStatus.FindAsync(cancelOrderStatusId);
            if (orderStatus == null)
            {
                return NotFound(new { message = "OrderStatus not found" });
            }


            OrderRefund? refund = await _context.OrderRefunds.FirstOrDefaultAsync(x => x.OrderId == order.Id);

            if (refund != null)
            {

                order.OrderStatusId = orderStatus.Id;
                order.OrderStatus = orderStatus;
                order.UpdatedAt = DateTime.Now;

                _context.Entry(order).State = EntityState.Modified;
                await _context.SaveChangesAsync();


                OrderStatusUpdate orderstatuupdate = new OrderStatusUpdate
                {
                    OrderId = order.Id,
                    Status = orderStatus.Name,
                    StatusDate = DateTime.Now,
                    UpdatedBy = order.ClientUserName,
                    Description = "Order Cancelled",
                    UpdatedAt = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    Order = order
                };

                _context.Orders.Attach(orderstatuupdate.Order);
                _context.OrderStatusUpdates.Add(orderstatuupdate);


                await _context.SaveChangesAsync();
                return Ok(new { message = "Order has already been refunded" });
            }

            Client? client = await _context.Clients.FirstOrDefaultAsync(x => x.Email == order.ClientUserName);
            if (client == null)
            {
                return BadRequest(new { message = "Client not found" });
            }
            //get the order status id for cancelled


            order.OrderStatusId = orderStatus.Id;
            order.OrderStatus = orderStatus;
            order.UpdatedAt = DateTime.Now;

            _context.Entry(order).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            //TODO: Refund will be done in another service form now cancel and store it RefundTable
            var orderPayment = order.OrderPayments.Select(x => x.Payment).FirstOrDefault(x => x.PaymentStatus == "Success");
            if (orderPayment != null && orderPayment.ExTransactionId != null)
            {

                //Creating a refund record.
                OrderRefund orderRefund = new OrderRefund
                {

                    RefundAmount = orderPayment.PaymentAmount,
                    RefundStatus = RefundStatus.PENDING,
                    UpdatedAt = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    Order = order,
                    Client = client,
                    RefundReason = "Order Cancelled",
                    ExtransacitonId = orderPayment.ExTransactionId,
                    LastRetryDate = DateTime.Now,
                    RefundRetry = 0,
                    ClientId = client.Id,
                    OrderId = order.Id,

                };
                _context.Clients.Attach(client);
                _context.Orders.Attach(order);
                _context.OrderRefunds.Add(orderRefund);
                await _context.SaveChangesAsync();
            }



            OrderStatusUpdate orderStatusUpdate = new OrderStatusUpdate
            {
                OrderId = order.Id,
                Status = orderStatus.Name,
                StatusDate = DateTime.Now,
                UpdatedBy = order.ClientUserName,
                Description = "Order Cancelled",
                UpdatedAt = DateTime.Now,
                CreatedAt = DateTime.Now,
                Order = order
            };

            _context.Orders.Attach(orderStatusUpdate.Order);
            _context.OrderStatusUpdates.Add(orderStatusUpdate);


            await _context.SaveChangesAsync();

            //send notification to client 
            this._yaboUtilService.SendNotification($"Order {order.RefNumber} has been cancelled", order.ClientUserName);

            return CreatedAtAction("GetOrder", new { id = order.Id }, _mapper.Map<OrderDto>(order));
        }

        // DELETE: api/Order/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            if (_context.Orders == null)
            {
                return NotFound(new { message = "Entity set 'DatabaseContext.Orders'  is null." });
            }
            var Order = await _context.Orders.FindAsync(id);
            if (Order == null)
            {
                return NotFound(new { message = "Order not found" });
            }

            _context.Orders.Remove(Order);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpPost("TestPayment")]
        public async Task<ActionResult> TestOrderPayemt()
        {
            return Ok(await this._authorizePaymentService.CreateOrderPayment(3, 1));
        }

        [HttpPost("GetTransactionDetail")]
        public async Task<ActionResult> TestGetTractionDetails(string refid)
        {
            return Ok(await _authorizePaymentService.GetAuthorizePaymentDetails(refid));
        }

        [HttpPost("TestRefundTransaction")]
        public async Task<ActionResult> TestRefundTransaction(string refid)
        {
            return Ok(await _authorizePaymentService.RefundTransaction(refid, 6));
        }




        [HttpGet("VerifyUserPromoCode")]
        public async Task<ActionResult> VerifyUserPromoCode(string promoCode, string clientemail)
        {
            if (_context.Orders == null)
            {
                return NotFound();
            }


            var promoCodeResponse = await VerifyPromoCode(promoCode, clientemail);
            if (!promoCodeResponse.Succeeded)
            {
                return BadRequest(new { message = promoCodeResponse.Message });
            }



            return Ok(new
            {
                message = promoCodeResponse.Message,
                Data = promoCodeResponse.Data
            });
        }
        private async Task<GenericResponse> VerifyPromoCode(string promoCode, string clientemail)
        {
            PromoCode? promoCodeDb = await _context.PromoCodes.Include(x => x.OrderPromoCodes).ThenInclude(x => x.Order).FirstOrDefaultAsync(x => x.CodeValue == promoCode);
            if (promoCodeDb == null)
            {
                return new GenericResponse
                {

                    Message = "Promo Code not found",
                    Succeeded = false
                };
            }

            if (promoCodeDb.PromoEndDate < DateTime.Now)
            {
                return new GenericResponse
                {

                    Message = "Promo Code has expired",
                    Succeeded = false
                };
            }

            if (promoCodeDb.Status == false)
            {
                return new GenericResponse
                {

                    Message = "Promo Code is not active",
                    Succeeded = false
                };
            }

            if (promoCodeDb.UsageCount == promoCodeDb.UsageLimit)
            {
                return new GenericResponse
                {

                    Message = "Promo Code has reached maximum usage",
                    Succeeded = false
                };
            };

            var client = await _context.Clients.FirstOrDefaultAsync(x => x.Email == clientemail);
            if (client == null)
            {
                return new GenericResponse
                {

                    Message = "Client not found",
                    Succeeded = false
                };
            }

            if (promoCodeDb.OrderPromoCodes.Any(x => x.Order.ClientUserName == clientemail))
            {
                return new GenericResponse
                {

                    Message = "Promo Code has already been used by you",
                    Succeeded = false
                };
            }

            return new GenericResponse
            {
                Data = new PromoCodeDto
                {
                    CodeValue = promoCodeDb.CodeValue,
                    Discount = promoCodeDb.Discount,
                    PromoEndDate = promoCodeDb.PromoEndDate,
                    PromoStartDate = promoCodeDb.PromoStartDate,
                    Status = promoCodeDb.Status,
                    UsageCount = promoCodeDb.UsageCount,
                    UsageLimit = promoCodeDb.UsageLimit,
                    Id = promoCodeDb.Id,
                    CodeName = promoCodeDb.CodeName,
                    MaxOrderValue = promoCodeDb.MaxOrderValue,
                    MinOrderValue = promoCodeDb.MinOrderValue,
                    Description = promoCodeDb.Description,
                },
                Message = "Promo Code is valid",
                Succeeded = true
            };
        }
    }
}
