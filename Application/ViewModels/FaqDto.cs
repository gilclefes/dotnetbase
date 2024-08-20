using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.ViewModels
{
    public class FaqDto
    {
        public int Id { get; set; }
        public required string Question { get; set; }
        public required string Answer { get; set; }
        public int Rank { get; set; }
        public Boolean Status { get; set; }
    }
}