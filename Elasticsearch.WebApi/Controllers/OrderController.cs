using Elasticsearch.Domain.Entity;
using Elasticsearch.Domain.IRepository;
using Microsoft.AspNetCore.Mvc;
using MTS.Infrastructure.Repository;

namespace Elasticsearch.WebApi.Controllers;

[Route("[controller]/[action]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly IOrderRepository orderRepository;

    public OrderController(IOrderRepository orderRepository)
    {
        this.orderRepository =orderRepository;
    }
    [HttpGet]
    public async Task<ActionResult<Order>> GetAllOrder()
    {
        var result = await orderRepository.GetAllAsync();

        return Ok(result);
    }
    [HttpGet("guid")]
    public async Task<ActionResult<Order>> GetAllOrderById(Guid guid)
    {
        var result = await orderRepository.GetOrderByIdAsync(guid);

        return Ok(result);
    }
}
