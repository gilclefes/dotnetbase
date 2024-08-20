using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.ViewModels
{
    public class CityTaxDto
    {
        public int Id { get; set; }
        public required string TaxName { get; set; }
        public required decimal TaxPercentage { get; set; }
        public int CityId { get; set; }
        public Boolean Status { get; set; }
        public string? CityName { get; set; }
    }
}