using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.ViewModels
{
    public class ChargeCategoryDto
    {

        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public required string Name { get; set; }

        public string? Code { get; set; }
        public Boolean Status { get; set; }
        public string? Description { get; set; }
        public string? Logo { get; set; }
    }
}