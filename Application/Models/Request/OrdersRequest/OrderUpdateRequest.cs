using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Request.OrdersRequest
{
    public class OrderUpdateRequest
    {
        public List<Items>? Items { get; set; }
    }
}
