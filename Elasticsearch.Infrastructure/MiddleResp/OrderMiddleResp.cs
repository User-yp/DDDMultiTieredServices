using Elasticsearch.Domain.Entity;
using Elasticsearch.Domain.IMiddleResp;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elasticsearch.Infrastructure.MiddleResp;

public class OrderMiddleResp : ElasticBaseRepository<Order>,IOrderMiddleResp
{
    public OrderMiddleResp(IElasticClient elasticClient) : base(elasticClient)
    {
    }
    public override string IndexName { get; } = nameof(Order).ToLower();
}
