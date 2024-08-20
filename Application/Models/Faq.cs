using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Spark.Library.Database;

namespace dotnetbase.Application.Models
{
    public class Faq : BaseModel
    {
        public required string Question { get; set; }
        public required string Answer { get; set; }
        public int Rank { get; set; }
        public Boolean Status { get; set; }
    }
}