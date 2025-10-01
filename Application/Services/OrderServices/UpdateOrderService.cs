using Application.Enums;
using Application.Exceptions;
using Application.Interfaces.IDish;
using Application.Interfaces.IOrder;
using Application.Interfaces.IOrder.IOrderServices;
using Application.Interfaces.IOrderItem;
using Application.Models.Request.OrdersRequest;
using Application.Models.Response.OrdersResponse;
using Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.OrderServices
{
    public class UpdateOrderService : IUpdateOrderService
    {
        private readonly IOrderQuery _orderQuery;
        private readonly IOrderCommand _orderCommand;
        private readonly IDishQuery _dishQuery;
        private readonly IOrderItemCommand _orderItemCommand;

        public UpdateOrderService (IOrderQuery orderQuery, IOrderCommand orderCommand, IDishQuery dishQuery, IOrderItemCommand orderItemCommand)
        {
            _orderQuery = orderQuery;
            _orderCommand = orderCommand;
            _dishQuery = dishQuery;
            _orderItemCommand = orderItemCommand;
        }

        public async Task<OrderUpdateResponse> UpdateOrder(long orderId, OrderUpdateRequest updateRequest)
        {
            if (updateRequest?.items == null || !updateRequest.items.Any())
                throw new BadRequestException("La solicitud para agregar ítems no puede estar vacía.");
            if (updateRequest.items.Any(item => item.quantity <= 0))
                throw new BadRequestException("La cantidad de cada plato debe ser mayor a 0.");

            var order = await _orderQuery.GetOrderById(orderId);
            if (order == null)
                throw new NotFoundException($"Orden con ID {orderId} no encontrada.");

            if (order.OverallStatusId >= (int)OrderStatus.InProgress)
                throw new ConflictException("No se puede modificar una orden que ya está en preparación o finalizada.");

            var requestedDishIds = updateRequest.items.Select(item => item.id).Distinct();
            var existingDishes = await _dishQuery.GetDishesByIds(requestedDishIds);

            if (existingDishes.Count != requestedDishIds.Count())
                throw new BadRequestException("El plato especificado no está disponible.");


            var newOrderItems = updateRequest.items.Select(item => new OrderItem
            {
                OrderId = order.OrderId,
                DishId = item.id,
                Quantity = item.quantity,
                Notes = item.notes,
                StatusId = 1
            }).ToList();

            await _orderItemCommand.InsertOrderItemRange(newOrderItems);

            order.Price += await CalculateTotalPrice(newOrderItems, existingDishes);
            order.UpdateDate = DateTime.UtcNow;

            await _orderCommand.UpdateOrder(order);

            return new OrderUpdateResponse
            {
                OrderNumber = order.OrderId,
                TotalAmount = (double)order.Price,
                UpdatedAt = order.UpdateDate
            };

        }

        private async Task<decimal> CalculateTotalPrice(List<OrderItem> newItems, List<Dish> dishes)
        {
            var dishDictionary = dishes.ToDictionary(d => d.DishId);
            decimal total = 0;

            foreach (var item in newItems)
            {
                if (dishDictionary.TryGetValue(item.DishId, out var dish))
                {
                    total += dish.Price * item.Quantity;
                }
            }
            return total;
        }


    }
}
