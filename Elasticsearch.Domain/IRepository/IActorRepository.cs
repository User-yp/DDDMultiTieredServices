using Elasticsearch.Domain.Entity;
using Elasticsearch.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elasticsearch.Domain.IRepository;

public interface IActorRepository
{
    Task<IApiCallDetails> TestConnectionAsync();
    Task InsertManyAsync();
    Task<ICollection<Actors>> GetAllAsync();
    Task<ICollection<Actors>> GetByNameWithTerm(string name);
    Task<ICollection<Actors>> GetByNameWithMatch(string name);
    Task<ICollection<Actors>> GetByNameAndDescriptionMultiMatch(string term);
    Task<ICollection<Actors>> GetByNameWithMatchPhrase(string name);
    Task<ICollection<Actors>> GetByNameWithMatchPhrasePrefix(string name);
    Task<ICollection<Actors>> GetByNameWithWildcard(string name);
    Task<ICollection<Actors>> GetByNameWithFuzzy(string name);
    Task<ICollection<Actors>> SearchInAllFiels(string term);
    Task<ICollection<Actors>> GetByDescriptionMatch(string description);
    Task<ICollection<Actors>> GetActorsCondition(string name, string description, DateTime? birthdate);
    Task<ICollection<Actors>> GetActorsAllCondition(string term);
    Task<ActorsAggregationModel> GetActorsAggregation();
}
