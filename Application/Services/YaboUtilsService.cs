using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnetbase.Application.Database;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using FirebaseAdmin.Messaging;
using System.Threading.Tasks;
using dotnetbase.Application.Models;
using Microsoft.EntityFrameworkCore;

using dotnetbase.Application.Wrappers;
using dotnetbase.Application.Events;
using Coravel.Events.Interfaces;
using ILogger = Spark.Library.Logging.ILogger;

namespace dotnetbase.Application.Services
{
    public class YaboUtilsService
    {
        private readonly DatabaseContext _db;
        private readonly IWebHostEnvironment _env;

        private readonly IDispatcher _dispatcher;

        private readonly ILogger _logger;

        private readonly AuthorizePaymentService _authorizePaymentService;

        public YaboUtilsService(DatabaseContext db, IWebHostEnvironment env, IDispatcher dispatcher, AuthorizePaymentService authorizePaymentService, ILogger logger)
        {
            _db = db;
            _env = env;
            _dispatcher = dispatcher;
            _authorizePaymentService = authorizePaymentService;
            _logger = logger;
        }



        public async Task<bool> CustomerHasPaymentProfile(int clientId)
        {
            var customerProfile = await _db.AuthorizeNetCustomerProfiles.FindAsync(clientId);

            if (customerProfile == null || (customerProfile.CardPaymentProfileId == null && customerProfile.BankAccountPaymentProfileId == null))
            {
                return false;
            }

            if (customerProfile.CardPaymentProfileId != null || customerProfile.BankAccountPaymentProfileId != null)
            {
                return true;
            }

            return false;

        }

        public async Task<string> SendNotification(string message, string clientEmail)
        {



            try
            {
                var path = Path.Combine(
                  _env.ContentRootPath,
                  "Storage/yabotest-b1e2dd0f6cd8.json");

                if (FirebaseApp.DefaultInstance == null)
                {
                    var credential = GoogleCredential.FromFile(path);
                    FirebaseApp.Create(new AppOptions()
                    {
                        Credential = credential,
                    });

                }

                var userToken = await _db.UserDeviceTokens.FirstOrDefaultAsync(x => x.Email == clientEmail);

                if (userToken != null)
                {
                    var fcmMessage = new Message()
                    {
                        Notification = new FirebaseAdmin.Messaging.Notification()
                        {
                            Title = "Yabo Message",
                            Body = message,
                        },
                        Token = userToken.DeviceToken,
                    };

                    string response = await FirebaseMessaging.DefaultInstance.SendAsync(fcmMessage);

                    return response;
                }


                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger?.Error($"Notification Error: {ex}");
                return string.Empty;

            }


        }

        public async Task<GenericResponse> SendOrderMessage(string FromUser, string ToUser, int OrderId, string OrderMessage)
        {

            var order = await _db.Orders.FirstOrDefaultAsync(x => x.Id == OrderId);

            if (order == null)
            {
                return new GenericResponse
                {
                    Succeeded = false,
                    Message = "Invalid data"
                };
            }

            var message = new OrderMessage
            {

                Order = order,
                Message = OrderMessage,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                MessageFrom = FromUser,
                MessageTo = ToUser,
                OrderId = OrderId,
                MessageStatus = "NEW",

            };

            _db.Orders.Attach(order);
            _db.OrderMessages.Add(message);

            await _db.SaveChangesAsync();

            return new GenericResponse
            {
                Succeeded = true,
                Message = "Message sent"
            };
        }

