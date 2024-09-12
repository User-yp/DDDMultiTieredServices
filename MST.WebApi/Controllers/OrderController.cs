using ASPNETCore;
using EventBus;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MTS.Domain;
using MTS.Domain.Entity;
using MTS.Infrastructure;
using MTS.WebApi.Mapping;
using MTS.WebApi.Requset_Validator;
using System.Runtime.CompilerServices;

namespace MTS.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[UnitOfWork(typeof(BaseDbContext))]
public class OrderController : ControllerBase
{
    private readonly DomainService domainService;
    private readonly BaseDbContext dbContext;
    private readonly IEventBus eventBus;
    private readonly IValidator validator;

    public OrderController(DomainService domainService, BaseDbContext dbContext,IEventBus eventBus, IValidator<AddOrderRequset> validator)
    {
        this.domainService = domainService;
        this.dbContext = dbContext;
        this.eventBus = eventBus;
        this.validator = validator;
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

    [HttpPost("[action]")]
    public async Task<ActionResult<string>> TestActionAsync()
    {
        eventBus.Publish("RabbitMqController", "eventTest");
        return ("Success!");
    }

    [HttpGet("")]
    public async Task<ActionResult<List<Order>>> GetAllAsync()
    {
        (var ope, var res) = await domainService.GetAllOrderAsync();
        if (!ope.Succeeded)
            return BadRequest(ope.Errors);
        return Ok(res);
    }

    [HttpPost("")]
    public async Task<ActionResult<bool>> AddOrderAsync([FromBody] AddOrderRequset request)
    {
        var val = await validator.ValidateAsync(new ValidationContext<AddOrderRequset>(request));
        if (!val.IsValid)
            return BadRequest(new { Code = 401, Msg = val.Errors.ToArray().ToString() });
        (var ope, var res) = await domainService.AddOrderAsync(request.AddOrderMapping());
        if (!ope.Succeeded)
            return BadRequest(ope.Errors);
        return Ok(res);
    }

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
    [HttpDelete("{id}")]
    public async Task<ActionResult<Order>> DeletedOderByIdAsync([FromRoute] Guid id)
    {
        (var ope, var res) = await domainService.DeletedByIdAsync(id);
        if (!ope.Succeeded)
            return BadRequest(ope.Errors);
        return Ok(res);
    }
}
