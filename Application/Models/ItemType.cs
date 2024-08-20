using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class ItemType : BaseModel
{

    public ItemType()
    {
        Items = new HashSet<LaundryItem>();
    }

    public required string Name { get; set; }

    public string? Code { get; set; }
    public Boolean? Status { get; set; }
    public string? Description { get; set; }
    public string? Logo { get; set; }

    public virtual ICollection<LaundryItem> Items { get; set; }
}