        public async Task<GenericResponse> UpdateOrderAssignmentStatus(int orderId, string statusName)
        {


            Order? order = await _db.Orders.Include(x => x.OrderAssigments).FirstOrDefaultAsync(x => x.Id == orderId);

            if (order == null)
            {
                return new GenericResponse
                {
                    Succeeded = false,
                    Message = "Order not found"
                };
            }

            OrderAssignment? orderAssignment = order.OrderAssigments.FirstOrDefault(x => x.AssignedStatus == OrderAssignmentStatus.NEW || x.AssignedStatus == null);

            if (orderAssignment != null)
            {



                if (statusName == OrderAssignmentStatus.ACCEPTED)
                {
                    orderAssignment.AssignedStatus = OrderAssignmentStatus.ACCEPTED;
                    orderAssignment.DateStatusChanged = DateTime.Now;
                    _db.Entry(orderAssignment).State = EntityState.Modified;

                    OrderStatusUpdate orderStatusUpdate = new OrderStatusUpdate
                    {
                        OrderId = orderAssignment.OrderId,
                        Status = OrderAssignmentStatus.ACCEPTED,
                        StatusDate = DateTime.Now,
                        UpdatedBy = orderAssignment.AssingedUserName,
                        Description = "Order Accepted",
                        UpdatedAt = DateTime.Now,
                        CreatedAt = DateTime.Now,
                        Order = order
                    };

                    _db.Orders.Attach(orderStatusUpdate.Order);
                    _db.OrderStatusUpdates.Add(orderStatusUpdate);


                    if (orderAssignment.OrderAssignmentType == OrderAssignmentType.PROVIDER)
                    {
                        Models.ServiceProvider? provider = await _db.ServiceProviders.FirstOrDefaultAsync(x => x.Email == orderAssignment.AssingedUserName);
                        if (provider != null)
                        {
                            var rating = provider.Rating ?? 100;
                            //calculate 5 percent of the current rating and add that value to rating  if the value is less than 100 and the new value should not be more than 100
                            provider.Rating = rating + (rating * 5 / 100) > 100 ? 100 : rating + (rating * 5 / 100);

                            _db.Entry(provider).State = EntityState.Modified;
                        }
                    }
                    else if (orderAssignment.OrderAssignmentType == OrderAssignmentType.PARTNER)
                    {
                        Partner? partner = await _db.Partners.FirstOrDefaultAsync(x => x.Email == orderAssignment.AssingedUserName);
                        if (partner != null)
                        {
                            var rating = partner.Rating ?? 100;
                            //calculate 5 percent of the current rating and add that value to rating  if the value is less than 100 and the new value should not be more than 100
                            partner.Rating = rating + (rating * 5 / 100) > 100 ? 100 : rating + (rating * 5 / 100);

                            _db.Entry(partner).State = EntityState.Modified;
                        }
                    }

                    await _db.SaveChangesAsync();

                    await SendNotification($"Order {order.RefNumber} status updated to ACCEPTED", order.ClientUserName);
                }
                else if (statusName == OrderAssignmentStatus.REJECTED)
                {
                    orderAssignment.AssignedStatus = OrderAssignmentStatus.REJECTED;
                    orderAssignment.DateStatusChanged = DateTime.Now;
                    _db.Entry(orderAssignment).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    var rejectedEmails = await _db.OrderAssignments.Where(x => x.OrderId == orderId && x.AssignedStatus == OrderAssignmentStatus.REJECTED).Select(x => x.AssingedUserName).ToArrayAsync();
                    var closedProvider = await this.GetClientCloseProviderExcludeRejected(order.ClientUserName, rejectedEmails);
                    if (closedProvider != null)
                    {
                        OrderAssignment orderAssignmentNew = new OrderAssignment
                        {
                            AssingedUserName = closedProvider.Email,
                            DateAssigned = DateTime.Now,
                            OrderAssignmentType = OrderAssignmentType.PROVIDER,
                            OrderId = orderId,
                            UpdatedAt = DateTime.Now,
                            CreatedAt = DateTime.Now,
                            Order = order,
                            AssignedStatus = OrderAssignmentStatus.NEW
                        };
                        _db.Orders.Attach(orderAssignmentNew.Order);
                        _db.OrderAssignments.Add(orderAssignmentNew);

                        if (orderAssignment.OrderAssignmentType == OrderAssignmentType.PROVIDER)
                        {
                            Models.ServiceProvider? provider = await _db.ServiceProviders.FirstOrDefaultAsync(x => x.Email == orderAssignment.AssingedUserName);
                            if (provider != null)
                            {
                                var rating = provider.Rating ?? 100;
                                //calculate 5 percent of the current rating and add that value to rating  if the value is less than 100 and the new value should not be more than 100
                                provider.Rating = rating - (rating * 10 / 100) < 0 ? 0 : rating - (rating * 10 / 100);

                                _db.Entry(provider).State = EntityState.Modified;
                            }
                        }
                        else if (orderAssignment.OrderAssignmentType == OrderAssignmentType.PARTNER)
                        {
                            Partner? partner = await _db.Partners.FirstOrDefaultAsync(x => x.Email == orderAssignment.AssingedUserName);
                            if (partner != null)
                            {
                                var rating = partner.Rating ?? 100;
                                //calculate 5 percent of the current rating and add that value to rating  if the value is less than 100 and the new value should not be more than 100
                                partner.Rating = rating - (rating * 10 / 100) < 0 ? 0 : rating - (rating * 10 / 100);

                                _db.Entry(partner).State = EntityState.Modified;
                            }
                        }

                        await _db.SaveChangesAsync();

                        await SendNotification($"Order {order.RefNumber} has been assigned to you, You have 30 minutes to Accept", closedProvider.Email);

                        var orderAssingedEvent = new OrderAssignedEvent(order.RefNumber, closedProvider.Email);
                        await _dispatcher.Broadcast(orderAssingedEvent);
                    }

                }

                return new GenericResponse
                {
                    Succeeded = true,
                    Message = "Order assignment updated"
                };

            }
            else
            {
                return new GenericResponse
                {
                    Succeeded = false,
                    Message = "Order assignment not found"
                };
            }
        }

