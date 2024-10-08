using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.ViewModels
{
    public class IdTypeDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        public string? Code { get; set; }
        public Boolean Status { get; set; }
    }
}