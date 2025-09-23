using Application.Interfaces.ICategory;
using Application.Interfaces.IDeliveryType;
using Application.Interfaces.IDish; 
using Application.Interfaces.IOrder.IOrderServices;
using Application.Models.Request;
using Application.Models.Request.OrdersRequest;
using Application.Models.Response; 
using Application.Models.Response.OrdersResponse; 
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ProyectoRestaurante.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ICreateOrderService _createOrderService;
        private readonly IDishQuery _dishQuery;
        private readonly IDeliveryExists _deliveryExists;


        public OrderController (ICreateOrderService createOrderService, IDishQuery dishQuery, IDeliveryExists deliveryExists)
        {
            _createOrderService = createOrderService;
            _dishQuery = dishQuery;
            _deliveryExists = deliveryExists;
        }


        // POST
        /// <summary>
        /// Crear nueva orden
        /// </summary>
        /// <remarks>
        /// Crea una nueva orden con los platos solicitados por el cliente.
        ///
        /// **Proceso:**
        /// 1. Se valida que todos los platos existan y estén activos
        /// 2.Se calcula el total de la orden
        /// 3. Se asigna un número de orden único
        /// 4.Se crean los items individuales de la orden
        ///
        /// **Validaciones**
        /// - Los platos deben existir y estar activos
        /// - Las cantidades deben ser mayores a 0
        /// - Debe especificarse tipo de entrega
        /// </remarks>
        
        [HttpPost]
        [SwaggerOperation(
        Summary = "Crear nueva orden",
        Description = "Crea una nueva orden con los platos solicitados por el cliente.")]
        [ProducesResponseType(typeof(OrderCreateResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status409Conflict)]

        public async Task<IActionResult> CreateOrder([SwaggerRequestBody(Description="Datos de la nueva orden", Required = true)][FromBody] OrderRequest orderRequest) 
        {
            if (orderRequest?.items == null || !orderRequest.items.Any())
            {
                return BadRequest(new ApiError("La orden debe contener al menos un ítem."));
            }


            if (orderRequest.items.Any(item => item.quantity <= 0))
            {
                return BadRequest(new ApiError("La cantidad de cada plato debe ser mayor a 0."));
            }

            var requestedDishIds = orderRequest.items.Select(item => item.id).Distinct();
            
            var existingDishes = await _dishQuery.GetDishesByIds(requestedDishIds);

            if (existingDishes.Count != requestedDishIds.Count())
            {
                return BadRequest(new ApiError("Uno o más platos especificados no existen o no están disponibles."));
            }
            var createdOrder = await _createOrderService.CreateOrder(orderRequest);

            var deliveryExists = await _deliveryExists.DeliveryExist(orderRequest.delivery.id);
            if (!deliveryExists)
            {
                return BadRequest(new ApiError($"Category with ID {orderRequest.delivery.id} not found."));
            }

            if (createdOrder == null)
            {
                return Conflict(new ApiError("No se pudo crear la orden debido a un conflicto."));
            }

            return CreatedAtAction("GetOrderById", new { id = createdOrder.orderNumber }, createdOrder);


        }

        [HttpGet("{id}")] // Esto define la ruta: GET /api/Order/{id}
        public async Task<IActionResult> GetOrderById(long id)
        {
            // Por ahora, solo devolvemos un Ok para que la ruta exista.
            // Aquí irá la lógica para buscar la orden en la base de datos.
            return Ok($"Lógica para buscar la orden con ID {id} irá aquí.");
        }
    }
}
