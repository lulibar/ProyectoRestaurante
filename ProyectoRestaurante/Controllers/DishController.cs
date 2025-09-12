using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces.IDish;
using Application.Models.Request;
using Application.Models.Response;
using Application.Enums;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.IdentityModel.Tokens;
using Application.Interfaces.IDish.IDishServices;
using Asp.Versioning;
using System.ComponentModel.DataAnnotations;
using Application.Interfaces.ICategory;


namespace ProyectoRestaurante.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")] 
    [ApiController]
    [ApiVersion("1.0")]
    public class DishController : ControllerBase
    {
        
        private readonly ICreateDishService _createDishService;
        private readonly ISearchAsyncService _searchAsyncService;
        private readonly IUpdateDishService _updateDishService;
        private readonly ICategoryExists _categoryExists;


        public DishController(ICreateDishService createDishService, ISearchAsyncService searchAsyncService, IUpdateDishService updateDishService, ICategoryExists categoryExists)
        {
            _createDishService = createDishService;
            _searchAsyncService = searchAsyncService;
            _updateDishService = updateDishService;
            _categoryExists = categoryExists;
        }

        // POST
        /// <summary>
        /// Crear nuevo plato
        /// </summary>
        /// <remarks>
        /// Crea un nuevo plato en el menú del restaurante.
        ///
        /// **Validaciones:**
        /// - El nombre del plato debe ser único
        /// - El precio debe ser mayor a 0
        /// - La categoría debe existir en el sistema
        ///
        /// **Casos de uso:**
        /// - Agregar nuevos platos al menú
        /// - Platos estacionales o especiales del chef
        /// </remarks>
        [HttpPost]
        [SwaggerOperation(
        Summary = "Crear nuevo plato",
        Description = "Crea un nuevo plato en el menú del restaurante.")]
        [ProducesResponseType(typeof(DishResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateDish([SwaggerRequestBody(
        Description = "Objeto JSON que representa un nuevo plato a agregar al menú.",
        Required = true)][FromBody][Required]DishRequest dishRequest)
        {
           
            if (dishRequest == null)
            {
                return BadRequest(new ApiError("Required dish data."));
            }
            if (string.IsNullOrWhiteSpace(dishRequest.Name))
            {
                return BadRequest(new ApiError("Name is required"));
            }
            if (dishRequest.Category == 0)
            {
                return BadRequest(new ApiError("Category is required."));
                
            }
            if (dishRequest.Price <= 0)
            {
                return BadRequest(new ApiError("Price must be greater than zero."));
               
            }

            //var categoryExists = await _categoryExists.CategoryExists(dishRequest.Category);
            //if (!categoryExists)
            //{
            //    return BadRequest(new ApiError($"Category with ID {dishRequest.Category} not found."));
            //}

            if (dishRequest.Category == 0 || dishRequest.Category >= 11)
            {
                return BadRequest(new ApiError("No se ingreso una categoria valida"));
            }

            var createdDish = await _createDishService.CreateDish(dishRequest);

            if (createdDish == null)
            {
                return Conflict(new ApiError("A dish with this name already exists."));
            }
            return CreatedAtAction(nameof(Search), new { id = createdDish.id }, createdDish);

        }
        // GET
        /// <summary>
        /// Buscar platos
        /// </summary>
        /// <remarks>
        /// Obtiene una lista de platos del menú con **opciones de filtrado y ordenamiento**.
        /// 
        /// **Filtros disponibles:**
        /// - Por nombre (búsqueda parcial).
        /// - Por categoría.
        /// - Mostrar **solo platos activos** o **todos**.
        /// 
        /// **Ordenamiento:**
        /// - Por precio ascendente o descendente
        /// - Sin ordenamiento específico.
        /// 
        /// **Casos de uso:**
        /// - Mostrar el menú completo a los clientes.
        /// - Buscar platos específicos.
        /// - Filtrar por categorías (entrantes, principales, postres).
        /// - Administración del menú (incluyendo platos inactivos)
        /// </remarks>
        [HttpGet]
        [SwaggerOperation(
        Summary = "Buscar platos",
        Description = "Obtiene una lista de platos del menú con opciones de filtrado y ordenamiento.")]

        [ProducesResponseType(typeof(IEnumerable<DishResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]

        public async Task<IActionResult> Search([FromQuery] string? name, [FromQuery] int? category,[FromQuery] bool? onlyActive = null, [FromQuery] SortOrder? sortByPrice = SortOrder.ASC )
        {
            
            var list = await _searchAsyncService.SearchAsync(name, category, onlyActive, sortByPrice);
            if (list == null || !list.Any())
            {
                return BadRequest(new ApiError("No dishes found matching the criteria."));
            }
            
            return Ok(list);
        }


        // PUT
        /// <summary>
        /// Actualizar plato existente
        /// </summary>
        /// <remarks>
        /// Actualiza todos los campos de un plato existente en el menú.
        /// 
        /// **Validaciones:**
        /// - El plato debe existir en el sistema
        /// - Si se cambia el nombre, debe ser único
        /// - El precio debe ser mayor a 0
        /// - La categoría debe existir
        /// 
        /// **Casos de uso:**
        /// - Actualizar precios de platos.
        /// - Modificar descripciones o ingredientes.
        /// - Cambiar categorías de platos.
        /// - Activar/desactivar platos del menú.
        /// - Actualizar imágenes de platos.
        /// </remarks>
        [HttpPut("{id}")]
        [SwaggerOperation(
        Summary = "Actualizar plato existente",
        Description = "Actualiza todos los campos de un plato existente en el menú."
        )]
        [ProducesResponseType(typeof(DishResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateDish(Guid id, [FromBody] DishUpdateRequest dishRequest)
        {
            if (dishRequest == null)
            {
                return BadRequest(new ApiError("Required dish data."));
            }
            if (string.IsNullOrWhiteSpace(dishRequest.Name))
            {
                return BadRequest(new ApiError("Name is required."));
            }
            if (dishRequest.Category == 0 )
            {
                return BadRequest(new ApiError("Category is required."));
            }
            if (dishRequest.Price <= 0)
            {
                return BadRequest(new ApiError("Price must be greater than zero."));
            }

            var result = await _updateDishService.UpdateDish(id, dishRequest);
            if (result.NotFound)
            {
                return NotFound(new ApiError($"Dish with ID {id} not found."));
            }

            if (result.NameConflict)
            {
                return Conflict(new ApiError($"A dish named '{dishRequest.Name}' already exists."));
            }

            return Ok(result.UpdatedDish);
        }







    }   


    
}




