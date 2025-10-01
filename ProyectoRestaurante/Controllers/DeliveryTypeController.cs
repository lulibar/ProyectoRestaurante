using Application.Interfaces.IDeliveryType.IDeliveryTypeServices;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoRestaurante.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class DeliveryTypeController : ControllerBase
    {
        private readonly IGetAllDeliveryTypesService _getAllDeliveryTypesService;

        public DeliveryTypeController(IGetAllDeliveryTypesService getAllDeliveryTypesService)
        {
            _getAllDeliveryTypesService = getAllDeliveryTypesService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _getAllDeliveryTypesService.GetAllDeliveryTypes();
            return Ok(result);
        }
    }
}
