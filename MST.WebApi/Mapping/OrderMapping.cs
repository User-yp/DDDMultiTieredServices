using MTS.Domain.Entity;
using MTS.WebApi.Requset;

namespace MTS.WebApi.Mapping;

public static  class OrderMapping
{
    public static Order AddOrderMapping(this AddOrderRequset requset)
    {
        return new Order(requset.OrderName, requset.ProductName, requset.ProductDescription);
    }
}
