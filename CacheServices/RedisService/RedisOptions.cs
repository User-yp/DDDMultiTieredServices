using System.ComponentModel.DataAnnotations;

namespace CacheServices.RedisService;

public class RedisOptions
{
    [Required(ErrorMessage = "redis connection string is required")]
    public string ConnectionString { get; set; }

    [Range(0, 15, ErrorMessage = "redis db number must be between 0 and 15")]
    public int DbNumber { get; set; }

    public RedisOptions()
    {
    }

    public RedisOptions(string connectionString, int dbNumber)
    {
        ConnectionString = connectionString;
        DbNumber = dbNumber;
    }
}