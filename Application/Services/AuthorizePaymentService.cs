using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coravel.Events.Interfaces;
using Newtonsoft.Json;
using dotnetbase.Application.Database;
using dotnetbase.Application.Models;
using dotnetbase.Application.Models.AuthorizeNet;
using dotnetbase.Application.ViewModels;
using dotnetbase.Application.Wrappers;

namespace dotnetbase.Application.Services
{
    public class AuthorizePaymentService
    {
        private const int CURRENCYID = 1;
        private const string OkReponseCode = "Ok";

        private readonly DatabaseContext _context;
        private readonly IWebHostEnvironment _env;

        private readonly IDispatcher _dispatcher;
        private readonly IHttpClientFactory _clientFactory;

        public AuthorizePaymentService(DatabaseContext db, IWebHostEnvironment env, IDispatcher dispatcher, IHttpClientFactory clientFactory)
        {
            _context = db;
            _env = env;
            _dispatcher = dispatcher;
            _clientFactory = clientFactory;
        }


        public async Task<AuthorizeGenericResponse> CreateOrderPayment(int orderId, int clientId)
        {

            DateTime payStartDate = DateTime.Now;
            //check if customer profile already exist and consider updates or else new customer profile
            AuthorizeNetCustomerProfile? customerProfileDb = await _context.AuthorizeNetCustomerProfiles.FindAsync(clientId);
            if (customerProfileDb == null || (customerProfileDb.CardPaymentProfileId == null && customerProfileDb.BankAccountPaymentProfileId == null))
            {
                return new AuthorizeGenericResponse
                {
                    errorCode = "Customer Profile not found",
                    message = "Customer Profile not found",
                    resultCode = "NOK"
                };
            };


            Order? order = await _context.Orders.FindAsync(orderId);

            if (order == null)
            {
                return new AuthorizeGenericResponse
                {
                    errorCode = "Order not found",
                    message = "Order not found",
                    resultCode = "NOK"
                };
            };




            var paymentProfileId = customerProfileDb.DefaultPaymentProfileId ?? customerProfileDb.CardPaymentProfileId ?? customerProfileDb.BankAccountPaymentProfileId;
            var customerProfileId = customerProfileDb.CustomerProfileId;
            string PayMethod;
            if (paymentProfileId == customerProfileDb.CardPaymentProfileId)
            {
                PayMethod = "Card";
            }
            else
            {
                PayMethod = "Bank";
            }

            var request = new CreateTransactionRequest
            {


                merchantAuthentication = new MerchantAuthentication
                {
                    name = AuthorizeNetConstants.ApiLoginId,
                    transactionKey = AuthorizeNetConstants.TransactioKeyApi,
                },
                refId = "YB" + order.Id,
                transactionRequest = new TransactionRequest
                {
                    amount = (order.NetAmount + order.TaxAmount).ToString("N"),
                    authorizationIndicatorType = new AuthorizationIndicatorType
                    {
                        authorizationIndicator = "final"
                    },
                    lineItems = new LineItems
                    {
                        lineItem = new LineItem
                        {
                            description = order.ClientUserName + " Order " + order.Id.ToString(),
                            itemId = "Yabo Order " + order.Id.ToString(),
                            name = "Yabo Order " + order.Id.ToString(),

                            quantity = "1",
                            unitPrice = (order.NetAmount + order.TaxAmount).ToString("N"),
                        }
                    },
                    processingOptions = new ProcessingOptions
                    {
                        isSubsequentAuth = "true"
                    },
                    profile = new Models.AuthorizeNet.Profile
                    {
                        customerProfileId = customerProfileId,
                        paymentProfile = new TransactionPaymentProfile
                        {
                            paymentProfileId = paymentProfileId
                        }
                    },
                    transactionType = "authCaptureTransaction"

                }



            };
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };

            string json = JsonConvert.SerializeObject(new { createTransactionRequest = request }, settings);
            json = json.Replace("\\", "");

            var content = new StringContent(json, Encoding.UTF8);

