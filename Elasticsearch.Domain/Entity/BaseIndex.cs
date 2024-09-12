namespace Elasticsearch.Domain.Entity;

public abstract class BaseIndex
{
    public string Id { get; set; }
    public DateTime? UpdateTime { get; set; }
}