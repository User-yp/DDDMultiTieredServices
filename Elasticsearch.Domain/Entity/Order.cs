namespace Elasticsearch.Domain.Entity;

public class Order : BaseIndex
{
    public Guid Guid { get; set; }
    public Guid TableId { get; set; }
    public Guid OrderId { get; set; }
    public string TableName { get; set; }
    public string ProjectText { get; set; }
}
