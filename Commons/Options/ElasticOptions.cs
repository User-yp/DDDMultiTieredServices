namespace Commons.Options;

public class ElasticOptions
{
    public Uri Uri { get; set; }
    public string? DefaultIndex { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
}