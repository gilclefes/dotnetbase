using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class OrderAssignment : BaseModel
{
    public int OrderId { get; set; }
    public required string OrderAssignmentType { get; set; } // whether partenr or provider
    public required string AssingedUserName { get; set; }
    public required DateTime DateAssigned { get; set; }
    public string? Comments { get; set; }

    public string? AssignedStatus { get; set; } // whether accepted or rejected

    public DateTime? DateStatusChanged { get; set; }

    public virtual required Order Order { get; set; }
}