using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class OrderItem
    {
        public long OrderItemId { get; set; }
       
        public int Quantity { get; set; }
        public string Notes { get; set; }

        public DateTime CreateDate { get; set; }

        public Guid DishId { get; set; }
        public Dish Dish { get; set; }

        public Order OrderRef { get; set; }
        public long Order { get; set; }

        public int Status { get; set; }
        public Status StatusRef { get; set; }

    }
}
