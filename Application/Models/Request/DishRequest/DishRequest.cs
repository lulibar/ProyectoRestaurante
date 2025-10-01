using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Request.DishRequest
{
    public class DishRequest
    {
        [Required(ErrorMessage = "El nombre del plato es requerido.")]
        [StringLength(255, ErrorMessage = "El nombre no puede tener más de 255 caracteres.")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "El precio es requerido.")]
        [Range(0.01, 100000.00, ErrorMessage = "El precio debe ser mayor a cero.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "La categoría es requerida.")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID de la categoría no es válido.")]
        public int Category { get; set; }
        public string? Image { get; set; }

	}
}
