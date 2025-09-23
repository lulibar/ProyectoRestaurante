﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Request.OrdersRequest
{
    public class OrderRequest
    {
        public List<Items>? items { get; set; }
        public Delivery? delivery { get; set; }
        public string? notes { get; set; }
    }
}
