using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NuGet.Protocol.Plugins;

namespace dotnetbase.Application.ViewModels
{
    public class LaundryItemDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        public string? Code { get; set; }
        public Boolean? Status { get; set; }
        public string? Description { get; set; }

        public int ItemTypeId { get; set; }
        public string? ItemTypeName { get; set; }
    }
}