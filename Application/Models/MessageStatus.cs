using Spark.Library.Database;

namespace dotnetbase.Application.Models;

public class MessageStatus : BaseModel
{
    public const string READ = nameof(READ);
    public const string PENDING = nameof(PENDING);

}