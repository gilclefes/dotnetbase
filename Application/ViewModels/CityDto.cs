using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.ViewModels
{
    public class CityDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "City Name is required")]
        public required string Name { get; set; }

        public required string Code { get; set; }
        public Boolean? Status { get; set; }

        public required int CountryId { get; set; }

        public string? CountryName { get; set; }


    }
}