using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class ChargeCategory : BaseModel
{

    public ChargeCategory()
    {
        Charges = new HashSet<Charge>();
    }
    public required string Name { get; set; }

    public string? Code { get; set; }
    public Boolean? Status { get; set; }
    public string? Description { get; set; }
    public string? Logo { get; set; }

    public virtual ICollection<Charge> Charges { get; set; }
}