using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class Currency : BaseModel
{

    public required string Name { get; set; }

    public required string Code { get; set; }
    public Boolean Status { get; set; }

    public string? Symbol { get; set; }


}