using Application.Interfaces.ICategory;
using Application.Interfaces.IDeliveryType;
using Application.Interfaces.IDish;
using Application.Interfaces.IOrder.IOrderService;
using Application.Interfaces.IOrder.IOrderServices;
using Application.Models.Request;
using Application.Models.Request.OrdersRequest;
using Application.Models.Response; 
using Application.Models.Response.OrdersResponse; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Runtime.ConstrainedExecution;

namespace ProyectoRestaurante.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ICreateOrderService _createOrderService;
        private readonly IDishQuery _dishQuery;
        private readonly IDeliveryExists _deliveryExists;
        private readonly IGetOrderByDateStatus _getOrderByDateStatus;
        private readonly IUpdateOrderService _updateOrderService;
        private readonly IGetOrderByIdService _getOrderByIdService;
        private readonly IUpdateOrderItemStatusService _updateOrderItemStatusService;


        public OrderController(ICreateOrderService createOrderService, IDishQuery dishQuery, IDeliveryExists deliveryExists, IGetOrderByDateStatus getOrderByDateStatus, IUpdateOrderService updateOrderService, IGetOrderByIdService getOrderByIdService, IUpdateOrderItemStatusService updateOrderItemStatusService)
        {
            _createOrderService = createOrderService;
            _dishQuery = dishQuery;
            _deliveryExists = deliveryExists;
            _getOrderByDateStatus = getOrderByDateStatus;
            _updateOrderService = updateOrderService;
            _getOrderByIdService = getOrderByIdService;
            _updateOrderItemStatusService = updateOrderItemStatusService;
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
        /// 2. Se calcula el total de la orden
        /// 3. Se asigna un número de orden único
        /// 4. Se crean los items individuales de la orden
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

        public async Task<IActionResult> CreateOrder([SwaggerRequestBody(Description = "Datos de la nueva orden", Required = true)][FromBody] OrderRequest orderRequest)
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

        // GET
        /// <summary>
        /// Buscar ordenes
        /// </summary>
        /// <remarks>
        /// Obtiene una lista de órdenes con filtros opcionales.
        /// 
        /// **Filtros disponibles:**
        ///- Por rango de fechas(desde/hasta)
        ///- Por estado de la orden
        ///
        /// ** Casos de uso:**
        ///- Ver órdenes del día para cocina
        ///- Historial de órdenes del cliente
        ///- Reportes de ventas por período
        ///- Seguimiento de órdenes pendientes
        ///</remarks>

        [HttpGet]
        [SwaggerOperation(
        Summary = "Buscar ordenes",
        Description = "Obtiene una lista de órdenes con filtros opcionales.")]

        [ProducesResponseType(typeof(IEnumerable<OrderDetailsResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> GetOrderById([FromQuery] int? statusId, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            try
            {
                var result = await _getOrderByDateStatus.GetOrderDateStatus(from, to, statusId);
                if (result == null || !result.Any())
                {
                    return NotFound(new ApiError("No orders found with the specified filters."));
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(new ApiError("An error occurred while processing the request."));
            }
        }

        // PUT
        /// <summary>
        /// Actualizar orden existente
        /// </summary>
        /// <remarks>
        /// Actualiza los items de una orden existente
        /// 
        /// **Limitaciones:**
        /// - Solo se pueden actualizar órdenes que no esten cerradas
        /// - No se pueden agregar items de platos inactivos
        /// - El total se recalcula automáticamente
        /// 
        /// **Casos de uso:**
        /// - Cliente quiere agregar más platos a su orden
        /// - Modificar cantidades antes de que comience la preparación
        /// - Cambiar notas especiales de los platos
        /// </remarks>
        [HttpPut("{id}")]
        [SwaggerOperation(
            Summary = "Actualizar orden existente",
            Description = "Actualiza los items de una orden existente."
        )]
        [ProducesResponseType(typeof(OrderUpdateResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> UpdateOrder(int id, [FromBody] OrderUpdateRequest updateRequest)
        {
            try
            {
                var response = await _updateOrderService.UpdateOrder(id, updateRequest);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET
        /// <summary>
        /// Obtener orden por número
        /// </summary>
        /// <remarks>
        /// Obtiene los detalles completos de una orden específica.
        /// 
        /// **Información incluida:**
        ///- Detalles de la orden (número, total, estado)
        ///- Información de entrega
        ///- Lista completa de items con sus estados individuales
        ///- Información de cada plato incluido
        ///
        /// ** Casos de uso:**
        ///- Seguimiento de orden por parte del cliente
        ///- Detalles para cocina y entrega
        ///- Historial detallado de órdenes
        ///</remarks>

        [HttpGet("{id}")]
        [SwaggerOperation(
        Summary = "Obtener orden por número",
        Description = "Obtiene los detalles completos de una orden específica."
        )]
        [ProducesResponseType(typeof(OrderDetailsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> GetOrderById(long id)
        {
            var result = await _getOrderByIdService.GetOrderById(id);

            if (result == null)
            {
                return NotFound(new ApiError($"No se encontró la orden con el número {id}."));
            }

            return Ok(result);
        }

        // PATCH
        /// <summary>
        /// Actualizar estado de item individual
        /// </summary>
        /// <remarks>
        /// Actualiza el estado de un item específico dentro de una orden.
        /// 
        /// **Casos de uso típicos:**
        ///- Cocina marca un plato como "En preparación"
        ///- Cocina marca un plato como "Listo"  
        ///- Cancelar un item específico si no se puede preparar
        ///
        /// ** Flujo de estados típico:**
        ///1. Pendiente → En preparación(cocina comienza)
        ///2. En preparación → Listo(plato terminado)
        ///3. Listo → Entregado(entregado al cliente)
        ///
        ///**Nota:** El estado de la orden general se actualiza automáticamente basado en el estado de todos sus items.
        /// 
        ///</remarks>
        ///

        [HttpPatch("{id}/item/{itemId}")]
        [SwaggerOperation(
        Summary = "Actualizar estado de item individual",
        Description = "Actualiza el estado de un item específico dentro de una orden."
        )]

        [ProducesResponseType(typeof(OrderUpdateResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateOrderItemStatus(long id, long itemId, [FromBody] OrderItemUpdateRequest request)
        {
            var result = await _updateOrderItemStatusService.UpdateOrderItemStatus(id, itemId, request);

            if (result == null)
            {
                return NotFound(new ApiError($"No se encontró el item {itemId} en la orden {id}, o la transición de estado no es válida."));
            }

            return Ok(result);
        }
    }
}
