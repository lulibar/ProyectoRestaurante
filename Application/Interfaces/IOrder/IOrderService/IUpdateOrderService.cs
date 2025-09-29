﻿using Application.Models.Request;
using Application.Models.Request.OrdersRequest;
using Application.Models.Response;
using Application.Models.Response.OrdersResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.IOrder.IOrderServices
{
    public interface IUpdateOrderService
    {
        Task<OrderUpdateResponse?> UpdateOrder(long orderId, OrderUpdateRequest request);


    }
}
