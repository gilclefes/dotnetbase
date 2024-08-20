using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class AuditLog : BaseModel
{
    public string UserEmail => "John.Doe@gmail.com";
    public required string EntityName { get; set; }
    public required string Action { get; set; }
    public required DateTime Timestamp { get; set; }
    public required string Changes { get; set; }
}