        public async Task<GenericResponse> ProcessRefundTransaction(int refundId)
        {
            var OrderRefund = await _db.OrderRefunds.FirstOrDefaultAsync(x => x.Id == refundId);
            if (OrderRefund == null)
            {
                return new GenericResponse
                {
                    Succeeded = false,
                    Message = "Refund not found"
                };
            }

            var refund = await _authorizePaymentService.RefundTransaction(OrderRefund.ExtransacitonId, OrderRefund.RefundAmount);


            OrderRefundDetail orderRefundDetails = new OrderRefundDetail
            {

                OrderRefund = OrderRefund,
                RequestDate = DateTime.Now,
                OrderRefundId = OrderRefund.Id,
                RefundStatus = RefundStatus.COMPLETED,
                ResponseDetails = refund.Message,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _db.OrderRefunds.Attach(OrderRefund);
            _db.OrderRefundDetails.Add(orderRefundDetails);


            OrderRefund.LastRetryDate = DateTime.Now;
            OrderRefund.RefundRetry = OrderRefund.RefundRetry + 1;
            OrderRefund.UpdatedAt = DateTime.Now;

            _db.Entry(OrderRefund).State = EntityState.Modified;

            if (refund.Succeeded)
            {
                orderRefundDetails.RefundStatus = RefundStatus.COMPLETED;
                OrderRefund.RefundStatus = RefundStatus.COMPLETED;
                await _db.SaveChangesAsync();
                return new GenericResponse
                {
                    Succeeded = true,
                    Message = "Refund processed"
                };
            }
            else
            {

                orderRefundDetails.RefundStatus = RefundStatus.FAILED;
                OrderRefund.RefundStatus = RefundStatus.PENDING;
                await _db.SaveChangesAsync();

                return new GenericResponse
                {
                    Succeeded = false,
                    Message = "Refund failed"
                };
            }
        }


        public async Task<Partner> GetClientClosePartner(string email)
        {

            var client = await _db.Clients.Include(x => x.ClientGeoLocations).FirstOrDefaultAsync(x => x.Email == email);

            if (client == null)
            {
                return null;
            }

            var clientCity = client.ClientAddresses.FirstOrDefault()?.City;



            var partners = await _db.Partners.Include(x => x.PartnerGeoLocations).Include(x => x.PartnerAddresses).Where(x => x.PartnerAddresses.Any(y => y.City == clientCity)).ToListAsync();
            var pagedData = partners
               .OrderBy(p => HaversineDistance(client.ClientGeoLocations.FirstOrDefault()?.Latitude ?? 0, client.ClientGeoLocations.FirstOrDefault()?.Longitude ?? 0, p.PartnerGeoLocations.FirstOrDefault()?.Latitude ?? 0, p.PartnerGeoLocations.FirstOrDefault()?.Longitude ?? 0))
               .FirstOrDefault();

            return pagedData;
        }

        public async Task<Models.ServiceProvider> GetClientCloseProvider(string email)
        {

            var client = await _db.Clients.Include(x => x.ClientGeoLocations).FirstOrDefaultAsync(x => x.Email == email);

            if (client == null)
            {
                return null;
            }

            var clientCity = client.ClientAddresses.FirstOrDefault()?.City;



            var partners = await _db.ServiceProviders.Include(x => x.ServiceProviderGeoLocations).Include(x => x.ServiceProviderAddresses).Where(x => x.ServiceProviderAddresses.Any(y => y.City == clientCity)).ToListAsync();
            var pagedData = partners
               .OrderBy(p => HaversineDistance(client.ClientGeoLocations.FirstOrDefault()?.Latitude ?? 0, client.ClientGeoLocations.FirstOrDefault()?.Longitude ?? 0, p.ServiceProviderGeoLocations.FirstOrDefault()?.Latitude ?? 0, p.ServiceProviderGeoLocations.FirstOrDefault()?.Longitude ?? 0))
               .FirstOrDefault();

            return pagedData;
        }

        public async Task<Models.ServiceProvider> GetClientCloseProviderExcludeRejected(string email, string[] rejectedProviders)
        {

            var client = await _db.Clients.Include(x => x.ClientGeoLocations).FirstOrDefaultAsync(x => x.Email == email);

            if (client == null)
            {
                return null;
            }

            var clientCity = client.ClientAddresses.FirstOrDefault()?.City;

            var partners = await _db.ServiceProviders
                .Include(x => x.ServiceProviderGeoLocations)
                .Include(x => x.ServiceProviderAddresses)
                .Where(x => x.ServiceProviderAddresses.Any(y => y.City == clientCity) && !rejectedProviders.Contains(x.Email))
                .ToListAsync();

            var pagedData = partners
               .OrderBy(p => HaversineDistance(client.ClientGeoLocations.FirstOrDefault()?.Latitude ?? 0, client.ClientGeoLocations.FirstOrDefault()?.Longitude ?? 0, p.ServiceProviderGeoLocations.FirstOrDefault()?.Latitude ?? 0, p.ServiceProviderGeoLocations.FirstOrDefault()?.Longitude ?? 0))
               .FirstOrDefault();

            return pagedData;
        }


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