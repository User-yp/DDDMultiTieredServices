using Elasticsearch.Domain.Entity;
using Elasticsearch.Domain.IMiddleResp;
using Elasticsearch.Domain.IRepository;
using Elasticsearch.Net;
using Nest;

namespace Elasticsearch.Infrastructure.Repository;

public class ActorRepository(IActorMiddleResp actorMiddleResp) : IActorRepository
{
    private readonly IActorMiddleResp actorMiddleResp = actorMiddleResp;

    public async Task<IApiCallDetails> TestConnectionAsync()
    {
        var result = await actorMiddleResp.TestConnectionAsync();
        return result;
    }
    public async Task InsertManyAsync()
    {
        await actorMiddleResp.InsertManyAsync(NestExtensions.GetSampleData());
    }

    public async Task<ICollection<Actors>> GetAllAsync()
    {
        var result = await actorMiddleResp.GetAllAsync();

        return result.ToList();
    }

    //lowcase
    public async Task<ICollection<Actors>> GetByNameWithTerm(string name)
    {
        var query = new QueryContainerDescriptor<Actors>()
            .Term(p => p.Field(p => p.Name)
            .Value(name).CaseInsensitive().Boost(6.0));
        var result = await actorMiddleResp.SearchAsync(_ => query);

        return result?.ToList();
    }

    //using operator OR in case insensitive
    public async Task<ICollection<Actors>> GetByNameWithMatch(string name)
    {
        var query = new QueryContainerDescriptor<Actors>()
            .Match(p => p.Field(f => f.Name)
            .Query(name).Operator(Operator.And));
        var result = await actorMiddleResp.SearchAsync(_ => query);

        return result?.ToList();
    }

    public async Task<ICollection<Actors>> GetByNameWithMatchPhrase(string name)
    {
        var query = new QueryContainerDescriptor<Actors>()
            .MatchPhrase(p => p.Field(f => f.Name)
            .Query(name));
        var result = await actorMiddleResp.SearchAsync(_ => query);

        return result?.ToList();
    }

    public async Task<ICollection<Actors>> GetByNameWithMatchPhrasePrefix(string name)
    {
        var query = new QueryContainerDescriptor<Actors>()
            .MatchPhrasePrefix(p => p.Field(f => f.Name)
            .Query(name));
        var result = await actorMiddleResp.SearchAsync(_ => query);

        return result?.ToList();
    }

    //contains
    public async Task<ICollection<Actors>> GetByNameWithWildcard(string name)
    {
        var query = new QueryContainerDescriptor<Actors>()
            .Wildcard(w => w.Field(f => f.Name)
            .Value($"*{name}*").CaseInsensitive());
        var result = await actorMiddleResp.SearchAsync(_ => query);

        return result?.ToList();
    }

    public async Task<ICollection<Actors>> GetByNameWithFuzzy(string name)
    {
        var query = new QueryContainerDescriptor<Actors>()
            .Fuzzy(descriptor => descriptor.Field(p => p.Name).Value(name));
        var result = await actorMiddleResp.SearchAsync(_ => query);

        return result?.ToList();
    }

    public async Task<ICollection<Actors>> SearchInAllFiels(string term)
    {
        var query = NestExtensions.BuildMultiMatchQuery<Actors>(term);
        var result = await actorMiddleResp.SearchAsync(_ => query);

        return result.ToList();
    }

    public async Task<ICollection<Actors>> GetByDescriptionMatch(string description)
    {
        //case insensitive
        var query = new QueryContainerDescriptor<Actors>()
            .Match(p => p.Field(f => f.Description).Query(description));
        var result = await actorMiddleResp.SearchAsync(_ => query);

        return result?.ToList();
    }

    public async Task<ICollection<Actors>> GetByNameAndDescriptionMultiMatch(string term)
    {
        var query = new QueryContainerDescriptor<Actors>()
            .MultiMatch(p => p.
            Fields(p => p.
            Field(f => f.Name).
                    Field(d => d.Description)).
                Query(term).Operator(Operator.And));

        var result = await actorMiddleResp.SearchAsync(_ => query);

        return result?.ToList();
    }

    public async Task<ICollection<Actors>> GetActorsCondition(string name, string description, DateTime? birthdate)
    {
        QueryContainer query = new QueryContainerDescriptor<Actors>();

        if (!string.IsNullOrEmpty(name))
        {
            query = query && new QueryContainerDescriptor<Actors>()
                .MatchPhrasePrefix(qs => qs.Field(fs => fs.Name)
                .Query(name));
        }
        if (!string.IsNullOrEmpty(description))
        {
            query = query && new QueryContainerDescriptor<Actors>()
                .MatchPhrasePrefix(qs => qs.Field(fs => fs.Description)
                .Query(description));
        }
        if (birthdate.HasValue)
        {
            query = query && new QueryContainerDescriptor<Actors>()
            .Bool(b => b.Filter(f => f.DateRange(dt => dt
                                       .Field(field => field.BirthDate)
                                       .GreaterThanOrEquals(birthdate)
                                       .LessThanOrEquals(birthdate)
                                       .TimeZone("+00:00"))));
        }

        var result = await actorMiddleResp.SearchAsync(_ => query);

        return result?.ToList();
    }

    public async Task<ICollection<Actors>> GetActorsAllCondition(string term)
    {
        var query = new QueryContainerDescriptor<Actors>()
            .Bool(b => b.Must(m => m.Exists(e => e.Field(f => f.Description))));
        int.TryParse(term, out var numero);

        query = query && new QueryContainerDescriptor<Actors>().Wildcard(w => w.Field(f => f.Name).Value($"*{term}*")) //bad performance, use MatchPhrasePrefix
                || new QueryContainerDescriptor<Actors>().Wildcard(w => w.Field(f => f.Description).Value($"*{term}*")) //bad performance, use MatchPhrasePrefix
                || new QueryContainerDescriptor<Actors>().Term(w => w.Age, numero)
                || new QueryContainerDescriptor<Actors>().Term(w => w.TotalMovies, numero);

        var result = await actorMiddleResp.SearchAsync(_ => query);

        return result?.ToList();
    }

    public async Task<ActorsAggregationModel> GetActorsAggregation()
    {
        var query = new QueryContainerDescriptor<Actors>()
            .Bool(b => b.Must(m => m.Exists(e => e.Field(f => f.Description))));

        var result = await actorMiddleResp.SearchAsync(_ => query, a =>
                    a.Sum("TotalAge", sa => sa.Field(o => o.Age))
                    .Sum("TotalMovies", sa => sa.Field(p => p.TotalMovies))
                    .Average("AvAge", sa => sa.Field(p => p.Age)));

        var totalAge = NestExtensions.ObterBucketAggregationDouble(result.Aggregations, "TotalAge");
        var totalMovies = NestExtensions.ObterBucketAggregationDouble(result.Aggregations, "TotalMovies");
        var avAge = NestExtensions.ObterBucketAggregationDouble(result.Aggregations, "AvAge");

        return new ActorsAggregationModel { TotalAge = totalAge, TotalMovies = totalMovies, AverageAge = avAge };
    }
}
