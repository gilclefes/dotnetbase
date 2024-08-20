using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.ViewModels
{
    public class ContactUsDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public required string Message { get; set; }
        public bool Read { get; set; }

        public string? ReadBy { get; set; }
        public DateTime? ReadAt { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}