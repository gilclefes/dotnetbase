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
    public class UtilsService
    {
        private readonly DatabaseContext _db;
        private readonly IWebHostEnvironment _env;

        private readonly IDispatcher _dispatcher;

        private readonly ILogger _logger;



        public UtilsService(DatabaseContext db, IWebHostEnvironment env, IDispatcher dispatcher, ILogger logger)
        {
            _db = db;
            _env = env;
            _dispatcher = dispatcher;

            _logger = logger;
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
                            Title = "DotNetBase Message",
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