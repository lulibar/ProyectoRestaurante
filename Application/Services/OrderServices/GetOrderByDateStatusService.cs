using Application.Interfaces.IOrder;
using Application.Interfaces.IOrder.IOrderService;
using Application.Models.Response.OrdersResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.OrderServices
{
    public class GetOrderByDateStatusService : IGetOrderByDateStatus
    {
        private readonly IOrderQuery _orderQuery;

        public Task<IEnumerable<OrderDetailsResponse?>> GetOrderDateStatus(DateTime? from, DateTime? to, int? statusid)
        {
            
        }
    }
}
