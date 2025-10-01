using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Request.OrdersRequest
{
    public class OrderUpdateRequest
    {
        [Required(ErrorMessage = "La solicitud debe contener al menos un ítem para agregar.")]
        [MinLength(1, ErrorMessage = "La solicitud debe contener al menos un ítem para agregar.")]
        public List<Items>? items { get; set; }
    }
}
