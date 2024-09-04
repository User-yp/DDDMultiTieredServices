using ASPNETCore;
using Microsoft.AspNetCore.Mvc;
using MTS.Domain;
using MTS.Domain.Entity;
using MTS.Infrastructure;

namespace MTS.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[UnitOfWork(typeof(BaseDbContext))]
public class OrderController : ControllerBase
{
    private readonly DomainService domainService;
    private readonly BaseDbContext dbContext;

    public OrderController(DomainService domainService, BaseDbContext dbContext)
    {
        this.domainService = domainService;
        this.dbContext = dbContext;
    }

    #region Init
    /*[HttpPost]
    public async Task<ActionResult<string>> InitAsync()
    {
        List<Order> orders = [];
        List<Model> models = [];
        for (int i = 1; i < 11; i++)
        {
            Order order = new Order($"名字{i}", $"产品名{i}", $"产品描述{i}");
            Model model = new Model($"名字{i}", $"版本{i}", $"模型描述{i}");
            orders.Add(order);
            models.Add(model);
        }
        await dbContext.AddRangeAsync(models);
        await dbContext.SaveChangesAsync();

        return Ok("Success!");
    }*/
    #endregion

    [HttpGet("{id}")]
    public async Task<ActionResult<Order>> GetOderByIdAsync([FromRoute] Guid id)
    {
        (var ope, var res) = await domainService.GetOrderByIdAsync(id);
        if (!ope.Succeeded)
            return BadRequest(ope.Errors);
        return Ok(res);
    }
    [HttpPost("{id},{name}")]
    public async Task<ActionResult<bool>> UpdateNameByIdAsync([FromRoute] Guid id,string name)
    {
        (var ope, var res) = await domainService.UpdateNameByIdAsync(id,name);
        if (!ope.Succeeded)
            return BadRequest(ope.Errors);
        return Ok(res);
    }
}
