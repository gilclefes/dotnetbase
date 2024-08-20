using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Spark.Library.Database;

namespace dotnetbase.Application.Models
{
    public class CodeGenerator : BaseModel
    {

        public string EntityName { get; set; }
        public int EntityId { get; set; }


    }
}