using Application.Enums;
using Application.Interfaces.IOrder;
using Application.Interfaces.IOrder.IOrderService;
using Application.Interfaces.IOrderItem;
using Application.Models.Request;
using Application.Models.Response.OrdersResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.OrderServices
{
    public class UpdateOrderItemStatusService : IUpdateOrderItemStatusService
    {

        private readonly IOrderQuery _orderQuery;
        private readonly IOrderCommand _orderCommand;

        public UpdateOrderItemStatusService (IOrderQuery orderQuery, IOrderCommand orderCommand)
        {
            _orderQuery = orderQuery;
            _orderCommand = orderCommand;
        }

        public async Task<OrderUpdateResponse?> UpdateOrderItemStatus(long orderId, long itemId, OrderItemUpdateRequest request)
        {
            var order = await _orderQuery.GetOrderByIdDetails(orderId);
            if (order == null)
            {
                return null;
            }

            var orderItem = order.OrderItems.FirstOrDefault(item => item.OrderItemId == itemId);
            if (orderItem == null)
            {
                return null;
            }

            if (orderItem.StatusId >= (int)OrderStatus.Ready && request.status < orderItem.StatusId)
            {
                return null;
            }

            orderItem.StatusId = request.status;

            var firstItemStatus = order.OrderItems.First().StatusId;
            if (order.OrderItems.All(item => item.StatusId == firstItemStatus))
            {
                order.OverallStatusId = firstItemStatus;
            }

            order.UpdateDate = DateTime.UtcNow;

            await _orderCommand.UpdateOrder(order);

            return new OrderUpdateResponse
            {
                OrderNumber = order.OrderId,
                TotalAmount = (double)order.Price,
                UpdatedAt = order.UpdateDate
            };

        }
    }
}
