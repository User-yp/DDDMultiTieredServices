using Elasticsearch.Domain.Entity;
using Elasticsearch.Domain.IRepository;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Elasticsearch.WebApi.Controllers;
[Route("[controller]/[action]")]
[ApiController]
public class ActorsController : ControllerBase
{
    private readonly IActorRepository actorRepository;

    public ActorsController(IActorRepository actorRepository)
    {
        this.actorRepository = actorRepository;
    }
    [HttpGet]
    public async Task<ActionResult> Connection()
    {
        var res = await actorRepository.TestConnectionAsync();
        return Ok(res.DebugInformation);
    }
    [HttpPost("sample")]
    public async Task<ActionResult> PostSampleData()
    {
        await actorRepository.InsertManyAsync();

        return Ok(new { Result = "Data successfully registered with Elasticsearch" });
    }

    [HttpPost("exception")]
    public IActionResult PostException()
    {
        throw new Exception("Generate sample exception");
    }

    [HttpGet("")]
    public async Task<ActionResult<Actors>> GetAllAct()
    {
        var result = await actorRepository.GetAllAsync();

        return Ok(result);
    }

    [HttpGet("name-match")]
    public async Task<ActionResult> GetByNameWithMatch([FromQuery] string name)
    {
        var result = await actorRepository.GetByNameWithMatch(name);

        return Ok(result);
    }

    [HttpGet("name-multimatch")]
    public async Task<ActionResult> GetByNameAndDescriptionMultiMatch([FromQuery] string term)
    {
        var result = await actorRepository.GetByNameAndDescriptionMultiMatch(term);

        return Ok(result);
    }

    [HttpGet("name-matchphrase")]
    public async Task<ActionResult> GetByNameWithMatchPhrase([FromQuery] string name)
    {
        var result = await actorRepository.GetByNameWithMatchPhrase(name);

        return Ok(result);
    }

    [HttpGet("name-matchphraseprefix")]
    public async Task<ActionResult> GetByNameWithMatchPhrasePrefix([FromQuery] string name)
    {
        var result = await actorRepository.GetByNameWithMatchPhrasePrefix(name);

        return Ok(result);
    }

    [HttpGet("name-term")]
    public async Task<ActionResult> GetByNameWithTerm([FromQuery] string name)
    {
        var result = await actorRepository.GetByNameWithTerm(name);

        return Ok(result);
    }

    [HttpGet("name-wildcard")]
    public async Task<ActionResult> GetByNameWithWildcard([FromQuery] string name)
    {
        var result = await actorRepository.GetByNameWithWildcard(name);

        return Ok(result);
    }

    [HttpGet("name-fuzzy")]
    public async Task<ActionResult> GetByNameWithFuzzy([FromQuery] string name)
    {
        var result = await actorRepository.GetByNameWithFuzzy(name);

        return Ok(result);
    }

    [HttpGet("description-match")]
    public async Task<ActionResult> GetByDescriptionMatch([FromQuery] string description)
    {
        var result = await actorRepository.GetByDescriptionMatch(description);

        return Ok(result);
    }

    [HttpGet("all-fields")]
    public async Task<ActionResult> SearchAllProperties([FromQuery] string term)
    {
        var result = await actorRepository.SearchInAllFiels(term);

        return Ok(result);
    }

    [HttpGet("condiction")]
    public async Task<ActionResult> GetByCondictions([FromQuery] string name, [FromQuery] string description, [FromQuery] DateTime? birthdate)
    {
        var result = await actorRepository.GetActorsCondition(name, description, birthdate);

        return Ok(result);
    }

    [HttpGet("term")]
    public async Task<ActionResult> GetByAllCondictions([FromQuery] string term)
    {
        var result = await actorRepository.GetActorsAllCondition(term);

        return Ok(result);
    }

    [HttpGet("aggregation")]
    public async Task<ActionResult> GetActorsAggregation()
    {
        var result = await actorRepository.GetActorsAggregation();

        return Ok(result);
    }
}