            // var client = new HttpClient();
            var client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));


            var apiResponse = await client.PostAsync(AuthorizeNetConstants.SandboxUrl, content);


            // Check the status code of the response
            if (apiResponse.IsSuccessStatusCode)
            {
                string responseJson = await apiResponse.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<RootTransactionResponse>(responseJson);
                // Read the response content as JSON

                // validate response 
                if (response != null)
                {
                    if (response.messages?.resultCode == "Ok")
                    {

                        if (response.transactionResponse.responseCode == "1")
                        {
                            if (response.messages.message != null)
                            {

                                Currency? currency = await _context.Currencies.FindAsync(CURRENCYID);

                                Payment payment = new Payment
                                {
                                    Currency = currency,
                                    PaymentAmount = order.NetAmount + order.TaxAmount,
                                    PaymentMethod = PayMethod,
                                    PaymentStart = payStartDate,
                                    PaymentStatus = "Success",
                                    PaymentType = "AuthorizeNet",
                                    RefNumber = "YB-" + order.RefNumber,
                                    CreatedAt = DateTime.Now,
                                    CurrencyId = CURRENCYID,
                                    Descripion = "Order for Client " + order.Id.ToString(),
                                    ExTransactionId = response.transactionResponse.transId,
                                    Name = "Order for Client " + order.Id.ToString(),
                                    PaymentEnd = DateTime.Now,
                                };

                                _context.Currencies.Attach(payment.Currency);
                                _context.Payments.Add(payment);
                                await _context.SaveChangesAsync();

                                OrderPayment orderPayment = new OrderPayment
                                {
                                    OrderId = order.Id,
                                    PaymentId = payment.Id,
                                    Order = order,
                                    Payment = payment,
                                    CreatedAt = DateTime.Now,
                                    UpdatedAt = DateTime.Now,

                                };



                                _context.Orders.Attach(orderPayment.Order);
                                _context.Payments.Attach(orderPayment.Payment);
                                _context.OrderPayments.Add(orderPayment);
                                await _context.SaveChangesAsync();


                                return new AuthorizeGenericResponse
                                {
                                    errorCode = response.messages.message[0].code,
                                    message = response.messages.message[0].text,
                                    resultCode = response.messages.resultCode
                                };
                            }
                        }
                        else
                        {
                            return new AuthorizeGenericResponse
                            {
                                errorCode = response.transactionResponse.responseCode,
                                message = response.messages?.message?[0].text,
                                resultCode = "NOK"
                            };
                        }

                    }
                    else
                    {


                        return new AuthorizeGenericResponse
                        {
                            errorCode = response.messages?.message?[0].code,
                            message = response.messages?.message?[0].text,
                            resultCode = response.messages.resultCode
                        };
                    }

                }
                else
                {

                    return new AuthorizeGenericResponse
                    {
                        errorCode = "Null Response.",
                        message = "Null Response.",
                        resultCode = response.messages.resultCode
                    };

                }

            }
            else
            {

                return new AuthorizeGenericResponse
                {
                    errorCode = "Null Response.",
                    message = "Null Response.",
                    resultCode = "NOK"
                };
            }
            return new AuthorizeGenericResponse
            {
                errorCode = "Null Response.",
                message = "Null Response.",
                resultCode = "NOK"
            };
        }

        public async Task<GetTransactionDetailResponse> GetAuthorizePaymentDetails(string transId)
        {

            var request = new GetTransactionDetailsRequest
            {


                merchantAuthentication = new MerchantAuthentication
                {
                    name = AuthorizeNetConstants.ApiLoginId,
                    transactionKey = AuthorizeNetConstants.TransactioKeyApi,
                },
                transId = transId,
            };
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };

            string json = JsonConvert.SerializeObject(new { getTransactionDetailsRequest = request }, settings);
            json = json.Replace("\\", "");

            var content = new StringContent(json, Encoding.UTF8);

            //var client = new HttpClient();
            var client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));


            var apiResponse = await client.PostAsync(AuthorizeNetConstants.SandboxUrl, content);


            // Check the status code of the response
            if (apiResponse.IsSuccessStatusCode)
            {
                string responseJson = await apiResponse.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<GetTransactionDetailResponse>(responseJson);
                // Read the response content as JSON

                // validate response 
                if (response != null)
                {

                    return response;

                }
                else
                {

                    return null;

                }

            }
            else
            {
                return null;

            }

        }


        public async Task<GenericResponse> RefundTransaction(string transId, decimal amount)
        {
            {


                var getTransactionDetail = await this.GetAuthorizePaymentDetails(transId);
                if (getTransactionDetail == null || getTransactionDetail.transaction?.responseCode == null)
                {
                    return new GenericResponse
                    {
                        Message = "Transaction not found",
                        Succeeded = false
                    };
                }

                if (getTransactionDetail.transaction?.responseCode != "1" || getTransactionDetail.transaction.payment == null)
                {
                    return new GenericResponse
                    {
                        Message = "Transaction found, but was not successfull, please contact support",
                        Succeeded = true
                    };
                }

                var request = new CreateRefundTransactionRequest
                {


                    merchantAuthentication = new MerchantAuthentication
                    {
                        name = AuthorizeNetConstants.ApiLoginId,
                        transactionKey = AuthorizeNetConstants.TransactioKeyApi,
                    },
                    refId = transId,
                    transactionRequest = new RefundTransactionRequest
                    {
                        amount = amount.ToString(),
                        refTransId = transId,
                        transactionType = "refundTransaction",
                        payment = getTransactionDetail.transaction.payment
                    }
                };
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    Formatting = Formatting.Indented
                };

                string json = JsonConvert.SerializeObject(new { createTransactionRequest = request }, settings);
                json = json.Replace("\\", "");

                var content = new StringContent(json, Encoding.UTF8);


                var client = _clientFactory.CreateClient();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));


                var apiResponse = await client.PostAsync(AuthorizeNetConstants.SandboxUrl, content);


                // Check the status code of the response
                if (apiResponse.IsSuccessStatusCode)
                {
                    string responseJson = await apiResponse.Content.ReadAsStringAsync();
                    var response = JsonConvert.DeserializeObject<RootTransactionResponse>(responseJson);
                    // Read the response content as JSON

                    // validate response 
                    if (response != null)
                    {



                        var messages = response.messages.message.Select(m => m.text).ToList();

                        if (response.messages?.resultCode == "Ok")
                        {
                            /*  if (response.transactionResponse.responseCode != "1")
                             {
                                 return new GenericResponse
                                 {
                                     Message = string.Join(", ", messages) + "Response Code : " + response.transactionResponse.responseCode,
                                     Succeeded = false
                                 };
                             }
                             return new GenericResponse
                             {
                                 Message = string.Join(", ", messages) + "Response Code : " + response.transactionResponse.responseCode,
                                 Succeeded = response.messages.resultCode == "Ok"
                             }; */

                            return new GenericResponse
                            {
                                Message = string.Join(", ", messages) + "Response Code : " + response.transactionResponse.responseCode,
                                Succeeded = false
                            };
                        }
                        else
                        {


                            return new GenericResponse
                            {
                                Message = string.Join(", ", messages),
                                Succeeded = false
                            };


                        }
                    }
                    else
                    {

                        return new GenericResponse
                        {
                            Message = "Refund failed contact the administrator",
                            Succeeded = false
                        };

                    }

                }
                else
                {
                    return new GenericResponse
                    {
                        Message = "Refund failed contact the administrator",
                        Succeeded = false
                    };


                }

            }
        }



        public async Task<AuthorizeGenericResponse> CreateSubscriptionPayment(SubscriptionPaymentDto subscriptionPaymentDto)
        {

            DateTime payStartDate = DateTime.Now;
            //check if customer profile already exist and consider updates or else new customer profile
            AuthorizeNetCustomerProfile? customerProfileDb = await _context.AuthorizeNetCustomerProfiles.FindAsync(subscriptionPaymentDto.clientId);
            if (customerProfileDb == null || (customerProfileDb.CardPaymentProfileId == null && customerProfileDb.BankAccountPaymentProfileId == null))
            {
                return new AuthorizeGenericResponse
                {
                    errorCode = "Customer Profile not found",
                    message = "Customer Profile not found",
                    resultCode = "NOK"
                };
            };

            ClientSubscriptionDetail? clientSubscription = await _context.ClientSubscriptionDetails.FindAsync(subscriptionPaymentDto.ClientSubscriptionDetailId);

            if (clientSubscription == null)
            {
                return new AuthorizeGenericResponse
                {
                    errorCode = "Client Subscription not found",
                    message = "Client Subscription not found",
                    resultCode = "NOK"
                };
            };




            var paymentProfileId = customerProfileDb.DefaultPaymentProfileId ?? customerProfileDb.CardPaymentProfileId ?? customerProfileDb.BankAccountPaymentProfileId;
            var customerProfileId = customerProfileDb.CustomerProfileId;
            string PayMethod;
            if (customerProfileDb.DefaultPaymentProfileId == customerProfileDb.CardPaymentProfileId)
            {
                PayMethod = "Card";
            }
            else
            {
                PayMethod = "Bank";
            }

            var request = new CreateTransactionRequest
            {


                merchantAuthentication = new MerchantAuthentication
                {
                    name = AuthorizeNetConstants.ApiLoginId,
                    transactionKey = AuthorizeNetConstants.TransactioKeyApi,
                },
                refId = "clientsubscription-" + subscriptionPaymentDto.ClientSubscriptionDetailId.ToString(),
                transactionRequest = new TransactionRequest
                {
                    amount = subscriptionPaymentDto.TotalAmount.ToString("N"),
                    authorizationIndicatorType = new AuthorizationIndicatorType
                    {
                        authorizationIndicator = "final"
                    },
                    lineItems = new LineItems
                    {
                        lineItem = new LineItem
                        {
                            description = subscriptionPaymentDto.PlanName + "Subscription for Client " + subscriptionPaymentDto.clientId.ToString(),
                            itemId = subscriptionPaymentDto.PlanName,
                            name = subscriptionPaymentDto.PlanName,

                            quantity = "1",
                            unitPrice = subscriptionPaymentDto.TotalAmount.ToString("N"),
                        }
                    },
                    processingOptions = new ProcessingOptions
                    {
                        isSubsequentAuth = "true"
                    },
                    profile = new Models.AuthorizeNet.Profile
                    {
                        customerProfileId = customerProfileId,
                        paymentProfile = new TransactionPaymentProfile
                        {
                            paymentProfileId = paymentProfileId
                        }
                    },
                    transactionType = "authCaptureTransaction"

                }



            };
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };

            string json = JsonConvert.SerializeObject(new { createTransactionRequest = request }, settings);
            json = json.Replace("\\", "");

            var content = new StringContent(json, Encoding.UTF8);

            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));


            var apiResponse = await client.PostAsync(AuthorizeNetConstants.SandboxUrl, content);


            // Check the status code of the response
            if (apiResponse.IsSuccessStatusCode)
            {
                string responseJson = await apiResponse.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<RootTransactionResponse>(responseJson);
                // Read the response content as JSON

                // validate response 
                if (response != null)
                {
                    if (response.messages?.resultCode == OkReponseCode)
                    {

                        if (response.transactionResponse.responseCode != "1")
                        {
                            return new AuthorizeGenericResponse
                            {
                                errorCode = response.transactionResponse.responseCode,
                                message = response.messages?.message?[0].text,
                                resultCode = "NOK"
                            };
                        }

                        if (response.messages.message != null)
                        {

                            Currency? currency = await _context.Currencies.FindAsync(CURRENCYID);

                            Payment payment = new Payment
                            {
                                Currency = currency,
                                PaymentAmount = subscriptionPaymentDto.TotalAmount,
                                PaymentMethod = PayMethod,
                                PaymentStart = payStartDate,
                                PaymentStatus = "Success",
                                PaymentType = "AuthorizeNet",
                                RefNumber = "clientsubscription-" + subscriptionPaymentDto.ClientSubscriptionDetailId.ToString(),
                                CreatedAt = DateTime.Now,
                                CurrencyId = CURRENCYID,
                                Descripion = subscriptionPaymentDto.PlanName + "Subscription for Client " + subscriptionPaymentDto.clientId.ToString(),
                                ExTransactionId = response.transactionResponse.refTransID,
                                Name = subscriptionPaymentDto.PlanName,
                                PaymentEnd = DateTime.Now,
                                UpdatedAt = DateTime.Now
                            };

                            _context.Currencies.Attach(payment.Currency);
                            _context.Payments.Add(payment);
                            await _context.SaveChangesAsync();

                            ClientSubscriptionDetailPayment clientSubscriptionDetailPayment = new ClientSubscriptionDetailPayment
                            {
                                ClientSubscriptionDetailId = subscriptionPaymentDto.ClientSubscriptionDetailId,
                                PaymentId = payment.Id,
                                ClientSubscriptionDetail = clientSubscription,
                                Payment = payment,
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now
                            };

                            _context.ClientSubscriptionDetails.Attach(clientSubscriptionDetailPayment.ClientSubscriptionDetail);
                            _context.Payments.Attach(clientSubscriptionDetailPayment.Payment);
                            _context.ClientSubscriptionDetailPayments.Add(clientSubscriptionDetailPayment);
                            await _context.SaveChangesAsync();


                            return new AuthorizeGenericResponse
                            {
                                errorCode = response.messages.message[0].code,
                                message = response.messages.message[0].text,
                                resultCode = response.messages.resultCode
                            };
                        }
                    }
                    else
                    {


                        return new AuthorizeGenericResponse
                        {
                            errorCode = response.messages.message[0].code,
                            message = response.messages.message[0].text,
                            resultCode = response.messages.resultCode
                        };
                    }
                }
                else
                {

                    return new AuthorizeGenericResponse
                    {
                        errorCode = "Null Response.",
                        message = "Null Response.",
                        resultCode = response.messages.resultCode
                    };

                }

            }
            else
            {

                return new AuthorizeGenericResponse
                {
                    errorCode = "Null Response.",
                    message = "Null Response.",
                    resultCode = "NOK"
                };
            }
            return new AuthorizeGenericResponse
            {
                errorCode = "Null Response.",
                message = "Null Response.",
                resultCode = "NOK"
            };
        }

    }
}