using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core.Features;
using Microsoft.EntityFrameworkCore;
using dotnetbase.Application.Database;
using dotnetbase.Application.Filter;
using dotnetbase.Application.Helpers;
using dotnetbase.Application.Models;
using dotnetbase.Application.Models.AuthorizeNet;
using dotnetbase.Application.Services;
using dotnetbase.Application.ViewModels;
using dotnetbase.Application.Wrappers;

using System.Collections.Generic;


using System.Diagnostics;
using NuGet.Protocol;
using Newtonsoft.Json;
using System.Text;


namespace dotnetbase.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthorizeNetCustomerProfileController : ControllerBase
    {
        private readonly DatabaseContext _context;


        private readonly IHttpClientFactory _clientFactory;

        const string Card = "CARD";

        public AuthorizeNetCustomerProfileController(DatabaseContext context, IHttpClientFactory clientFactory)
        {
            _context = context;
            _clientFactory = clientFactory;

        }



        // GET: api/Charge/5
        [HttpGet("{clientId}")]
        public async Task<ActionResult> GetCustomerProfile(int clientId)
        {
            if (_context.Charges == null)
            {
                return NotFound();
            }
            var data = await _context.AuthorizeNetCustomerProfiles.FindAsync(clientId);

            if (data == null)
            {
                return NotFound();
            }


            return Ok(new ApiResponse<AuthorizeNetCustomerProfile>(data));

        }

        [HttpGet("PaymentProfile/{clientId}")]
        public async Task<ActionResult> GetCustomerPaymentProfile(int clientId)
        {
            if (_context.Charges == null)
            {
                return NotFound();
            }

            var clientData = await _context.Clients.FindAsync(clientId);
            if (clientData == null)
            {
                return NotFound(new { message = "Client not found" });
            }

            var data = await _context.AuthorizeNetCustomerProfiles.FindAsync(clientId);

            if (data == null)
            {
                return NotFound();
            }


            var request = new GetCustomerProfileRequest
            {
                merchantAuthentication = new MerchantAuthentication
                {
                    name = AuthorizeNetConstants.ApiLoginId,
                    transactionKey = AuthorizeNetConstants.TransactioKeyApi,
                },
                customerProfileId = data.CustomerProfileId,
                includeIssuerInfo = "true",
            };

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };

            string json = JsonConvert.SerializeObject(new { getCustomerProfileRequest = request }, settings);




            var content = new StringContent(json, Encoding.UTF8);

            //var client = new HttpClient();
            var client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));


            var apiResponse = await client.PostAsync(AuthorizeNetConstants.SandboxUrl, content);

            if (apiResponse.IsSuccessStatusCode)
            {
                string responseJson = await apiResponse.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<GetCustomerProfileResponse>(responseJson);


                // validate response 
                if (response != null)
                {
                    if (response.messages?.resultCode == "Ok")
                    {
                        if (response.messages.message != null)
                        {

                            //saving new ids from authroize.net

                            return Ok(new
                            {
                                title = "Get Customer Profile Request Sucessfull",
                                message = response.messages.message[0].text,
                                errorCode = response.messages.message[0].code,
                                data = response.profile?.paymentProfiles
                            });


                        }
                        else
                        {
                            return BadRequest(new
                            {
                                message = "Null Response.",
                                errorCode = "",
                                title = "Get Customer Profile Request Failed"
                            });
                        }
                    }
                    else
                    {


                        return BadRequest(new
                        {
                            message = response.messages.message[0].text,
                            errorCode = response.messages.message[0].code,
                            title = "Get Customer Profile Request Failed"
                        });
                    }
                }
                else
                {
                    return BadRequest(new
                    {
                        message = "Null Response.",
                        errorCode = "",
                        title = "Get Customer Profile Request Failed"
                    });
                }

            }
            else
            {

                return BadRequest(new
                {
                    message = "Null Response. API response",
                    errorCode = "Status code: " + apiResponse.StatusCode + " Reason phrase: " + apiResponse.ReasonPhrase,
                    title = "Get Customer Profile Request Failed"
                });
            }



        }



        /// <summary>
        /// Creates or updates a customer profile with payment information.
        /// </summary>
        /// <param name="customerProfileDto">The customer profile data transfer object.</param>
        /// <returns>An asynchronous task that represents the HTTP response.</returns>
        [HttpPost("PostCustomerProfile")]
        public async Task<ActionResult> PostCustomerProfile(CustomerProfileDto customerProfileDto)
        {



            if (_context.AuthorizeNetCustomerProfiles == null)
            {
                return Problem("Entity set 'DatabaseContext.AuthorizeNetCustomerProfiles'  is null.");
            }

            if (customerProfileDto.CreditCard == null && customerProfileDto.BankAccount == null)
            {
                return BadRequest(new { message = "Card or Bank Account must be provided" });
            }

            if (customerProfileDto.BillTo == null)
            {
                return BadRequest(new { message = "Customer Address must be provided" });
            }

            //check if customer profile already exist and consider updates or else new customer profile
            AuthorizeNetCustomerProfile? customerProfileDb = await _context.AuthorizeNetCustomerProfiles.FindAsync(customerProfileDto.ClientId);

            //check if cleitn proflie is created, if not  create a profile with the payment
            //if profile is already created, check if bank or card paymentid already exist, if there are 
            // rause update payment request, if they dont create a payment profile request

            if (customerProfileDb != null)
            {

                var resullts = await CreateCustomerPaymentProfile(customerProfileDto, customerProfileDb);

                if (resullts.resultCode == "Ok")
                {
                    return Ok(resullts);
                }
                else
                {
                    return BadRequest(resullts);
                }

            }
            else
            {
                var resullts = await CreateCustomerProfile(customerProfileDto);

                if (resullts.resultCode == "Ok")
                {
                    customerProfileDb = await _context.AuthorizeNetCustomerProfiles.FindAsync(customerProfileDto.ClientId);

                    if (customerProfileDb != null)
                    {
                        var resullts2 = await CreateCustomerPaymentProfile(customerProfileDto, customerProfileDb);
                        if (resullts2.resultCode == "Ok")
                        {
                            return Ok(resullts2);
                        }
                        else
                        {
                            return BadRequest(resullts2);
                        }
                    }
                    else
                    {
                        return BadRequest(new { message = "Customer Profile not found" });
                    }

                }
                else
                {
                    return BadRequest(resullts);
                }

                // return resullts.;
            }



        }



        private async Task<AuthorizeGenericResponse> CreateCustomerPaymentProfile(CustomerProfileDto customerProfileDto, AuthorizeNetCustomerProfile customerProfileDb)
        {

            AuthorizePayment payment;

            if (customerProfileDto.PayType == Card)
            {
                payment = new AuthorizePayment
                {
                    creditCard = new CreditCard
                    {
                        cardNumber = customerProfileDto?.CreditCard?.CardNumber,
                        expirationDate = customerProfileDto.CreditCard.ExpirationDate,
                        cardCode = customerProfileDto.CreditCard.IssuerNumber
                    }
                };

                if (!string.IsNullOrEmpty(customerProfileDb.CardPaymentProfileId))
                {



                    var request = new UpdateCustomerPaymentProfileRequest
                    {
                        customerProfileId = customerProfileDb.CustomerProfileId,

                        paymentProfile = new PaymentProfile
                        {

                            payment = payment,
                            billTo = new BillTo
                            {
                                address = customerProfileDto.BillTo.Address,
                                city = customerProfileDto.BillTo.City,
                                country = customerProfileDto.BillTo.Country,
                                firstName = customerProfileDto.BillTo.FirstName,
                                lastName = customerProfileDto.BillTo.LastName,
                                phoneNumber = customerProfileDto.BillTo.PhoneNumber,
                                state = customerProfileDto.BillTo.State,
                                zip = customerProfileDto.BillTo.Zip,

                            },
                            customerPaymentProfileId = customerProfileDb.CardPaymentProfileId,

                            defaultPaymentProfile = customerProfileDto.CreditCard.IsDefault
                        },
                        validationMode = "liveMode",
                        merchantAuthentication = new MerchantAuthentication
                        {
                            name = AuthorizeNetConstants.ApiLoginId,
                            transactionKey = AuthorizeNetConstants.TransactioKeyApi,
                        },


                    };

                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        Formatting = Formatting.Indented
                    };

                    string json = JsonConvert.SerializeObject(new { updateCustomerPaymentProfileRequest = request }, settings);
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
                        var response = JsonConvert.DeserializeObject<UpdateCustomerPaymentProfileResponse>(responseJson);
                        // Read the response content as JSON

                        // validate response 
                        if (response != null)
                        {
                            if (response.messages?.resultCode == "Ok")
                            {
                                if (response.messages.message != null)
                                {

                                    if (customerProfileDto?.CreditCard.IsDefault == true)
                                    {
                                        customerProfileDb.DefaultPaymentProfileId = customerProfileDb.CardPaymentProfileId;
                                    }
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
                                resultCode = "NOK"
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
                else
                {
                    var request = new CreateCustomerPaymentProfileRequest
                    {
                        customerProfileId = customerProfileDb.CustomerProfileId,

                        paymentProfile = new PaymentProfile
                        {

                            billTo = new BillTo
                            {
                                address = customerProfileDto.BillTo.Address,
                                city = customerProfileDto.BillTo.City,
                                country = customerProfileDto.BillTo.Country,
                                firstName = customerProfileDto.BillTo.FirstName,
                                lastName = customerProfileDto.BillTo.LastName,
                                phoneNumber = customerProfileDto.BillTo.PhoneNumber,
                                state = customerProfileDto.BillTo.State,
                                zip = customerProfileDto.BillTo.Zip,

                            },
                            payment = payment,

                            defaultPaymentProfile = customerProfileDto.CreditCard.IsDefault
                        },
                        validationMode = "liveMode",
                        merchantAuthentication = new MerchantAuthentication
                        {
                            name = AuthorizeNetConstants.ApiLoginId,
                            transactionKey = AuthorizeNetConstants.TransactioKeyApi,
                        },


                    };

                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        Formatting = Formatting.Indented
                    };

                    string json = JsonConvert.SerializeObject(new { createCustomerPaymentProfileRequest = request }, settings);
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
                        var response = JsonConvert.DeserializeObject<CreateCustomerPaymentProfileResponse>(responseJson);
                        // Read the response content as JSON

                        // validate response 
                        if (response != null)
                        {
                            if (response.messages?.resultCode == "Ok")
                            {
                                if (response.messages.message != null)
                                {

                                    //saving new ids from authroize.net
                                    string paymentProfileId = response.customerPaymentProfileId ?? "";
                                    customerProfileDb.CardPaymentProfileId = paymentProfileId;
                                    if (customerProfileDto?.CreditCard.IsDefault == true)
                                    {
                                        customerProfileDb.DefaultPaymentProfileId = paymentProfileId;
                                    }




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
                                resultCode = "NOK"
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


                }
            }
            else
            {
                payment = new AuthorizePayment
                {
                    bankAccount = new BankAccount
                    {
                        accountNumber = customerProfileDto.BankAccount.AccountNumber,
                        // bankName = customerProfileDto.BankAccount.BankName,
                        nameOnAccount = customerProfileDto.BankAccount.NameOnAccount,
                        routingNumber = customerProfileDto.BankAccount.RoutingNumber,
                        //accountType = customerProfileDto.BankAccount.AccountType,
                        // echeckType = "Online"
                    }
                };

                if (!string.IsNullOrEmpty(customerProfileDb.BankAccountPaymentProfileId))
                {



                    var request = new UpdateCustomerPaymentProfileRequest
                    {
                        customerProfileId = customerProfileDb.CustomerProfileId,

                        paymentProfile = new PaymentProfile
                        {

                            payment = payment,
                            billTo = new BillTo
                            {
                                address = customerProfileDto.BillTo.Address,
                                city = customerProfileDto.BillTo.City,
                                country = customerProfileDto.BillTo.Country,
                                firstName = customerProfileDto.BillTo.FirstName,
                                lastName = customerProfileDto.BillTo.LastName,
                                phoneNumber = customerProfileDto.BillTo.PhoneNumber,
                                state = customerProfileDto.BillTo.State,
                                zip = customerProfileDto.BillTo.Zip,

                            },
                            customerPaymentProfileId = customerProfileDb.BankAccountPaymentProfileId,

                            defaultPaymentProfile = customerProfileDto.BankAccount.IsDefault
                        },
                        validationMode = "liveMode",
                        merchantAuthentication = new MerchantAuthentication
                        {
                            name = AuthorizeNetConstants.ApiLoginId,
                            transactionKey = AuthorizeNetConstants.TransactioKeyApi,
                        },


                    };

                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        Formatting = Formatting.Indented
                    };

                    string json = JsonConvert.SerializeObject(new { updateCustomerPaymentProfileRequest = request }, settings);
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
                        var response = JsonConvert.DeserializeObject<UpdateCustomerPaymentProfileResponse>(responseJson);
                        // Read the response content as JSON

                        // validate response 
                        if (response != null)
                        {
                            if (response.messages?.resultCode == "Ok")
                            {
                                if (response.messages.message != null)
                                {

                                    if (customerProfileDto?.BankAccount.IsDefault == true)
                                    {
                                        customerProfileDb.DefaultPaymentProfileId = customerProfileDb.BankAccountPaymentProfileId;
                                    }
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
                                resultCode = "NOK"
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
                else
                {
                    var request = new CreateCustomerPaymentProfileRequest
                    {
                        customerProfileId = customerProfileDb.CustomerProfileId,

                        paymentProfile = new PaymentProfile
                        {

                            payment = payment,
                            billTo = new BillTo
                            {
                                address = customerProfileDto.BillTo?.Address,
                                city = customerProfileDto.BillTo?.City,
                                country = customerProfileDto.BillTo?.Country,
                                firstName = customerProfileDto.BillTo?.FirstName,
                                lastName = customerProfileDto.BillTo?.LastName,
                                phoneNumber = customerProfileDto.BillTo?.PhoneNumber,
                                state = customerProfileDto.BillTo?.State,
                                zip = customerProfileDto.BillTo?.Zip,

                            },


                            defaultPaymentProfile = customerProfileDto.BankAccount.IsDefault
                        },
                        validationMode = "liveMode",
                        merchantAuthentication = new MerchantAuthentication
                        {
                            name = AuthorizeNetConstants.ApiLoginId,
                            transactionKey = AuthorizeNetConstants.TransactioKeyApi,
                        },


                    };

                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        Formatting = Formatting.Indented
                    };

                    string json = JsonConvert.SerializeObject(new { createCustomerPaymentProfileRequest = request }, settings);
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
                        var response = JsonConvert.DeserializeObject<CreateCustomerPaymentProfileResponse>(responseJson);
                        // Read the response content as JSON

                        // validate response 
                        if (response != null)
                        {
                            if (response.messages?.resultCode == "Ok")
                            {
                                if (response.messages.message != null)
                                {

                                    //saving new ids from authroize.net
                                    string paymentProfileId = response.customerPaymentProfileId ?? "";

                                    customerProfileDb.BankAccountPaymentProfileId = paymentProfileId;

                                    if (customerProfileDto?.BankAccount.IsDefault == true)
                                    {
                                        customerProfileDb.DefaultPaymentProfileId = paymentProfileId;
                                    }

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
                                resultCode = "NOK"
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


                }
            }







            return new AuthorizeGenericResponse
            {
                errorCode = "Null Response.",
                message = "Null Response.",
                resultCode = "NOK"
            };
        }

        private async Task<AuthorizeGenericResponse> CreateCustomerProfile(CustomerProfileDto customerProfileDto)
        {



            var request = new CreateCustomerProfileRequest
            {
                profile = new CreateProfile
                {
                    merchantCustomerId = customerProfileDto.ClientId.ToString(),
                    email = customerProfileDto.Email,
                    description = "Customer Profile for " + customerProfileDto.Email,

                },

                merchantAuthentication = new MerchantAuthentication
                {
                    name = AuthorizeNetConstants.ApiLoginId,
                    transactionKey = AuthorizeNetConstants.TransactioKeyApi,
                },


            };
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };

            string json = JsonConvert.SerializeObject(new { createCustomerProfileRequest = request }, settings);
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
                var response = JsonConvert.DeserializeObject<CreateCustomerProfileResponse>(responseJson);
                // Read the response content as JSON

                // validate response 
                if (response != null)
                {
                    if (response.messages?.resultCode == "Ok")
                    {
                        if (response.messages.message != null)
                        {



                            var customerProfileDb = new AuthorizeNetCustomerProfile
                            {
                                Email = customerProfileDto.Email,
                                ClientId = customerProfileDto.ClientId,
                                //BankAccountPaymentProfileId = bankAccountPaymentProfileId,
                                // CardPaymentProfileId = cardPaymentProfileId,
                                CustomerProfileId = response.customerProfileId,

                                // DefaultPaymentProfileId = customerProfileDto.PayType == "Card" ? cardPaymentProfileId : bankAccountPaymentProfileId,
                                ShippingAddressId = response.customerShippingAddressIdList?.Length >= 1 ? response.customerShippingAddressIdList[0] : ""

                            };

                            _context.AuthorizeNetCustomerProfiles.Add(customerProfileDb);
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

