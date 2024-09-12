using Elasticsearch.Domain.Entity;
using Elasticsearch.Domain.IMiddleResp;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elasticsearch.Infrastructure.MiddleResp;

public class ActorMiddleResp:ElasticBaseRepository<Actors>, IActorMiddleResp
{

    public ActorMiddleResp(IElasticClient elasticClients) : base(elasticClients)
    {

    }
    public override string IndexName { get; } = nameof(Actors).ToLower();

}
