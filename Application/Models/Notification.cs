using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Spark.Library.Database;

namespace dotnetbase.Application.Models
{
    public class Notification : BaseModel
    {
        public required string Sender { get; set; }
        public required string Receiver { get; set; }
        public required string Subject { get; set; }
        public required string Message { get; set; }
        public DateTime SentDate { get; set; }
        public string? CC { get; set; }
        public string? BBC { get; set; }
        public string? Attachments { get; set; }
        public required string NotificationType { get; set; }
        public required string NotificationStatus { get; set; }

        public DateTime? EndDate { get; set; }
    }
}