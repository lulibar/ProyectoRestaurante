using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Request.OrdersRequest
{
    public class OrderRequest
    {
        [Required(ErrorMessage = "La orden debe contener al menos un ítem.")]
        [MinLength(1, ErrorMessage = "La orden debe contener al menos un ítem.")]
        public List<Items>? items { get; set; }

        [Required(ErrorMessage = "Los datos de entrega son requeridos.")]
        public Delivery? delivery { get; set; }

        [StringLength(500, ErrorMessage = "Las notas no pueden superar los 500 caracteres.")]
        public string? notes { get; set; }
    }
}
