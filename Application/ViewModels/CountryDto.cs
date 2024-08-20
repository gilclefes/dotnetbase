using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.ViewModels
{
    public class CountryDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(50, ErrorMessage = "Name cannot be longer than 50 characters")]
        public required string Name { get; set; }

        public required string Code { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [DefaultValue(false)]
        public Boolean Status { get; set; }
    }
}