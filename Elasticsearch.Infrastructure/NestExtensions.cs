using Elasticsearch.Domain.Entity;
using Nest;

namespace Elasticsearch.Infrastructure;

public static class NestExtensions
{
    public static QueryContainer BuildMultiMatchQuery<T>(string queryValue) where T : class
    {
        var fields = typeof(T).GetProperties().Select(p => p.Name.ToLower()).ToArray();

        return new QueryContainerDescriptor<T>()
            .MultiMatch(c => c
                .Type(TextQueryType.Phrase)
                .Fields(f => f.Fields(fields)).Lenient().Query(queryValue));
    }

    public static List<Actors> GetSampleData()
    {
        var list = new List<Actors>
        {
            new() {Id = Guid.NewGuid().ToString(), RegistrationDate = DateTime.Now, BirthDate = new DateTime(1969, 9, 25), Age = 50, TotalMovies = 25, Name = "张三", Description="你好" }
        };
        return list;
    }

    public static double ObterBucketAggregationDouble(AggregateDictionary agg, string bucket)
    {
        return agg.BucketScript(bucket).Value.HasValue ? agg.BucketScript(bucket).Value.Value : 0;
    }
}