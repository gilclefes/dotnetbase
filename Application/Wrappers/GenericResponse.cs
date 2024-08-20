using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.Wrappers
{
    public class GenericResponse
    {
        public bool Succeeded { get; set; }
        public string? Message { get; set; }
        public Object? Data { get; set; }
    }
}