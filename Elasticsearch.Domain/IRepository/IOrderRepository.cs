using Elasticsearch.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elasticsearch.Domain.IRepository;

public interface IOrderRepository
{
    Task<ICollection<Order>> GetAllAsync();
    Task<Order> GetOrderByIdAsync(Guid guid);
}
