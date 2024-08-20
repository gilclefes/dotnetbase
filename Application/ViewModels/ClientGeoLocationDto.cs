using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.ViewModels
{
    public class ClientGeoLocationDto
    {
        public int Id { get; set; }
        public int ClientId { get; set; }

        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public decimal Accuracy { get; set; }

        public decimal? AltitudeAccuracy { get; set; }

        public decimal? Altitude { get; set; }

        public decimal? Speed { get; set; }

        public decimal? Heading { get; set; }

        public string? ClientName { get; set; }
    }
}