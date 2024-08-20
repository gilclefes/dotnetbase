using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Web.CodeGeneration.Utils;

namespace dotnetbase.Application.ViewModels
{
    public class OrderMessageDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public required string MessageFrom { get; set; }
        public required string MessageTo { get; set; }

        public required string Message { get; set; }

        public string? MessageType { get; set; }

        public required string MessageStatus { get; set; }
        public string? OrderName { get; set; }
    }
}